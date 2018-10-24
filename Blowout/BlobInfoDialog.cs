using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Infantry.Blob;

namespace Blowout
{
    public partial class BlobInfoDialog : Form
    {
        private BlobFile _file;

        public BlobFile BlobFile
        {
            get { return _file; }
            set
            {
                _file = value;

                UpdateUIText();
            }
        }        

        private void UpdateUIText()
        {
            FileInfo info;
            string filename;

            if (_file != null)
            {
                versionLabel.Text = _file.Version.ToString();
                countLabel.Text = _file.Count.ToString();

                info = new FileInfo(_file.Filename);
                sizeLabel.Text = info.Length.ToString();

                filename = Path.GetFileName(_file.Filename);
                Text = "Blob Info - " + filename;
            }
        }

        public BlobInfoDialog()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}