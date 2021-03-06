using AutoMapper;
using DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Service : IChatService, IClientService, IContactService, IChatMemberService, IMessageService
    {
        private List<CallBack> callbacks = new List<CallBack>();

        private IUnitOfWork repositories;
        private IMapper clientMapper;
        private IMapper chatMapper;
        private IMapper contactMapper;
        private IMapper messageMapper;
        private IMapper chatMemberMapper;

        private string pathToClientsPhoto;
        private string pathToChatsPhoto;

        private static Random random = new Random();
        public Service()
        {
            this.repositories = new UnitOfWork();

            IConfigurationProvider clientConfig = new MapperConfiguration(
                cfg =>
                {
                    // Entity to DTO
                    cfg.CreateMap<Client, ClientDTO>();

                    cfg.CreateMap<Account, AccountDTO>().ForMember(dst => dst.Password, opt => opt.Ignore());

                    cfg.CreateMap<ClientDTO, Client>();

                    cfg.CreateMap<AccountDTO, Account>().ForMember(dst => dst.Password, opt => opt.MapFrom(src => ComputeSha256Hash(src.Password)));
                });
            clientMapper = new Mapper(clientConfig);

            IConfigurationProvider chatConfig = new MapperConfiguration(
                cfg =>
                {
                    // Entity to DTO
                    cfg.CreateMap<Chat, ChatDTO>();

                    cfg.CreateMap<Client, ClientDTO>();

                    cfg.CreateMap<ChatDTO, Chat>();

                    cfg.CreateMap<ClientDTO, Client>();

                });
            chatMapper = new Mapper(chatConfig);

            IConfigurationProvider contactConfig = new MapperConfiguration(
                cfg =>
                {
                    // Entity to DTO
                    cfg.CreateMap<Contact, ContactDTO>();

                    cfg.CreateMap<Client, ClientDTO>();

                    cfg.CreateMap<Account, AccountDTO>().ForMember(dst => dst.Password, opt => opt.Ignore());

                    cfg.CreateMap<ContactDTO, Contact>();

                    cfg.CreateMap<ClientDTO, Client>();

                    cfg.CreateMap<AccountDTO, Account>().ForMember(dst => dst.Password, opt => opt.Ignore());
                });
            contactMapper = new Mapper(contactConfig);

            IConfigurationProvider messageConfig = new MapperConfiguration(
                cfg =>
                {
                    // Entity to DTO
                    cfg.CreateMap<Message, MessageDTO>();

                    cfg.CreateMap<Client, ClientDTO>();

                    cfg.CreateMap<Chat, ChatDTO>();

                    cfg.CreateMap<Account, AccountDTO>().ForMember(dst => dst.Password, opt => opt.Ignore());

                    cfg.CreateMap<MessageDTO, Message>();

                    cfg.CreateMap<ClientDTO, Client>();

                    cfg.CreateMap<ChatDTO, Chat>();

                    cfg.CreateMap<AccountDTO, Account>().ForMember(dst => dst.Password, opt => opt.Ignore());
                });
            messageMapper = new Mapper(messageConfig);

            IConfigurationProvider chatMemberConfig = new MapperConfiguration(
                cfg =>
                {
                    // Entity to DTO
                    cfg.CreateMap<ChatMember, ChatMemberDTO>().ForMember(dst => dst.Chat, opt => opt.Ignore());

                    cfg.CreateMap<Client, ClientDTO>();

                    cfg.CreateMap<Account, AccountDTO>().ForMember(dst => dst.Password, opt => opt.Ignore());

                    cfg.CreateMap<ChatMemberDTO, ChatMember>().ForMember(dst => dst.Chat, opt => opt.Ignore());

                    cfg.CreateMap<ClientDTO, Client>();

                    cfg.CreateMap<AccountDTO, Account>().ForMember(dst => dst.Password, opt => opt.Ignore());
                });
            chatMemberMapper = new Mapper(chatMemberConfig);
        }
        private bool IsExistUniqueName(int id,string uniqueName, bool isClient)
        {
            var client = repositories.ClientRepos.Get().Where(c => c.UniqueName == uniqueName && ((isClient && c.Id!=id) || !isClient) ).FirstOrDefault();
            var chat = repositories.ChatRepos.Get().Where(c => c.UniqueName == uniqueName && ((!isClient && c.Id != id) || isClient)).FirstOrDefault();
            if (client == null && chat == null)
                return false;
            return true;
        }
        //--------------------------Client Methods--------------------//
        #region
        private Client GetClientByAccount(AccountDTO accountDTO, string password)
        {
            var client = repositories.ClientRepos.Get().Where(c => (c.Account.Email == accountDTO.Email || c.Account.Phone == accountDTO.Phone) && c.Account.Password == ComputeSha256Hash(password)).FirstOrDefault();
            if (client != null)
                return client;
            else
                return null;
        }
        private int IsNotExistClient(ClientDTO clientDTO)
        {
            var client = repositories.ClientRepos.Get().Where(c => c.Id != clientDTO.Id && (c.UniqueName == clientDTO.UniqueName || c.Account.Email == clientDTO.Account.Email || c.Account.Phone == clientDTO.Account.Phone)).FirstOrDefault();
            if (client == null)
                return -1;
            else
                return client.Id;
        }
        private int IsExistClient(ClientDTO clientDTO)
        {
            var client = repositories.ClientRepos.Get().Where(c => c.Account.Email == clientDTO.Account.Email || c.Account.Phone == clientDTO.Account.Phone).FirstOrDefault();
            if (client == null)
                return -1;
            else
                return client.Id;
        }
        public ClientDTO CreateNewClient(ClientDTO client, string password)
        {
            var id = IsExistClient(client);
            if (id == -1 && !IsExistUniqueName(client.Id, client.UniqueName,true))
            {
                client.Account.Password = password;
                repositories.ClientRepos.Insert(clientMapper.Map<Client>(client));
                repositories.Save();
                repositories.ClientRepos.Get().FirstOrDefault(c=>c.UniqueName==client.UniqueName).AccountId = repositories.AccountRepos.Get().FirstOrDefault(a => a.Email == client.Account.Email).ClientId;
                repositories.Save();
                ClientDTO addedClient = clientMapper.Map<ClientDTO>(repositories.ClientRepos.Get().FirstOrDefault(c => c.UniqueName == client.UniqueName));
                callbacks.Add(new CallBack() { ClientId = addedClient.Id, Callback = OperationContext.Current.GetCallbackChannel<ICallback>()});
                return addedClient;
            }
            else
                return null;
        }
        public ClientDTO GetClient(AccountDTO account, string password)
        {
            var client = GetClientByAccount(account, password);
            if (client != null)
            {
                DirectoryInfo directory = new DirectoryInfo(pathToClientsPhoto);
                if (!directory.Exists)
                    directory.Create();
                callbacks.Add(new CallBack() { ClientId = client.Id, Callback = OperationContext.Current.GetCallbackChannel<ICallback>() });
                return clientMapper.Map<ClientDTO>(client);
            }
            else
                return null;
        }
        public string FreePath(string path)
        {
            int index = 0;
            string tmp;
            do
            {
                tmp = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + (index == 0 ? null : $"({index})") + Path.GetExtension(path));
                index++;
            } while (System.IO.File.Exists(tmp));
            return tmp;
        }
        public void SetPhoto(int clientId, InfoFile info)
        {
            string path = FreePath(Path.Combine(pathToClientsPhoto, clientId.ToString() + Path.GetExtension(info.Name)));
            DirectoryInfo directory = new DirectoryInfo(pathToClientsPhoto);
            if (!directory.Exists)
                directory.Create();
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                fs.Write(info.Data, 0, info.Data.Length);
            }
            repositories.ClientRepos.Get().Where(c => c.Id == clientId).FirstOrDefault().PhotoPath = path;
            repositories.Save();
            var result = directory.GetFiles().Where(d => ((!d.Name.Contains('(') && Path.GetFileNameWithoutExtension(d.Name) == clientId.ToString()) || (d.Name.Contains('(') && d.Name.Replace(d.Name.Substring(d.Name.IndexOf('(')), null) == clientId.ToString())) && d.FullName != path);
            foreach (var item in result)
            {
                try
                {
                    item.Delete();
                }
                catch (Exception) { }
            }

            foreach (var item in callbacks)
            {
                if (item.ClientId != clientId)
                {
                    var contacts = TakeContacts(item.ClientId);
                    if (contacts.FirstOrDefault(c => c.Id == clientId) != null)
                        try
                        {
                            item.Callback.GetNewClientPhoto(clientId, info);
                        }
                        catch (Exception)
                        { }
                }
            }
            foreach (var item in callbacks)
            {
                if (item.ClientId != clientId)
                {
                    var chats = TakeChats(item.ClientId);
                    foreach (var item2 in chats)
                    {
                        var members = TakeClients(item2.Id);
                        foreach (var item3 in members)
                        {
                            if(item3.ClientId == clientId)
                            try
                            {
                                item.Callback.GetNewClientPhoto(clientId, info);
                            }
                            catch (Exception)
                            { }
                        }
                    }
                    
                }
            }
            var pmChats = repositories.ChatRepos.Get().Where(c => c.IsPM && c.ChatMembers.Where(cm => cm.Client.Id == clientId).Count() > 0);
            foreach (var item in pmChats)
            {
                foreach (var item2 in callbacks)
                {
                    if (item2.ClientId != clientId)
                    {
                        var oponent = item.ChatMembers.FirstOrDefault(cm => cm.Client.Id != clientId);
                        if (oponent != null)
                            if (oponent.Client.Id == item2.ClientId)
                            {
                                item2.Callback.SetNewPMChatPhoto(item.Id, info);
                            }
                    }
                }
            }
        }
        public void GetPathToPhoto(string pathToPhoto)
        {
            this.pathToClientsPhoto = Path.Combine(pathToPhoto, "ClientsPhoto");
            this.pathToChatsPhoto = Path.Combine(pathToPhoto, "ChatsPhoto");
        }
        public InfoFile GetPhoto(int clientId)
        {
            DirectoryInfo di = new DirectoryInfo(pathToClientsPhoto);
            if (!di.Exists)
                di.Create();
            string path = repositories.ClientRepos.Get().Where(c => c.Id == clientId).Select(c => c.PhotoPath).FirstOrDefault();
            if (path == null) return null;
            var result = di.GetFiles().Where(f => f.FullName == path);
            if (result.Count() == 0)
            {
                repositories.ClientRepos.Get().Where(c => c.Id == clientId).FirstOrDefault().PhotoPath = null;
                repositories.Save();
                return null;
            }
            InfoFile info = new InfoFile() { Name = path };
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                byte[] fileData = new byte[fs.Length];
                fs.Read(fileData, 0, fileData.Length);
                info.Data = fileData;
            }
            return info;
        }
        public void SetProperties(ClientDTO client, out bool result)
        {
            var id = IsNotExistClient(client);
            if (id == -1)
            {
                var oldClient = repositories.ClientRepos.Get().Where(c => c.Id == client.Id).FirstOrDefault();
                var pmChats = repositories.ChatRepos.Get().Where(c => c.IsPM && c.ChatMembers.Where(cm => cm.Client.Id == oldClient.Id).Count() > 0);
                foreach (var item in pmChats)
                {
                    item.Name = item.Name.Replace(oldClient.Name, client.Name);
                    foreach (var item2 in callbacks)
                    {
                        if (item2.ClientId != client.Id)
                        {
                            var oponent = item.ChatMembers.FirstOrDefault(cm => cm.Client.Id != oldClient.Id);
                            if (oponent != null)
                                if (oponent.Client.Id == item2.ClientId)
                                {
                                    item2.Callback.SetNewPMChatProperties(chatMapper.Map<ChatDTO>(item));
                                }
                        }
                    }
                }
                repositories.Save();
                oldClient.Name = client.Name;
                oldClient.UniqueName = client.UniqueName;
                oldClient.Account.Email = client.Account.Email;
                oldClient.Account.Phone = client.Account.Phone;
                repositories.ClientRepos.Update(oldClient);
                repositories.Save();
                result = true;
                foreach (var item in callbacks)
                {
                    if (item.ClientId != client.Id)
                    {
                        var contacts = TakeContacts(item.ClientId);
                        if (contacts.FirstOrDefault(c => c.Id == client.Id) != null)
                            try
                            {
                                item.Callback.GetNewClientProperties(clientMapper.Map<ClientDTO>(oldClient));
                            }
                            catch (Exception)
                            { }
                    }
                }
                foreach (var item in callbacks)
                {
                    if (item.ClientId != client.Id)
                    {
                        var chats = TakeChats(item.ClientId);
                        foreach (var item2 in chats)
                        {
                            var members = TakeClients(item2.Id);
                            foreach (var item3 in members)
                            {
                                if(item3.ClientId == client.Id)
                                try
                                {
                                    item.Callback.GetNewClientProperties(clientMapper.Map<ClientDTO>(oldClient));
                                }
                                catch (Exception)
                                { }
                            }
                        }

                    }
                }
            }
            else
                result = false;
        }
        public void Disconnect()
        {
            callbacks.Remove(callbacks.Where(c => c.Callback == OperationContext.Current.GetCallbackChannel<ICallback>()).FirstOrDefault());
        }
        static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        #endregion
        //--------------------------Contacts Methods--------------------//
        #region
        private bool IsContactExist(int clientID, int contactId)
        {
            var contact = repositories.ContactRepos.Get().Where(c => c.ClientId == clientID && c.ContactClient.Id == contactId).FirstOrDefault();
            if (contact == null)
            {
                return false;
            }
            return true;
        }
        public ClientDTO AddContact(int clientID, int contactId)
        {
            if (!IsContactExist(clientID, contactId) && repositories.ClientRepos.Get().FirstOrDefault(c=>c.Id == contactId) != null)
            {
                var contactClient = repositories.ClientRepos.Get().Where(c => c.Id == contactId).FirstOrDefault();
                repositories.ContactRepos.Insert(new Contact() { ClientId = clientID, ContactClientId = contactClient.Id });
                repositories.Save();
                return contactMapper.Map<ClientDTO>(contactClient);
            }
            return null;
        }
        public bool DeleteContact(int clientID, int contactId)
        {
            if (IsContactExist(clientID, contactId))
            {
                var contactClient = repositories.ContactRepos.Get().Where(c => c.ClientId == clientID && c.ContactClient.Id == contactId).FirstOrDefault();
                repositories.ContactRepos.Delete(contactClient);
                repositories.Save();
                return true;
            }
            return false;
        }

        public IEnumerable<ClientDTO> TakeContacts(int clientId)
        {
            var result = repositories.ContactRepos.Get().Where(c => c.ClientId == clientId).Select(c => c.ContactClient);
            return contactMapper.Map<IEnumerable<ClientDTO>>(result);
        }
        public IEnumerable<ClientDTO> SearchClients(string uniqueName)
        {
            var result = repositories.ClientRepos.Get().Where(c => c.UniqueName.Contains(uniqueName));
            return clientMapper.Map<IEnumerable<ClientDTO>>(result);
        }
        #endregion
        //--------------------------Chat Methods--------------------//
        #region
        public bool IsChatExist(ChatDTO chatDTO)
        {
            var result = repositories.ChatRepos.Get().Where(c => c.Id != chatDTO.Id && c.UniqueName == chatDTO.UniqueName).FirstOrDefault();
            if (result == null)
                return false;
            return true;
        }
        public ChatDTO CreateNewChat(ChatDTO newChatDTO)
        {
            if (!IsExistUniqueName(newChatDTO.Id, newChatDTO.UniqueName,false))
            {
                var chat = chatMapper.Map<Chat>(newChatDTO);
                repositories.ChatRepos.Insert(chat);
                repositories.Save();
                return chatMapper.Map<ChatDTO>(chat);
            }
            else
                return null;
        }
        public void CreatePMChat(int clientId, int companionId,out int chatId)
        {
            var client = repositories.ClientRepos.Get().Where(c => c.Id == clientId).FirstOrDefault();
            var companion = repositories.ClientRepos.Get().Where(c => c.Id == companionId).FirstOrDefault();
            Chat pmChat = new Chat() { Id = 0, IsPM = true, MaxUsers = 2, IsPrivate = true, Name = $"{client.Name}{companion.Name}"};
            do
            {
                pmChat.UniqueName = RandomString();

            } while (IsExistUniqueName(pmChat.Id, pmChat.UniqueName, false));
            var chat = repositories.ChatRepos.Get().FirstOrDefault(c => c.IsPM && c.ChatMembers.FirstOrDefault(cm => cm.ClientId == clientId) != null && c.ChatMembers.FirstOrDefault(cm => cm.ClientId == companionId) != null);
            if (chat == null)
            {
                repositories.ChatRepos.Insert(pmChat);
                repositories.Save();
                chatId = -1;
                repositories.ChatMemberRepos.Insert(new ChatMember() { ClientId = clientId, ChatId = pmChat.Id, IsAdmin = false, DateLastReadMessage = DateTime.Now });
                repositories.ChatMemberRepos.Insert(new ChatMember() { ClientId = companionId, ChatId = pmChat.Id, IsAdmin = false, DateLastReadMessage = DateTime.Now });
                repositories.Save();
                try
                {
                    var clientCallback = callbacks.FirstOrDefault(c => c.ClientId == clientId);
                    if (clientCallback != null)
                        clientCallback.Callback.TakeChat(chatMapper.Map<ChatDTO>(pmChat), GetPhoto(companionId));

                    var companionCallback = callbacks.FirstOrDefault(c => c.ClientId == companionId);
                    if (companionCallback != null)
                        companionCallback.Callback.TakeChat(chatMapper.Map<ChatDTO>(pmChat), GetPhoto(clientId));
                }
                catch (Exception)
                { }
            }
            else
                chatId = chat.Id;
        }
        public static string RandomString(int length = 25)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public void SetChatProperties(ChatDTO chatDTO, out bool result)
        {
          
            if (!IsExistUniqueName(chatDTO.Id, chatDTO.UniqueName,false) && TakeClients(chatDTO.Id).Count()<=chatDTO.MaxUsers)
            {
                var chat = repositories.ChatRepos.Get().Where(c => c.Id == chatDTO.Id).FirstOrDefault();
                chat.Name = chatDTO.Name;
                chat.UniqueName = chatDTO.UniqueName;
                chat.MaxUsers = chatDTO.MaxUsers;
                chat.IsPrivate = chatDTO.IsPrivate;
                repositories.ChatRepos.Update(chat);
                repositories.Save();
                result = true;

                foreach (var item in TakeClients(chat.Id))
                {
                    foreach (var item2 in callbacks)
                    {
                        if(item.Client.Id == item2.ClientId)
                            item2.Callback.GetNewChatProperties(chatMapper.Map<ChatDTO>(chat));
                    }
                }
            }
            else
                result = false;
        }
        public void SetChatPhoto(int chatId, InfoFile info)
        {
            string path = FreePath(Path.Combine(pathToChatsPhoto, chatId.ToString() + Path.GetExtension(info.Name)));
            DirectoryInfo directory = new DirectoryInfo(pathToChatsPhoto);
            if (!directory.Exists)
                directory.Create();
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                fs.Write(info.Data, 0, info.Data.Length);
            }
            repositories.ChatRepos.Get().Where(c => c.Id == chatId).FirstOrDefault().PhotoPath = path;
            repositories.Save();
            var result = directory.GetFiles().Where(d => ((!d.Name.Contains('(') && Path.GetFileNameWithoutExtension(d.Name) == chatId.ToString()) || (d.Name.Contains('(') && d.Name.Replace(d.Name.Substring(d.Name.IndexOf('(')), null) == chatId.ToString())) && d.FullName != path);
            foreach (var item in result)
            {
                try
                {
                    item.Delete();
                }
                catch (Exception) { }
            }
            foreach (var item in TakeClients(chatId))
            {
                foreach (var item2 in callbacks)
                {
                    if (item.Client.Id == item2.ClientId)
                        item2.Callback.GetNewChatPhoto(chatId, info);
                }
            }
        }

        public InfoFile GetChatPhoto(int chatId)
        {
            DirectoryInfo di = new DirectoryInfo(pathToChatsPhoto);
            if (!di.Exists)
                di.Create();
            string path = repositories.ChatRepos.Get().Where(c => c.Id == chatId).Select(c => c.PhotoPath).FirstOrDefault();
            if (path == null) return null;
            var result = di.GetFiles().Where(f => f.FullName == path);
            if (result.Count() == 0)
            {
                repositories.ChatRepos.Get().Where(c => c.Id == chatId).FirstOrDefault().PhotoPath = null;
                repositories.Save();
                return null;
            }
            InfoFile info = new InfoFile() { Name = path };
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                byte[] fileData = new byte[fs.Length];
                fs.Read(fileData, 0, fileData.Length);
                info.Data = fileData;
            }
            return info;
        }
        public IEnumerable<ChatDTO> SearchChats(string uniqueName)
        {
            var result = repositories.ChatRepos.Get().Where(c => c.UniqueName.Contains(uniqueName) && !c.IsPM && !c.IsPrivate);
            return chatMapper.Map<IEnumerable<ChatDTO>>(result);
        }

        public void DeleteChat(int chatId,out bool isRemoved)
        {
            var chat = repositories.ChatRepos.Get().FirstOrDefault(c => c.Id == chatId && !c.IsPM);
            if (chat != null)
            {
                foreach (var item in TakeClients(chatId))
                {
                    foreach (var item2 in callbacks)
                    {
                        if (item2.ClientId == item.Client.Id)
                            try
                            {
                                item2.Callback.DeleteChatForAll(chatId);
                            }
                            catch (Exception)
                            { }
                    }
                }
                repositories.ChatRepos.Delete(chat);
                var members = repositories.ChatMemberRepos.Get().Where(cm => cm.ChatId == chatId);
                foreach (var item in members)
                {
                    repositories.ChatMemberRepos.Delete(item);
                }
                var messages = repositories.MessageRepos.Get().Where(m => m.ChatId == chatId);
                foreach (var item in messages)
                {
                    repositories.MessageRepos.Delete(item);
                }
                isRemoved = true;
                repositories.Save();
            }
            else
                isRemoved = false;
        }
        #endregion
        //--------------------------ChatMember Methods--------------------//
        #region
        private bool IsThereClientInChat(int clientId, int chatId)
        {
            var result = repositories.ChatMemberRepos.Get().Where(c => c.ClientId == clientId && c.ChatId == chatId).FirstOrDefault();
            if (result == null)
            {
                return false;
            }
            return true;
        }
        private bool IsChatExist(string chatUniqueName)
        {
            var chat = repositories.ChatRepos.Get().Where(c => c.UniqueName == chatUniqueName).FirstOrDefault();
            if (chat != null)
                return true;
            return false;
        }
        public void JoinToChat(int clientId, string chatUniqueName, bool isAdmin, out ChatDTO newChat)
        {
            var chat = repositories.ChatRepos.Get().Where(c => c.UniqueName == chatUniqueName).FirstOrDefault();
            if (chat != null && !IsThereClientInChat(clientId, chat.Id) && IsChatExist(chatUniqueName) && (chat.ChatMembers == null || chat.ChatMembers.Count()<chat.MaxUsers))
            {
                var chatMember = new ChatMember() { ClientId = clientId, ChatId = chat.Id, IsAdmin = isAdmin, DateLastReadMessage = DateTime.Now };
                repositories.ChatMemberRepos.Insert(chatMember);
                repositories.Save();
                newChat = chatMapper.Map<ChatDTO>(chat);

                foreach (var item in TakeClients(chat.Id))
                {
                    foreach (var item2 in callbacks)
                    {
                        if (item2.ClientId == item.Client.Id && item2.ClientId != clientId)
                        try
                        {
                            item2.Callback.Joined(chatMemberMapper.Map<ChatMemberDTO>(chatMember), chat.Id, GetPhoto(clientId));
                        }
                        catch (Exception)
                        {}
                    }
                }
            }
            else 
                newChat = null;
        }
        public void LeaveFromChat(int clientId, int chatId, out bool result)
        {
            var chatMember = repositories.ChatMemberRepos.Get().Where(c => c.ClientId == clientId && c.ChatId == chatId).FirstOrDefault();
            if (chatMember != null)
            {
                result = true;
                foreach (var item in TakeClients(chatMember.Chat.Id))
                {
                    foreach (var item2 in callbacks)
                    {
                        if (item2.ClientId == item.Client.Id && item2.ClientId != clientId)
                            try
                            {
                                item2.Callback.Left(chatMember.Client.Id, chatMember.Chat.Id);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                    }
                }
                repositories.ChatMemberRepos.Delete(chatMember);
                repositories.Save();
            }
            else
                result = false;
        }
        public void InviteContact(int chatId, int contactId,out bool result)
        {
            var chat = repositories.ChatRepos.Get().FirstOrDefault(c => c.Id == chatId && !c.IsPM);
            var client = repositories.ClientRepos.Get().FirstOrDefault(c => c.Id == contactId);
            var res = chat.ChatMembers.FirstOrDefault(cm => cm.ClientId == contactId);
            if(chat!=null && client != null && !IsThereClientInChat(contactId, chatId) && chat.ChatMembers.Count() < chat.MaxUsers)
            {
                var chatMember = new ChatMember() { ClientId = contactId, ChatId = chat.Id, IsAdmin = false, DateLastReadMessage = DateTime.Now };
                repositories.ChatMemberRepos.Insert(chatMember);
                repositories.Save();
                var clientCallback=callbacks.FirstOrDefault(c => c.ClientId == contactId);
                if (clientCallback != null)
                    clientCallback.Callback.AddChatForContact(chatMapper.Map<ChatDTO>(chat), GetChatPhoto(chat.Id));

                foreach (var item in TakeClients(chat.Id))
                {
                    foreach (var item2 in callbacks)
                    {
                        if (item2.ClientId == item.Client.Id && item2.ClientId != contactId)
                            try
                            {
                                item2.Callback.Joined(chatMemberMapper.Map<ChatMemberDTO>(chatMember), chat.Id, GetPhoto(contactId));
                            }
                            catch (Exception)
                            { }
                    }
                }
                result = true;
            }
            else
            {
                result = false;
            }
        }
        public IEnumerable<ChatDTO> TakeChats(int clientId)
        {
            var result = repositories.ChatMemberRepos.Get().Where(c => c.ClientId == clientId).Select(c => c.Chat);
            return chatMapper.Map<IEnumerable<ChatDTO>>(result);
        }
        public IEnumerable<ChatMemberDTO> TakeClients(int chatId)
        {
            var result = repositories.ChatMemberRepos.Get().Where(c => c.ChatId == chatId);
            return chatMemberMapper.Map<IEnumerable<ChatMemberDTO>>(result);
        }
        #endregion
        //--------------------------Message Methods--------------------//
        #region
        private bool IsChatAndClientExist(int clientId, int chatId)
        {
            var client = repositories.ClientRepos.GetById(clientId);
            var chat = repositories.ChatRepos.GetById(chatId);
            if (chat != null && client != null)
                return true;
            return false;
        }
        public void SendMessage(int clientId, int chatId, MessageInfo message)
        {
            if (IsChatAndClientExist(clientId, chatId))
            {
                Message newMessage = new Message() { ClientId = clientId, ChatId = chatId, SendingTime = DateTime.Now, Text = message.Text };
                repositories.MessageRepos.Insert(newMessage);
                repositories.Save();
                foreach (var item in TakeClients(chatId))
                {
                    foreach (var item2 in callbacks)
                    {
                        if(item2.ClientId == item.Client.Id)
                            try
                            {
                                item2.Callback.TakeMessage(messageMapper.Map<MessageDTO>(newMessage));
                            }
                            catch (Exception)
                            {}
                    }
                }
            }
        }
        public IEnumerable<MessageDTO> TakeMessages(int chatId)
        {
            var result = repositories.MessageRepos.Get().Where(c => c.ChatId == chatId);
            return messageMapper.Map<IEnumerable<MessageDTO>>(result);
        }

        public bool IsMessageExist(int messageId)
        {
            var message = repositories.MessageRepos.Get().Where(m => m.Id == messageId).FirstOrDefault();
            if (message != null) return true;
            return false;
        }

        public void DeleteMessageForAll(int messageId, out bool result)
        {

            if (IsMessageExist(messageId))
            {
                int chatId = repositories.MessageRepos.Get().FirstOrDefault(c => c.Id == messageId).ChatId;
                repositories.MessageRepos.Delete(messageId);
                repositories.Save();
                result = true;
                foreach (var item in TakeClients(chatId))
                {
                    foreach (var item2 in callbacks)
                    {
                        if (item2.ClientId == item.Client.Id)
                            try
                            {
                                item2.Callback.RemoveMessageForAll(chatId,messageId);
                            }
                            catch (Exception)
                            { }
                    }
                }

            }
            else
                result = false;

        }
        #endregion
    }
    [DataContract]
    public class InfoFile
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public byte[] Data { get; set; }

    }
    [DataContract]
    public class MessageInfo
    {
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public InfoFile[] Files { get; set; }
    }
    internal class CallBack
    {
        public int ClientId { get; set; }
        public ICallback Callback { get; set; }
    }
}
