using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    internal class ClientConfig : EntityTypeConfiguration<Client>
    {
        public ClientConfig()
        {
            this.HasKey(c => c.Id);

            this.Property(t => t.Name)
                .HasMaxLength(50)
                .IsRequired();

            this.HasMany(c => c.Contacts)
                .WithMany(i => i.Clients);
        }
    }
}
