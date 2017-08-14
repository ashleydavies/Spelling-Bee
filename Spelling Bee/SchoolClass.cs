using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spelling_Bee
{
    class SchoolClass
    {
        int ID;
        string name;
        StudentYear year;
        List<Student> students;


        public static SchoolClass getClass(int fID)
        {
            // Load from file given fID as ID //
            return new SchoolClass();
        }
    }
}
