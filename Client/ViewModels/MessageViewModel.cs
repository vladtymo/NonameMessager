using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class MessageViewModel : ViewModelBase
    {
        private string text;
        private DateTime sendingTime;
        private ClientViewModel client;

        public int Id { get; set; }
        public bool IsMyMessage { get; set; }
        public string Text { get => text; set => SetProperty(ref text, value); }
        public DateTime SendingTime { get => sendingTime; set => SetProperty(ref sendingTime, value); }
        public ClientViewModel Client { get => client; set => SetProperty(ref client, value); }
    }
}
