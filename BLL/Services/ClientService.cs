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
        void CreateNewClient(ClientDTO clientDTO);
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
        public void CreateNewClient(ClientDTO clientDTO)
        {
            repositories.ClientRepos.Insert(mapper.Map<Client>(clientDTO));
            repositories.Save();
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
