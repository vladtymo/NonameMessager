using System.ComponentModel.DataAnnotations;

namespace DAL
{
    public class File
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string FilePath { get; set; }

        // FOREIGN KEYS
        public int MessageId { get; set; }

        // NAVIGATION PROPERTIES
        public virtual Message Message { get; set; }
    }
}