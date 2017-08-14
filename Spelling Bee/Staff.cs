using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spelling_Bee
{
    partial class Staff
    {
        public string getFullName()
        {
            return StaffPrefix + " " + StaffForename + " " + StaffSurname;
        }

        public string FullName
        {
            get
            {
                return getFullName();
            }
        }
    }
}
