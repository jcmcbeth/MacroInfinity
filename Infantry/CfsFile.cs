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
    public enum BlitMode : byte
    {
        Normal = 0,
        Alpha25Percent = 1,
        Alpha33Percent = 2,
        Alpha50Percent = 3,
        Alpha66Percent = 4,
        Alpha75Percent = 5,
    }

    /// <summary>
    /// Determines have infantry treats the rows of a CFS file.
    /// </summary>
    public enum RowMode : byte
    {
        /// <summary>
        /// The rows do not have any special meaning.
        /// </summary>
        None = 0,

        /// <summary>
        /// Unknown
        /// </summary>
        RotationPoints = 1,

        /// <summary>
        /// Unknown
        /// </summary>
        HeightPoints = 2
    }

    /// <summary>
    /// Unknown
    /// </summary>
    public enum SortTransform : byte
    {
        Normal = 0,
        DiagonalLeft = 1,
        DiagonalRight = 2,
        HorizontalThin = 3,
        VerticalThink = 4,
        Intermediate = 5
    }

    /// <summary>
    /// These flags indicate which frames are redundant and can be referenced 
    /// again.
    /// </summary>
    [Flags]
    public enum CompressionFlags : byte
    {

        /// <summary>
        /// Duplicate each row and flip the frame vertically.
        /// </summary>
        DupeRowVertical = 0x01,

        /// <summary>
        /// Duplicate each row and flip the frame horizontally.
        /// </summary>
        DupeRowHorizontal = 0x02,

        /// <summary>
        /// Columns are half rotations, mirror columns to other half.
        /// </summary>
        ColumnHalfRotation = 0x04,

        /// <summary>
        /// Columns are quarter rotations, mirror columns to the other quadrants.
        /// </summary>
        ColumnQuarterRotation = 0x08,

        /// <summary>
        /// There are only lights and shadows.
        /// </summary>
        NoPixels = 0x10,

        /// <summary>
        /// Rows are half rotation, mirror them to the other half.
        /// </summary>
        RowHalfRotation = 0x20,

        /// <summary>
        /// Rows are quarter rotation, mirror them to the other quadrants.
        /// </summary>
        RowQuarterRotation = 0x40,

        /// <summary>
        /// No compression is used.
        /// </summary>
        None = 0x80
    }

    /// <summary>
    /// A parser for an Infantry CFS file.
    /// </summary>
    public class CfsFile
    {
        /// <summary>
        /// The individual data for each frame in a CFS file.
        /// </summary>
        private struct Frame
        {
            /// <summary>
            /// The Y-Coordinate offset of the transparent frame within a frame.
            /// </summary>
            public int YOffset;
            public int XOffset;
            public int Width;
            public int Height;
            public int DataOffset;
            public byte[] Data;
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
        private CompressionFlags _compression;
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
        private Frame[] _frames;
        private int _totalSize;

        public Color[] ColorPalette
        {
            get
            {
                Color[] colors = new Color[256];

                for (int i = 0; i < 256; i++)
                {
                    colors[i] = Color.FromArgb(_palette[i]);
                }

                return colors;
            }
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
        public CompressionFlags Compression
        {
            get { return _compression; }
        }

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

        /// <summary>
        /// Creates a new CFS file.
        /// </summary>
        /// <param name="path">The path of an existing CFS file.</param>
        public CfsFile(string path)
        {
            FileStream stream;
            byte[] buffer;

            if (!File.Exists(path))
                throw new FileNotFoundException();

            stream = new FileStream(path, FileMode.Open);
            buffer = new byte[stream.Length];
            stream.Read(buffer, 0, (int)stream.Length);

            Parse(buffer);
        }

        private CfsFile()
        {
        }

        /// <summary>
        /// Parses a CFS file.
        /// </summary>
        private void Parse(byte[] buffer)
        {
            int offset = 0;

            _totalSize = buffer.Length;

            _version = (int)BitConverter.ToInt16(buffer, offset); offset += 2;

            if (_version == 4)
            {
                _frameCount = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                if (_frameCount <= 0)
                    throw new Exception("There is an invalid number of frames: " + _frameCount);

                _animationTime = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                _width = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                _height = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                _rows = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                _columns = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                // TODO: docs say shadows then lights; verify
                _lightLength = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                _shadowLength = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                _userDataLength = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                _compression = (CompressionFlags)buffer[offset]; offset++;
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
                for (int i = 0; i < _palette.Length; i++)
                {
                    int argb;

                    argb = BitConverter.ToInt32(buffer, offset); offset += 4;
                    argb |= unchecked((int)0xFF000000);

                    _palette[i] = argb;
                }

                _frames = new Frame[_frameCount];

                // Read all the frame meta info
                for (int i = 0; i < _frameCount; i++)
                {
                    _frames[i] = ReadFrameInfo(buffer, offset);
                    offset += 12;
                }

                // Read the user data
                if (_userDataLength > 0)
                {
                    _userData = new byte[_userDataLength];
                    Array.Copy(buffer, offset, _userData, 0, _userDataLength);

                    offset += _userDataLength;
                }
                else
                {
                    _userData = new byte[] { };
                }

                // Read the frames
                for (int i = 0; i < _frameCount; i++)
                {
                    //_frames[i].Data = ReadFrameData(_frames[i], buffer, offset, _compression);
                }
            }
            else
            {
                throw new Exception("Unknown file version: " + _version);
            }
        }

        /// <summary>
        /// This converts the CFS file into a list of bitmaps, one for each frame.
        /// </summary>
        /// <returns></returns>
        public Bitmap[] GetBitmapFrames()
        {
            Bitmap bitmap;
            BitmapData data;
            List<Bitmap> bitmaps;

            return new Bitmap[0];

            bitmaps = new List<Bitmap>(_frameCount);

            foreach (Frame frame in _frames)
            {
                bitmap = new Bitmap(_width, _height, PixelFormat.Format8bppIndexed);
                for (int i = 0; i < 256; i++)
                {
                    bitmap.Palette.Entries[i] = Color.FromArgb(_palette[i]);
                }

                data = bitmap.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.WriteOnly,
                    PixelFormat.Format8bppIndexed);

                //Marshal.Copy(frame.Data, 0, data.Scan0, frame.Data.Length);

                int offset = 0;
                long ptr = data.Scan0.ToInt64();
                for (int i = 0; i < _height; i++)
                {
                    Marshal.Copy(frame.Data, offset, new IntPtr(ptr), _width * 3);
                    offset += _width * 3;
                    ptr += data.Stride;
                }

                bitmap.UnlockBits(data);

                bitmaps.Add(bitmap);
            }

            return bitmaps.ToArray();
        }

        //public Bitmap GetBitmap() {
        //    Bitmap bitmap;
        //    GifBitmapEncoder encoder;
        //    MemoryStream stream;
        //    BitmapPalette palette;
        //    List<System.Windows.Media.Color> paletteColors;
        //    System.Windows.Media.Color mediaColor;
        //    byte alpha, red, green, blue;

        //    // Jump through hoops to Create a bitmap palette.
        //    paletteColors = new List<System.Windows.Media.Color>();
        //    for (int i = 0; i < _palette; i++) {
        //        alpha = (_palette[i] & 0xFF000000) >> 24;
        //        red = (_palette[i] & 0x00FF0000) >> 16;
        //        green = (_palette[i] & 0x0000FF00) >> 8;
        //        blue = (_palette[i] & 0x000000FF);

        //        mediaColor = System.Windows.Media.Color.FromArgb(alpha, red, green, blue);                
        //        paletteColors.Add(mediaColor);
        //    }

        //    palette = new BitmapPalette(paletteColors);

        //    BitmapSource.Create(_width, _height, 96, 96, System.Windows.Media.PixelFormats.Indexed8, palette, _frames[0].Data, 0);

        //    stream = new MemoryStream();            
        //}



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
        /// Reads the bitmap data for the frame from a buffer and decompresses it.
        /// </summary>
        /// <param name="frame">The frame to read.</param>
        /// <param name="buffer">The buffer to read the frame from.</param>
        /// <param name="dataOffset">The index where the frame data for the file begins.</param>
        /// <param name="flags">The details of the compression.</param>
        /// <returns>The decompressed pixel data.</returns>
        private byte[] ReadFrameData(Frame frame, byte[] buffer, int dataOffset,
            CompressionFlags flags)
        {
            byte[] croppedData, data;
            int croppedSize;
            int height, width;
            int lineLength;
            bool compressed;
            byte[] decodedLine;
            int stride = CalculateStride(_width);

            height = Math.Abs(frame.Height);
            width = Math.Abs(frame.Width);

            croppedSize = frame.Width * frame.Height;
            croppedData = new byte[croppedSize];

            compressed = (CompressionFlags.None & flags) != CompressionFlags.None;

            if (compressed)
            {
                // Decode the data
                for (int line = 0; line < frame.Height; line++)
                {
                    lineLength = buffer[dataOffset + line];

                    decodedLine = DecodeLine(buffer, dataOffset + height, lineLength, width);

                    Array.Copy(decodedLine, 0, croppedData, line * width, width);
                }

                data = UnCrop(frame, croppedData);
            }
            else
            {
                data = new byte[(_width * stride) * _height];
                Array.Copy(buffer, dataOffset, data, 0, data.Length);
            }

            return data;
        }

        /// <summary>
        /// Takes a frame width cropped data and expands it to the original frame size.
        /// </summary>
        /// <param name="frame">The frame to uncrop.</param>
        /// <param name="buffer">The cropped pixel data for the frame.</param>
        /// <returns>The pixel data restored to the original size.</returns>
        private byte[] UnCrop(Frame frame, byte[] buffer)
        {
            byte[] result;
            int width, height;
            int resultIndex, bufferIndex;
            int stride = CalculateStride(_width);

            width = Math.Abs(frame.Width);
            height = Math.Abs(frame.Height);

            result = new byte[(_width * stride) * _height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    resultIndex = ((frame.YOffset + y) * height) + (x + frame.XOffset);
                    bufferIndex = (y * height) + x;

                    result[resultIndex] = buffer[bufferIndex];
                }
            }

            return result;
        }

        /// <summary>
        /// Takes a line of encoded data and decodes it.
        /// </summary>
        /// <param name="buffer">An array of bytes containing the encoded data.</param>
        /// <param name="offset">The offset to the encoded data.</param>
        /// <param name="length">The length of the encoded data.</param>
        /// <param name="decodedLength">The length of the decoded data.</param>
        /// <returns>The decoded data.</returns>
        private byte[] DecodeLine(byte[] buffer, int offset, int length, int decodedLength)
        {
            byte[] decodedData;
            byte current;
            int data, run;
            int decodedOffset;
            int i, j;

            decodedData = new byte[decodedLength];

            i = 0;
            decodedOffset = 0;
            while (i < length)
            {
                // Get the current character to decode
                current = buffer[offset + i];
                run = current >> 4;
                data = current & 0x0F;

                // Expand the transparent run
                for (j = 0; j < run; j++)
                {
                    decodedData[decodedOffset++] = 0;
                }

                i++;

                // Copy the regular pixel data
                for (j = 0; j < data; j++)
                {
                    decodedData[decodedOffset++] = buffer[offset + i + j]; // This is confusing!
                }
            }

            return decodedData;
        }

        /// <summary>
        /// Saves the CFS file to the specified path.
        /// </summary>
        /// <param name="path">The path the file is to be saved at.</param>
        public void SaveAs(string path)
        {
            MemoryStream stream;
            BinaryWriter buffer;
            byte[] bytes;

            stream = new MemoryStream(_totalSize);
            buffer = new BinaryWriter(stream);

            if (_version == 4)
            {
                byte[] str;

                buffer.Write((short)_version);
                buffer.Write((short)_frameCount);
                buffer.Write((short)_animationTime);
                buffer.Write((short)_width);
                buffer.Write((short)_height);
                buffer.Write((short)_rows);
                buffer.Write((short)_columns);
                buffer.Write((short)_lightLength);
                buffer.Write((short)_shadowLength);
                buffer.Write((short)_userDataLength);
                buffer.Write((byte)_compression);
                buffer.Write((byte)_maxSolidIndex);
                buffer.Write((int)_dataSize);

                str = new byte[16];
                ASCIIEncoding.ASCII.GetBytes(_category, 0, str.Length, str, 0);
                str[15] = 0; // Make sure the last byte is the null terminator
                buffer.Write(str);

                buffer.Write((byte)_blitMode);
                buffer.Write((byte)_rowMode);
                buffer.Write((short)_sortAdjust);
                buffer.Write((byte)_sortTransform);
                buffer.Write((byte)_userPaletteStart);
                buffer.Write((byte)_userPaletteLength);

                str = new byte[48];
                ASCIIEncoding.ASCII.GetBytes(_description, 0, str.Length, str, 0);
                str[47] = 0; // Make sure the last byte is the null terminator
                buffer.Write(str);

                buffer.Write(_unknownData);

                //foreach (Color color in _palette)
                //{                                        
                //    buffer.Write(color.B);
                //    buffer.Write(color.G);
                //    buffer.Write(color.R);
                //    buffer.Write((byte)0);
                //}

                foreach (Frame frame in _frames)
                {
                    buffer.Write((short)frame.XOffset);
                    buffer.Write((short)frame.YOffset);
                    buffer.Write((short)frame.Width);
                    buffer.Write((short)frame.Height);
                    buffer.Write((int)frame.DataOffset);
                }

                if (_userDataLength > 0)
                    buffer.Write(_userData);

                foreach (Frame frame in _frames)
                {
                    //buffer.Write(frame.Strides);
                    //buffer.Write(frame.ScanLines);
                }

                bytes = stream.ToArray();

                buffer.Close();

                File.WriteAllBytes(path, bytes);
            }
            else
            {
                throw new Exception("Unknown file version: " + _version);
            }
        }

        /// <summary>
        /// Reads a CFS file from a byte array.
        /// </summary>
        /// <param name="buffer">An array containing the bytes of a CFS file.</param>
        /// <param name="offset">The offset in the array to begin reading.</param>
        /// <param name="length"></param>
        /// <returns>A CFS file initialized with the array of bytes.</returns>
        public static CfsFile Read(byte[] buffer, int offset, int length)
        {
            CfsFile file;

            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if ((offset < 0) || (offset > buffer.Length))
                throw new ArgumentOutOfRangeException("offset");
            if ((length - offset) > buffer.Length)
                throw new ArgumentOutOfRangeException("length");

            file = new CfsFile();
            file._totalSize = buffer.Length;

            file._version = (int)BitConverter.ToInt16(buffer, offset); offset += 2;

            if (file._version == 4)
            {
                file._frameCount = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                if (file._frameCount <= 0)
                    throw new Exception("There is an invalid number of frames: " + file._frameCount);

                file._animationTime = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                file._width = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                file._height = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                file._rows = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                file._columns = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                file._lightLength = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                file._shadowLength = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                file._userDataLength = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                file._compression = (CompressionFlags)buffer[offset]; offset++;
                file._maxSolidIndex = (int)buffer[offset]; offset++;
                file._dataSize = (int)BitConverter.ToInt32(buffer, offset); offset += 4;

                file._category = ASCIIEncoding.ASCII.GetString(buffer, offset, 16); offset += 16;
                file._blitMode = (BlitMode)buffer[offset]; offset++;
                file._rowMode = (RowMode)buffer[offset]; offset++;
                file._sortAdjust = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                file._sortTransform = (SortTransform)buffer[offset]; offset++;
                file._userPaletteStart = (int)buffer[offset]; offset++;
                file._userPaletteLength = (int)buffer[offset]; offset++;
                file._description = ASCIIEncoding.ASCII.GetString(buffer, offset, 48); offset += 48;

                file._unknownData = new byte[32];
                Buffer.BlockCopy(buffer, offset, file._unknownData, 0, file._unknownData.Length);
                offset += file._unknownData.Length;

                //file._palette = new Color[256];
                //for (int i = 0; i < file._palette.Length; i++)
                //{
                //    Color color;
                //    int argb;

                //    argb = BitConverter.ToInt32(buffer, offset); offset += 4;
                //    argb |= unchecked((int)0xFF000000);

                //    color = Color.FromArgb(argb);

                //    file._palette[i] = color;
                //}

                file._frames = new Frame[file._frameCount];

                for (int i = 0; i < file._frames.Length; i++)
                {
                    //ReadFrameInfo(out file._frames[i], buffer, offset);
                    offset += 12;
                }

                //for (int i = 0; i < file._frames.Length; i++)
                //ReadFrameData(ref file._frames[i], buffer, offset, file._transparent);
            }
            else if (file._version == 3)
            {
                file._frameCount = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                if (file._frameCount <= 0)
                    throw new Exception("There is an invalid number of frames: " + file._frameCount);

                file._animationTime = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                file._width = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                file._height = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                file._rows = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                file._columns = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                file._lightLength = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                file._shadowLength = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                file._userDataLength = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                file._compression = (CompressionFlags)buffer[offset]; offset++;
                file._maxSolidIndex = (int)buffer[offset]; offset++;
                file._dataSize = (int)BitConverter.ToInt32(buffer, offset); offset += 4;

                // Seems like the best way to check for transparency
                //int regularSize = (file._width * file._height) * file._frameCount;
                //if (regularSize != file._dataSize)
                //    file._transparent = true;
                //else
                //    file._transparent = false;

                file._category = ASCIIEncoding.ASCII.GetString(buffer, offset, 16); offset += 16;
                file._blitMode = (BlitMode)buffer[offset]; offset++;
                file._rowMode = (RowMode)buffer[offset]; offset++;
                file._sortAdjust = (int)BitConverter.ToInt16(buffer, offset); offset += 2;
                file._sortTransform = (SortTransform)buffer[offset]; offset++;
                file._userPaletteStart = (int)buffer[offset]; offset++;
                file._userPaletteLength = (int)buffer[offset]; offset++;

                file._unknownData = new byte[9];
                Buffer.BlockCopy(buffer, offset, file._unknownData, 0, file._unknownData.Length);
                offset += file._unknownData.Length;

                //file._palette = new Color[256];
                //for (int i = 0; i < file._palette.Length; i++)
                //{
                //    Color color;
                //    int argb;

                //    argb = BitConverter.ToInt32(buffer, offset); offset += 4;
                //    argb |= unchecked((int)0xFF000000);

                //    color = Color.FromArgb(argb);

                //    file._palette[i] = color;
                //}

                file._frames = new Frame[file._frameCount];

                for (int i = 0; i < file._frames.Length; i++)
                {
                    //ReadFrameInfo(out file._frames[i], buffer, offset);
                    offset += 12;
                }

                //for (int i = 0; i < file._frames.Length; i++)
                //    ReadFrameData(ref file._frames[i], buffer, offset, file._transparent);

            }
            else
            {
                throw new Exception("Unknown file version: " + file._version);
            }

            return file;
        }

        /// <summary>
        /// Calulates the length of a scan line in bytes aligned to a 32-bit boundry.
        /// </summary>
        /// <param name="width">The width in pixels.</param>        
        /// <returns></returns>
        private static int CalculateStride(int width)
        {
            return (int)Math.Ceiling(width / 4.0);
        }
    }
}
