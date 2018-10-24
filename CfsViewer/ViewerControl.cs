using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

using System.Windows.Forms;
using System.ComponentModel;

using Tao.OpenGl;
using Tao.Platform.Windows;

namespace JoelMcbeth.CfsEdit
{

    struct ColorF
    {
        public float Red;
        public float Green;
        public float Blue;
        public float Alpha;
    };

    [ToolboxItem(true)]
    public class ViewerControl
    {

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ViewerControl
            // 
            this.Name = "ViewerControl";
            this.Size = new System.Drawing.Size(300, 300);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ViewerControl_Paint);
            this.ResumeLayout(false);

            //InitOpenGL();
        }

        private void InitOpenGL()
        {
            Rectangle rect;
            ColorF backColor;

            rect = this.ClientRectangle;
            Gl.glOrtho(rect.Left, rect.Right, rect.Bottom, rect.Top, -1.0, 1.0);

            backColor = ToColorF(this.BackColor);
            Gl.glClearColor(backColor.Red, backColor.Green, backColor.Blue, backColor.Alpha);

            this.InitializeContexts();
        }

        private static ColorF ToColorF(Color color)
        {
            ColorF colorF;

            colorF = new ColorF();
            colorF.Red = color.R / 255.0f;
            colorF.Green = color.G / 255.0f;
            colorF.Blue = color.B / 255.0f;
            colorF.Alpha = color.A / 255.0f;

            return colorF;
        }

        private void ViewerControl_Paint(object sender, PaintEventArgs e)
        {            
            Gl.glBegin(Gl.GL_POINT);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
            Gl.glEnd();
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        this.DestroyContexts();
        //    }

        //    base.Dispose(disposing);
        //}
    }
}
