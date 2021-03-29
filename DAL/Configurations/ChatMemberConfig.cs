using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    internal class ChatMemberConfig : EntityTypeConfiguration<ChatMember>
    {
        public ChatMemberConfig()
        {
            this.HasRequired(c => c.Client)
                .WithMany(m => m.ChatMembers)
                .HasForeignKey(f => f.ClientId)
                .WillCascadeOnDelete(false);

            this.HasRequired(c => c.Chat)
               .WithMany(m => m.ChatMembers)
               .HasForeignKey(f => f.ChatId)
               .WillCascadeOnDelete(false);
        }
    }
}
