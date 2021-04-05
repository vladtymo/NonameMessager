using System.Runtime.Serialization;

namespace WcfService
{
    [DataContract]
    public class ChatDTO
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string PhotoPath { get; set; }
        [DataMember]
        public string UniqueName { get; set; }
        [DataMember]
        public bool IsPrivate { get; set; }
        [DataMember]
        public int MaxUsers { get; set; }
        [DataMember]
        public bool IsPM { get; set; }

    }
}
