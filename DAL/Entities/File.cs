using System.ComponentModel.DataAnnotations;

namespace DAL
{
    public class File
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public byte[] Data { get; set; }

        // FOREIGN KEYS
        public int? MessageId { get; set; }
        public int ProfileId { get; set; }

        // NAVIGATION PROPERTIES
        public virtual Message Message { get; set; }
        public virtual Profile Profile { get; set; }
    }
}