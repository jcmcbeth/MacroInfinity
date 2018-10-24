using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Serialization;

namespace BlobContentsExtractor {
    [Serializable]
    [XmlRoot("Blob")]
    public class Blob {
        private List<BlobContent> _contents;
        private int _version;
        private string _path;
        private string _name;
        private int _fileSize;
        private string _extention;

        public Blob() {
            _contents = new List<BlobContent>();
        }

        [XmlAttribute("Name")]
        public string Name {
            get { return _name; }
            set { _name = value; }
        }

        [XmlAttribute("Version")]
        public int Version {
            get { return _version; }
            set { _version = value; }
        }

        [XmlAttribute("Size")]
        public int FileSize {
            get { return _fileSize; }
            set { _fileSize = value; }
        }

        [XmlAttribute("Extention")]
        public string Extention {
            get { return _extention; }
            set { _extention = value; }
        }

        [XmlAttribute("Path")]
        public string Path {
            get { return _path; }
            set { _path = value; }
        }                

        [XmlArrayItem("File")]
        public List<BlobContent> Files {
            get { return _contents; }
        }
        
        public void AddFile(BlobContent content) {
            _contents.Add(content);
        }
    }
}
