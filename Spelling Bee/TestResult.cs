using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spelling_Bee
{
    partial class TestResult
    {
        public int MaxScore
        {
            get
            {
                return Test.MaxScore;
            }
        }

        public int Percentage
        {
            get
            {
                int questions = Test.TestQuestions.Count();

                // Division by zero error catch
                if (questions == 0)
                    return 0;

                return (100 / MaxScore) * Score.Value;
            }
        }
    }
}
