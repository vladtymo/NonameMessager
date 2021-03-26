using System.ComponentModel.DataAnnotations;

namespace DAL
{
    public class Account
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MaxLength(50)]
        [Phone]
        public string Phone { get; set; }
        [Required]
        [MaxLength(100)]
        public string Password { get; set; }

        // FOREIGN KEYS
        public int ClientId { get; set; }

        // NAVIGATION PROPERTIES
        public virtual Client Client { get; set; }

    }
}