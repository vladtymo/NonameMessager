using System.Runtime.Serialization;

namespace WcfService
{
    [DataContract]
    public class ClientDTO
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string UniqueName { get; set; }
        [DataMember]
        public string PhotoPath { get; set; }
        [DataMember]
        public int AccountId { get; set; }
        [DataMember]
        public virtual AccountDTO Account { get; set; }
    }
}
