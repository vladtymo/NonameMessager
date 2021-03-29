using System;

namespace BLL
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime SendingTime { get; set; }
        public int ClientId { get; set; }
        public int ChatId { get; set; }
        public ClientDTO Client { get; set; }
        public ChatDTO Chat { get; set; }
    }
}
