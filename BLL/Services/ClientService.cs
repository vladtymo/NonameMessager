using AutoMapper;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public interface IClientService
    {
        IEnumerable<ClientDTO> GetAllClients();
        ClientDTO CreateNewClient(ClientDTO clientDTO);
        ClientDTO GetClient(AccountDTO accountDTO);
        bool SetProperties(ClientDTO clientDTO);
    }
    public class ClientService : IClientService
    {
        private IUnitOfWork repositories;
        private IMapper mapper;

        public ClientService()
        {
            this.repositories = new UnitOfWork();

            IConfigurationProvider config = new MapperConfiguration(
                cfg =>
                {
                    // Entity to DTO
                    cfg.CreateMap<Client, ClientDTO>();

                    cfg.CreateMap<Account, AccountDTO>().ForMember(dst => dst.Password, opt => opt.Ignore());

                    cfg.CreateMap<ClientDTO, Client>();

                    cfg.CreateMap<AccountDTO, Account>().ForMember(dst => dst.Password, opt => opt.MapFrom(src => ComputeSha256Hash(src.Password)));
                });

            mapper = new Mapper(config);
        }
        public IEnumerable<ClientDTO> GetAllClients()
        {
            var result = repositories.ClientRepos.Get(includeProperties: $"{nameof(Client.Account)}");
            return mapper.Map<IEnumerable<ClientDTO>>(result);
        }
        private Client GetClientByAccount(AccountDTO accountDTO)
        {
            var client = repositories.ClientRepos.Get().Where(c => c.Account.Email == accountDTO.Email || c.Account.Phone == accountDTO.Phone && c.Account.Password == ComputeSha256Hash(accountDTO.Password)).FirstOrDefault();
            if (client != null)
                return client;
            else
                return null;
        }
        private int IsExistClient(ClientDTO clientDTO)
        {
            var client = repositories.ClientRepos.Get().Where(c => c.UniqueName == clientDTO.UniqueName || c.Account.Email == clientDTO.Account.Email || c.Account.Phone == clientDTO.Account.Phone).FirstOrDefault();
            if (client == null)
                return -1;
            else
                return client.Id;
        }
        public ClientDTO CreateNewClient(ClientDTO clientDTO)
        {
            var id = IsExistClient(clientDTO);
            if (id == -1)
            {
                repositories.ClientRepos.Insert(mapper.Map<Client>(clientDTO));
                repositories.Save();
                repositories.ClientRepos.Get().LastOrDefault().AccountId = repositories.AccountRepos.Get().LastOrDefault().ClientId;
                repositories.Save();
                return mapper.Map<ClientDTO>(repositories.ClientRepos.Get().LastOrDefault());
            }
            else
                return null;
        }
        public ClientDTO GetClient(AccountDTO accountDTO)
        {
            var client = GetClientByAccount(accountDTO);
            if (client != null)
            {
                return mapper.Map<ClientDTO>(client);
            }
            else
                return null;
        }
        public bool SetProperties(ClientDTO clientDTO)
        {
            var id = IsExistClient(clientDTO);
            if (id != -1)
            {
                var client = repositories.ClientRepos.Get().Where(c => c.Id == id).FirstOrDefault();
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
    }
}
