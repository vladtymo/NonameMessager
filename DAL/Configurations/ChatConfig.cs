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

            this.HasRequired(c => c.ChatName)
                .WithMany(c => c.Chats)
                .HasForeignKey(c => c.ChatNameId)
                .WillCascadeOnDelete(false);
        }
    }
}
