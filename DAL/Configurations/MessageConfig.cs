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
            this.HasKey(m => m.Id);

            this.HasRequired(i => i.Client)
                .WithRequiredPrincipal(i => i.MessageClient);

            this.HasRequired(i => i.Chat)
                .WithRequiredPrincipal(i => i.MessageChat);

        }
    }
}
