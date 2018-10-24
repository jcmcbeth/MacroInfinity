using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlobContentsExtractor {
    public class ExtractionResults {
        private List<Blob> _blobFiles;

        private int _v4Files;
        private int _v3Files;
        private int _otherFiles;
        private int _unknownFiles;
        private int _wavFiles;

        private int _total;

        private Blob _lastBlob;

        /// <summary>
        /// Gets the last blob file read.
        /// </summary>
        public Blob LastBlobFile {
            get { return _lastBlob; }
        }

        /// <summary>
        /// Gets the total number of files contained in the blob files.
        /// </summary>
        public int ContentCount {
            get { return _total; }
            set { _total = value; }
        }

        /// <summary>
        /// Gets the number of blob files in the results.
        /// </summary>       
        public int BlobFileCount {
            get { return _blobFiles.Count; }
        }

        /// <summary>
        /// Gets an array of the blob filenames in the results.
        /// </summary>
        public List<Blob> BlobFiles {
            get { return _blobFiles; }
        }

        /// <summary>
        /// Gets the number of version 4 cfs files found.
        /// </summary>
        public int CfsVersion4Files {
            get { return _v4Files; }
            set { _v4Files = value; }
        }

        /// <summary>
        /// Gets the number of version 4 cfs files found.
        /// </summary>
        public int CfsVersion3Files {
            get { return _v3Files; }
            set { _v3Files = value; }
        }

        /// <summary>
        /// Gets the number of unknown version cfs files found.
        /// </summary>
        public int CfsVersionUnknownFiles {
            get { return _otherFiles; }
            set { _otherFiles = value; }
        }

        /// <summary>
        /// Gets the number of unexpected file types.
        /// </summary>
        public int UnknownFiles {
            get { return _unknownFiles; }
            set { _unknownFiles = value; }
        }

        /// <summary>
        /// Gets the number of WAV files found.
        /// </summary>
        public int WavFiles {
            get { return _wavFiles; }
            set { _wavFiles = value; }
        }

        public ExtractionResults() {
            _blobFiles = new List<Blob>();
        }

        /// <summary>
        /// Adds a blob to the list of blobs found.
        /// </summary>
        public void AddBlob(Blob blob) {
            _blobFiles.Add(blob);
            _lastBlob = blob;
        }
    }
}
