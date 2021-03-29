using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    internal class MessageConfig : EntityTypeConfiguration<Message>
    {
        public MessageConfig()
        {
            this.HasRequired(i => i.Client)
                .WithMany(c => c.Messages)
                .HasForeignKey(i => i.ClientId)
                .WillCascadeOnDelete(false);

            this.HasRequired(i => i.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(i => i.ChatId)
                .WillCascadeOnDelete(false);

        }
    }
}
