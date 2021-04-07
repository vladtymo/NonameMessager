using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Contact
    {
        public int Id { get; set; }

        // FOREIGN KEYS
        public int ClientId { get; set; }
        public int ContactClientId { get; set; }

        // NAVIGATION PROPERTIES
        public virtual Client ContactClient { get; set; }
        public virtual Client Client { get; set; }

        public static implicit operator Contact(int v)
        {
            throw new NotImplementedException();
        }
    }
}
