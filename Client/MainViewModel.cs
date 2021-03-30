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
        private IClientService clientService = new ClientService();
        private IMapper mapper;

        private ClientViewModel currentClient;
        private ClientViewModel clientForChange;

        public MainViewModel()
        {
            IConfigurationProvider config = new MapperConfiguration(
                cfg =>
                {
                    cfg.CreateMap<AccountDTO, AccountViewModel>();
                    cfg.CreateMap<ClientDTO, ClientViewModel>();

                    cfg.CreateMap<AccountViewModel, AccountDTO>();
                    cfg.CreateMap<ClientViewModel, ClientDTO>();
                });

            mapper = new Mapper(config);
            loginCommand = new DelegateCommand(Login);
            IsOpenLoginRegistrationDialog = true;
            signUpCommand = new DelegateCommand(SignUp);
            currentClient = new ClientViewModel() { Account = new AccountViewModel() };
        }


        public ClientViewModel CurrentClient { get { return currentClient; } set { SetProperty(ref currentClient, value); } }
        public ClientViewModel ClientForChange { get { return clientForChange; } set { SetProperty(ref clientForChange, value); } }

        public void Login()
        {
            CurrentClient.Account.Phone = CurrentClient.Account.Email;
            var result = mapper.Map<ClientViewModel>(clientService.GetClient(mapper.Map<AccountDTO>(CurrentClient.Account)));
            if (result != null)
            {
                CurrentClient = result;
                IsOpenLoginRegistrationDialog = false;
            }
        }

        public void SignUp()
        {

            var result = mapper.Map<ClientViewModel>(clientService.CreateNewClient(mapper.Map<ClientDTO>(CurrentClient)));
            if (result != null)
            {
                CurrentClient = result;
                IsOpenLoginRegistrationDialog = false;
            }
        }

        private Command loginCommand;
        private Command signUpCommand;
        

        private bool isOpenLoginRegistrationDialog;
        public bool IsOpenLoginRegistrationDialog { get { return isOpenLoginRegistrationDialog; } set { SetProperty(ref isOpenLoginRegistrationDialog, value); } }
       
        public ICommand LoginCommand => loginCommand;
        public ICommand SignUpCommand => signUpCommand;


    }
}
