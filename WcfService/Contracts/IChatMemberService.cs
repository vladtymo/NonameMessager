using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    [ServiceContract]
    public interface IChatMemberService
    {
        [OperationContract]
        ChatDTO JoinToChat(int clientId, string chatUniqueName, bool isAdmin);
        [OperationContract]
        IEnumerable<ChatDTO> TakeChats(int clientId);
    }
}
