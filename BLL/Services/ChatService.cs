using AutoMapper;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public interface IChatService
    {
    }
    public class ChatService : IChatService
    {
        private IUnitOfWork repositories;
        private IMapper mapper;

        public ChatService()
        {
            this.repositories = new UnitOfWork();

            IConfigurationProvider config = new MapperConfiguration(
                cfg =>
                {
                    // Entity to DTO
                    cfg.CreateMap<Chat, ChatDTO>();

                    cfg.CreateMap<ChatDTO, Chat>();

                });

            mapper = new Mapper(config);
        }
    }
}
