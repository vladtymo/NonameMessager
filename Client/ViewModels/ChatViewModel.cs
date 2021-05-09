using Client.Properties;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace Client
{
    public class ChatViewModel : ViewModelBase
    {
        private string name;
        private string photoPath;
        private BitmapImage photo;
        private string uniqueName;
        private bool isPrivate;
        private string maxUsersString;
        private int? maxUsers;
        private bool isPM;

        private readonly Regex int_regex = new Regex(@"^[1-9]\d*$");
        public bool IsTextAllowedInt(string text)
        {
            return int_regex.IsMatch(text);
        }

        public int Id { get; set; }
        [Required(ErrorMessageResourceType =typeof(Resources), ErrorMessageResourceName = "ErrorMessageRequiredChatName") ]
        public string Name { get => name; set => SetProperty(ref name, value); }
        public string PhotoPath { get => photoPath; set => SetProperty(ref photoPath, value); }
        public BitmapImage Photo { get => photo; set => SetProperty(ref photo, value); }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorMessageRequiredChatUniqueName")]
        public string UniqueName { get => uniqueName; set => SetProperty(ref uniqueName, value); }
        public bool IsPrivate { get => isPrivate; set => SetProperty(ref isPrivate, value); }
        public int? MaxUsers { get => maxUsers; set { SetProperty(ref maxUsers, value); SetProperty(ref maxUsersString, value.ToString()); } }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorMessageRequiredChatMaxUsers")]
        [Range(1,2000, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorMessageInvalidChatMaxUsersNumber")]
        public string MaxUsersString { get => maxUsersString; set { if (!IsTextAllowedInt(value)) { SetProperty(ref maxUsersString, null); SetProperty(ref maxUsers, null); } else { SetProperty(ref maxUsersString, value); SetProperty(ref maxUsers,int.Parse(value)); } } }
        public bool IsPM { get => isPM; set => SetProperty(ref isPM, value); }
        public ChatViewModel Clone()
        {
            return new ChatViewModel() { Name = Name, PhotoPath = PhotoPath, UniqueName = UniqueName, Id = Id,IsPM=IsPM, IsPrivate=IsPrivate, MaxUsers=MaxUsers, Photo=Photo};
        }
    }
}
