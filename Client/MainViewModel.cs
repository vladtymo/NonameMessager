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
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Client
{
    class MainViewModel : ViewModelBase, IMessageServiceCallback, IClientServiceCallback,IChatMemberServiceCallback,IChatServiceCallback,IContactServiceCallback
    {
        #region Collections

        private readonly ICollection<ClientViewModel> contacts = new ObservableCollection<ClientViewModel>();
        private readonly ICollection<ClientViewModel> allContacts = new ObservableCollection<ClientViewModel>();
        private readonly ICollection<ClientViewModel> clientsForAdd = new ObservableCollection<ClientViewModel>();

        private readonly ICollection<ChatMemberViewModel> members = new ObservableCollection<ChatMemberViewModel>();
        private readonly ICollection<ChatViewModel> chats = new ObservableCollection<ChatViewModel>();
        private readonly ICollection<ChatViewModel> joinToChatChats = new ObservableCollection<ChatViewModel>();

        private readonly ICollection<ChatViewModel> allChats = new ObservableCollection<ChatViewModel>();
        private readonly ICollection<Language> languages = new ObservableCollection<Language>();
        private readonly ICollection<MessageViewModel> chatMessages = new ObservableCollection<MessageViewModel>();

        public IEnumerable<ClientViewModel> Contacts => contacts;
        public IEnumerable<ClientViewModel> AllContacts => allContacts;
        public IEnumerable<ClientViewModel> ClientsForAdd => clientsForAdd;


        public IEnumerable<ChatMemberViewModel> Members => members;
        public IEnumerable<ChatViewModel> Chats => chats;
        public IEnumerable<ChatViewModel> AllChats => allChats;
        public IEnumerable<ChatViewModel> JoinToChatChats => joinToChatChats;

        public IEnumerable<Language> Languages => languages;
        public IEnumerable<MessageViewModel> ChatMessages => chatMessages;



        #endregion
        #region Properties
        private ClientServiceClient clientService;
        private ChatServiceClient chatService;
        private ContactServiceClient contactService;
        private ChatMemberServiceClient chatMemberService;
        private MessageServiceClient messageService;

        private IMapper mapper;

        private ClientViewModel currentClient;
        private ClientViewModel clientForChange;
        private ChatViewModel chatForChange;
        private ChatViewModel selectedChat;
        private ClientViewModel opponentClient;

        private ChatViewModel selectedChatForJoin;

        private ClientViewModel selectedContact;
        private ClientViewModel selectedClientForAdd;

        private ClientViewModel selectedClientForInvite;
        private MessageViewModel selectedMessage;




        private bool isOpenLoginRegistrationDialog;
        private bool isOpenProfileDialog;

        private bool isOpenContactsDialog;
        private bool isOpenAddEditChatDialog;

        private bool isOpenJoinToChatDialog;
        private bool isOpenChatInfoDialog;

        private bool isOpenAddMembersForChatDialog;

        private bool isAddChatDialog;

        private bool isChatSelected;


        private bool isOpenInfoDialog;
        private bool isOpenProfileInfoDialog;

        private string textForInfoDialog;

        private string password;
        private string textMessage;
        private string uniqueNameContact;
        private string uniqueNameChat;
        private string uniqueNameContactForInvite;

        private int countMembers;

        private string searchChatUniqueName;
      


        private Language selectedLanguage;



        public bool IsOpenLoginRegistrationDialog { get { return isOpenLoginRegistrationDialog; } set { SetProperty(ref isOpenLoginRegistrationDialog, value); } }
        public bool IsOpenProfileDialog { get { return isOpenProfileDialog; } set { SetProperty(ref isOpenProfileDialog, value); } }
        
        public bool IsOpenContactsDialog { get { return isOpenContactsDialog; } set { SetProperty(ref isOpenContactsDialog, value); } }
        public bool IsOpenAddEditChatDialog { get { return isOpenAddEditChatDialog; } set { SetProperty(ref isOpenAddEditChatDialog, value); } }
        public bool IsOpenJoinToChatDialog { get { return isOpenJoinToChatDialog; } set { SetProperty(ref isOpenJoinToChatDialog, value); } }
        public bool IsOpenChatInfoDialog { get { return isOpenChatInfoDialog; } set { SetProperty(ref isOpenChatInfoDialog, value); } }

        public bool IsOpenInfoDialog { get { return isOpenInfoDialog; } set { SetProperty(ref isOpenInfoDialog, value); } }
        public bool IsOpenProfileInfoDialog { get { return isOpenProfileInfoDialog; } set { SetProperty(ref isOpenProfileInfoDialog, value); } }

        public bool IsOpenAddMembersForChatDialog { get { return isOpenAddMembersForChatDialog; } set { SetProperty(ref isOpenAddMembersForChatDialog, value); } }



        public bool IsAddChatDialog { get { return isAddChatDialog; } set { SetProperty(ref isAddChatDialog, value); } }

        public bool IsChatSelected { get { return isChatSelected; } set { SetProperty(ref isChatSelected, value); } }

        public string TextForInfoDialog { get { return textForInfoDialog; } set { SetProperty(ref textForInfoDialog, value); } }

        private static readonly Regex passordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&_-])[A-Za-z\d@$!%*?&_-]{8,}$");

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorMessageRequiredPassword")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&_-])[A-Za-z\d@$!%*?&_-]{8,}$",
         ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorMessageInvalidPasswordFormat")]
        public string Password { get => password; set => SetProperty(ref password, value); }
        public string TextMessage { get => textMessage; set => SetProperty(ref textMessage, value); }
        public string UniqueNameContact { get => uniqueNameContact; set => SetProperty(ref uniqueNameContact, value); }
        public string UniqueNameContactForInvite { get => uniqueNameContactForInvite; set => SetProperty(ref uniqueNameContactForInvite, value); }

        public string UniqueNameChat { get => uniqueNameChat; set => SetProperty(ref uniqueNameChat, value); }
        public int CountMembers { get => countMembers; set => SetProperty(ref countMembers, value); }

        
        
        private bool isCorrectSignUpData;
        public bool IsCorrectSignUpData { get { return isCorrectSignUpData; } set { SetProperty(ref isCorrectSignUpData, value); } }

        private bool isCorrectUserInfo;
        public bool IsCorrectUserInfo { get { return isCorrectUserInfo; } set { SetProperty(ref isCorrectUserInfo, value); } }

        private bool isCorrectChatData;
        public bool IsCorrectChatData { get { return isCorrectChatData; } set { SetProperty(ref isCorrectChatData, value); } }




        public ClientViewModel CurrentClient { get { return currentClient; } set { SetProperty(ref currentClient, value); } }
        public ClientViewModel ClientForChange { get { return clientForChange; } set { SetProperty(ref clientForChange, value); } }
        public ChatViewModel ChatForChange { get { return chatForChange; } set { SetProperty(ref chatForChange, value); } }
        public ChatViewModel SelectedChat { get { return selectedChat; } set { SetProperty(ref selectedChat, value); } }
        public ClientViewModel OpponentClient { get { return opponentClient; } set { SetProperty(ref opponentClient, value); } }

        public string SearchChatUniqueName { get => searchChatUniqueName; set => SetProperty(ref searchChatUniqueName, value); }

        public ChatViewModel SelectedChatForJoin { get { return selectedChatForJoin; } set { SetProperty(ref selectedChatForJoin, value); } }
        public ClientViewModel SelectedContact { get { return selectedContact; } set { SetProperty(ref selectedContact, value); } }
        public ClientViewModel SelectedClientForAdd { get { return selectedClientForAdd; } set { SetProperty(ref selectedClientForAdd, value); } }

        public ClientViewModel SelectedClientForInvite { get { return selectedClientForInvite; } set { SetProperty(ref selectedClientForInvite, value); } }
        public MessageViewModel SelectedMessage { get { return selectedMessage; } set { SetProperty(ref selectedMessage, value); } }


        public Language SelectedLanguage { get { return selectedLanguage; } set { SetProperty(ref selectedLanguage, value); } }



        #endregion
        #region Commands
        private Command setProfileDialogOpenCommand;
        private Command contactsDialogOpenCommand;
        private Command chatAddDialogOpenCommand;
        private Command joinToChatDialogOpenCommand;
        private Command chatInfoDialogOpenCommand;
        private Command manageChatDialogOpenCommand;
        private Command profileInfoDialogOpenCommand;
        private Command inviteContactsDialogOpenCommand;

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
        private Command createOrSelectPMChatCommand;
        private Command deleteChatCommand;

        private Command inviteContactCommand;
        private Command joinToChatCommand;
        private Command leaveFromChatCommand;

        private Command sendMessageCommand;
        private Command deleteMessageForAllCommand;

        private Command iValidateSignUpCommand;

        private Command iValidateUserInfoCommand;
        private Command iValidateChatInfoCommand;
        


        public ICommand SetProfileDialogOpenCommand => setProfileDialogOpenCommand;
        public ICommand ContactsDialogOpenCommand => contactsDialogOpenCommand;
        public ICommand ChatAddDialogOpenCommand => chatAddDialogOpenCommand;
        public ICommand JoinToChatDialogOpenCommand => joinToChatDialogOpenCommand;
        public ICommand ChatInfoDialogOpenCommand => chatInfoDialogOpenCommand;
        public ICommand ManageChatDialogOpenCommand => manageChatDialogOpenCommand;
        public ICommand ProfileInfoDialogOpenCommand => profileInfoDialogOpenCommand;
        public ICommand InviteContactsDialogOpenCommand => inviteContactsDialogOpenCommand;


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
        public ICommand CreateOrSelectPMChatCommand => createOrSelectPMChatCommand;
        public ICommand DeleteChatCommand => deleteChatCommand;
        public ICommand LeaveFromChatCommand => leaveFromChatCommand;

        public ICommand InviteContactCommand => inviteContactCommand;


        public ICommand SendMessageCommand => sendMessageCommand;
        public ICommand DeleteMessageForAllCommand => deleteMessageForAllCommand;


        public ICommand IValidateSignUpCommand => iValidateSignUpCommand;
        public ICommand IValidateUserInfoCommand => iValidateUserInfoCommand;
        public ICommand IValidateChatInfoCommand => iValidateChatInfoCommand;


        #endregion
        public MainViewModel()
        {
            messageService = new MessageServiceClient(new InstanceContext(this));
            clientService = new ClientServiceClient(new InstanceContext(this));
            chatMemberService = new ChatMemberServiceClient(new InstanceContext(this));
            chatService = new ChatServiceClient(new InstanceContext(this));
            contactService = new ContactServiceClient(new InstanceContext(this));

            IConfigurationProvider config = new MapperConfiguration(
                cfg =>
                {
                    
                    cfg.CreateMap<AccountDTO, AccountViewModel>();
                    cfg.CreateMap<ClientDTO, ClientViewModel>();
                    cfg.CreateMap<ChatDTO, ChatViewModel>();
                    cfg.CreateMap<MessageDTO, MessageViewModel>();
                    cfg.CreateMap<ChatMemberDTO, ChatMemberViewModel>();

                    cfg.CreateMap<AccountViewModel, AccountDTO>();
                    cfg.CreateMap<ClientViewModel, ClientDTO>();
                    cfg.CreateMap<ChatViewModel, ChatDTO>();
                    cfg.CreateMap<MessageViewModel, MessageDTO>();
                    cfg.CreateMap<ChatMemberViewModel, ChatMemberDTO>();
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
                        TakeMembers();                      
                        TakeMessages();
                        IsChatSelected = true;
                    }
                    deleteChatCommand.RaiseCanExecuteChanged();
                    inviteContactsDialogOpenCommand.RaiseCanExecuteChanged();
                }
                else if(args.PropertyName == nameof(UniqueNameContact))
                {
                    if (String.IsNullOrEmpty(UniqueNameContact))
                        clientsForAdd.Clear();
                    else
                    SearchClients();

                    SearchContacts();
           

                }
                else if (args.PropertyName == nameof(UniqueNameContactForInvite))
                {
                    SearchContactsForInvite();
                }
                else if (args.PropertyName == nameof(UniqueNameChat))
                {
                    if (String.IsNullOrEmpty(UniqueNameChat))
                        joinToChatChats.Clear();
                    else 
                    SearchToJoinChats();
                  
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
                else if (args.PropertyName == nameof(SearchChatUniqueName))
                {
                    Search();
                }
                else if (args.PropertyName == nameof(SelectedChatForJoin))
                {
                    joinToChatCommand.RaiseCanExecuteChanged();
                 
                }
                else if (args.PropertyName == nameof(SelectedContact))
                {
                    deleteContactCommand.RaiseCanExecuteChanged();
                   
                }
                else if (args.PropertyName == nameof(SelectedClientForAdd))
                {
                    addContactCommand.RaiseCanExecuteChanged();
                   
                }
                else if (args.PropertyName == nameof(OpponentClient))
                {
                    profileInfoDialogOpenCommand.RaiseCanExecuteChanged();

                }
                else if (args.PropertyName == nameof(SelectedClientForInvite))
                {
                    inviteContactCommand.RaiseCanExecuteChanged();
                }
                else if (args.PropertyName == nameof(IsCorrectSignUpData))
                {
                    signUpCommand.RaiseCanExecuteChanged();

                }
                else if (args.PropertyName == nameof(IsCorrectUserInfo))
                {
                    setProfileCommand.RaiseCanExecuteChanged();
                }
                else if (args.PropertyName == nameof(IsCorrectChatData))
                {
                    setChatPropertiesCommand.RaiseCanExecuteChanged();
                    addChatCommand.RaiseCanExecuteChanged();

                }
            };

            loginCommand = new DelegateCommand(Login);
            signUpCommand = new DelegateCommand(SignUp,() => IsCorrectSignUpData);
            exitCommand = new DelegateCommand(Exit);
            closedCommand = new DelegateCommand(Disconnect);

            setProfileDialogOpenCommand = new DelegateCommand(ShowSetProfileDialog);
            contactsDialogOpenCommand = new DelegateCommand(ShowContactsDialog);
           
            joinToChatDialogOpenCommand = new DelegateCommand(ShowJoinToChatDialog);
            chatAddDialogOpenCommand = new DelegateCommand(ShowAddChatDialog);
            chatInfoDialogOpenCommand = new DelegateCommand(ShowChatInfo);
            profileInfoDialogOpenCommand = new DelegateCommand(ShowProfileInfo, () => OpponentClient != null);
            manageChatDialogOpenCommand = new DelegateCommand(ShowEditChatDialog);
            inviteContactsDialogOpenCommand = new DelegateCommand(ShowAddMembersForChat, () => SelectedChat != null);

            setProfileCommand = new DelegateCommand(SetProfile, () => IsCorrectUserInfo);
            setPhotoCommand = new DelegateCommand(SetPhoto);
            
            addContactCommand = new DelegateCommand(AddContact, () => SelectedClientForAdd!=null);
            deleteContactCommand = new DelegateCommand(DeleteContact, () => SelectedContact!=null);
            
            addChatCommand = new DelegateCommand(CreateNewChat, () => IsCorrectChatData);
            joinToChatCommand = new DelegateCommand(JoinToChat, ()=> SelectedChatForJoin!=null);
            leaveFromChatCommand = new DelegateCommand(LeaveFromChat);
            setChatPropertiesCommand = new DelegateCommand(SetChatProperties, () => IsCorrectChatData);
            setChatPhotoCommand = new DelegateCommand(SetChatPhoto);
            createOrSelectPMChatCommand = new DelegateCommand(CreateOrSelectPMChat, () => SelectedContact != null || SelectedClientForAdd != null);
            deleteChatCommand = new DelegateCommand(DeleteChat, () => SelectedChat != null);
            inviteContactCommand = new DelegateCommand(InviteContact, () => SelectedClientForInvite != null);

            sendMessageCommand = new DelegateCommand(SendMessage, ()=>!string.IsNullOrEmpty(TextMessage));
            deleteMessageForAllCommand = new DelegateCommand(DeleteMessageForAll);

            iValidateSignUpCommand = new DelegateCommand(ValidateSignUp);
            iValidateUserInfoCommand = new DelegateCommand(ValidateUserInfo);
            iValidateChatInfoCommand = new DelegateCommand(ValidateChatInfo);

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
                var chatsq = mapper.Map<IEnumerable<ChatViewModel>>(chatMemberService.TakeChatsAsync(currentClient.Id).Result);
                foreach (var item in chatsq)
                {
                    if (item.IsPM)
                    {
                        ReplacePMChatName(item);
                        var companion = mapper.Map<ChatMemberViewModel>(chatMemberService.TakeClients(item.Id).FirstOrDefault(cm => cm.Id != CurrentClient.Id));
                        if (companion != null)
                        {
                            var photo = clientService.GetPhotoAsync(companion.Client.Id).Result;
                            if (photo != null)
                                item.Photo = ToImage(photo.Data);
                        }
                    }
                    allChats.Add(item);
                    chats.Add(item);
                    GetChatPhoto(item);
                }
                foreach (var item in mapper.Map<IEnumerable<ClientViewModel>>(contactService.TakeContactsAsync(currentClient.Id).Result))
                {

                    allContacts.Add(item);
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
                image.CacheOption = BitmapCacheOption.OnLoad; 
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
            if (CurrentClient != ClientForChange)
            {
                if (clientService.SetProperties(mapper.Map<ClientDTO>(ClientForChange)))
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
            if (string.IsNullOrEmpty(ChatForChange.Name) || string.IsNullOrEmpty(ChatForChange.UniqueName) || ChatForChange.MaxUsers==null || ChatForChange.MaxUsers<1 || ChatForChange.MaxUsers>2000 )
            {
                OpenInfoDialog(Resources.EmptyFieldsString);
                return;
            }
            var result = mapper.Map<ChatViewModel>(chatService.CreateNewChatAsync(mapper.Map<ChatDTO>(ChatForChange)).Result);

            if (result != null)
            {   
                chatMemberService.JoinToChatAsync(CurrentClient.Id, result.UniqueName, true);
                allChats.Add(result);
                Search();
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
            var result = mapper.Map<ChatViewModel>(chatMemberService.JoinToChatAsync(CurrentClient.Id, SelectedChatForJoin.UniqueName, true).Result);
            if (result != null)
            {
                GetChatPhoto(result);
                allChats.Add(result);
                Search();
                OpenInfoDialog(Resources.SuccessfulJoinToChatString);
                IsOpenJoinToChatDialog = false;
            }
            else
            {
                OpenInfoDialog(Resources.FailedJoinToChatString);
            }
        }
        public void CreateOrSelectPMChat()
        {
            var chatId = chatService.CreatePMChat(CurrentClient.Id, SelectedClientForAdd == null ? SelectedContact.Id : SelectedClientForAdd.Id);
            if (chatId != -1)
            {
                SelectedChat = allChats.FirstOrDefault(ac => ac.Id == chatId);
                IsOpenContactsDialog = false;
            }
            else
            {
                OpenInfoDialog(Resources.SuccessfulCreateChatString);
            }
        }
        public void SetChatProperties()
        {
            if (string.IsNullOrEmpty(ChatForChange.Name) || string.IsNullOrEmpty(ChatForChange.UniqueName) || ChatForChange.MaxUsers == null ||   ChatForChange.MaxUsers < 1 || ChatForChange.MaxUsers > 2000 )
            {
                OpenInfoDialog(Resources.EmptyFieldsString);
                return;
            }
            if (chatService.SetChatProperties(mapper.Map<ChatDTO>(ChatForChange)))
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
                    chatService.SetChatPhoto(SelectedChat.Id, info);
                }
                OpenInfoDialog(Resources.SuccessfulSetChatPropertiesString);
            }
            else
            {
                OpenInfoDialog(Resources.FailedSetChatPropertiesString);
                ChatForChange = SelectedChat.Clone();
            }
        }
        public void DeleteChat()
        {
            if(!chatService.DeleteChat(SelectedChat.Id))
            {
                OpenInfoDialog(Resources.FailedDeleteChatString);
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
                allChats.Remove(SelectedChat);
                Search();
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
                    Application.Current.Dispatcher.Invoke(() => {
                        item.Client = members.FirstOrDefault(m => m.Client.Id == item.Client.Id).Client;
                        if (item.Client.Id == CurrentClient.Id)
                            item.IsMyMessage = true;
                        chatMessages.Add(item); 
                    });
                }
            });
        }
        public void TakeMembers()
        {
            members.Clear();
            var result = mapper.Map<IEnumerable<ChatMemberViewModel>>(chatMemberService.TakeClientsAsync(SelectedChat.Id).Result);
            Task.Run(() =>
            {
                foreach (var item in result)
                {
                    Application.Current.Dispatcher.Invoke(() => { 
                        members.Add(item);
                        GetPhoto(item.Client);
                    });
                }
            }).ContinueWith((res) =>
            {
                Application.Current.Dispatcher.Invoke(() => {
                    CountMembers = members.Count;
                if (SelectedChat.IsPM) 
                    TakeOpponent();
                });
            });

        }
        public void TakeOpponent()
        {
            var opponent = members.FirstOrDefault(w => w.Client.Id != CurrentClient.Id);
            OpponentClient = opponent.Client;

        }
        public void AddContact()
        {
            var result = mapper.Map<ClientViewModel>(contactService.AddContactAsync(CurrentClient.Id, SelectedClientForAdd.Id).Result);
            if (result != null)
            {
                GetPhoto(result);
                allContacts.Add(result);
                SearchContacts();
                SearchClients();
                OpenInfoDialog(Resources.SuccessfulContactAddString);
            }
            else
            {
                OpenInfoDialog(Resources.FailedContactAddString);          
            }
        }
        public void DeleteContact()
        {
            if (contactService.DeleteContactAsync(CurrentClient.Id, SelectedContact.Id).Result)
            {
                allContacts.Remove(allContacts.Where(c => c.UniqueName == SelectedContact.UniqueName).FirstOrDefault());
                SearchContacts();
                SearchClients();
                OpenInfoDialog(Resources.SuccessfulContactDeleteString);

            }
            else
            {
                OpenInfoDialog(Resources.FailedContactDeletedString);
            }
        }
        public void SendMessage()
        {
            messageService.SendMessage(CurrentClient.Id, SelectedChat.Id, new MessageInfo() { Text = TextMessage });
            TextMessage = String.Empty;
        }

        public void DeleteMessageForAll()
        {
            if (SelectedMessage.Client.Id == currentClient.Id)
            {
                if(messageService.DeleteMessageForAll(SelectedMessage.Id))
                    OpenInfoDialog(Resources.SuccessfullDeleteMessageString);
                else
                    OpenInfoDialog(Resources.FailedDeleteMessageString);

            }
            else
                OpenInfoDialog(Resources.FailedDeleteMessage_NotYourMessageString);
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
            UniqueNameContact = String.Empty;
            SearchContacts();
            clientsForAdd.Clear();
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
            joinToChatChats.Clear();
            UniqueNameChat = string.Empty;
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
        public void ShowProfileInfo()
        {
            IsOpenProfileInfoDialog = true;
        }
       
        public void ShowAddMembersForChat()
        {
            UniqueNameContact = String.Empty;
            SearchContactsForInvite();
            IsOpenAddMembersForChatDialog = true;
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
            allContacts.Clear();                  
            allChats.Clear();
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
            {
                var mes = mapper.Map<MessageViewModel>(message);
                mes.Client = members.FirstOrDefault(m => m.Client.Id == mes.Client.Id).Client;
                if (mes.Client.Id == CurrentClient.Id)
                    mes.IsMyMessage = true;
                chatMessages.Add(mes);
            }
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

        public void Search()
        {
            chats.Clear();
            if (String.IsNullOrEmpty(SearchChatUniqueName))
            {
                foreach (var item in allChats)
                {
                    chats.Add(item);
                }
            }
            else
            {
                var res = allChats.Where(c => c.UniqueName.Contains(SearchChatUniqueName));
                foreach (var item in res)
                {
                    chats.Add(item);
                }
            }
        }
        public void SearchToJoinChats()
        {
            if (!String.IsNullOrEmpty(UniqueNameChat))
            {
                joinToChatChats.Clear();
                var result = mapper.Map<IEnumerable<ChatViewModel>>(chatService.SearchChats(UniqueNameChat));
                foreach (var item in result)
                {
                    if (allChats.Where(c => c.Id == item.Id).FirstOrDefault() == null)
                    {
                        GetChatPhoto(item);
                        joinToChatChats.Add(item);
                    }
                }
            }
        }

        public void SearchContacts()
        {
            contacts.Clear();
            if (String.IsNullOrEmpty(UniqueNameContact))
            {
                foreach (var item in allContacts)
                {
                    contacts.Add(item);
                }
            }
            else
            {
                var res = allContacts.Where(c => c.UniqueName.Contains(UniqueNameContact));
                foreach (var item in res)
                {
                    contacts.Add(item);
                }
            }
        }
        public void SearchContactsForInvite()
        {
            contacts.Clear();
            if (String.IsNullOrEmpty(UniqueNameContact))
            {
                foreach (var item in allContacts)
                {
                    contacts.Add(item);
                }
            }
            else
            {
                var res = allContacts.Where(c => c.UniqueName.Contains(UniqueNameContact));
                foreach (var item in res)
                {
                  if(members.FirstOrDefault(m=>m.Client.Id==item.Id)==null)                 
                    contacts.Add(item);
                }
            }
        }
        public void SearchClients()
        {
            if (!String.IsNullOrEmpty(UniqueNameContact))
            {
                clientsForAdd.Clear();
                var result = mapper.Map<IEnumerable<ClientViewModel>>(clientService.SearchClients(UniqueNameContact));
                foreach (var item in result)
                {
                    if (allContacts.Where(c => c.Id == item.Id).FirstOrDefault() == null && item.Id!=CurrentClient.Id)
                    {
                        GetPhoto(item);
                        clientsForAdd.Add(item);
                    }
                }
            }
        }
        public void Joined(ChatMemberDTO chatMember, int chatId, InfoFile photo)
        {
            if (SelectedChat.Id == chatId)
            {
                var cl = mapper.Map<ChatMemberViewModel>(chatMember);
                if (photo != null)
                    cl.Client.Photo = ToImage(photo.Data);
                members.Add(cl);
                CountMembers = members.Count;
            }
        }
        public void Left(int clientId, int chatId)
        {
            if (SelectedChat.Id == chatId)
            {
                members.Remove(members.Where(m => m.Client.Id == clientId).FirstOrDefault());
                CountMembers = members.Count;
            }
        }
        public void TakeChat(ChatDTO pmChat, InfoFile photo)
        {
            var chat = mapper.Map<ChatViewModel>(pmChat);
            if (photo != null)
                chat.Photo = ToImage(photo.Data);
            ReplacePMChatName(chat);
            allChats.Add(chat);
            Search();
        }
        public void DeleteChatForAll(int chatId)
        {
            var chatForDelete = allChats.FirstOrDefault(ca => ca.Id == chatId);
            if (chatForDelete != null)
            {
                if (SelectedChat != null && SelectedChat.Id == chatId)
                    OpenInfoDialog(Resources.SuccessfulDeleteChatString);
                allChats.Remove(chatForDelete);
                Search();
            } 
                
        }

        public void RemoveMessageForAll(int chatId, int messageId)
        {
            if(SelectedChat.Id==chatId)
            {
                chatMessages.Remove(chatMessages.FirstOrDefault(c=>c.Id==messageId));
            }

        }
        public void AddChatForContact(ChatDTO chat, InfoFile photo)
        {
            if (chat!=null& allChats.FirstOrDefault(c => c.Id == chat.Id) == null)
            {
                var newChat = mapper.Map<ChatViewModel>(chat);
                if(photo!=null)
                newChat.Photo = ToImage(photo.Data);
                
                allChats.Add(newChat);
                Search();
            }
        }

        public void InviteContact()
        {
            if(chatMemberService.InviteContactAsync(SelectedChat.Id, SelectedClientForInvite.Id).Result)
            {
                OpenInfoDialog(Resources.SuccessfulContactInviteInChatString);             
            }
            else
            {
                OpenInfoDialog(Resources.FailedContactInviteInChatString);
            }
        }

        public void GetNewClientProperties(ClientDTO client)
        {
            var newClient = mapper.Map<ClientViewModel>(client);
            var contact = allContacts.FirstOrDefault(c => c.Id == newClient.Id);
            if (contact != null)
            {
                SetClientProperties(contact, newClient);
                if (OpponentClient != null && OpponentClient.Id == client.Id)
                    SetClientProperties(OpponentClient, newClient);
                SearchContacts();
            }
        }
        public void GetNewClientPhoto(int clientId, InfoFile photo)
        {
            var contact = allContacts.FirstOrDefault(c => c.Id == clientId);
            if (contact != null)
            {
                contact.Photo = ToImage(photo.Data);
            }
        }
        public void SetNewPMChatProperties(ChatDTO chat)
        {
            var newChat = mapper.Map<ChatViewModel>(chat);
            ReplacePMChatName(newChat);
            chats.FirstOrDefault(c => c.Id == newChat.Id).Name = newChat.Name;
        }

        public void GetNewChatProperties(ChatDTO chat)
        {
            var newChat = mapper.Map<ChatViewModel>(chat);
            var oldChat = allChats.FirstOrDefault(c => c.Id == newChat.Id);
            if (oldChat != null)
            {
                if (SelectedChat == null || SelectedChat.Id != newChat.Id)
                    SetChatProperties(oldChat, newChat);
                else
                {
                    SetChatProperties(SelectedChat, newChat);
                }
            }
        }
        public void GetNewChatPhoto(int chatId, InfoFile photo)
        {
            var chat = allChats.FirstOrDefault(c => c.Id == chatId);
            if (chat != null)
            {
                chat.Photo = ToImage(photo.Data);
            }
        }

        public bool IsPasswordCorrect()
        {
            return passordRegex.IsMatch(Password);
        }
        public bool Validate(object instanse)
        {
            if (instanse == null)
                return false;

            var results = new List<ValidationResult>();
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(instanse, null, null);
            if (Validator.TryValidateObject(instanse, context, results, true))
                return true;
            return false;
        }
        public void ValidateSignUp()
        {

            if (CurrentClient != null && CurrentClient.Account != null && !String.IsNullOrEmpty(Password))
            {
                if (!Validate(CurrentClient) || !IsPasswordCorrect())
                {
                    IsCorrectSignUpData = false;
                }
                else
                    IsCorrectSignUpData = true;
            }
            else
                IsCorrectSignUpData = false;
        }
        public void ValidateUserInfo()

        {

            if (ClientForChange != null && ClientForChange.Account != null)
            {

                if (!Validate(ClientForChange))
                {
                    IsCorrectUserInfo = false;
                }
                else
                    IsCorrectUserInfo = true;
            }
            else
                IsCorrectUserInfo = false;
        }
        public void ValidateChatInfo()
        {

            if (ChatForChange != null)
            {

                if (!Validate(ChatForChange))
                {
                    IsCorrectChatData = false;
                }
                else
                    IsCorrectChatData = true;
            }
            else
                IsCorrectChatData = false;
        }


        private void ReplacePMChatName(ChatViewModel chat)
        {
            chat.Name = chat.Name.Replace(CurrentClient.Name, "");
            //chat.Name = chat.Name.Remove(chat.Name.IndexOf(CurrentClient.Name), CurrentClient.Name.Length);
        }
        private void SetClientProperties(ClientViewModel oldProperties, ClientViewModel newProperties)
        {
            oldProperties.Name = newProperties.Name;
            oldProperties.Account.Phone = newProperties.Account.Phone;
            oldProperties.Account.Email = newProperties.Account.Email;
            oldProperties.UniqueName = newProperties.UniqueName;
        }
        private void SetChatProperties(ChatViewModel oldProperties, ChatViewModel newProperties)
        {
            oldProperties.Name = newProperties.Name;
            oldProperties.UniqueName = newProperties.UniqueName;
            oldProperties.IsPrivate = newProperties.IsPrivate;
            oldProperties.MaxUsers = newProperties.MaxUsers;
        }


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
