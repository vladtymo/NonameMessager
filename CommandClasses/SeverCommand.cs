using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandClasses
{
    [Serializable]
    public class ServerCommand
    {
        public CommandType Type { get; set; }
        public CommandStatus Status { get; set; }
        public string Text { get; set; }
    }
    [Serializable]
    public class ServerCommandJoinChat : ServerCommand
    {
        //public ChatDTO Chat { get; set; }
    }
    [Serializable]
    public class ServerCommandChatSendMessage : ServerCommand
    {
        //public MessageDTO Message { get; set; }
    }
    [Serializable]
    public class ServerCommandLoginAndRegistration : ServerCommand
    {
        //public ClientDTO Client { get; set; }
    }
}
