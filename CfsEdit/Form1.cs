using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using Infantry.Cfs;

namespace JoelMcbeth.CfsEdit
{
    public partial class CfsEditForm : Form
    {
        private CfsFile _file;

        public CfsEditForm()
        {
            InitializeComponent();

            _file = null;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result;

            result = openFileDialog.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                _file = new CfsFile(openFileDialog.FileName);

                //try
                {
                    //_file.Load();

                    FillFormData();

                    //palettePanel.Palette = _file.ColorPalette;                    
                    palettePanel.Shadows = _file.Shadows;
                    palettePanel.Lights = _file.Lights;

                    viewerPanel.ImageFile = _file;
                    viewerPanel.CurrentFrame = 0;
                }
                //catch (Exception ex)
                {
                //    MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FillFormData()
        {
            if (_file != null)
            {
                this.versionLabel.Text = _file.Version.ToString();
                this.framesLabel.Text = _file.FrameCount.ToString();
                this.animationTimeTextBox.Text = _file.AnimationTime.ToString();
                this.widthLabel.Text = _file.Width.ToString();
                this.heightLabel.Text = _file.Height.ToString();
                this.rowsLabel.Text = _file.Rows.ToString();
                this.columnsLabel.Text = _file.Columns.ToString();
                this.lightsLabel.Text = _file.Lights.ToString();
                this.shadowsLabel.Text = _file.Shadows.ToString();
                this.userDataLengthLabel.Text = _file.UserDataLength.ToString();
                this.unknownLabel.Text = _file.Compression.ToString();
                this.lastSolidIndexLabel.Text = _file.MaxSolidIndex.ToString();
                this.dataSizeLabel.Text = _file.DataSize.ToString();
                this.totalSizeLabel.Text = _file.TotalSize.ToString();
                this.categoryTextBox.Text = _file.Category;
                this.blitModeComboBox.SelectedIndex = (int)_file.BlitMode;
                this.rowModeComboBox.SelectedIndex = (int)_file.RowMode;
                this.sortAdjustTextBox.Text = _file.SortAdjust.ToString();
                this.sortTransformComboBox.SelectedIndex = (int)_file.SortTransform;
                this.descriptionTextBox.Text = _file.Description;                
            }
        }                

        private void viewerPanel_FrameChanged(object sender, FrameChangedEventArgs args)
        {
            UpdateFrameData(args.NewFrameIndex);
        }

        private void UpdateFrameData(int index)
        {
            //Frame frame;

            //if (_file != null)
            //{
            //    frame = _file.Frames[index];

            //    frameDataSizeLabel.Text = frame.Length.ToString();
            //    frameHeightLabel.Text = frame.Height.ToString();
            //    frameWidthLabel.Text = frame.Width.ToString();
            //    frameXOffsetLabel.Text = frame.XOffset.ToString();
            //    frameYOffsetLabel.Text = frame.YOffset.ToString();
            //    frameOffsetLabel.Text = frame.DataOffset.ToString();

            //    frameNumericUpDown.Value = (decimal)index;
            //    frameNumericUpDown.Maximum = (decimal)(_file.FrameCount - 1);
            //}
        }

        private void animateCheckBox_CheckedChanged(object sender, EventArgs e)
        {               
            viewerPanel.Animate = animateCheckBox.Checked;
        }

        private void transparentCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            viewerPanel.Transparency = transparentCheckBox.Checked;
        }

        private void showOriginCheckBox_CheckedChanged(object sender, EventArgs e)
        {            
            viewerPanel.DisplayOrigin = showOriginCheckBox.Checked;
        }

        private void viewerPanel_Click(object sender, EventArgs e)
        {

        }

        private void boundingBoxCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            viewerPanel.DisplayBoundingBoxes = boundingBoxCheckBox.Checked;
        }

        private void shadowsCheckBox_CheckedChanged(object sender, EventArgs e)
        {            
            viewerPanel.RenderShadows = shadowsCheckBox.Checked;
        }

        private void lightsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            viewerPanel.RenderLights = lightsCheckBox.Checked;
        }

        private void frameNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (_file != null && frameNumericUpDown.Value != (int)viewerPanel.CurrentFrame)
            {
                viewerPanel.CurrentFrame = (int)frameNumericUpDown.Value;
            }
        }
    }
}