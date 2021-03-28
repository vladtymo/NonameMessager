using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL
{
    public class Chat
    {
        public int Id { get; set; }
        public bool IsPrivate { get; set; }
        public int? MaxUsers { get; set; }
        public bool IsPM { get; set; }

        // FOREIGN KEYS
        public int ChatInfoId { get; set; }

        // NAVIGATION PROPERTIES
        public virtual Identifier ChatInfo { get; set; }
        public virtual ICollection<Identifier> Clients { get; set; }
    }
}