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
    /// A ini file parser.
    /// </summary>
    public class IniFile
    {
        private string _filename;
        private List<IniSection> _sections;

        /// <summary>
        /// Filename of the ini file.
        /// </summary>
        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        /// <summary>
        /// Creates an empty ini file with no filename.
        /// </summary>
        public IniFile() : this("")
        {
        }
        
        /// <summary>
        /// Loads an ini file.
        /// </summary>
        /// <param name="filename">Filename of the file to load.</param>
        public IniFile(string filename)
        {
            Filename = filename;
            _sections = new List<IniSection>();

            if (this.Filename.Length > 0)
                Load();
        }

        /// <summary>
        /// Loads the ini file.
        /// </summary>
        public void Load()
        {
            FileStream stream;
            StreamReader reader;
            string line, name, key, value;
            IniSection section;
            int split;

            stream = File.OpenRead(this.Filename);
            reader = new StreamReader(stream);

            section = new IniSection();

            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    name = line.Substring(1, line.Length - 2);

                    section = new IniSection(name);
                    _sections.Add(section);
                }
                else
                {
                    split = line.IndexOf('=');
                    if (split == -1)
                        continue;

                    key = line.Substring(0, split);
                    value = line.Substring(split + 1);

                    section[key] = value;
                }
            }
        }

        /// <summary>
        /// Loads the ini file given the filename.
        /// </summary>
        /// <param name="filename">The filename of the ini file.</param>
        public void Load(string filename)
        {
            this.Filename = filename;

            Load();
        }


        /// <summary>
        /// Saves the ini file.
        /// </summary>
        public void Save()
        {
            FileStream stream;
            StreamWriter writer;

            stream = File.OpenWrite(this.Filename);
            writer = new StreamWriter(stream);

            foreach (IniSection section in _sections)
            {
                writer.WriteLine("[" + section.Name + "]");

                foreach (KeyValuePair<string, string> item in section)
                {
                    writer.WriteLine(item.Key + "=" + item.Value);
                }
            }

            writer.Close();
        }

        /// <summary>
        /// Saves the ini file using the given filename.
        /// </summary>
        /// <param name="filename">The filename to save the file as.</param>
        public void Save(string filename)
        {
            this.Filename = filename;

            Save();
        }
    }
}
