using System;
using System.Runtime.Serialization;

namespace WcfService
{
    [DataContract]
    public class ChatMemberDTO
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public DateTime DateLastReadMessage { get; set; }
        [DataMember]
        public bool IsAdmin { get; set; }
        [DataMember]
        public int ChatId { get; set; }
        [DataMember]
        public int ClientId { get; set; }
        [DataMember]
        public ClientDTO Client { get; set; }
        [DataMember]
        public ChatDTO Chat { get; set; }
    }
}
