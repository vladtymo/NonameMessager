using System.Runtime.Serialization;

namespace WcfService
{
    [DataContract]
    public class FileDTO
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string FilePath { get; set; }
        [DataMember]
        public int MessageId { get; set; }
        [DataMember]
        public MessageDTO Message { get; set; }
    }
}
