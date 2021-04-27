using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface IChatService
    {
        [OperationContract]
        ChatDTO CreateNewChat(ChatDTO newChatDTO);

        [OperationContract]
        bool SetChatProperties(ChatDTO chatDTO);
        [OperationContract]
        void SetChatPhoto(int chatId, InfoFile info);
        [OperationContract]
        InfoFile GetChatPhoto(int chatId);
        [OperationContract]
        IEnumerable<ChatDTO> SearchChats(string uniqueName);
        [OperationContract]
        void CreatePMChat(int clientId, int companionId, out int chatId);
        [OperationContract]
        void DeleteChat(int chatId, out bool isRemoved);

    }
}
