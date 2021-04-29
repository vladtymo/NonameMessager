using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class ChatMemberViewModel : ViewModelBase
    {
        private DateTime dateLastReadMessage;
        private bool isAdmin;
        private int chatId;
        private int clientId;
        private ClientViewModel client;

        public int Id { get; set; }

        public DateTime DateLastReadMessage { get => dateLastReadMessage; set => SetProperty(ref dateLastReadMessage, value); }
        public bool IsAdmin { get => isAdmin; set => SetProperty(ref isAdmin, value); }
        public int ChatId { get => chatId; set => SetProperty(ref chatId, value); }
        public int ClientId { get => clientId; set => SetProperty(ref clientId, value); }
        public ClientViewModel Client { get => client; set => SetProperty(ref client, value); }


    }
}
