using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Infantry.Cfs
{
    /// <summary>
    /// Unknown
    /// </summary>
    //public enum BlitMode : byte
    //{
    //    Normal = 0,
    //    Alpha25Percent = 1,
    //    Alpha33Percent = 2,
    //    Alpha50Percent = 3,
    //    Alpha66Percent = 4,
    //    Alpha75Percent = 5,        
    //}

    /// <summary>
    /// Determines have infantry treats the rows of a CFS file.
    /// </summary>
    //public enum RowMode : byte
    //{
    //    /// <summary>
    //    /// The rows do not have any special meaning.
    //    /// </summary>
    //    None = 0,

    //    /// <summary>
    //    /// Unknown
    //    /// </summary>
    //    RotationPoints = 1,

    //    /// <summary>
    //    /// Unknown
    //    /// </summary>
    //    HeightPoints = 2
    //}

    /// <summary>
    /// Unknown
    /// </summary>
    //public enum SortTransform : byte
    //{
    //    Normal = 0,
    //    DiagonalLeft = 1,
    //    DiagonalRight = 2,
    //    HorizontalThin = 3,
    //    VerticalThink = 4,
    //    Intermediate = 5
    //}

    /// <summary>
    /// These flags indicate which frames are redundant and can be referenced 
    /// again.
    /// </summary>
    //[Flags]
    //public enum CompressionFlags : byte {

    //    /// <summary>
    //    /// Duplicate each row and flip the frame vertically.
    //    /// </summary>
    //    DupeRowVertical = 0x01,

    //    /// <summary>
    //    /// Duplicate each row and flip the frame horizontally.
    //    /// </summary>
    //    DupeRowHorizontal = 0x02,

    //    /// <summary>
    //    /// Columns are half rotations, mirror columns to other half.
    //    /// </summary>
    //    ColumnHalfRotation = 0x04,

    //    /// <summary>
    //    /// Columns are quarter rotations, mirror columns to the other quadrants.
    //    /// </summary>
    //    ColumnQuarterRotation = 0x08,

    //    /// <summary>
    //    /// There are only lights and shadows.
    //    /// </summary>
    //    NoPixels = 0x10,

    //    /// <summary>
    //    /// Rows are half rotation, mirror them to the other half.
    //    /// </summary>
    //    RowHalfRotation = 0x20,

    //    /// <summary>
    //    /// Rows are quarter rotation, mirror them to the other quadrants.
    //    /// </summary>
    //    RowQuarterRotation = 0x40,

    //    /// <summary>
    //    /// No compression is used.
    //    /// </summary>
    //    None = 0x80
    //}

    /// <summary>
    /// A parser for an Infantry CFS file.
    /// </summary>
    public class CfsFile_Refactor
    {
        /// <summary>
        /// The individual data for each frame in a CFS file.
        /// </summary>
        private struct Frame {
            public int Row;
            public int Column;
            /// <summary>
            /// The Y-Coordinate offset of the transparent frame within a frame.
            /// </summary>
            public int YOffset;
            public int XOffset;
            public int Width;
            public int Height;
            public int DataOffset;
        }

        private int _version;
        private int _frameCount;
        private int _animationTime;
        private int _width;
        private int _height;
        private int _rows;
        private int _columns;
        private int _shadowLength;
        private int _lightLength;
        private int _userDataLength;
        //private CompressionFlags _compression;
        private int _maxSolidIndex;
        private int _dataSize;
        private string _category;
        private BlitMode _blitMode;
        private RowMode _rowMode;
        private int _sortAdjust;
        private SortTransform _sortTransform;
        private int _userPaletteStart;
        private int _userPaletteLength;
        private string _description;
        private byte[] _unknownData;
        private int[] _palette;
        private byte[] _userData;
        private int _totalSize;
        private byte[] _data;

        /// <summary>
        /// Gets the palette as an array of Color objects.
        /// </summary>
        /// <returns>Array of Color objects.</returns>
        public Color[] GetColorPalette() {
            Color[] colors = new Color[256];

            for (int i = 0; i < 256; i++) {
                colors[i] = Color.FromArgb(_palette[i]);
            }

            return colors;
        }

        /// <summary>
        /// Gets the user data, the function of this is unknown.
        /// </summary>
        public byte[] UserData
        {
            get { return _userData; }
        }

        /// <summary>
        /// Gets and sets the colors used by the sprite.
        /// </summary>
        public int[] Palette
        {
            get { return _palette; }
            set { _palette = value; }
        }

        /// <summary>
        /// A 32 byte long bit of unknown information.
        /// </summary>
        public byte[] UnknownData
        {
            get { return _unknownData; }
        }

        /// <summary>
        /// Gets and sets a short description of the sprite.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Gets the number of user colors.
        /// </summary>
        public int UserPaletteLength
        {
            get { return _userPaletteLength; }
        }

        /// <summary>
        /// The starting index of the user colors.
        /// </summary>
        public int UserPaletteStart
        {
            get { return _userPaletteStart; }
        }

        /// <summary>
        /// The function of this value is unknown.
        /// </summary>
        public SortTransform SortTransform
        {
            get { return _sortTransform; }
            set { _sortTransform = value; }
        }

        /// <summary>
        /// The function of this value is unknown.
        /// </summary>
        public int SortAdjust
        {
            get { return _sortAdjust; }
            set { _sortAdjust = value; }
        }

        /// <summary>
        /// Gets and sets how the sprite's rows are handled.
        /// </summary>
        public RowMode RowMode
        {
            get { return _rowMode; }
            set { _rowMode = value; }
        }

        /// <summary>
        /// The function of this is unknown.
        /// </summary>
        public BlitMode BlitMode
        {
            get { return _blitMode; }
            set { _blitMode = value; }
        }

        /// <summary>
        /// Gets and sets a short name or category to identify the sprite.
        /// </summary>
        /// <remarks>Any string longer than 14 characters will be truncated.</remarks>
        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }

        /// <summary>
        /// Gets the total size of all the bitmap data for all the frames.
        /// </summary>
        public int DataSize
        {
            get { return _dataSize; }
        }

        /// <summary>
        /// Gets the index of the last color that is a solid color.
        /// </summary>
        /// <remarks>The other colors are either transparent, lights, or shadows.</remarks>
        public int MaxSolidIndex
        {
            get { return _maxSolidIndex; }
        }

        /// <summary>
        /// Gets the compression settings.
        /// </summary>
        //public CompressionFlags Compression
        //{
        //    get { return _compression; }
        //}

        /// <summary>
        /// Gets the length of the user data.
        /// </summary>
        public int UserDataLength
        {
            get { return _userDataLength; }
        }

        /// <summary>
        /// Gets the number of lights in the palette.
        /// </summary>
        public int Lights
        {
            get { return _lightLength; }
        }

        /// <summary>
        /// Gets the number of shadows in the palette.
        /// </summary>
        public int Shadows
        {
            get { return _shadowLength; }
        }

        /// <summary>
        /// Gets the number of columns of frames.
        /// </summary>
        public int Columns
        {
            get { return _columns; }
        }

        /// <summary>
        /// Gets the number of rows of frames.
        /// </summary>
        public int Rows
        {
            get { return _rows; }
        }

        /// <summary>
        /// Gets the width of the sprite.
        /// </summary>
        public int Width
        {
            get { return _width; }
        }

        /// <summary>
        /// Gets the height of the sprite.
        /// </summary>
        public int Height
        {
            get { return _height; }
        }
        
        /// <summary>
        /// Gets and sets the amount of time to animate all the frames.
        /// </summary>
        public int AnimationTime
        {
            get { return _animationTime; }
            set { _animationTime = value; }
        }

        /// <summary>
        /// Gets the file format version.
        /// </summary>
        public int Version
        {
            get { return _version; }
        }

        /// <summary>
        /// Gets the number of frames in the file.
        /// </summary>
        public int FrameCount
        {
            get { return _frameCount; }
        }

        /// <summary>
        /// Gets the total number of bytes in the file.
        /// </summary>
        public int TotalSize
        {
            get { return _totalSize; }
        }

        public byte[] BitmapData {
            get { return _data; }
        }

        /// <summary>
        /// Creates a new CFS file.
        /// </summary>
        /// <param name="path">The path of an existing CFS file.</param>
        public CfsFile_Refactor(string path)
        {
            FileStream stream;
            byte[] buffer;

            if (!File.Exists(path))
                throw new FileNotFoundException();            

            stream = new FileStream(path, FileMode.Open);
            buffer = new byte[stream.Length];
            stream.Read(buffer, 0, (int)stream.Length);                        

            Parse(buffer);

            stream.Close();
        }

        /// <summary>
        /// Parses a CFS file.
        /// </summary>
        private void Parse(byte[] buffer)
        {            
            int offset = 0;
            Frame[] frames;
            byte[] frameData;

            _totalSize = buffer.Length;

            _version = (int)BitConverter.ToInt16(buffer, offset); offset += 2;

            if (_version == 4) {
                _frameCount = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                if (_frameCount <= 0)
                    throw new Exception("There is an invalid number of frames: " + _frameCount);

                _animationTime = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                _width = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                _height = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                _rows = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                _columns = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                _lightLength = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                _shadowLength = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                _userDataLength = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                //_compression = (CompressionFlags)buffer[offset]; offset++;                                    
                _maxSolidIndex = (int)buffer[offset]; offset++;
                _dataSize = (int)BitConverter.ToInt32(buffer, offset); offset += 4;
                _category = ASCIIEncoding.ASCII.GetString(buffer, offset, 16); offset += 16;
                _blitMode = (BlitMode)buffer[offset]; offset++;
                _rowMode = (RowMode)buffer[offset]; offset++;
                _sortAdjust = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                _sortTransform = (SortTransform)buffer[offset]; offset++;
                _userPaletteStart = (int)buffer[offset]; offset++;
                _userPaletteLength = (int)buffer[offset]; offset++;
                _description = ASCIIEncoding.ASCII.GetString(buffer, offset, 48); offset += 48;

                _unknownData = new byte[32];
                Buffer.BlockCopy(buffer, offset, _unknownData, 0, _unknownData.Length);
                offset += _unknownData.Length;

                _palette = new int[256];
                for (int i = 0; i < _palette.Length; i++) {
                    int argb;

                    argb = BitConverter.ToInt32(buffer, offset); offset += 4;
                    argb |= unchecked((int)0xFF000000);

                    _palette[i] = argb;
                }

                frames = new Frame[_frameCount];

                // Read all the frame meta info
                for (int i = 0; i < _frameCount; i++) {
                    frames[i] = ReadFrameInfo(buffer, offset);
                    offset += 12;

                    frames[i].Row = i / _columns;
                    frames[i].Column = i - (frames[i].Row * _columns);
                }      

                // Read the user data
                if (_userDataLength > 0) {
                    _userData = new byte[_userDataLength];
                    Array.Copy(buffer, offset, _userData, 0, _userDataLength);

                    offset += _userDataLength;
                } else {
                    _userData = new byte[] { };
                }

                _data = new byte[_width * _height * _frameCount];

                // Decode the frames
                foreach (Frame frame in frames) {
                    //frameData = DecodeFrameData(frame, buffer, offset);

                    // TODO: finish me
                }

                //DecodeFrameData(frames[17], buffer, offset);



                //foreach (Frame frame in frames) {
                //    DecodeFrameData(frame, buffer, offset);
                //}                
                //frameData = DecodeFrameData(frames[0], buffer, offset);
            }
            else
            {
                throw new Exception("Unknown file version: " + _version);
            }
        }

        public Bitmap ToBitmap() {
            Bitmap bitmap;
            BitmapData bitmapData;
            ColorPalette palette; 
            int totalWidth, totalHeight;

            totalWidth = _width * _columns;
            totalHeight = _height * Rows;

            bitmap = new Bitmap(totalWidth, totalHeight, PixelFormat.Format8bppIndexed);
            palette = bitmap.Palette;

            for (int i = 0; i < _palette.Length; i++) {
                palette.Entries[i] = Color.FromArgb(_palette[i]);
            }
            bitmap.Palette = palette;
            bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, totalWidth, totalHeight),
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed);

            Marshal.Copy(_data, 0, bitmapData.Scan0, _data.Length);
            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }

        /// <summary>
        /// Reads frame information from a buffer.
        /// </summary>        
        /// <param name="buffer">The buffer containing the CFS file data.</param>
        /// <param name="offset">The location of the frame in the buffer.</param>
        /// <returns>The frame.</returns>
        private Frame ReadFrameInfo(byte[] buffer, int offset)
        {
            Frame frame;

            frame = new Frame();
            frame.XOffset = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
            frame.YOffset = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
            frame.Width = (int)BitConverter.ToInt16(buffer, offset); offset += 2;            
            frame.Height = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
            frame.DataOffset = (int)BitConverter.ToInt32(buffer, offset); offset += 4;

            return frame;
        }

        /// <summary>
        /// Decodes data from the input buffer and stores it in another data buffer.
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="buffer">Input buffer that contains the encoded data.</param>
        /// <param name="offset">Index where the frame data begins.</param>
        private void DecodeFrame(
            Frame frame,
            byte[] src,
            int srcIndex,
            byte[] dest,
            int destIndex)
        {
            int frameHeight;
            int frameWidth;
            int bufferOffset;
            int dataOffset;         
            int frameNumber;
            int framePixels;
            int lineLength;
            int lineOffset;

            // The height and width can have negative values with special meaning.
            frameHeight = Math.Abs(frame.Height);
            frameWidth = Math.Abs(frame.Width);

            frameNumber = (frame.Row * _columns) + frame.Column;

            //if (compressed) {
                //// Decode the data
                //    lineLength = buffer[offset + line];

                //    decodedLine = DecodeLine(buffer, offset + height, lineLength, width);

                //    Array.Copy(decodedLine, 0, croppedData, line * width, width);
//                ////data = UnCrop(frame, croppedData);       

//                framePixels = _width * _height;                

//                int start = offset;
//                int dataStart = offset + height;

//                lineOffset = frame.DataOffset;
//                bufferOffset = offset + height;
//                dataOffset = 0;
//                for (int line = 0; line < frame.Height; line++) {
//                    // Length of the encoded data for the current line
//                    lineLength = buffer[lineOffset++];
             
////                    dataOffset = (framePixels * _columns * frame.Row) + (frame.Column * _width) + (line * _columns * _width);

//                    DecodeLine(buffer, bufferOffset, _data, dataOffset, lineLength);

//                    dataOffset += line * _width;
//                    bufferOffset += lineLength;
//                }

            //    lineOffset = offset + frame.DataOffset;
            //    bufferOffset = offset + frame.DataOffset + height;
            //    dataOffset = _width * frame.YOffset;
            //    for (int line = 0; line < frame.Height; line++) {
            //        lineLength = buffer[lineOffset++];

            //        System.Diagnostics.Debug.WriteLine("Scanline " + line + ": ");
            //        DecodeLine(buffer, bufferOffset, _data, dataOffset + frame.XOffset, lineLength);

            //        bufferOffset += lineLength;
            //        dataOffset += _width;
            //    }
            //} else {                
            //    framePixels = _width * _height;

            //    for (int line = 0; line < frame.Height; line++) {
            //        bufferOffset = offset + (framePixels * frameNumber) + (line * _width);
            //        dataOffset = (framePixels * _columns * frame.Row) + (frame.Column * _width) + (line * _columns * _width);                    

            //        if (compressed) {
            //        } else {
            //            Array.Copy(buffer, bufferOffset, _data, dataOffset, frame.Width);
            //        }
            //    }
            //}            
        }

        /// <summary>
        /// Takes a line of encoded data and decodes it.
        /// </summary>
        private void DecodeLine(byte[] src, int srcIndex, byte[] dest, int destIndex, int length) {
            byte current;
            int data, run;
            int destOffset, srcOffset;
            int index, j; 
           
            index = 0;
            destOffset = destIndex;
            srcOffset = srcIndex;
            while (index < length) {                                
                // Get the current character to decode
                current = src[srcOffset++];
                run = current >> 4;
                data = current & 0x0F;

                System.Diagnostics.Debug.WriteLine(String.Format("0x{0:X2}", current));

                // The transparent pixels are already zero in the buffer, so skip them.
                destOffset += run;
            
                // Copy the regular pixel data
                for (j = 0; j < data; j++) {                   
                    dest[destOffset++] = src[srcOffset++];
                }

                // There were data bytes read and 1 encoding byte read.
                index += data + 1;
            }
        }
    }
}
