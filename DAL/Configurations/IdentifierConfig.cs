using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    internal class IdentifierConfig : EntityTypeConfiguration<Identifier>
    {
        public IdentifierConfig()
        {
            this.HasKey(i => i.Id);

            this.Property(i => i.Name)
                .HasMaxLength(50)
                .IsRequired();

            this.HasOptional(i => i.Client)
                .WithRequired(c => c.ClientInfo);

            this.HasOptional(i => i.Chat)
                .WithRequired(c => c.ChatInfo);

        }
    }
}
