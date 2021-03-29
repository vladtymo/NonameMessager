using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL
{
    public class Chat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UniqueName { get; set; }
        public bool IsPrivate { get; set; }
        public int MaxUsers { get; set; }
        public bool IsPM { get; set; }


        // NAVIGATION PROPERTIES
        public virtual ICollection<ChatMember> ChatMembers { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}