using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    internal class ContactConfig : EntityTypeConfiguration<Contact>
    {
        public ContactConfig()
        {
            this.HasRequired(c => c.ContactClient)
                .WithMany(m => m.Contacts)
                .HasForeignKey(f => f.ContactClientId)
                .WillCascadeOnDelete(false);

            this.HasRequired(c => c.Client)
               .WithMany()
               .HasForeignKey(f => f.ClientId)
               .WillCascadeOnDelete(false);
        }
    }
}
