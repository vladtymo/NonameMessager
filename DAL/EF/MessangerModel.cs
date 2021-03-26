using System.Data.Entity;
using System.Linq;

namespace DAL
{
    public class MessangerModel : DbContext
    {
        public MessangerModel()
            : base("name=MessangerModel")
        {
            Database.SetInitializer(new Initializer());
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new AccountConfig());
            modelBuilder.Configurations.Add(new ChatConfig());
            modelBuilder.Configurations.Add(new ClientConfig());
            modelBuilder.Configurations.Add(new FileConfig());
            modelBuilder.Configurations.Add(new IdentifierConfig());
            modelBuilder.Configurations.Add(new MessageConfig());
            modelBuilder.Configurations.Add(new ProfileConfig());
        }

        public virtual DbSet<Identifier> Identifiers { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
    }
}