using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spelling_Bee
{
    partial class Test
    {
        public int MaxScore
        {
            get
            {
                int questions = TestQuestions.Count();

                return questions * 2;
            }
        }
    }
}
