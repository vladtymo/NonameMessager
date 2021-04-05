using System.Runtime.Serialization;

namespace WcfService
{
    [DataContract]
    public class ContactDTO
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int ClientId { get; set; }
        [DataMember]
        public int ContactClientId { get; set; }
        [DataMember]
        public ClientDTO Client { get; set; }
        [DataMember]
        public ClientDTO ContactClient { get; set; }
    }
}
