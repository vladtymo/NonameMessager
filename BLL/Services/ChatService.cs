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
        ChatDTO CreateNewChat(ChatDTO newChatDTO);
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
                repositories.ChatRepos.Insert(mapper.Map<Chat>(newChatDTO));
                repositories.Save();
                return mapper.Map<ChatDTO>(repositories.ChatRepos.Get().LastOrDefault());
            }
            else
                return null;
        }
    }
}
