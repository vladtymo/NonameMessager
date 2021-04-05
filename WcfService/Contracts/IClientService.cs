using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    [ServiceContract]
    public interface IClientService
    {
        [OperationContract]
        IEnumerable<ClientDTO> GetAllClients();
        [OperationContract]
        ClientDTO CreateNewClient(ClientDTO clientDTO, string password);
        [OperationContract]
        ClientDTO GetClient(AccountDTO accountDTO, string password);
        [OperationContract]
        bool SetProperties(ClientDTO clientDTO);
    }
}
