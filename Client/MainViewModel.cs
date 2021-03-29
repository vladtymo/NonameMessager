using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class MainViewModel
    {
        private ClientViewModel currentClient;
        private ClientViewModel clientForChange;

        public MainViewModel()
        {

        }


        public ClientViewModel CurrentClient => currentClient;
        public ClientViewModel ClientForChange => clientForChange;
    }
}
