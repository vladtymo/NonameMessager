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

        private string pathToPhoto;
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
        }
        private bool IsExistUniqueName(string uniqueName)
        {
            var client = repositories.ClientRepos.Get().Where(c => c.UniqueName == uniqueName).FirstOrDefault();
            var chat = repositories.ChatRepos.Get().Where(c => c.UniqueName == uniqueName).FirstOrDefault();
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
        public ClientDTO CreateNewClient(ClientDTO clientDTO, string password)
        {
            var id = IsExistClient(clientDTO);
            if (id == -1 && !IsExistUniqueName(clientDTO.UniqueName))
            {
                clientDTO.Account.Password = password;
                repositories.ClientRepos.Insert(clientMapper.Map<Client>(clientDTO));
                repositories.Save();
                repositories.ClientRepos.Get().FirstOrDefault(c=>c.UniqueName==clientDTO.UniqueName).AccountId = repositories.AccountRepos.Get().FirstOrDefault(a => a.Email == clientDTO.Account.Email).ClientId;
                repositories.Save();
                ClientDTO addedClient = clientMapper.Map<ClientDTO>(repositories.ClientRepos.Get().FirstOrDefault(c => c.UniqueName == clientDTO.UniqueName));
                callbacks.Add(new CallBack() { ClientId = addedClient.Id, Callback = OperationContext.Current.GetCallbackChannel<ICallback>()});
                return addedClient;
            }
            else
                return null;
        }
        public ClientDTO GetClient(AccountDTO accountDTO, string password)
        {
            var client = GetClientByAccount(accountDTO, password);
            if (client != null)
            {
                DirectoryInfo directory = new DirectoryInfo(pathToPhoto);
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
            string path = FreePath(Path.Combine(pathToPhoto, clientId.ToString() + Path.GetExtension(info.Name)));
            DirectoryInfo directory = new DirectoryInfo(pathToPhoto);
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
        }
        public void GetPathToPhoto(string pathToPhoto)
        {
            this.pathToPhoto = pathToPhoto;
        }
        public InfoFile GetPhoto(int clientId)
        {
            DirectoryInfo di = new DirectoryInfo(pathToPhoto);
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
        public bool SetProperties(ClientDTO clientDTO)
        {
            var id = IsNotExistClient(clientDTO);
            if (id == -1)
            {
                var client = repositories.ClientRepos.Get().Where(c => c.Id == clientDTO.Id).FirstOrDefault();
                client.Name = clientDTO.Name;
                client.UniqueName = clientDTO.UniqueName;
                client.Account.Email = clientDTO.Account.Email;
                client.Account.Phone = clientDTO.Account.Phone;
                repositories.ClientRepos.Update(client);
                repositories.Save();
                return true;
            }
            else
                return false;
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
        private bool IsContactExist(int clientID, string uniqueNameContact)
        {
            var contact = repositories.ContactRepos.Get().Where(c => c.ClientId == clientID && c.ContactClient.UniqueName == uniqueNameContact).FirstOrDefault();
            if (contact == null)
            {
                return false;
            }
            return true;
        }
        private bool IsClientExist(string uniqueName)
        {
            var client = repositories.ClientRepos.Get().Where(c => c.UniqueName == uniqueName).FirstOrDefault();
            if (client != null)
                return true;
            return false;
        }
        public ClientDTO AddContact(int clientID, string uniqueNameContact)
        {
            if (!IsContactExist(clientID, uniqueNameContact) && IsClientExist(uniqueNameContact))
            {
                var contactClient = repositories.ClientRepos.Get().Where(c => c.UniqueName == uniqueNameContact).FirstOrDefault();
                repositories.ContactRepos.Insert(new Contact() { ClientId = clientID, ContactClientId = contactClient.Id });
                repositories.Save();
                return contactMapper.Map<ClientDTO>(contactClient);
            }
            return null;
        }
        public bool DeleteContact(int clientID, string uniqueNameContact)
        {
            if (IsContactExist(clientID, uniqueNameContact))
            {
                var contactClient = repositories.ContactRepos.Get().Where(c => c.ClientId == clientID && c.ContactClient.UniqueName == uniqueNameContact).FirstOrDefault();
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
        #endregion
        //--------------------------Chat Methods--------------------//
        #region
        public ChatDTO CreateNewChat(ChatDTO newChatDTO)
        {
            if (!IsExistUniqueName(newChatDTO.UniqueName))
            {
                repositories.ChatRepos.Insert(chatMapper.Map<Chat>(newChatDTO));
                repositories.Save();
                return chatMapper.Map<ChatDTO>(repositories.ChatRepos.Get().LastOrDefault());
            }
            else
                return null;
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
        public ChatDTO JoinToChat(int clientId, string chatUniqueName, bool isAdmin)
        {
            var chat = repositories.ChatRepos.Get().Where(c => c.UniqueName == chatUniqueName).FirstOrDefault();
            if (!IsThereClientInChat(clientId, chat.Id) && IsChatExist(chatUniqueName))
            {
                repositories.ChatMemberRepos.Insert(new ChatMember() { ClientId = clientId, ChatId = chat.Id, IsAdmin = isAdmin, DateLastReadMessage = DateTime.Now });
                repositories.Save();
                return chatMapper.Map<ChatDTO>(repositories.ChatRepos.Get().Where(c => c.UniqueName == chatUniqueName).FirstOrDefault());
            }
            return null;
        }
        public bool LeaveFromChat(int clientId, int chatId)
        {
            var chatMember = repositories.ChatMemberRepos.Get().Where(c => c.ClientId == clientId && c.ChatId == chatId).FirstOrDefault();
            if (chatMember != null)
            {
                repositories.ChatMemberRepos.Delete(chatMember);
                repositories.Save();
                return true;
            }
            return false;
        }
        public IEnumerable<ChatDTO> TakeChats(int clientId)
        {
            var result = repositories.ChatMemberRepos.Get().Where(c => c.ClientId == clientId).Select(c => c.Chat);
            return chatMapper.Map<IEnumerable<ChatDTO>>(result);
        }
        public IEnumerable<ClientDTO> TakeClients(int chatId)
        {
            var result = repositories.ChatMemberRepos.Get().Where(c => c.ChatId == chatId).Select(c => c.Client);
            return clientMapper.Map<IEnumerable<ClientDTO>>(result);
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
                        if(item2.ClientId == item.Id)
                            try
                            {
                                item2.Callback.TakeMessage(messageMapper.Map<MessageDTO>(newMessage));
                            }
                            catch (Exception)
                            {}
                    }
                }
            }
            else
                callbacks.Where(c => c.ClientId == clientId).FirstOrDefault().Callback.TakeMessage(null);
        }
        public IEnumerable<MessageDTO> TakeMessages(int chatId)
        {
            var result = repositories.MessageRepos.Get().Where(c => c.ChatId == chatId);
            return messageMapper.Map<IEnumerable<MessageDTO>>(result);
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
