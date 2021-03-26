using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL
{
    public class Profile
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }


        // FOREIGN KEYS
        public int IdentifierId { get; set; }
        public int? FileId { get; set; }

        // NAVIGATION PROPERTIES
        public virtual File File { get; set; }
        public virtual Identifier Identifier { get; set; }

    }
}