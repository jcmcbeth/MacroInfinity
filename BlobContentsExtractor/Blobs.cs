using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BlobContentsExtractor {
    [Serializable]
    public class Blobs {
        private List<Blob> _blobs;
        
        [XmlArrayItem("Blob")]
        public List<Blob> BlobFiles {
            get { return _blobs; }
            set { _blobs = value; }
        }

        public Blobs() {
        }

        public Blobs(List<Blob> blobs) {
            _blobs = blobs;
        }
    }
}
