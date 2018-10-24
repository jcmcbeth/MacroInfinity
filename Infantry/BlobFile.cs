using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
#if DEBUG
using System.Diagnostics;
#endif

namespace Infantry.Blob
{
    /// <summary>
    /// An Infantry blob file.
    /// </summary>
    public class BlobFile : IEnumerable
    {
        private int _version;
        private int _count;
        private List<BlobEntry> _entries;
        private string _filename;

        public int Version
        {
            get { return _version; }
        }

        /// <summary>
        /// Gets the number of files stored in the blob file.
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        /// Gets the filename of the blob file.
        /// </summary>
        public string Filename
        {
            get { return _filename; }
        }

        /// <summary>
        /// Loads the blob file information.
        /// </summary>
        private void Load()
        {
            FileStream stream;
            byte[] buffer;
            BlobEntry entry;
            int offset;
            int end;

            buffer = new byte[8];
            stream = File.OpenRead(_filename);
            stream.Read(buffer, 0, 8);
            
            _version = BitConverter.ToInt32(buffer, 0);
            _count = BitConverter.ToInt32(buffer, 4);

            offset = 0;

            if (_version == 1)
            {
                buffer = new byte[22 * _count];
                stream.Read(buffer, 0, buffer.Length);
                stream.Close();

                _entries = new List<BlobEntry>(_count);

                for (int i = 0; i < _count; i++)
                {
                    entry = new BlobEntry();
                    entry.Filename = ASCIIEncoding.ASCII.GetString(buffer, offset, 14); offset += 14;
                    end = entry.Filename.IndexOf('\0');
                    if (end > 0)
                        entry.Filename = entry.Filename.Substring(0, end);
                    entry.Offset = BitConverter.ToInt32(buffer, offset); offset += 4;
                    entry.Size = BitConverter.ToInt32(buffer, offset); offset += 4;

                    _entries.Add(entry);
                }

            }
            else if (_version == 2)
            {
                buffer = new byte[40 * _count];
                stream.Read(buffer, 0, buffer.Length);
                stream.Close();

                _entries = new List<BlobEntry>(_count);

                for (int i = 0; i < _count; i++)
                {
                    entry = new BlobEntry();
                    entry.Filename = ASCIIEncoding.ASCII.GetString(buffer, offset, 32); offset += 32;
                    end = entry.Filename.IndexOf('\0');
                    if (end > 0)
                        entry.Filename = entry.Filename.Substring(0, end);                    
                    entry.Offset = BitConverter.ToInt32(buffer, offset); offset += 4;
                    entry.Size = BitConverter.ToInt32(buffer, offset); offset += 4;

                    _entries.Add(entry);
                }
            }
            else
            {
                throw new Exception("Unknown file version: " + _version);            
            }
        }

        /// <summary>
        /// Loads a blob file.
        /// </summary>
        /// <param name="filename">The filename of the blob file.</param>
        public BlobFile(string filename)
        {
            _filename = filename;
            _count = 0;
            _version = 2;

            _entries = new List<BlobEntry>();

            Load();            
        }

        public BlobEntry GetEntry(string filename)
        {
            foreach (BlobEntry entry in _entries)
            {
                if (entry.Filename.Equals(filename, StringComparison.OrdinalIgnoreCase))
                    return entry;
            }

            return null;
        }

        /// <summary>
        /// Extracts the specified entry to the specified location.
        /// </summary>
        /// <param name="entryFilename">The filename of the blob entry.</param>
        /// <param name="destination">The directory to extract it to.</param>
        public void Extract(string entryFilename, string path)
        {
            FileStream input, output;
            byte[] buffer;
            BlobEntry entry;

            entry = GetEntry(entryFilename);
            if (entry != null)
            {
#if DEBUG
                Trace.WriteLine(_filename, "Filename");
                Trace.WriteLine(path + entry.Filename, "Entry Filename");                
#endif
                input = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                output = new FileStream(path + "\\" + entry.Filename, FileMode.Create, FileAccess.Write, FileShare.None);            

                buffer = new byte[entry.Size];

                input.Seek((long)entry.Offset, SeekOrigin.Begin);            
                input.Read(buffer, 0, buffer.Length);
                output.Write(buffer, 0, buffer.Length);                

                input.Close();
                output.Close();
            }
        }

        /// <summary>
        /// Extracts an entry from the blob file and puts the bytes into memory.
        /// </summary>
        /// <param name="entryFilename">The filename of the entry to extract to memory.</param>
        /// <returns>The bytes of the entry.</returns>
        public byte[] ExtractBytes(string entryFilename)
        {
            BlobEntry entry;
            byte[] buffer;

            entry = GetEntry(entryFilename);
            buffer = ExtractBytes(entry);

            return buffer;
        }

        /// <summary>
        /// Extracts an entry from the blob file and puts the bytes into memory.
        /// </summary>
        /// <param name="entry">The BlobEntry to extract to memory.</param>
        /// <returns>The bytes of the entry in the blob file.</returns>
        public byte[] ExtractBytes(BlobEntry entry)
        {
            FileStream input;
            byte[] buffer;

            if (entry == null)
                throw new ArgumentNullException("entry");

#if DEBUG
            Trace.WriteLine(_filename, "Filename");            
#endif

            buffer = new byte[entry.Size];

            input = File.OpenRead(_filename);
            input.Seek((long)entry.Offset, SeekOrigin.Begin);
            input.Read(buffer, 0, buffer.Length);

            return buffer;
        }

        private void Save()
        {
            FileStream originalStream, tempStream;
            BinaryWriter writer;
            int size;
            int headerLength;
            int offset;
            byte[] buffer, data;
            string filename;

            filename = Path.GetTempFileName();

            originalStream = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            tempStream = new FileStream(filename, FileMode.Open, FileAccess.Write, FileShare.None);

            writer = new BinaryWriter(tempStream);

            writer.Write(_version);
            writer.Write(_count);

            if (_version == 1)            
                size = 14;            
            else            
                size = 32;

            headerLength = size * _count;

            // Write the header
            offset = headerLength;
            for (int i = 0; i < _entries.Count; i++)
            {                                
                // Make sure the string is the correct length
                data = ASCIIEncoding.ASCII.GetBytes(_entries[i].Filename);
                buffer = new byte[size];
                Array.Clear(buffer, 0, buffer.Length);
                Array.Copy(data, 0, buffer, 0, data.Length);

                writer.Write(buffer);   // filename
                _entries[i].Offset = offset;
                writer.Write(_entries[i].Offset);   // offset
                offset += _entries[i].Size;
                writer.Write(_entries[i].Size);   // file size
            }

            // Write the file data
            foreach (BlobEntry entry in _entries)
            {
                originalStream.Seek((long)entry.Offset, SeekOrigin.Begin);
                buffer = new byte[entry.Size];

                originalStream.Read(buffer, 0, buffer.Length);
                writer.Write(buffer);
            }

            originalStream.Close();
            tempStream.Close();

            File.Delete(_filename);
            File.Move(filename, _filename);
        }

        public void Remove(string entryFilename)
        {
            BlobEntry entry;

            entry = GetEntry(entryFilename);
            if (entry != null)
            {
                _entries.Remove(entry);

                _count--;

                Save();
            }
        }

        public void Add(string filename)
        {
            FileStream originalStream, tempStream;
            BinaryWriter writer;
            int size;
            int headerLength;
            int offset;
            string tempname;
            byte[] buffer, data;
            BlobEntry item;
            FileInfo info;

            info = new FileInfo(filename);

            item = new BlobEntry();
            item.Filename = Path.GetFileName(filename);
            item.Size = (int)info.Length;            

            _count++;

            tempname = Path.GetTempFileName();

            originalStream = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            tempStream = new FileStream(tempname, FileMode.Open, FileAccess.Write, FileShare.None);

            writer = new BinaryWriter(tempStream);

            writer.Write(_version);
            writer.Write(_count);

            if (_version == 1)
                size = 14;
            else
                size = 32;

            headerLength = size * _count;

            // Write the header
            offset = headerLength;
            for (int i = 0; i < _entries.Count; i++)
            {
                // Make sure the string is the correct length
                data = ASCIIEncoding.ASCII.GetBytes(_entries[i].Filename);
                buffer = new byte[size];
                Array.Clear(buffer, 0, buffer.Length);
                Array.Copy(data, 0, buffer, 0, data.Length);

                writer.Write(buffer);   // filename
                _entries[i].Offset = offset;
                writer.Write(_entries[i].Offset);   // offset
                offset += _entries[i].Size;
                writer.Write(_entries[i].Size);   // file size
            }

            // Make sure the string is the correct length
            data = ASCIIEncoding.ASCII.GetBytes(item.Filename);
            buffer = new byte[size];
            Array.Clear(buffer, 0, buffer.Length);
            Array.Copy(data, 0, buffer, 0, data.Length);

            writer.Write(buffer);   // filename
            item.Offset = offset;
            writer.Write(item.Offset);   // offset
            offset += item.Size;
            writer.Write(item.Size);   // file size

            // Write the file data
            foreach (BlobEntry entry in _entries)
            {
                originalStream.Seek((long)entry.Offset, SeekOrigin.Begin);
                buffer = new byte[entry.Size];

                originalStream.Read(buffer, 0, buffer.Length);
                writer.Write(buffer);
            }

            buffer = File.ReadAllBytes(filename);
            writer.Write(buffer);

            originalStream.Close();
            tempStream.Close();

            _entries.Add(item);

            File.Delete(_filename);
            File.Move(tempname, _filename);
        }

        /// <summary>
        /// Converts a version 1 file to a verson 2 file.
        /// </summary>
        public void Convert()
        {
            if (_version == 1)
            {
                _version = 2;
                Save();
            }
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        #endregion
    }
}
