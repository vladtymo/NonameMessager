using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface IMessageService
    {
        [OperationContract(IsOneWay = true)]
        void SendMessage(int clientId, int chatId, MessageInfo message);
        [OperationContract]
        IEnumerable<MessageDTO> TakeMessages(int chatId);
    }
    public interface ICallback
    {
        [OperationContract(IsOneWay = true)]
        void TakeMessage(MessageDTO message, InfoFile photoClient);
    }
}
