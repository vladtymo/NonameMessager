using System.ServiceModel;

namespace WcfService
{
    public interface ICallback
    {
        [OperationContract(IsOneWay = true)]
        void TakeMessage(MessageDTO message, InfoFile photoClient);
        [OperationContract(IsOneWay = true)]
        void Joined(ClientDTO client, int chatId, InfoFile photo);
        [OperationContract(IsOneWay = true)]
        void Left(int clientId, int chatId);
        [OperationContract(IsOneWay = true)]
        void TakeChat(ChatDTO chat, InfoFile photo);
        [OperationContract(IsOneWay = true)]
        void DeleteChatForAll(int chatId);
        [OperationContract(IsOneWay = true)]
        void DeleteMessageForAll(int chatId, int messageId);
        [OperationContract(IsOneWay = true)]
        void AddContactInChat(ChatDTO chat, InfoFile photo);
    }
}
