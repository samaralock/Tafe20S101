using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartFinance.Models
{
    class Contact
    {
        [PrimaryKey, AutoIncrement]
        public int ContactID { get; set; }
        [NotNull]
        public string ContactFirstName { get; set; }
        [NotNull]
        public string ContactLastName { get; set; }
        [NotNull]
        public string ContactCompany { get; set; }
        [NotNull]
        public string ContactPhone { get; set; }
    }
}
