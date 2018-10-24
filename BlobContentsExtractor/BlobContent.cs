using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Serialization;

namespace BlobContentsExtractor {
    public enum FileType {
        [XmlEnum("Cfs")]
        Cfs,
        [XmlEnum("Wav")]
        Wav
    }

    [XmlRoot("File")]
    public class BlobContent {
        private int _size;
        private string _name;        
        private FileType _type;

        [XmlAttribute("Type")]
        public FileType Type {
            get { return _type; }
            set { _type = value; }
        }

        [XmlAttribute("Size")]
        public int Size {
            get { return _size; }
            set { _size = value; }
        }        

        [XmlElement("Name")]
        public string Name {
            get { return _name; }
            set { _name = value; }
        }        
    }
}
