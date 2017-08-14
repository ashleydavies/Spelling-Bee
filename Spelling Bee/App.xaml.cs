using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Spelling_Bee
{
    public partial class App : Application
    {
        // Static database access instance used by everything \\
        public static SpellingBeeDataClassesDataContext db = new SpellingBeeDataClassesDataContext();
        
        /// <summary>
        /// Output a message to the user in a modal dialog box
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        public static void Message(string title, string content, bool sound = true)
        {
            new MessageWindow(title, content, sound).ShowDialog();
        }

        /// <summary>
        /// Get a string input from the user in a modal dialog box
        /// </summary>
        /// <param name="title"></param>
        /// <returns>Inputted string</returns>
        public static string getString(string title)
        {
            RequestStringWindow rsw = new RequestStringWindow(title);
            rsw.ShowDialog();
            return rsw.str;
        }

        /// <summary>
        /// Returns a year (3-6) represented as an integer inputted from the user in a modal dialog box
        /// </summary>
        /// <param name="title"></param>
        /// <returns>Inputted year</returns>
        public static int getYear(string title)
        {
            RequestYearWindow ryw = new RequestYearWindow(title);
            ryw.ShowDialog();
            return ryw.year;
        }

        /// <summary>
        /// Returns a boolean inputted from the user in a modal dialog box
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <returns>Inputted boolean</returns>
        public static bool getBoolean(string title, string content)
        {
            RequestBoolWindow rbw = new RequestBoolWindow(title, content);
            rbw.ShowDialog();
            return rbw.answer;
        }
    }
}
