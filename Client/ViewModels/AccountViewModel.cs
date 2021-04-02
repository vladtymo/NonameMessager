namespace Client
{
    public class AccountViewModel : ViewModelBase
    {
        private string email;
        private string phone;
        public int Id { get; set; }

        public string Email { get => email; set => SetProperty(ref email, value); }
        public string Phone { get => phone; set => SetProperty(ref phone, value); }
    }
}
