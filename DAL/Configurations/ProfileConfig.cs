using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    internal class ProfileConfig : EntityTypeConfiguration<Profile>
    {
        public ProfileConfig()
        {
            this.HasKey(m => m.Id);

            this.Property(t => t.Name)
                .HasMaxLength(50)
                .IsRequired();

            this.HasOptional(i => i.File)
                .WithRequired(i => i.Profile);

        }
    }
}
