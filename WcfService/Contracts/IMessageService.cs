using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    [ServiceContract]
    public interface IMessageService
    {
        [OperationContract]
        MessageDTO SendMessage(int clientId, int chatId, MessageInfo message);
        [OperationContract]
        IEnumerable<MessageDTO> TakeMessages(int chatId);
    }
}
