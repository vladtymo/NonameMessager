using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    internal class ChatConfig : EntityTypeConfiguration<Chat>
    {
        public ChatConfig()
        {
            this.HasKey(c => c.Id);

            this.HasMany(c => c.Clients)
                .WithMany(c => c.Chats);
        }
    }
}
