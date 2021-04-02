namespace BLL
{
    public class ChatDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhotoPath { get; set; }
        public string UniqueName { get; set; }
        public bool IsPrivate { get; set; }
        public int MaxUsers { get; set; }
        public bool IsPM { get; set; }

    }
}
