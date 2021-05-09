using Client.Properties;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media.Imaging;

namespace Client
{
    public class ClientViewModel : ViewModelBase
    {
        private string name;
        private string uniqueName;
        private string photoPath;
        private BitmapImage photo;
        private AccountViewModel account;

        public  int Id { get; set; }
        [Required(ErrorMessageResourceType =typeof(Resources) ,ErrorMessageResourceName = "ErrorMessageRequiredClientName")]
        public string Name { get => name; set => SetProperty(ref name, value); }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorMessageRequiredClientUniqueName")]
        public string UniqueName { get => uniqueName; set => SetProperty(ref uniqueName, value); }
        public string PhotoPath { get => photoPath; set => SetProperty(ref photoPath, value); }
        public BitmapImage Photo { get => photo; set => SetProperty(ref photo, value); }
        public AccountViewModel Account { get => account; set => SetProperty(ref account, value); }

        public ClientViewModel Clone()
        {
            return new ClientViewModel() { Account = new AccountViewModel() { Email = Account.Email, Phone = Account.Phone, Id = Account.Id }, Name = Name, PhotoPath = PhotoPath, UniqueName = UniqueName, Id = Id, Photo=Photo };
        }
    }
}
