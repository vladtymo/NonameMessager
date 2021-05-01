using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface IClientService
    {
        [OperationContract]
        ClientDTO CreateNewClient(ClientDTO client, string password);
        [OperationContract]
        ClientDTO GetClient(AccountDTO account, string password);
        [OperationContract]
        void SetProperties(ClientDTO client, out bool result);
        [OperationContract]
        void SetPhoto(int clientId, InfoFile info);
        [OperationContract]
        InfoFile GetPhoto(int clientId);
        [OperationContract]
        void GetPathToPhoto(string pathToPhoto);
        [OperationContract]
        void Disconnect();
        [OperationContract]
        IEnumerable<ClientDTO> SearchClients(string uniqueName);
    }
}
