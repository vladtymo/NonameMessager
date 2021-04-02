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
    }
}
