using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Account> AccountRepos { get; }
        IRepository<Chat> ChatRepos { get; }
        IRepository<ChatMember> ChatMemberRepos { get; }
        IRepository<Client> ClientRepos { get; }
        IRepository<Contact> ContactRepos { get; }
        IRepository<File> FileRepos { get; }
        IRepository<Message> MessageRepos { get; }
        void Save();
    }
}
