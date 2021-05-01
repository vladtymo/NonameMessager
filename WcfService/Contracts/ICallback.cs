using System.ServiceModel;

namespace WcfService
{
    public interface ICallback
    {
        [OperationContract(IsOneWay = true)]
        void TakeMessage(MessageDTO message);
        [OperationContract(IsOneWay = true)]
        void Joined(ChatMemberDTO chatMember, int chatId, InfoFile photo);
        [OperationContract(IsOneWay = true)]
        void Left(int clientId, int chatId);
        [OperationContract(IsOneWay = true)]
        void TakeChat(ChatDTO chat, InfoFile photo);
        [OperationContract(IsOneWay = true)]
        void DeleteChatForAll(int chatId);
        [OperationContract(IsOneWay = true)]
        void RemoveMessageForAll(int chatId, int messageId);
        [OperationContract(IsOneWay = true)]
        void AddChatForContact(ChatDTO chat, InfoFile photo);
        [OperationContract(IsOneWay = true)]
        void GetNewClientProperties(ClientDTO client);
        [OperationContract(IsOneWay = true)]
        void GetNewClientPhoto(int clientId, InfoFile photo);
        [OperationContract(IsOneWay = true)]
        void SetNewPMChatProperties(ChatDTO chat);
        [OperationContract(IsOneWay = true)]
        void GetNewChatProperties(ChatDTO chat);
        [OperationContract(IsOneWay = true)]
        void GetNewChatPhoto(int chatId, InfoFile photo);
    }
}
