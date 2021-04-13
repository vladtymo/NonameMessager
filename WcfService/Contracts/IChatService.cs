using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    [ServiceContract]
    public interface IChatService
    {
        [OperationContract]
        ChatDTO CreateNewChat(ChatDTO newChatDTO);

        [OperationContract]
        bool SetChatProperties(ChatDTO chatDTO);
    }
}
