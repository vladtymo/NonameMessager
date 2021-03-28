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
        public bool IsChat { get; set; }

        // FOREIGN KEYS
        public int? ClientId { get; set; }
        public int? ChatId { get; set; }
        public int ProfileId { get; set; }

        // NAVIGATION PROPERTIES
        public virtual Client Client { get; set; }
        public virtual Chat Chat { get; set; }
        public virtual Profile Profile { get; set; }
        public virtual ICollection<Client> Clients { get; set; }
        public virtual ICollection<Chat> Chats { get; set; }
        public virtual ICollection<Message> MessagesClient { get; set; }
        public virtual ICollection<Message> MessagesChat { get; set; }

    }
}