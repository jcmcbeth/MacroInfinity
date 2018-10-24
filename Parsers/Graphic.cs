using System;
using System.Collections.Generic;
using System.Text;

namespace Infantry.Items
{
    /// <summary>
    /// Graphical data for an item.
    /// </summary>
    public class Graphic
    {
        private string _blob;
        private string _filename;
        private int _permutation;
        private int _offset;
        private int _hue;
        private int _saturation;
        private int _value;
        private int _time;

        /// <summary>
        /// The filename of the blob file the graphic is in.
        /// </summary>
        public string BlobFilename
        {
            get { return _blob; }
            set { _blob = value; }
        }

        /// <summary>
        /// The filename of the cfs file in the blob file.
        /// </summary>
        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        /// <summary>
        /// Light and color intensity. 
        /// </summary>
        /// <remarks>
        /// 0-23 white, 24-47 red, 48-71 green, 72-95 blue, higher is full.
        /// </remarks>
        public int LightPermuation
        {
            get { return _permutation; }
            set { _permutation = value; }
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// Negative numbers mean special effects.
        /// </remarks>
        public int PaletteOffset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        /// <summary>
        /// The Hue  shift for the HSV for the graphic.
        /// </summary>
        public int Hue
        {
            get { return _hue; }
            set { _hue = value; }
        }

        /// <summary>
        /// The saturation shift for the HSV for the graphic.
        /// </summary>
        public int Saturation
        {
            get { return _saturation; }
            set { _saturation = value; }
        }

        /// <summary>
        /// The value shift for the HSV for the graphic.
        /// </summary>
        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// The amount of time to do one frame in ticks.
        /// </summary>
        /// <remarks>
        /// Use 0 to use the animation time in the cfs file.
        /// </remarks>
        public int AnimationTime
        {
            get { return _time; }
            set { _time = value; }
        }

        /// <summary>
        /// Creates a graphic with default settings.
        /// </summary>
        public Graphic()
        {            
            this.BlobFilename = "None";
            this.Filename = "None";
        }
    }
}
