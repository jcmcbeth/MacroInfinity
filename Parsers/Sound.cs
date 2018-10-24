using System;
using System.Collections.Generic;
using System.Text;

namespace Infantry.Items
{
    /// <summary>
    /// Sounds data for an item.
    /// </summary>
    public class Sound
    {
        private string _blob;
        private string _filename;
        private int _simultaneous;
        private int _unknown;

        /// <summary>
        /// Unknown
        /// </summary>
        public int Unknown
        {
            get { return _unknown; }
            set { _unknown = value; }
        }

        /// <summary>
        /// The number of times the file can be played simultaneously.
        /// </summary>
        public int Simultaneous
        {
            get { return _simultaneous; }
            set { _simultaneous = value; }
        }

        /// <summary>
        /// The filename of the blob file the sound is in.
        /// </summary>
        public string BlobFilename
        {
            get { return _blob; }
            set { _blob = value; }
        }

        /// <summary>
        /// The filename of the wav file in the blob file.
        /// </summary>
        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        public Sound()
        {
            _blob = "None";
            _filename = "None";
        }
    }
}
