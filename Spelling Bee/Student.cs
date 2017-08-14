using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spelling_Bee
{
    enum StudentYear
    {
        YEAR_3,
        YEAR_4,
        YEAR_5,
        YEAR_6,
    }

    class Student
    {
        int ID;
        string password;
        string name;
        StudentYear year;


        public static Student getStudent(int fID)
        {
            // Load from file given fID as ID //
            return new Student();
        }
    }
}
