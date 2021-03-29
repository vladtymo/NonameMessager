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
            this.HasRequired(f => f.Message)
                .WithMany(m => m.Files)
                .HasForeignKey(f => f.MessageId)
                .WillCascadeOnDelete(false);

        }
    }
}
