using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

#if DEBUG
using System.Diagnostics;
#endif

namespace MacroInfinity
{
    /// <summary>
    /// A text macro used in infantry/
    /// </summary>
    public class Macro
    {
        private string _text;

        /// <summary>
        /// Gets and sets the text for the macro.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        /// <summary>
        /// Creates an empty macro.
        /// </summary>
        public Macro() : this("")
        {
        }

        /// <summary>
        /// Creates a macro and initializes the text.
        /// </summary>
        /// <param name="text">The text that is the macro.</param>
        public Macro(string text)
        {
            Text = text;
        }

        /// <summary>
        /// Gets a string representation of the macro, which is just the text of the macro.
        /// </summary>
        /// <returns>The text of the macro.</returns>
        public override string ToString()
        {
            return Text;
        }        

        /// <summary>
        /// Loads a list of macros from the specified file.
        /// </summary>
        /// <param name="filename">Filename of the file that macros are to be loaded from.</param>
        /// <returns>Array of the macros loaded from the file.</returns>
        public static Macro[] LoadMacros(string filename)
        {
            FileStream stream;            
            StreamReader reader;
            Macro[] macros;
            int index;
            string line;

            stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            reader = new StreamReader(stream);

            index = 0;
            macros = new Macro[12];
            while (((line = reader.ReadLine()) != null) && (index < 12))
            {
#if DEBUG
                Trace.WriteLine(line, "File Read");
#endif

                macros[index] = new Macro(line);

                index++;
            }

            reader.Close();

            return macros;
        }        

        /// <summary>
        /// Saves a list of macros to the specified file.
        /// </summary>
        /// <param name="filename">Filename of the file the macros will be saved in.</param>
        /// <param name="macros">An array of macros that will be saved in the file.</param>
        public static void SaveMacros(string filename, Macro[] macros)
        {
            FileStream stream;
            StreamWriter writer;            
            int index;            

            stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            writer = new StreamWriter(stream);

            index = 0;
            for (index = 0; index < 12; index++)
            {
#if DEBUG
                Trace.WriteLine(macros[index].Text, "File Write");
#endif

                writer.WriteLine(macros[index].Text);
            }

            writer.Close();
        }
    }
}
