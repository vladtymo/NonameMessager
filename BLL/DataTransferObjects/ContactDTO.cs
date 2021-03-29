namespace BLL
{
    public class ContactDTO
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ContactClientId { get; set; }
        public ClientDTO Client { get; set; }
        public ClientDTO ContactClient { get; set; }
    }
}
