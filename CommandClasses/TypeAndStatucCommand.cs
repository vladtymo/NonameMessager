using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandClasses
{
    public enum CommandType
    {
        LOGIN,
        REGISTRATION,
        EXIT,
        CONNECT,
        DISCONNECT,
        ADDFRIEND,
        DELETEFRIEND,
        JOINTOCHAT,
        LEAVEFROMCHAT,
        CREATECHAT,
        CHANGECLIENT,
        CHANGECHAT,
        SENDMESSAGE,
    }
    public enum CommandStatus
    {
        SUCCESS,
        FAIL,
    }
}
