using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL
{
    public class Account
    {
        [ForeignKey(nameof(Client)),Key]
        public int ClientId { get; set; }
        [Required]
        [MaxLength(50)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MaxLength(50)]
        [Phone]
        public string Phone { get; set; }
        [Required]
        [MaxLength(200)]
        public string Password { get; set; }

        // NAVIGATION PROPERTIES
        public virtual Client Client { get; set; }

    }
}