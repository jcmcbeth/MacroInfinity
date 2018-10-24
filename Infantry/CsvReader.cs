using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Infantry.Csv {
    /// <summary>
    /// Reads Cvs formated data from a stream.
    /// </summary>
    public class CsvReader : IDisposable {
        private Stream _stream;
        private StreamReader _streamReader;
        private int _record;

        /// <summary>
        /// Gets the underlying stream used by the reader.
        /// </summary>
        public Stream BaseStream {
            get { return _stream; }
        }

        /// <summary>
        /// Creates a new CvsReader for the supplied stream.
        /// </summary>
        /// <param name="stream"></param>
        public CsvReader(Stream stream) {

            _stream = stream;
            _streamReader = new StreamReader(_stream);            
        }

        /// <summary>
        /// Reads one record from the stream.
        /// </summary>
        /// <returns>An array of the records fields.</returns>
        public string[] Read() {
            string line, result;
            StringBuilder field;
            List<string> fields;
            bool quoted, escaped;

            line = _streamReader.ReadLine();

            if (line == null)
                return null;

            fields = new List<string>();
            field = new StringBuilder();
            quoted = false;
            escaped = false;
            for (int i = 0; i < line.Length; i++) {
                
                if (line[i] == '\"') {
                    if (quoted) {                        
                    }
                    else {
                        quoted = true;
                    }
                } else if (line[i] == ',') {
                    if (quoted) {
                        field.Append(line[i]);
                    }
                    else {
                        result = field.ToString();
                        result = result.Trim();
                        
                        if (result.StartsWith("\"")) {
                            result = result.Substring(1);
                        }
                        if (result.EndsWith("\"")) {
                            result = result.Substring(0, result.Length - 1);
                        }

                        fields.Add(result);
                        field = new StringBuilder();
                    }
                } else {                    
                    field.Append(line[i]);
                }

                // Check for a new line in a quote
                if (quoted && i == (line.Length - 1)) {
                    // XXX: can't figure out how to get the new line character of the stream
                    field.Append("\n\r");
                    line += _streamReader.ReadLine();
                }
            }

            result = field.ToString();
            result = result.Trim();

            if (result.StartsWith("\"")) {
                result = result.Substring(1);
            }
            if (result.EndsWith("\"")) {
                result = result.Substring(0, result.Length - 1);
            }

            fields.Add(result);
            field = new StringBuilder();

            return fields.ToArray();
        }

        #region IDisposable Members

        /// <summary>
        /// Frees up unmanaged resources.
        /// </summary>
        public void Dispose() {
            if (_stream != null)
                _stream.Dispose();

            if (_streamReader != null)
                _streamReader.Dispose();
        }

        #endregion
    }
}
