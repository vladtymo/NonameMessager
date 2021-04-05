using AutoMapper;
using BLL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client
{
    class MainViewModel : ViewModelBase
    {
        #region Properties
        private IClientService clientService = new ClientService();
        private IChatService chatService = new ChatService();
        private IMapper mapper;

        private ClientViewModel currentClient;
        private ClientViewModel clientForChange;
        private ChatViewModel currentChat;


        private bool isOpenLoginRegistrationDialog;
        private bool isOpenProfileDialog;

        private bool isOpenContactsDialog;

        private bool isOpenInfoDialog;
        private string textForInfoDialog;
        private string password;


        public bool IsOpenLoginRegistrationDialog { get { return isOpenLoginRegistrationDialog; } set { SetProperty(ref isOpenLoginRegistrationDialog, value); } }
        public bool IsOpenProfileDialog { get { return isOpenProfileDialog; } set { SetProperty(ref isOpenProfileDialog, value); } }

        public bool IsOpenContactsDialog { get { return isOpenContactsDialog; } set { SetProperty(ref isOpenContactsDialog, value); } }


        public bool IsOpenInfoDialog { get { return isOpenInfoDialog; } set { SetProperty(ref isOpenInfoDialog, value); } }

        public string TextForInfoDialog { get { return textForInfoDialog; } set { SetProperty(ref textForInfoDialog, value); } }

        public string Password { get => password; set => SetProperty(ref password, value); }


        public ClientViewModel CurrentClient { get { return currentClient; } set { SetProperty(ref currentClient, value); } }
        public ClientViewModel ClientForChange { get { return clientForChange; } set { SetProperty(ref clientForChange, value); } }
        public ChatViewModel CurrentChat { get { return currentChat; } set { SetProperty(ref currentChat, value); } }
        #endregion

        public MainViewModel()
        {
            IConfigurationProvider config = new MapperConfiguration(
                cfg =>
                {
                    cfg.CreateMap<AccountDTO, AccountViewModel>();
                    cfg.CreateMap<ClientDTO, ClientViewModel>();
                    cfg.CreateMap<ChatDTO, ChatViewModel>();

                    cfg.CreateMap<AccountViewModel, AccountDTO>();
                    cfg.CreateMap<ClientViewModel, ClientDTO>();
                    cfg.CreateMap<ChatViewModel, ChatDTO>();
                });

            mapper = new Mapper(config);

            currentClient = new ClientViewModel() { Account = new AccountViewModel() };
            clientForChange = new ClientViewModel() { Account = new AccountViewModel() };
            PropertyChanged += (sender, args) =>
            {


                if (args.PropertyName == nameof(CurrentClient))
                {
                    ClientForChange = CurrentClient.Clone();

                }

            };

            loginCommand = new DelegateCommand(Login);
            signUpCommand = new DelegateCommand(SignUp);
            exitCommand = new DelegateCommand(Exit);
            setProfileCommand = new DelegateCommand(SetProfile);
            setProfileDialogOpenCommand = new DelegateCommand(ShowSetProfileDialog);
            contactsDialogOpenCommand = new DelegateCommand(ShowContactsDialog);

            IsOpenLoginRegistrationDialog = true;
        }



        public void Login()
        {
            CurrentClient.Account.Phone = CurrentClient.Account.Email;
            var result = mapper.Map<ClientViewModel>(clientService.GetClient(mapper.Map<AccountDTO>(CurrentClient.Account), this.Password));
            if (result != null)
            {
                CurrentClient = result;
                IsOpenLoginRegistrationDialog = false;
                OpenInfoDialog($"Login successful. Hi, {CurrentClient.Name}");

            }
            else
            {
                OpenInfoDialog($"It looks like our system does not have a user with enetered data. Please, try again.");

            }
        }

        public void SignUp()
        {

            var result = mapper.Map<ClientViewModel>(clientService.CreateNewClient(mapper.Map<ClientDTO>(CurrentClient), this.Password));
            if (result != null)
            {
                CurrentClient = result;
                IsOpenLoginRegistrationDialog = false;
                OpenInfoDialog($"Registration successful. Welcome, {CurrentClient.Name}");

            }
            else
            {
                OpenInfoDialog($"Registration problems. Try to change data.");

            }
        }
        public void Exit()
        {
            CurrentClient = new ClientViewModel() { Account = new AccountViewModel() };
            ClientForChange = new ClientViewModel() { Account = new AccountViewModel() };
            Password = String.Empty;
            IsOpenLoginRegistrationDialog = true;
        }

        public void SetProfile()
        {
            if (clientService.SetProperties(mapper.Map<ClientDTO>(ClientForChange)))
            {
                OpenInfoDialog($"Data changed successfully.");
                CurrentClient = ClientForChange.Clone();

            }
            else
            {
                OpenInfoDialog($"The data has not been changed.");

                ClientForChange = CurrentClient.Clone();
            }
        }
        public void CreateNewChat()
        {
            var result = mapper.Map<ChatViewModel>(chatService.CreateNewChat(mapper.Map<ChatDTO>(CurrentChat)));
            if (result != null)
            {
                CurrentChat = result;


            }
            else
            {
                OpenInfoDialog($"");

            }
        }
        public void OpenInfoDialog(string text)
        {
            TextForInfoDialog = text;
            IsOpenInfoDialog = true;
        }
        public void ShowSetProfileDialog()
        {
            ClientForChange = CurrentClient.Clone();

            IsOpenProfileDialog = true;
        }
        public void ShowContactsDialog()
        {

            IsOpenContactsDialog = true;
        }


        #region Commands
        private Command setProfileDialogOpenCommand;
        private Command contactsDialogOpenCommand;


        private Command loginCommand;
        private Command signUpCommand;
        private Command exitCommand;

        private Command setProfileCommand;

        public ICommand SetProfileDialogOpenCommand => setProfileDialogOpenCommand;
        public ICommand ContactsDialogOpenCommand => contactsDialogOpenCommand;


        public ICommand LoginCommand => loginCommand;
        public ICommand SignUpCommand => signUpCommand;
        public ICommand ExitCommand => exitCommand;

        public ICommand SetProfileCommand => setProfileCommand;


        #endregion

    }
}
