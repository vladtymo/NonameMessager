namespace BLL
{
    public class FileDTO
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public int MessageId { get; set; }
        public MessageDTO Message { get; set; }
    }
}
