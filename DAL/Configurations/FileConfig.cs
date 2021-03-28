using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    internal class FileConfig : EntityTypeConfiguration<File>
    {
        public FileConfig()
        {
            this.HasKey(f => f.Id);

            this.Property(f => f.Name)
                .HasMaxLength(50)
                .IsRequired();

            this.HasOptional(i => i.Profile)
                .WithRequired(i => i.File);

            this.HasOptional(f => f.Message)
                .WithMany(m => m.Files)
                .HasForeignKey(f => f.MessageId)
                .WillCascadeOnDelete(false);

        }
    }
}
