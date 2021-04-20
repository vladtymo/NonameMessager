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
        ClientDTO CreateNewClient(ClientDTO clientDTO, string password);
        [OperationContract]
        ClientDTO GetClient(AccountDTO accountDTO, string password);
        [OperationContract]
        bool SetProperties(ClientDTO clientDTO);
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
