using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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


        public IEnumerable<ClientViewModel> Contacts => contacts;
        public IEnumerable<ChatViewModel> Chats => chats;


        #endregion
        #region Properties
        private ClientServiceClient clientService = new ClientServiceClient();
        private ChatServiceClient chatService = new ChatServiceClient();
        private ContactServiceClient contactService = new ContactServiceClient();
        private ChatMemberServiceClient chatMemberService = new ChatMemberServiceClient();

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



        private bool isOpenInfoDialog;
        private string textForInfoDialog;

        private string password;
        private string uniqueNameContact;
        private string uniqueNameChat;

        private string pathToPhoto;

        

        public bool IsOpenLoginRegistrationDialog { get { return isOpenLoginRegistrationDialog; } set { SetProperty(ref isOpenLoginRegistrationDialog, value); } }
        public bool IsOpenProfileDialog { get { return isOpenProfileDialog; } set { SetProperty(ref isOpenProfileDialog, value); } }
        
        public bool IsOpenContactsDialog { get { return isOpenContactsDialog; } set { SetProperty(ref isOpenContactsDialog, value); } }

        public bool IsOpenAddEditChatDialog { get { return isOpenAddEditChatDialog; } set { SetProperty(ref isOpenAddEditChatDialog, value); } }

        public bool IsOpenInfoDialog { get { return isOpenInfoDialog; } set { SetProperty(ref isOpenInfoDialog, value); } }

        public bool IsOpenJoinToChatDialog { get { return isOpenJoinToChatDialog; } set { SetProperty(ref isOpenJoinToChatDialog, value); } }

        public bool IsAddChatDialog { get { return isAddChatDialog; } set { SetProperty(ref isAddChatDialog, value); } }

        public string TextForInfoDialog { get { return textForInfoDialog; } set { SetProperty(ref textForInfoDialog, value); } }

        public string Password { get => password; set => SetProperty(ref password, value); }
        public string UniqueNameContact { get => uniqueNameContact; set => SetProperty(ref uniqueNameContact, value); }
        public string UniqueNameChat { get => uniqueNameChat; set => SetProperty(ref uniqueNameChat, value); }


        public ClientViewModel CurrentClient { get { return currentClient; } set { SetProperty(ref currentClient, value); } }
        public ClientViewModel ClientForChange { get { return clientForChange; } set { SetProperty(ref clientForChange, value); } }
        public ChatViewModel ChatForChange { get { return chatForChange; } set { SetProperty(ref chatForChange, value); } }
        public ChatViewModel SelectedChat { get { return selectedChat; } set { SetProperty(ref selectedChat, value); } }


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
            chatForChange = new ChatViewModel();           
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
                OpenInfoDialog($"Data changed successfully.");
                CurrentClient = ClientForChange.Clone();

            }
            else
            {
                OpenInfoDialog($"The data has not been changed.");

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
            }
            else
            {
                OpenInfoDialog("Chat was not created.");
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
                OpenInfoDialog($"Contact added successfully.");
            }
            else
            {
                OpenInfoDialog($"Contact could not be added.");
            }
        }
        public void DeleteContact()
        {
            if (contactService.DeleteContact(CurrentClient.Id, UniqueNameContact))
            {
                contacts.Remove(contacts.Where(c => c.UniqueName == UniqueNameContact).FirstOrDefault());
                OpenInfoDialog($"Contact successfully delete.");
            }
            else
            {
                OpenInfoDialog($"Failed to delete contact.");

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

        #endregion

    }
}
