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
            this.HasRequired(a => a.Client)
                .WithRequiredPrincipal(c => c.Account);
        }
    }
}
