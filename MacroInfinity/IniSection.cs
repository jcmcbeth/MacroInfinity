using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace MacroInfinity
{
    /// <summary>
    /// A section in an ini file.
    /// </summary>
    public class IniSection : IEnumerable
    {
        private string _name;
        private Dictionary<string, string> _contents;

        /// <summary>
        /// Gets and sets the name of the section.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets and sets the contents of the section.
        /// </summary>
        public Dictionary<string, string> Contents
        {
            get { return _contents; }
            set { _contents = value; }
        }

        /// <summary>
        /// Gets and sets the contents of the section.
        /// </summary>
        /// <param name="key">The key to get the value for.</param>
        /// <returns>The value for the specified key.</returns>
        public string this[string key]
        {
            get { return this.Contents[key]; }
            set { this.Contents[key] = value; }
        }
	

        /// <summary>
        /// Creates an empty section.
        /// </summary>
        public IniSection() : this("")
        {            
        }

        /// <summary>
        /// Creates an empty section with a name.
        /// </summary>
        /// <param name="name">The name of the section.</param>
        public IniSection(string name)
        {
            this.Name = name;
            this.Contents = new Dictionary<string,string>();
        }
    
        #region IEnumerable Members

        public IEnumerator  GetEnumerator()
        {
 	        return _contents.GetEnumerator();
        }

        #endregion
    }
}
