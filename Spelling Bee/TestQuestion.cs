using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spelling_Bee
{
    partial class TestQuestion
    {
        // Enum to handle different types of error (Including none / Correct)
        public enum ErrorType
        {
            ERRORMAJOR,
            ERRORMINOR,
            CORRECT
        }

        public ErrorType getErrorStatus(string word)
        {
            // Trim word from spaces
            word = word.Trim();

            // Lowercase both words (Since capitalisation is not spelling per se)
            string inputWord = word.ToLower();
            string checkWord = Answer.ToLower();

            // If it's correct just return immediately
            if (inputWord == checkWord)
                return ErrorType.CORRECT;

            // We have identified the following types of minor errors:
            //
            // 1.	If the word is identical, return correct (Above)
            // 2.	If the word has one letter less, but all other letters exist in the correct order, return minor
            if (inputWord.Length == checkWord.Length - 1)
            {
                for (int i = 0; i < inputWord.Length; i++)
                {
                    char inOne = inputWord[i];
                    char checkOne = checkWord[i];
                    char checkTwo = checkWord[i+1];

                    if (inOne != checkOne && inOne != checkTwo)
                    {
                        // We can establish that there is more than one incorrect character
                        //  as if only one letter is missing, but all others are in the correct order
                        //  then the character in the input should *always* equal either the same-index character
                        //  in the other word, or the character one place to the right of it.
                        return ErrorType.ERRORMAJOR;
                    }
                }

                // Clearly, then, this must be only one character off.
                return ErrorType.ERRORMINOR;
            }
            // 3.	If the word has one letter too many, but all other letters exist in the correct order, return 1
            if (inputWord.Length == checkWord.Length + 1)
            {
                for (int i = 0; i < checkWord.Length; i++)
                {
                    char inOne = inputWord[i];
                    char inTwo = inputWord[i + 1];
                    char checkOne = checkWord[i];

                    if (inOne != checkOne && inTwo != checkOne)
                    {
                        // See above logic
                        return ErrorType.ERRORMAJOR;
                    }
                }

                return ErrorType.ERRORMINOR;
                // In theory, the above code could be merged into a single solution with condition 2,
                //  by swapping the two strings, but that could cause confusion and for such a small space saving
                //  it would not necessarily be worth doing.
            }
            // 4.	If the word has the right amount of letters, with only two switched from adjacent positions, return 1
            if (inputWord.Length == checkWord.Length)
            {
                int wrongCount = 0;

                for (int i = 0; i < checkWord.Length; i++)
                {
                    char inOne = inputWord[i];
                    char checkOne = checkWord[i];
                    
                    // Rule out correctness straight away
                    if (inOne == checkOne)
                        continue;

                    if (i != 0)
                        // Since i = 0 is the only place where there are no preceding characters.
                        if (inOne == checkWord[i - 1])
                        {
                            wrongCount++;
                            continue;
                        }
                    if (i != checkWord.Length - 1)
                        // since checkWord.Length is the only place where are no postceding characters.
                        if (inOne == checkWord[i + 1])
                        {
                            wrongCount++;
                            continue;
                        }

                    // The character did not have a correct adjacency, so increment wrongcount by two.
                    //  this means that they can only have one incorrect character
                    //  if there is not a correct character adjacent.
                    //  i.e, "bacun" or "bacno" would be acceptable for "bacon" but not
                    //  "bakun"
                    wrongCount += 2;
                }

                if (wrongCount < 3)
                    // They only swapped one set of adjacent characters, so return a minor error.
                    return ErrorType.ERRORMINOR;
                else
                    // They swapped more than one set, so return major.
                    return ErrorType.ERRORMAJOR;
            }
            // 5.	If no other paths have been taken, return major
            return ErrorType.ERRORMAJOR;
        }
    }
}
