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
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Client
{
    class MainViewModel : ViewModelBase, IMessageServiceCallback, IClientServiceCallback
    {
        #region Collections

        private readonly ICollection<ClientViewModel> contacts = new ObservableCollection<ClientViewModel>();
        private readonly ICollection<ClientViewModel> members = new ObservableCollection<ClientViewModel>();
        private readonly ICollection<ChatViewModel> chats = new ObservableCollection<ChatViewModel>();
        private readonly ICollection<Language> languages = new ObservableCollection<Language>();
        private readonly ICollection<MessageViewModel> chatMessages = new ObservableCollection<MessageViewModel>();

        public IEnumerable<ClientViewModel> Contacts => contacts;
        public IEnumerable<ClientViewModel> Members => members;
        public IEnumerable<ChatViewModel> Chats => chats;
        public IEnumerable<Language> Languages => languages;
        public IEnumerable<MessageViewModel> ChatMessages => chatMessages;



        #endregion
        #region Properties
        private ClientServiceClient clientService;
        private ChatServiceClient chatService = new ChatServiceClient();
        private ContactServiceClient contactService = new ContactServiceClient();
        private ChatMemberServiceClient chatMemberService = new ChatMemberServiceClient();
        private MessageServiceClient messageService;

        private IMapper mapper;

        private ClientViewModel currentClient;
        private ClientViewModel clientForChange;
        private ChatViewModel chatForChange;
        private ChatViewModel selectedChat;
        private ClientViewModel opponentClient;




        private bool isOpenLoginRegistrationDialog;
        private bool isOpenProfileDialog;

        private bool isOpenContactsDialog;
        private bool isOpenAddEditChatDialog;

        private bool isOpenJoinToChatDialog;
        private bool isOpenChatInfoDialog;

        private bool isAddChatDialog;

        private bool isChatSelected;


        private bool isOpenInfoDialog;
        private bool isOpenProfileInfoDialog;

        private string textForInfoDialog;

        private string password;
        private string textMessage;
        private string uniqueNameContact;
        private string uniqueNameChat;

        private int countMembers;

        private Language selectedLanguage;

        public bool IsOpenLoginRegistrationDialog { get { return isOpenLoginRegistrationDialog; } set { SetProperty(ref isOpenLoginRegistrationDialog, value); } }
        public bool IsOpenProfileDialog { get { return isOpenProfileDialog; } set { SetProperty(ref isOpenProfileDialog, value); } }
        
        public bool IsOpenContactsDialog { get { return isOpenContactsDialog; } set { SetProperty(ref isOpenContactsDialog, value); } }
        public bool IsOpenAddEditChatDialog { get { return isOpenAddEditChatDialog; } set { SetProperty(ref isOpenAddEditChatDialog, value); } }
        public bool IsOpenJoinToChatDialog { get { return isOpenJoinToChatDialog; } set { SetProperty(ref isOpenJoinToChatDialog, value); } }
        public bool IsOpenChatInfoDialog { get { return isOpenChatInfoDialog; } set { SetProperty(ref isOpenChatInfoDialog, value); } }

        public bool IsOpenInfoDialog { get { return isOpenInfoDialog; } set { SetProperty(ref isOpenInfoDialog, value); } }
        public bool IsOpenProfileInfoDialog { get { return isOpenProfileInfoDialog; } set { SetProperty(ref isOpenProfileInfoDialog, value); } }




        public bool IsAddChatDialog { get { return isAddChatDialog; } set { SetProperty(ref isAddChatDialog, value); } }

        public bool IsChatSelected { get { return isChatSelected; } set { SetProperty(ref isChatSelected, value); } }

        public string TextForInfoDialog { get { return textForInfoDialog; } set { SetProperty(ref textForInfoDialog, value); } }

        public string Password { get => password; set => SetProperty(ref password, value); }
        public string TextMessage { get => textMessage; set => SetProperty(ref textMessage, value); }
        public string UniqueNameContact { get => uniqueNameContact; set => SetProperty(ref uniqueNameContact, value); }
        public string UniqueNameChat { get => uniqueNameChat; set => SetProperty(ref uniqueNameChat, value); }
        public int CountMembers { get => countMembers; set => SetProperty(ref countMembers, value); }



        public ClientViewModel CurrentClient { get { return currentClient; } set { SetProperty(ref currentClient, value); } }
        public ClientViewModel ClientForChange { get { return clientForChange; } set { SetProperty(ref clientForChange, value); } }
        public ChatViewModel ChatForChange { get { return chatForChange; } set { SetProperty(ref chatForChange, value); } }
        public ChatViewModel SelectedChat { get { return selectedChat; } set { SetProperty(ref selectedChat, value); } }
        public ClientViewModel OpponentClient { get { return opponentClient; } set { SetProperty(ref opponentClient, value); } }



        public Language SelectedLanguage { get { return selectedLanguage; } set { SetProperty(ref selectedLanguage, value); } }

        #endregion

        public MainViewModel()
        {
            messageService = new MessageServiceClient(new InstanceContext(this));
            clientService = new ClientServiceClient(new InstanceContext(this));

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
                else if (args.PropertyName == nameof(SelectedLanguage))
                {
                    EditLanguage();

                }
                else if (args.PropertyName == nameof(SelectedChat))
                {
                    if (SelectedChat == null)
                        IsChatSelected = false;
                    else
                    {
                        TakeMessages();
                        TakeMembers();                      
                        IsChatSelected = true;
                    }
                }
                else if(args.PropertyName == nameof(UniqueNameContact))
                {
                    addContactCommand.RaiseCanExecuteChanged();
                    deleteContactCommand.RaiseCanExecuteChanged();

                }
                else if (args.PropertyName == nameof(UniqueNameChat))
                {
                    joinToChatCommand.RaiseCanExecuteChanged();
                }
                else if (args.PropertyName == nameof(TextMessage))
                {
                    sendMessageCommand.RaiseCanExecuteChanged();
                }
                else if (args.PropertyName==nameof(IsOpenContactsDialog))
                {
                    if(IsOpenContactsDialog == true)
                        UniqueNameContact = String.Empty;
                }
                else if (args.PropertyName == nameof(IsOpenJoinToChatDialog))
                {
                    if (IsOpenJoinToChatDialog == true)
                        UniqueNameChat = String.Empty;
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
            addContactCommand = new DelegateCommand(AddContact, () => !string.IsNullOrEmpty(UniqueNameContact));
            deleteContactCommand = new DelegateCommand(DeleteContact, () => !string.IsNullOrEmpty(UniqueNameContact));
            addChatCommand = new DelegateCommand(CreateNewChat);
            joinToChatCommand = new DelegateCommand(JoinToChat, ()=> !string.IsNullOrEmpty(UniqueNameChat));
            chatAddDialogOpenCommand = new DelegateCommand(ShowAddChatDialog);
            sendMessageCommand = new DelegateCommand(SendMessage, ()=>!string.IsNullOrEmpty(TextMessage));
            closedCommand = new DelegateCommand(Disconnect);
            leaveFromChatCommand = new DelegateCommand(LeaveFromChat);
            chatInfoDialogOpenCommand = new DelegateCommand(ShowChatInfo);
            setChatPropertiesCommand = new DelegateCommand(SetChatProperties);
            manageChatDialogOpenCommand = new DelegateCommand(ShowEditChatDialog);
            setChatPhotoCommand = new DelegateCommand(SetChatPhoto);
            profileInfoDialogOpenCommand = new DelegateCommand(ShowProfileInfo);
            IsOpenLoginRegistrationDialog = true;


            clientService.GetPathToPhotoAsync(Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).FullName).FullName).FullName, "WcfService"));
            InitializeLanguages();
            GetRegistry();
        }


        public void Login()
        {
            if(string.IsNullOrEmpty(CurrentClient.Account.Email)||string.IsNullOrEmpty(Password))
            {
                OpenInfoDialog(Resources.EmptyFieldsString);
                return;
            }
            CurrentClient.Account.Phone = CurrentClient.Account.Email;
            var result = mapper.Map<ClientViewModel>(clientService.GetClientAsync(mapper.Map<AccountDTO>(CurrentClient.Account), this.Password).Result);
            if (result != null)
            {
                CurrentClient = result;
                IsOpenLoginRegistrationDialog = false;
                GetPhoto();
                foreach (var item in mapper.Map<IEnumerable<ChatViewModel>>(chatMemberService.TakeChatsAsync(currentClient.Id).Result))
                {
                    chats.Add(item);
                    GetChatPhoto(item);
                }
                foreach (var item in mapper.Map<IEnumerable<ClientViewModel>>(contactService.TakeContactsAsync(currentClient.Id).Result))
                {
                    contacts.Add(item);
                    GetPhoto(item);
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
            if (string.IsNullOrEmpty(CurrentClient.Account.Email) || string.IsNullOrEmpty(CurrentClient.Account.Phone) || string.IsNullOrEmpty(CurrentClient.UniqueName) || string.IsNullOrEmpty(CurrentClient.Name) || string.IsNullOrEmpty(Password))
            {
                OpenInfoDialog(Resources.EmptyFieldsString);
                return;
            }
            var result = mapper.Map<ClientViewModel>(clientService.CreateNewClientAsync(mapper.Map<ClientDTO>(CurrentClient), this.Password).Result);
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
            InfoFile info = clientService.GetPhotoAsync(CurrentClient.Id).Result;
            if (info != null)
            {
                CurrentClient.Photo = ToImage(info.Data);
            }
            
        }
        public void GetPhoto(ClientViewModel client)
        {
            InfoFile info = clientService.GetPhotoAsync(client.Id).Result;
            if (info != null)
            {
                client.Photo = ToImage(info.Data);
            }

        }
        private BitmapImage ToImage(byte[] array)
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
        public void SetProfile()
        {
            if (string.IsNullOrEmpty(ClientForChange.Account.Email) || string.IsNullOrEmpty(ClientForChange.Account.Phone) || string.IsNullOrEmpty(ClientForChange.UniqueName) || string.IsNullOrEmpty(ClientForChange.Name))
            {
                OpenInfoDialog(Resources.EmptyFieldsString);
                return;
            }
            if (clientService.SetPropertiesAsync(mapper.Map<ClientDTO>(ClientForChange)).Result)
            {
                if (CurrentClient.Photo != ClientForChange.Photo)
                {
                    InfoFile info = new InfoFile() { Name = ClientForChange.PhotoPath };
                    using (FileStream fs = new FileStream(clientForChange.PhotoPath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] fileData = new byte[fs.Length];
                        fs.Read(fileData, 0, fileData.Length);
                        info.Data = fileData;
                    }
                    clientService.SetPhotoAsync(clientForChange.Id, info);

                    ClientForChange.Photo = ToImage(info.Data);            
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
                using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
                {
                    byte[] fileData = new byte[fs.Length];
                    fs.Read(fileData, 0, fileData.Length);
                    ClientForChange.Photo = ToImage(fileData);
                    ClientForChange.PhotoPath = ofd.FileName;
                }
            }
        }
        public void SetChatPhoto()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (ofd.ShowDialog() == true)
            {
                using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
                {
                    byte[] fileData = new byte[fs.Length];
                    fs.Read(fileData, 0, fileData.Length);
                    ChatForChange.Photo = ToImage(fileData);
                    ChatForChange.PhotoPath = ofd.FileName;
                }
            }
        }
        public void CreateNewChat()
        {
            if (string.IsNullOrEmpty(ChatForChange.Name) || string.IsNullOrEmpty(ChatForChange.UniqueName) || ChatForChange.MaxUsers < 1)
            {
                OpenInfoDialog(Resources.EmptyFieldsString);
                return;
            }
            var result = mapper.Map<ChatViewModel>(chatService.CreateNewChatAsync(mapper.Map<ChatDTO>(ChatForChange)).Result);
            if (result != null)
            {   
                chatMemberService.JoinToChatAsync(CurrentClient.Id, result.UniqueName, true);
                chats.Add(result);
                SelectedChat = result;
                if (ChatForChange.Photo != null)
                {
                    InfoFile info = new InfoFile() { Name = ChatForChange.PhotoPath };
                    using (FileStream fs = new FileStream(ChatForChange.PhotoPath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] fileData = new byte[fs.Length];
                        fs.Read(fileData, 0, fileData.Length);
                        info.Data = fileData;
                    }
                    chatService.SetChatPhotoAsync(SelectedChat.Id, info);
                    SelectedChat.Photo = ChatForChange.Photo;
                }
                ChatForChange = new ChatViewModel();
                IsOpenAddEditChatDialog = false;
                OpenInfoDialog(Resources.SuccessfulCreateChatString);
            }
            else
            {
                OpenInfoDialog(Resources.FailedCreateChatString);
            }
        }
        public void JoinToChat()
        {
            var result = mapper.Map<ChatViewModel>(chatMemberService.JoinToChatAsync(CurrentClient.Id, UniqueNameChat, true).Result);
            if (result != null)
            {
                GetChatPhoto(result);
                chats.Add(result);
                OpenInfoDialog(Resources.SuccessfulJoinToChatString);
                IsOpenJoinToChatDialog = false;
            }
            else
            {
                OpenInfoDialog(Resources.FailedJoinToChatString);
            }
        }
        public void SetChatProperties()
        {
            if (chatService.SetChatPropertiesAsync(mapper.Map<ChatDTO>(ChatForChange)).Result)
            {
                if (SelectedChat.Photo != ChatForChange.Photo)
                {
                    InfoFile info = new InfoFile() { Name = ChatForChange.PhotoPath };
                    using (FileStream fs = new FileStream(ChatForChange.PhotoPath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] fileData = new byte[fs.Length];
                        fs.Read(fileData, 0, fileData.Length);
                        info.Data = fileData;
                    }
                    chatService.SetChatPhotoAsync(SelectedChat.Id, info);

                    ChatForChange.Photo = ToImage(info.Data);
                }

                SelectedChat.Name = ChatForChange.Name;
                SelectedChat.UniqueName = ChatForChange.UniqueName;
                SelectedChat.MaxUsers = ChatForChange.MaxUsers;
                SelectedChat.IsPrivate = ChatForChange.IsPrivate;
                SelectedChat.IsPM=ChatForChange.IsPM;
                SelectedChat.Photo = ChatForChange.Photo;
                SelectedChat.PhotoPath = ChatForChange.PhotoPath;
                OpenInfoDialog(Resources.SuccessfulSetChatPropertiesString);
            }
            else
            {
                OpenInfoDialog(Resources.FailedSetChatPropertiesString);
                ChatForChange = SelectedChat.Clone();
            }
        }
        public void GetChatPhoto(ChatViewModel chat)
        {
            InfoFile info = chatService.GetChatPhotoAsync(chat.Id).Result;
            if (info != null)
            {
                chat.Photo = ToImage(info.Data);
            }

        }
        public void LeaveFromChat()
        {
            var result = chatMemberService.LeaveFromChatAsync(CurrentClient.Id, SelectedChat.Id).Result;
            if (result == true)
            {
                chats.Remove(SelectedChat);
                OpenInfoDialog(Resources.SuccessfulLeaveFromChatString);
            }
            else
            {
                OpenInfoDialog(Resources.FailedLeaveFromChatString);
            }
        }
        public void TakeMessages()
        {
            chatMessages.Clear();
            var result = mapper.Map<IEnumerable<MessageViewModel>>(messageService.TakeMessagesAsync(SelectedChat.Id).Result);

            Task.Run(() =>
            {
                foreach (var item in result)
                {
                    Application.Current.Dispatcher.Invoke(() => { chatMessages.Add(item); });
                }
            });
        }
        public void TakeMembers()
        {
            members.Clear();
            var result = mapper.Map<IEnumerable<ClientViewModel>>(chatMemberService.TakeClientsAsync(SelectedChat.Id).Result);
            Task.Run(() =>
            {
                foreach (var item in result)
                {
                    Application.Current.Dispatcher.Invoke(() => { members.Add(item); });
                }
            }).ContinueWith((res) =>
            {
                CountMembers = members.Count;
                if (SelectedChat.IsPM) 
                    TakeOpponent();

            });

        }


        public void TakeOpponent()
        {
            var opponent = members.Where(w => w.Id != CurrentClient.Id).FirstOrDefault();
            OpponentClient = opponent;


        }
        public void AddContact()
        {
            var result = mapper.Map<ClientViewModel>(contactService.AddContactAsync(CurrentClient.Id, UniqueNameContact).Result);
            if (result != null)
            {
                contacts.Add(result);
                GetPhoto(result);
                OpenInfoDialog(Resources.SuccessfulContactAddString);
            }
            else
            {
                OpenInfoDialog(Resources.FailedContactAddString);          
            }
        }
        public void DeleteContact()
        {
            if (contactService.DeleteContactAsync(CurrentClient.Id, UniqueNameContact).Result)
            {
                contacts.Remove(contacts.Where(c => c.UniqueName == UniqueNameContact).FirstOrDefault());
                OpenInfoDialog(Resources.SuccessfulContactDeleteString);

            }
            else
            {
                OpenInfoDialog(Resources.FailedContactDeletedString);
            }
        }
        public void SendMessage()
        {
            messageService.SendMessageAsync(CurrentClient.Id, SelectedChat.Id, new MessageInfo() { Text = TextMessage });
            TextMessage = String.Empty;
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
            ChatForChange = new ChatViewModel();
            IsOpenAddEditChatDialog = true;
        }
        public void ShowJoinToChatDialog()
        {
            IsOpenJoinToChatDialog = true;
        }
        public void ShowChatInfo()
        {
            IsOpenChatInfoDialog = true;
        }
        public void ShowEditChatDialog()
        {
            ChatForChange = SelectedChat.Clone();
            IsAddChatDialog = false;
            IsOpenAddEditChatDialog = true;
        }

        public void Disconnect()
        {
            clientService.DisconnectAsync();
        }
        public void Exit()
        {
            clientService.DisconnectAsync();
            CurrentClient = new ClientViewModel() { Account = new AccountViewModel() };
            ClientForChange = new ClientViewModel() { Account = new AccountViewModel() };
            Password = String.Empty;
            IsOpenLoginRegistrationDialog = true;
            contacts.Clear();
            chats.Clear();

            Password = String.Empty;
            IsOpenAddEditChatDialog = false;
            IsOpenContactsDialog = false;
            IsOpenJoinToChatDialog = false;
            IsOpenProfileDialog = false;
            IsOpenLoginRegistrationDialog = true;
            TextMessage = String.Empty;
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

        public void TakeMessage(MessageDTO message)
        {
            if (SelectedChat.Id == message.ChatId)
                chatMessages.Add(mapper.Map<MessageViewModel>(message));
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

        public void ShowProfileInfo()
        {
            IsOpenProfileInfoDialog = true;
        }










        #region Commands
        private Command setProfileDialogOpenCommand;
        private Command contactsDialogOpenCommand;
        private Command chatAddDialogOpenCommand;
        private Command joinToChatDialogOpenCommand;
        private Command chatInfoDialogOpenCommand;
        private Command manageChatDialogOpenCommand;
        private Command profileInfoDialogOpenCommand;



        private Command loginCommand;
        private Command signUpCommand;
        private Command exitCommand;
        private Command closedCommand;

        private Command setProfileCommand;
        private Command setPhotoCommand;

        private Command addContactCommand;
        private Command deleteContactCommand;

        private Command addChatCommand;
        private Command setChatPhotoCommand;
        private Command setChatPropertiesCommand;

        private Command joinToChatCommand;
        private Command leaveFromChatCommand;

        private Command sendMessageCommand;



        public ICommand SetProfileDialogOpenCommand => setProfileDialogOpenCommand;
        public ICommand ContactsDialogOpenCommand => contactsDialogOpenCommand;
        public ICommand ChatAddDialogOpenCommand => chatAddDialogOpenCommand;
        public ICommand JoinToChatDialogOpenCommand => joinToChatDialogOpenCommand;
        public ICommand ChatInfoDialogOpenCommand => chatInfoDialogOpenCommand;
        public ICommand ManageChatDialogOpenCommand => manageChatDialogOpenCommand;
        public ICommand ProfileInfoDialogOpenCommand => profileInfoDialogOpenCommand;



        public ICommand LoginCommand => loginCommand;
        public ICommand SignUpCommand => signUpCommand;
        public ICommand ExitCommand => exitCommand;
        public ICommand ClosedCommand => closedCommand;

        public ICommand SetProfileCommand => setProfileCommand;
        public ICommand SetPhotoCommand => setPhotoCommand;

        public ICommand AddContactCommand => addContactCommand;
        public ICommand DeleteContactCommand => deleteContactCommand;

        public ICommand AddChatCommand => addChatCommand;
        public ICommand SetChatPhotoCommand => setChatPhotoCommand;
        public ICommand JoinToChatCommand => joinToChatCommand;
        public ICommand SetChatPropertiesCommand => setChatPropertiesCommand;

        public ICommand LeaveFromChatCommand => leaveFromChatCommand;


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
