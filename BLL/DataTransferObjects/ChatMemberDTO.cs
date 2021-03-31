using System;

namespace BLL
{
    public class ChatMemberDTO
    {
        public int Id { get; set; }
        public DateTime DateLastReadMessage { get; set; }
        public bool IsAdmin { get; set; }
        public int ChatId { get; set; }
        public int ClientId { get; set; }
        public ClientDTO Client { get; set; }
        public ChatDTO Chat { get; set; }
    }
}
