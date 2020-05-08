using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartFinance.Models
{
    class PersonalInfo
    {
        [PrimaryKey, AutoIncrement]
        public int PersonalID { get; set; }

        [NotNull]
        public string FirstName { get; set; }

        [NotNull]
        public string LastName { get; set; }

    }
}
