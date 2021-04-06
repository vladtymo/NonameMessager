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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service : IChatService, IClientService, IContactService
    {
        private IUnitOfWork repositories;
        private IMapper clientMapper;
        private IMapper chatMapper;
        private IMapper contactMapper;

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

                    cfg.CreateMap<ChatDTO, Chat>();

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
            

        }
        //--------------------------Client Methods--------------------//
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
            var client = repositories.ClientRepos.Get().Where(c => c.UniqueName == clientDTO.UniqueName || c.Account.Email == clientDTO.Account.Email || c.Account.Phone == clientDTO.Account.Phone).FirstOrDefault();
            if (client == null)
                return -1;
            else
                return client.Id;
        }
        public ClientDTO CreateNewClient(ClientDTO clientDTO, string password)
        {
            var id = IsExistClient(clientDTO);
            if (id == -1)
            {
                clientDTO.Account.Password = password;
                repositories.ClientRepos.Insert(clientMapper.Map<Client>(clientDTO));
                repositories.Save();
                repositories.ClientRepos.Get().LastOrDefault().AccountId = repositories.AccountRepos.Get().LastOrDefault().ClientId;
                repositories.Save();
                return clientMapper.Map<ClientDTO>(repositories.ClientRepos.Get().LastOrDefault());
            }
            else
                return null;
        }
        public ClientDTO GetClient(AccountDTO accountDTO, string password)
        {
            var client = GetClientByAccount(accountDTO, password);
            if (client != null)
            {
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
      
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                fs.Write(info.Data, 0, info.Data.Length);
            }
            repositories.ClientRepos.Get().Where(c => c.Id == clientId).FirstOrDefault().PhotoPath=path;
            repositories.Save();
           
        }
        public void GetPathToPhoto(string pathToPhoto)
        {
            this.pathToPhoto = pathToPhoto;
        }
        public InfoFile GetPhoto(int clientId)
        {
            DirectoryInfo di = new DirectoryInfo(pathToPhoto);
            string path = repositories.ClientRepos.Get().Where(c => c.Id == clientId).Select(c => c.PhotoPath).FirstOrDefault();
            if (path == null) return null;
            var result = di.GetFiles().Where(f => f.FullName==path).FirstOrDefault();
            if (result == null)
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

        //--------------------------Contacts Methods--------------------//
        private bool IsContactExist(int clientID, string uniqueNameContact)
        {
            var contact = repositories.ContactRepos.Get().Where(c => c.ClientId == clientID && c.ContactClient.UniqueName == uniqueNameContact).FirstOrDefault();
            if (contact == null)
            {
                return false;
            }
            return true;
        }
        public ClientDTO AddContact(int clientID, string uniqueNameContact)
        {
            if (!IsContactExist(clientID, uniqueNameContact))
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
            var result = repositories.ContactRepos.Get(includeProperties: $"{nameof(Contact.ContactClient)}").Where(c => c.ClientId == clientId).Select(c => c.ContactClient);
            return contactMapper.Map<IEnumerable<ClientDTO>>(result);
        }

        //--------------------------Chat Methods--------------------//
        private int IsExistChat(ChatDTO chatDTO)
        {
            var chat = repositories.ChatRepos.Get().Where(ch => ch.UniqueName == chatDTO.UniqueName).FirstOrDefault();
            if (chat == null) return -1;
            else
                return chat.Id;
        }
        public ChatDTO CreateNewChat(ChatDTO newChatDTO)
        {
            var id = IsExistChat(newChatDTO);
            if (id == -1)
            {
                repositories.ChatRepos.Insert(chatMapper.Map<Chat>(newChatDTO));
                repositories.Save();
                return chatMapper.Map<ChatDTO>(repositories.ChatRepos.Get().LastOrDefault());
            }
            else
                return null;
        }

    }
    [DataContract]
    public class InfoFile
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public byte[] Data { get; set; }

    }
}
