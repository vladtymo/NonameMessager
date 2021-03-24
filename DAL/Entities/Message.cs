using System;
using System.Collections.Generic;

namespace DAL
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime SendingTime { get; set; }


        // FOREIGN KEYS
        public int ClientId { get; set; }
        public int ChatId { get; set; }

        // NAVIGATION PROPERTIES
        public virtual Identifier Client { get; set; }
        public virtual Identifier Chat { get; set; }
        public virtual ICollection<File> Files { get; set; }
    }
}