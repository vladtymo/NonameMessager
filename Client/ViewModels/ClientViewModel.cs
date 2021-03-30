namespace Client
{
    public class ClientViewModel : ViewModelBase
    {
        private string name;
        private string uniqueName;
        private string photoPath;
        private AccountViewModel account;

        public  int Id { get; set; }

        public string Name { get => name; set => SetProperty(ref name, value); }
        public string UniqueName { get => uniqueName; set => SetProperty(ref uniqueName, value); }
        public string PhotoPath { get => photoPath; set => SetProperty(ref photoPath, value); }
        public AccountViewModel Account { get => account; set => SetProperty(ref account, value); }

        public ClientViewModel Clone()
        {
            return new ClientViewModel() { Account = new AccountViewModel() { Email = Account.Email, Password = Account.Password, Phone = Account.Phone, Id=Account.Id }, Name = Name, PhotoPath =PhotoPath, UniqueName = UniqueName, Id=Id };
        }
    }
}
