using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandClasses
{
    [Serializable]
    public class ClientCommand
    {
        public CommandType Type { get; set; }
    }
    [Serializable]
    public class ClientCommandLogin : ClientCommand
    {
        //public AccountDTO Account { get; set; }
    }
    [Serializable]
    public class ClientCommandRegistration : ClientCommand
    {
        //public ClientDTO Client { get; set; }
    }
    [Serializable]
    public class ClientCommandCreateOrChangeChat : ClientCommand
    {
        //public ChatDTO Chat { get; set; }
    }
    [Serializable]
    public class ClientCommandSendMessage : ClientCommand
    {
        //public MessageDTO Message { get; set; }
    }
    [Serializable]
    public class ClientCommandJoinOrLeaveChat : ClientCommand
    {
        public string UniqueNameUser { get; set; }
        public string UniqueNameChat { get; set; }
    }
    [Serializable]
    public class ClientCommandAddOrDeleteFriend : ClientCommand
    {
        public string UniqueName { get; set; }
        public string UniqueNameFriend { get; set; }
    }
}
