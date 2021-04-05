using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    [ServiceContract]
    public interface IContactService
    {
        [OperationContract]
        ClientDTO AddContact(int clientID, string uniqueNameContact);
        [OperationContract]
        bool DeleteContact(int clientID, string uniqueNameContact);
    }
}
