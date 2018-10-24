using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.ComponentModel;

using System.Runtime.InteropServices;

using Infantry.Cfs;

using System.Diagnostics;

namespace JoelMcbeth.CfsEdit
{    
    [ToolboxItem(true)]
    public class ViewerPanel : Panel
    {
        private CfsFile _file;
        private int _currentFrame;
        private bool _shadows;
        private bool _lights;
        private bool _transparency;
        private bool _boundingBox;
        private bool _showOrigin;
        private bool _animate;

        private Bitmap[] _frames;

        private Timer _animationTimer;
        
        public delegate void FrameChangedEventHandler(Object sender, FrameChangedEventArgs args);

        /// <summary>
        /// This is called when an animiated image's frame is changed.
        /// </summary>
        [Browsable(true)]
        public event FrameChangedEventHandler FrameChanged;

        [Browsable(false)]
        public CfsFile ImageFile
        {
            get { return _file; }
            set
            {
                int interval;
                _file = value;

                if (_file != null)
                {
                    _frames = _file.GetBitmapFrames();
                    this.Invalidate();

                    // Make sure the interval is atleast 1
                    interval = _file.AnimationTime / _file.FrameCount;
                    interval = Math.Max(1, interval);
                    _animationTimer.Interval = interval;

                    if (_animate)
                        _animationTimer.Start();
                }
            }
        }

        [Browsable(true)]
        [Description("The current frame being viewed.")]
        [DefaultValue(0)]
        public int CurrentFrame
        {
            get { return _currentFrame; }
            set
            {
                if (_currentFrame < 0)
                    throw new IndexOutOfRangeException();
                
                if (_file != null && value >= _file.FrameCount)                
                    throw new IndexOutOfRangeException();                

                _currentFrame = value;

                if (_file != null)
                    Invalidate();
                
                OnFrameChange(new FrameChangedEventArgs(_currentFrame));                
            }
        }

        [Browsable(true)]
        [Description("If the light pixels should be drawn.")]
        [DefaultValue(false)]
        public bool DisplayBoundingBoxes
        {
            get { return _boundingBox; }
            set
            {
                _boundingBox = value;

                if (_file != null)
                {
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Description("If the light pixels should be drawn.")]
        [DefaultValue(false)]
        public bool DisplayOrigin
        {
            get { return _showOrigin; }
            set
            {
                _showOrigin = value;

                    Invalidate();
            }
        }

        [Browsable(true)]
        [Description("If the shadow pixels should be drawn.")]
        [DefaultValue(true)]
        public bool RenderShadows
        {
            get { return _shadows; }
            set
            {
                _shadows = value;

                if (_file != null)
                {
                    this.Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Description("If the light pixels should be drawn.")]
        [DefaultValue(true)]
        public bool RenderLights
        {
            get { return _lights; }
            set
            {
                _lights = value;

                if (_file != null)
                {
                    this.Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Description("If the transparent pixels should be rendered as transparent.")]
        [DefaultValue(true)]
        public bool Transparency
        {
            get { return _transparency; }
            set
            {
                _transparency = value;

                if (_file != null)
                {
                    this.Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Description("If the panel should animate the image.")]
        [DefaultValue(true)]
        public bool Animate
        {
            get { return _animate; }
            set
            {
                if (_file != null)
                {
                    if (_animate)
                        _animationTimer.Stop();
                    else
                        _animationTimer.Start();
                }

                _animate = value;
            }
        }
        
        public ViewerPanel() : base()
        {
            _animationTimer = new Timer();
            _animationTimer.Tick += new EventHandler(_animationTimer_Tick);

            _currentFrame = 0;
            _shadows = true;
            _lights = true;
            _transparency = true;
            _boundingBox = false;
            _showOrigin = false;
            _animate = true;

            _frames = null;
        }

        protected void OnFrameChange(FrameChangedEventArgs args)
        {            
            if (FrameChanged != null)
                FrameChanged(this, args);
        }

        private void _animationTimer_Tick(object sender, EventArgs e)
        {
            if (_file != null)
            {
                if (this.CurrentFrame == (_file.FrameCount - 1))
                    this.CurrentFrame = 0;
                else
                    this.CurrentFrame++;                
            }            
        }

        public static Pen GetContrastingPenColor(Color color)
        {
            Pen answer;
            float average;

            average = (color.B + color.R + color.G) / 3.0f;

            if (average < 128)
                answer = Pens.White;
            else
                answer = Pens.Black;

            return answer;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g;
            //Frame frame;
            int width, height;
            Point clientCenter;
            Point imageCenter;
            Point imageLocation;
            Pen color;

            g = e.Graphics;

            width = this.ClientRectangle.Width;
            height = this.ClientRectangle.Height;

            // Get the center of the client area
            clientCenter = new Point();
            clientCenter.X = width / 2;
            clientCenter.Y = height / 2;

            color = GetContrastingPenColor(this.BackColor);

            if (_file != null)
            {                
                // Get the center of the image
                imageCenter = new Point();
                imageCenter.X = _file.Width / 2;
                imageCenter.Y = _file.Height / 2;

                // Get the x and y coordinates to draw the image
                imageLocation = new Point();
                imageLocation.X = clientCenter.X - imageCenter.X;
                imageLocation.Y = clientCenter.Y - imageCenter.Y;                
                
                g.DrawImageUnscaled(_frames[_currentFrame], imageLocation);                

                // Draw the bounding boxes
                if (_boundingBox)
                {
                    g.DrawRectangle(Pens.Black, imageLocation.X - 1, imageLocation.Y - 1, _file.Width + 1, _file.Height + 1);

                    //if (_file.Transparent)
                    //{
                    //    frame = _file.Frames[_currentFrame];

                    //    g.DrawRectangle(color,
                    //                    imageLocation.X + frame.XOffset - 1,
                    //                    imageLocation.Y + frame.YOffset - 1,
                    //                    frame.Width + 1,
                    //                    frame.Height + 1);
                    //}
                }

                
            }

            // Draw the origin
            if (_showOrigin)
            {
                g.DrawLine(color, clientCenter.X, 0, clientCenter.X, height);
                g.DrawLine(color, 0, clientCenter.Y, width, clientCenter.Y);
            }

            base.OnPaint(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_frames != null)
                {
                    for (int i = 0; i < _frames.Length; i++)
                        if (_frames[i] != null)
                            _frames[i].Dispose();
                }

                _animationTimer.Dispose();
            }

            base.Dispose(disposing);
        }
    }

    public class FrameChangedEventArgs : EventArgs
    {
        private int _index;

        public int NewFrameIndex
        {
            get { return _index; }
        }

        public FrameChangedEventArgs(int index)
        {
            _index = index;
        }
    }
}
