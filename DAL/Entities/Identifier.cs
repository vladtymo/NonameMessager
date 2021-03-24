using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL
{
    public class Identifier
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        // FOREIGN KEYS
        public int? ClientId { get; set; }
        public int? ChatId { get; set; }
        public int MessageClientId { get; set; }
        public int MessageChatId { get; set; }

        // NAVIGATION PROPERTIES
        public virtual Client Client { get; set; }
        public virtual Chat Chat { get; set; }
        public virtual Message MessageClient { get; set; }
        public virtual Message MessageChat { get; set; }
        public virtual ICollection<Chat> Chats { get; set; }
        public virtual ICollection<Client> Clients { get; set; }

    }
}