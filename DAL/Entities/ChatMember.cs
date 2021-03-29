using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ChatMember
    {
        public int Id { get; set; }
        public DateTime DateLastReadMessage { get; set; }
        public bool IsAdmin { get; set; }

        // FOREIGN KEYS
        public int ChatId { get; set; }
        public int ClientId { get; set; }

        // NAVIGATION PROPERTIES
        public virtual Client Client { get; set; }
        public virtual Chat Chat { get; set; }
    }
}
