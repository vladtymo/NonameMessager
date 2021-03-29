using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL
{
    public class Client
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public string UniqueName { get; set; }
        public string PhotoPath { get; set; }

        // FOREIGN KEYS
        public int AccountId { get; set; }

        // NAVIGATION PROPERTIES
        public virtual Account Account { get; set; }
        public virtual ICollection<ChatMember> ChatMembers { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }

    }
}