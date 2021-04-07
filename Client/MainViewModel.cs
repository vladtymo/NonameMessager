using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Client.Properties;
using System.Windows.Input;
using Client.MessangerServices;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.IO;

namespace Client
{
    class MainViewModel : ViewModelBase
    {
        #region Collections

        private readonly ICollection<ClientViewModel> contacts = new ObservableCollection<ClientViewModel>();
        private readonly ICollection<ChatViewModel> chats = new ObservableCollection<ChatViewModel>();
        private readonly ICollection<Language> languages = new ObservableCollection<Language>();
        private readonly ICollection<MessageViewModel> chatMessages = new ObservableCollection<MessageViewModel>();

        public IEnumerable<ClientViewModel> Contacts => contacts;
        public IEnumerable<ChatViewModel> Chats => chats;
        public IEnumerable<Language> Languages => languages;
        public IEnumerable<MessageViewModel> ChatMessages => chatMessages;


        #endregion
        #region Properties
        private ClientServiceClient clientService = new ClientServiceClient();
        private ChatServiceClient chatService = new ChatServiceClient();
        private ContactServiceClient contactService = new ContactServiceClient();
        private ChatMemberServiceClient chatMemberService = new ChatMemberServiceClient();
        private MessageServiceClient messageService = new MessageServiceClient();

        private IMapper mapper;

        private ClientViewModel currentClient;
        private ClientViewModel clientForChange;
        private ChatViewModel chatForChange;
        private ChatViewModel selectedChat;


        private bool isOpenLoginRegistrationDialog;
        private bool isOpenProfileDialog;

        private bool isOpenContactsDialog;

        private bool isOpenAddEditChatDialog;

        private bool isOpenJoinToChatDialog;

        private bool isAddChatDialog;

        private bool isChatSelected;


        private bool isOpenInfoDialog;
        private string textForInfoDialog;

        private string password;
        private string textMessage;
        private string uniqueNameContact;
        private string uniqueNameChat;

        private string pathToPhoto;

        private Language selectedLanguage;

        public bool IsOpenLoginRegistrationDialog { get { return isOpenLoginRegistrationDialog; } set { SetProperty(ref isOpenLoginRegistrationDialog, value); } }
        public bool IsOpenProfileDialog { get { return isOpenProfileDialog; } set { SetProperty(ref isOpenProfileDialog, value); } }
        
        public bool IsOpenContactsDialog { get { return isOpenContactsDialog; } set { SetProperty(ref isOpenContactsDialog, value); } }

        public bool IsOpenAddEditChatDialog { get { return isOpenAddEditChatDialog; } set { SetProperty(ref isOpenAddEditChatDialog, value); } }

        public bool IsOpenInfoDialog { get { return isOpenInfoDialog; } set { SetProperty(ref isOpenInfoDialog, value); } }

        public bool IsOpenJoinToChatDialog { get { return isOpenJoinToChatDialog; } set { SetProperty(ref isOpenJoinToChatDialog, value); } }

        public bool IsAddChatDialog { get { return isAddChatDialog; } set { SetProperty(ref isAddChatDialog, value); } }

        public bool IsChatSelected { get { return isChatSelected; } set { SetProperty(ref isChatSelected, value); } }

        public string TextForInfoDialog { get { return textForInfoDialog; } set { SetProperty(ref textForInfoDialog, value); } }

        public string Password { get => password; set => SetProperty(ref password, value); }
        public string TextMessage { get => textMessage; set => SetProperty(ref textMessage, value); }
        public string UniqueNameContact { get => uniqueNameContact; set => SetProperty(ref uniqueNameContact, value); }
        public string UniqueNameChat { get => uniqueNameChat; set => SetProperty(ref uniqueNameChat, value); }


        public ClientViewModel CurrentClient { get { return currentClient; } set { SetProperty(ref currentClient, value); } }
        public ClientViewModel ClientForChange { get { return clientForChange; } set { SetProperty(ref clientForChange, value); } }
        public ChatViewModel ChatForChange { get { return chatForChange; } set { SetProperty(ref chatForChange, value); } }
        public ChatViewModel SelectedChat { get { return selectedChat; } set { SetProperty(ref selectedChat, value); } }

        public Language SelectedLanguage { get { return selectedLanguage; } set { SetProperty(ref selectedLanguage, value); } }

        #endregion

        public MainViewModel()
        {
            IConfigurationProvider config = new MapperConfiguration(
                cfg =>
                {
                    
                    cfg.CreateMap<AccountDTO, AccountViewModel>();
                    cfg.CreateMap<ClientDTO, ClientViewModel>();
                    cfg.CreateMap<ChatDTO, ChatViewModel>();
                    cfg.CreateMap<MessageDTO, MessageViewModel>();

                    cfg.CreateMap<AccountViewModel, AccountDTO>();
                    cfg.CreateMap<ClientViewModel, ClientDTO>();
                    cfg.CreateMap<ChatViewModel, ChatDTO>();
                    cfg.CreateMap<MessageViewModel, MessageDTO>();
                });

            mapper = new Mapper(config);

            currentClient = new ClientViewModel() { Account = new AccountViewModel() };
            clientForChange = new ClientViewModel() { Account = new AccountViewModel() };
            chatForChange = new ChatViewModel();           
            PropertyChanged += (sender, args) =>
            {


                if (args.PropertyName == nameof(CurrentClient))
                {
                    ClientForChange = CurrentClient.Clone();
                }
                if (args.PropertyName == nameof(SelectedLanguage))
                {
                    EditLanguage();

                }
                if (args.PropertyName == nameof(SelectedChat))
                {
                    if (SelectedChat == null)
                        IsChatSelected = false;
                    else
                        IsChatSelected = true;

                }

            };

            loginCommand = new DelegateCommand(Login);
            signUpCommand = new DelegateCommand(SignUp);
            exitCommand = new DelegateCommand(Exit);
            setProfileCommand = new DelegateCommand(SetProfile);
            setPhotoCommand = new DelegateCommand(SetPhoto);
            setProfileDialogOpenCommand = new DelegateCommand(ShowSetProfileDialog);
            contactsDialogOpenCommand = new DelegateCommand(ShowContactsDialog);
            joinToChatDialogOpenCommand = new DelegateCommand(ShowJoinToChatDialog);
            addContactCommand = new DelegateCommand(AddContact);
            deleteContactCommand = new DelegateCommand(DeleteContact);
            addChatCommand = new DelegateCommand(CreateNewChat);
            joinToChatCommand = new DelegateCommand(JoinToChat);
            chatAddDialogOpenCommand = new DelegateCommand(ShowAddChatDialog);
            IsOpenLoginRegistrationDialog = true;
            
            
            pathToPhoto = Path.Combine(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).FullName).FullName, "ClientsPhoto");

            clientService.GetPathToPhoto(Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).FullName).FullName).FullName, "WcfService", "ClientsPhoto"));
            InitializeLanguages();
            GetRegistry();


        }



        public void Login()
        {
            CurrentClient.Account.Phone = CurrentClient.Account.Email;
            var result = mapper.Map<ClientViewModel>(clientService.GetClient(mapper.Map<AccountDTO>(CurrentClient.Account), this.Password));
            if (result != null)
            {
                CurrentClient = result;
                IsOpenLoginRegistrationDialog = false;
                GetPhoto();
                foreach (var item in chatMemberService.TakeChats(currentClient.Id))
                {
                    chats.Add(mapper.Map<ChatViewModel>(item));
                }
                OpenInfoDialog($"Login successful. Hi, {CurrentClient.Name}");
                foreach (var item in contactService.TakeContacts(currentClient.Id))
                {
                    contacts.Add(mapper.Map<ClientViewModel>(item));
                }
                OpenInfoDialog(Resources.SuccessfulLoginString +$"{CurrentClient.Name}!");
            }
            else
            {
                OpenInfoDialog(Resources.FailedLoginString);

            }
        }

        public void SignUp()
        {

            var result = mapper.Map<ClientViewModel>(clientService.CreateNewClient(mapper.Map<ClientDTO>(CurrentClient), this.Password));
            if (result != null)
            {
                CurrentClient = result;
                IsOpenLoginRegistrationDialog = false;
                OpenInfoDialog(Resources.SuccessfulSignUpString + $"{CurrentClient.Name}!");

            }
            else
            {
                OpenInfoDialog(Resources.FailedSignUpString);

            }
        }
        
        public void GetPhoto()
        {
            InfoFile info = clientService.GetPhoto(CurrentClient.Id);
            if (info != null)
            {
                string path = FreePath(Path.Combine(pathToPhoto, clientForChange.Id.ToString() + Path.GetExtension(info.Name)));

                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(info.Data, 0, info.Data.Length);
                }
                CurrentClient.PhotoPath = Path.Combine(pathToPhoto, clientForChange.Id.ToString() + Path.GetExtension(info.Name));
            }
            
        }
        public void SetProfile()
        {
            if (clientService.SetProperties(mapper.Map<ClientDTO>(ClientForChange)))
            {
                if (CurrentClient.PhotoPath != ClientForChange.PhotoPath)
                {

                   
                    InfoFile info = new InfoFile() { Name = ClientForChange.PhotoPath };
                    using (FileStream fs = new FileStream(clientForChange.PhotoPath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] fileData = new byte[fs.Length];
                        fs.Read(fileData, 0, fileData.Length);
                        info.Data = fileData;
                    }
                    clientService.SetPhoto(clientForChange.Id, info);

                    string path =FreePath( Path.Combine( pathToPhoto, clientForChange.Id.ToString() + Path.GetExtension(info.Name)));
                    File.Copy(clientForChange.PhotoPath, path, true);
                    ClientForChange.PhotoPath = path;              
                }
                OpenInfoDialog(Resources.SuccessfulSetProfileString);
                CurrentClient = ClientForChange.Clone();

            }
            else
            {
                OpenInfoDialog(Resources.FailedSetProfileString);

                ClientForChange = CurrentClient.Clone();
            }
        }
    
        public void SetPhoto()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (ofd.ShowDialog() == true)
            {
                ClientForChange.PhotoPath = ofd.FileName;
            }
        }
        public void CreateNewChat()
        {
            var result = mapper.Map<ChatViewModel>(chatService.CreateNewChat(mapper.Map<ChatDTO>(ChatForChange)));
            if (result != null)
            {   
                chats.Add(result);
                SelectedChat = result;
                ChatForChange = new ChatViewModel();
                IsOpenAddEditChatDialog = false;
                chatMemberService.JoinToChat(CurrentClient.Id, result.UniqueName, true);
                OpenInfoDialog("Chat successsfully created.");
                OpenInfoDialog(Resources.SuccessfulCreateChatString);
            }
            else
            {
                OpenInfoDialog(Resources.FailedCreateChatString);
            }
        }
        public void JoinToChat()
        {
            var result = chatMemberService.JoinToChat(CurrentClient.Id, UniqueNameChat, true);
            if (result != null)
            {
                chats.Add(mapper.Map<ChatViewModel>(result));
                OpenInfoDialog("Join to Chat successsfully.");
            }
            else
            {
                OpenInfoDialog("Join to Chat failed.");
            }
        }

        public void AddContact()
        {
            var result = mapper.Map<ClientViewModel>(contactService.AddContact(CurrentClient.Id, UniqueNameContact));
            if (result != null)
            {
                contacts.Add(result);
                OpenInfoDialog(Resources.SuccessfulContactAddString);
            }
            else
            {
                OpenInfoDialog(Resources.FailedContactAddString);          
            }
        }
        public void DeleteContact()
        {
            if (contactService.DeleteContact(CurrentClient.Id, UniqueNameContact))
            {
                contacts.Remove(contacts.Where(c => c.UniqueName == UniqueNameContact).FirstOrDefault());
                OpenInfoDialog(Resources.SuccessfulContactDeleteString);

            }
            else
            {
                OpenInfoDialog(Resources.FailedContactDeletedString);
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
        public void ShowAddChatDialog()
        {
            IsAddChatDialog = true;
            IsOpenAddEditChatDialog = true;
        }
        public void ShowJoinToChatDialog()
        {
            IsOpenJoinToChatDialog = true;
        }
        public void Exit()
        {
            CurrentClient = new ClientViewModel() { Account = new AccountViewModel() };
            ClientForChange = new ClientViewModel() { Account = new AccountViewModel() };
            Password = String.Empty;
            IsOpenLoginRegistrationDialog = true;
            contacts.Clear();
            chats.Clear();
        }
        public string FreePath(string path)
        {
            int index = 0;
            string tmp;
            do
            {
                tmp = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + (index == 0 ? null : $"({index})") + Path.GetExtension(path));
                index++;
            } while (System.IO.File.Exists(tmp));
            return tmp;
        }

        public void EditLanguage()
        {
            Properties.ResourceService.Current.ChangeCulture(SelectedLanguage.Culture);
            EditRegistry();
        }
        private void EditRegistry()
        {
            RegistryKey currentUserKey = Registry.CurrentUser;
            RegistryKey key = currentUserKey.CreateSubKey("NonameMessangerSettings");
            key.SetValue("Language", SelectedLanguage.Culture, RegistryValueKind.String);
            key.Close();
        }
        private void CreateRegistry()
        {
            RegistryKey key = Registry.CurrentUser;

            if (key.OpenSubKey("NonameMessangerSettings") == null)
            {
                key.CreateSubKey("NonameMessangerSettings");
            }
            key.Close();
        }
        private void GetRegistry()
        {
            CreateRegistry();
            string culture = (string)Registry.GetValue(@"HKEY_CURRENT_USER\NonameMessangerSettings", "Language", null);
            var result = languages.Where(l => l.Culture == culture).FirstOrDefault();
            if (result != null)
                SelectedLanguage = result;
        }
        public void InitializeLanguages()
        {
            languages.Add(new Language() { Name = "English", Culture = "en-US" });
            languages.Add(new Language() { Name = "Русский", Culture = "ru-RU" });

        }

        #region Commands
        private Command setProfileDialogOpenCommand;
        private Command contactsDialogOpenCommand;
        private Command chatAddDialogOpenCommand;
        private Command joinToChatDialogOpenCommand;


        private Command loginCommand;
        private Command signUpCommand;
        private Command exitCommand;

        private Command setProfileCommand;
        private Command setPhotoCommand;

        private Command addContactCommand;
        private Command deleteContactCommand;

        private Command addChatCommand;
        private Command joinToChatCommand;

        private Command sendMessageCommand;


        public ICommand SetProfileDialogOpenCommand => setProfileDialogOpenCommand;
        public ICommand ContactsDialogOpenCommand => contactsDialogOpenCommand;
        public ICommand ChatAddDialogOpenCommand => chatAddDialogOpenCommand;
        public ICommand JoinToChatDialogOpenCommand => joinToChatDialogOpenCommand;


        public ICommand LoginCommand => loginCommand;
        public ICommand SignUpCommand => signUpCommand;
        public ICommand ExitCommand => exitCommand;

        public ICommand SetProfileCommand => setProfileCommand;
        public ICommand SetPhotoCommand => setPhotoCommand;

        public ICommand AddContactCommand => addContactCommand;
        public ICommand DeleteContactCommand => deleteContactCommand;

        public ICommand AddChatCommand => addChatCommand;
        public ICommand JoinToChatCommand => joinToChatCommand;

        public ICommand SendMessageCommand => sendMessageCommand;

        #endregion

    }
    class Language : ViewModelBase
    {
        private string name;
        public string Name { get { return name; } set { SetProperty(ref name, value); } }

        private string culture;
        public string Culture { get { return culture; } set { SetProperty(ref culture, value); } }
    }
}
namespace Client.Properties
{
    using System.Globalization;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;


    public class ResourceService : INotifyPropertyChanged
    {
        #region singleton members

        private static readonly ResourceService _current = new ResourceService();
        public static ResourceService Current
        {
            get { return _current; }
        }
        #endregion

        readonly Resources _resources = new Resources();

        public Resources Resources
        {
            get { return this._resources; }
        }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public void ChangeCulture(string name)
        {
            Resources.Culture = CultureInfo.GetCultureInfo(name);
            this.RaisePropertyChanged("Resources");
        }

    }
}
