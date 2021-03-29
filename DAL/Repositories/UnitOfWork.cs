using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private MessangerModel context = new MessangerModel();

        private GenericRepository<Account> accountRepository;
        private GenericRepository<Chat> chatRepository;
        private GenericRepository<ChatMember> chatMemberRepository;
        private GenericRepository<Client> clientRepository;
        private GenericRepository<Contact> contactRepository;
        private GenericRepository<File> fileRepository;
        private GenericRepository<Message> messageRepository;

        public IRepository<Account> AccountRepos
        {
            get
            {
                if (this.accountRepository == null)
                {
                    this.accountRepository = new GenericRepository<Account>(context);
                }
                return accountRepository;
            }
        }
        public IRepository<Chat> ChatRepos
        {
            get
            {
                if (this.chatRepository == null)
                {
                    this.chatRepository = new GenericRepository<Chat>(context);
                }
                return chatRepository;
            }
        }
        public IRepository<ChatMember> ChatMemberRepos
        {
            get
            {
                if (this.chatMemberRepository == null)
                {
                    this.chatMemberRepository = new GenericRepository<ChatMember>(context);
                }
                return chatMemberRepository;
            }
        }
        public IRepository<Client> ClientRepos
        {
            get
            {
                if (this.clientRepository == null)
                {
                    this.clientRepository = new GenericRepository<Client>(context);
                }
                return clientRepository;
            }
        }
        public IRepository<Contact> ContactRepos
        {
            get
            {
                if (this.contactRepository == null)
                {
                    this.contactRepository = new GenericRepository<Contact>(context);
                }
                return contactRepository;
            }
        }
        public IRepository<File> FileRepos
        {
            get
            {
                if (this.fileRepository == null)
                {
                    this.fileRepository = new GenericRepository<File>(context);
                }
                return fileRepository;
            }
        }
        public IRepository<Message> MessageRepos
        {
            get
            {
                if (this.messageRepository == null)
                {
                    this.messageRepository = new GenericRepository<Message>(context);
                }
                return messageRepository;
            }
        }
        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
