using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infantry;
using Infantry.Cfs;

namespace CfsTest {
    public partial class Form1 : Form {

        private CfsFile _file;

        public Form1() {
            InitializeComponent();

            _file = null;
        }

        private void browseButton_Click(object sender, EventArgs e) {
            DialogResult result;
            Bitmap bitmap;

            result = openFileDialog.ShowDialog(this);
            if (result == DialogResult.OK) {
                try {
                    _file = new CfsFile(openFileDialog.FileName);
                    bitmap = _file.ToBitmap();

                    cfsPictureBox.Image = bitmap;
                    
                } catch (Exception ex) {
                    MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
