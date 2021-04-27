using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface IContactService
    {
        [OperationContract]
        ClientDTO AddContact(int clientID, string uniqueNameContact);
        [OperationContract]
        bool DeleteContact(int clientID, string uniqueNameContact);
        [OperationContract]
        IEnumerable<ClientDTO> TakeContacts(int clientId);
    }


}
