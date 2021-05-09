using Client.Properties;
using System.ComponentModel.DataAnnotations;

namespace Client
{
    public class AccountViewModel : ViewModelBase
    {
        private string email;
        private string phone;
        public int Id { get; set; }
        [Required(ErrorMessageResourceType =typeof(Resources),ErrorMessageResourceName = "ErrorMessageRequiredEmail")]
        [RegularExpression(@"^[\w.-]{3,}\@[0-9a-zA-Z]+\.[0-9a-zA-Z]+$",
           ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorMessageInvalidEmailFormat")]
        public string Email { get => email; set => SetProperty(ref email, value); }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorMessageRequiredPhone")]
        [RegularExpression(@"\(?\+[0-9]{1,3}\)? ?-?[0-9]{1,3} ?-?[0-9]{3,5} ?-?[0-9]{4}( ?-?[0-9]{3})? ?(\w{1,10}\s?\d{1,6})?",
          ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorMessageInvalidPhoneFormat")]
        public string Phone { get => phone; set => SetProperty(ref phone, value); }

    }
}

