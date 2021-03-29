namespace BLL
{
    public class ClientDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UniqueName { get; set; }
        public string PhotoPath { get; set; }
        public int AccountId { get; set; }
        public virtual AccountDTO Account { get; set; }
    }
}
