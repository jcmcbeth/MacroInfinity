using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace JoelMcbeth.CfsEdit
{
    
    [ToolboxItem(true)]
    public class PalettePanel : Panel
    {
        private Color[] _palette;
        private int _padding;
        private int _colorsPerLine;
        private int _shadows;
        private int _lights;
        private Brush[] _brushes;
        private float _boxWidth;

        [Browsable(true)]
        [Description("The number of shadows the in the palette.")]
        [Category("Appearance")]
        [DefaultValue(8)]
        public int Shadows
        {
            get { return _shadows; }
            set
            {
                _shadows = value;
                this.Invalidate();
            }
        }

        [Browsable(true)]
        [Description("The number of lights in the palette")]
        [Category("Appearance")]
        [DefaultValue(32)]
        public int Lights
        {
            get { return _lights; }
            set
            {
                _lights = value;
                this.Invalidate();
            }
        }

        [Browsable(true)]
        [Description("The padding between the colors.")]
        [Category("Appearance")]
        [DefaultValue(4)]
        public int ColorPadding
        {
            get { return _padding; }
            set 
            {
                _padding = value;
                CalculateBoxWidth();

                this.Invalidate();
            }
        }

        [Browsable(true)]
        [Description("The number of colors to display on each line of the panel.")]
        [Category("Appearance")]
        [DefaultValue(16)]
        public int ColorsPerLine
        {
            get { return _colorsPerLine; }
            set
            {
                _colorsPerLine = value;
                CalculateBoxWidth();

                this.Invalidate();
            }
        }

        [Browsable(false)]
        public Color[] Palette
        {
            get { return _palette; }
            set
            {
                if (value != null)
                {
                    if (_palette != null)
                    {
                        for (int i = 0; i < _palette.Length; i++)
                            _brushes[i].Dispose();
                    }

                    _palette = value;

                    _brushes = new Brush[_palette.Length];
                    for (int i = 0; i < _palette.Length; i++)
                        _brushes[i] = new SolidBrush(_palette[i]);
                }
                else
                {
                    _palette = null;
                }

                this.Invalidate();
            }
        }

        public PalettePanel() : base()
        {
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            _palette = null;

            _padding = 4;
            _lights = 32;
            _shadows = 8;
            _colorsPerLine = 16;
        }

        public PalettePanel(Color[] palette, int lights, int shadows) : this()
        {
            this.Palette = palette;
            this.Lights = lights;
            this.Shadows = shadows;
        }

        protected override void  OnClientSizeChanged(EventArgs e)
        {
            CalculateBoxWidth();

 	        base.OnClientSizeChanged(e);
        }

        private void CalculateBoxWidth()
        {
            float totalPadding;
            float width;

            totalPadding = _padding * (_colorsPerLine + 1);
            width = this.ClientRectangle.Width - totalPadding;

            _boxWidth = width / (float)_colorsPerLine;
        }

        private void DrawCenteredString(string text, float col, float row, Brush brush, float boxWidth, Graphics g)
        {
            SizeF size;
            float x, y;
            PointF point;

            size = g.MeasureString(text, this.Font);

            x = col + ((boxWidth / 2) - (size.Width / 2));
            y = row + ((boxWidth / 2) - (size.Height / 2));
            point = new PointF(x, y);

            g.DrawString(text, this.Font, brush, point);            
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g;
            float row, col;
            float average;
            Brush brush;
            Color color;
            int shadowStart, lightStart;

            g = e.Graphics;
            
            if (_palette != null)
            {                
                shadowStart = 256 - _shadows;
                lightStart = shadowStart - _lights;  

                row = _padding;
                col = 0;
                for (int i = 0; i < 256; i++)
                {                    
                    col += _padding;                    
                    g.FillRectangle(_brushes[i], col, row, _boxWidth, _boxWidth);
        //            g.DrawRectangle(Pens.Black, col, row, _boxWidth, boxWidth);

                    color = _palette[i];
                    average = (color.B + color.R + color.G) / 3;
                    if (average < 128)
                        brush = Brushes.White;
                    else
                        brush = Brushes.Black;                    
                    
                    if (i == 0)
                    {
                       DrawCenteredString("T", col, row, brush, _boxWidth, g);                     
                    }
                    else if (i >= shadowStart)
                    {
                       DrawCenteredString("S", col, row, brush, _boxWidth, g); 
                    }
                    else if (i >= lightStart)
                    {
                       DrawCenteredString("L", col, row, brush, _boxWidth, g); 
                    }

                    col += _boxWidth;

                    if ((col + _padding) >= e.ClipRectangle.Width)
                    {
                        row += _padding + _boxWidth;
                        col = 0;
                    }
                }
            }        
             
            base.OnPaint(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_palette != null)
                    for (int i = 0; i < _palette.Length; i++)
                        _brushes[i].Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
