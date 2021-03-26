using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL
{
    public class Client
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        // FOREIGN KEYS
        public int IdentifierId { get; set; }
        public int AccountId { get; set; }

        // NAVIGATION PROPERTIES
        public virtual Identifier Identifier { get; set; }
        public virtual Account Account { get; set; }
        public virtual ICollection<Identifier> Contacts { get; set; }
    }
}