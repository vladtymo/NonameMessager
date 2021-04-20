using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface IChatMemberService
    {
        [OperationContract]
        void JoinToChat(int clientId, string chatUniqueName, bool isAdmin, out ChatDTO newChat);
        [OperationContract]
        void LeaveFromChat(int clientId, int chatId, out bool result);
        [OperationContract]
        IEnumerable<ChatDTO> TakeChats(int clientId);
        [OperationContract]
        IEnumerable<ClientDTO> TakeClients(int chatId);
    }
}
