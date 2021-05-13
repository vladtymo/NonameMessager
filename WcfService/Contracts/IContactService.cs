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
        ClientDTO AddContact(int clientID, int contactId);
        [OperationContract]
        bool DeleteContact(int clientID, int contactId);
        [OperationContract]
        IEnumerable<ClientDTO> TakeContacts(int clientId);
    }


}
