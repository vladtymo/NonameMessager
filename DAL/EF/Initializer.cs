using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    internal class Initializer : CreateDatabaseIfNotExists<MessangerModel>
    {
        protected override void Seed(MessangerModel context)
        {
            base.Seed(context);

        }
    }
}
