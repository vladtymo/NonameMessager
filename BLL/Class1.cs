using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class AccountDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public int ClientId { get; set; }
        public ClientDTO Client { get; set; }
    }
    public class ClientDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int IdentifierId { get; set; }
        public virtual Identifier Identifier { get; set; }
        public int AccountId { get; set; }
        public virtual AccountDTO Account { get; set; }
        public virtual ICollection<Identifier> Contacts { get; set; }
    }
    public class Identifier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsChat { get; set; }
        public int? ClientId { get; set; }
        public virtual Client Client { get; set; }
        public int? ChatId { get; set; }
        public virtual Chat Chat { get; set; }
        //public virtual ICollection<Message> MessagesClient { get; set; }
        //public virtual ICollection<Message> MessagesChat { get; set; }
        //public virtual ICollection<Chat> Chats { get; set; }
        //public virtual ICollection<Client> Clients { get; set; }
    }
    public class Class1
    {
        private IUnitOfWork repositories;

        public Class1()
        {
            this.repositories = new UnitOfWork();
        }
    }
}
