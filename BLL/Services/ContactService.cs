using AutoMapper;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public interface IContactService
    {
        ClientDTO AddContact(int clientID, string uniqueNameContact);
        bool DeleteContact(int clientID, string uniqueNameContact);
    }
    public class ContactService : IContactService
    {
        private IUnitOfWork repositories;
        private IMapper mapper;

        public ContactService()
        {
            this.repositories = new UnitOfWork();

            IConfigurationProvider config = new MapperConfiguration(
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

            mapper = new Mapper(config);
        }
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
                return mapper.Map<ClientDTO>(contactClient);
            }
            return null;
        }

        public bool DeleteContact(int clientID, string uniqueNameContact)
        {
            if (IsContactExist(clientID, uniqueNameContact))
            {
                var contactClient = repositories.ContactRepos.Get().Where(c => c.ClientId == clientID && c.ContactClient.UniqueName == uniqueNameContact).FirstOrDefault();
                repositories.ContactRepos.Delete(contactClient);
                return true;
            }
            return false;
        }

    }
}
