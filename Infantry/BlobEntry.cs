using System;
using System.Collections.Generic;
using System.Text;

namespace Infantry.Blob
{
    /// <summary>
    /// A file stored in a blow file.
    /// </summary>
    public class BlobEntry
    {
        private int _offset;
        private int _size;
        private string _filename;

        public int Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }        

        public BlobEntry()
        {
        }
    }
}
