using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    internal class AccountConfig : EntityTypeConfiguration<Account>
    {
        public AccountConfig()
        {
            this.HasKey(a => a.Id);

            this.Property(t => t.Email)
                .HasMaxLength(50)
                .IsRequired();

            this.Property(t => t.Phone)
                .HasMaxLength(50)
                .IsRequired();
            
            this.Property(t => t.Password)
                .HasMaxLength(50)
                .IsRequired();

            this.HasRequired(a => a.Client)
                .WithRequiredPrincipal(c => c.Account);
        }
    }
}
