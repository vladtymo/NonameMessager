using System.Runtime.Serialization;

namespace WcfService
{
    [DataContract]
    public class AccountDTO
    {
        [DataMember]
        public int ClientId { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public string Password { get; set; }
    }
}
