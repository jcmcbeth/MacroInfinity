using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Infantry.Blob;

#if DEBUG
using System.Diagnostics;
#endif

namespace Blowout
{
    public partial class BlowoutForm : Form
    {
        private BlobFile _document;
        private bool _open;
        private BlowoutAboutBox _aboutBox;
        private BlobInfoDialog _infoDialog;

        public BlowoutForm()
        {
            InitializeComponent();

            _aboutBox = new BlowoutAboutBox();
            _infoDialog = new BlobInfoDialog();

            DocumentClosed();
        }

        /// <summary>
        /// Updates the user interface to match the data for the document.
        /// </summary>
        private void UpdateBlobUI()
        {
            ListViewItem item;

            listView.Items.Clear();
            foreach (BlobEntry entry in _document)
            {
                item = new ListViewItem(entry.Filename);
                item.SubItems.Add(entry.Size.ToString());

                listView.Items.Add(item);
            }

            countStripStatusLabel.Text = _document.Count + " file(s)";
        }

        /// <summary>
        /// Handles updating the UI buttons when a document is opened.
        /// </summary>
        private void DocumentOpened()
        {               
            addToolStripButton.Enabled = true;
            extractStripButton.Enabled = true;
            removeStripButton.Enabled = true;
            infoToolStripButton.Enabled = true;
            if (_document.Version == 1)
                convertToolStripButton.Enabled = true;
            

            addToolStripMenuItem.Enabled = true;
            extractToolStripMenuItem.Enabled = true;
            removeToolStripMenuItem.Enabled = true;
            infoToolStripMenuItem.Enabled = true;
            if (_document.Version == 1)
                convertToolStripMenuItem.Enabled = true;
            
            extractToolStripMenuItem1.Enabled = true;
            removeToolStripMenuItem1.Enabled = true;

            _open = true;

            documentStatusLabel.Text = "Ready";            
        }

        /// <summary>
        /// Handles updating the UI buttons when a document is not opened.
        /// </summary>
        private void DocumentClosed()
        {
            addToolStripButton.Enabled = false;
            extractStripButton.Enabled = false;
            removeStripButton.Enabled = false;
            infoToolStripButton.Enabled = false;
            convertToolStripButton.Enabled = false;

            addToolStripMenuItem.Enabled = false;
            extractToolStripMenuItem.Enabled = false;
            removeToolStripMenuItem.Enabled = false;
            infoToolStripMenuItem.Enabled = false;
            convertToolStripMenuItem.Enabled = false;

            extractToolStripMenuItem1.Enabled = false;
            removeToolStripMenuItem1.Enabled = false;

            _open = false;

            documentStatusLabel.Text = "Click New or Open to begin";
        }

        /// <summary>
        /// The operation of creating a new blank document.
        /// </summary>
        private void NewDocumentOperation()
        {
            DialogResult result;

            result = newFileDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                _document = new BlobFile(newFileDialog.FileName);

                _open = true;

                DocumentOpened();
                UpdateBlobUI();
            }
        }

        /// <summary>
        /// The operation of loading a document from file.
        /// </summary>
        private void LoadDocumentOperation()
        {
            DialogResult result;
      
            result = openBlobDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                try
                {
                    _document = new BlobFile(openBlobDialog.FileName);

                    DocumentOpened();
                    UpdateBlobUI();

                    documentStatusLabel.Text = "\"" + openBlobDialog.FileName + "\"" + " loaded";
                }
                catch (Exception e)
                {
#if DEBUG
                    Trace.WriteLine("Exception thrown when trying to LoadDocument(): " + e, "Error");
#endif

                    MessageBox.Show(this, "Unable to load file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Removes the selected items from the blob file.
        /// </summary>
        private void RemoveOperation()
        {
            int count = 0;

            try
            {
                foreach (ListViewItem item in listView.SelectedItems)
                {
                    _document.Remove(item.Text);
                    count++;
                }

                UpdateBlobUI();
                documentStatusLabel.Text = count + " file(s) removed";
            }
            catch (Exception e)
            {
#if DEBUG
                Trace.WriteLine("Exception thrown when trying RemoveOperation(): " + e, "Error");
#endif

                MessageBox.Show(this, "Unable to remove files.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Adds files to the blob file.
        /// </summary>
        private void AddOperation()
        {
            DialogResult result;
            string error;
            bool hasError;
            int size, count;
                  
            result = addFileDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                try
                {
                    if (_document.Version == 1)
                        size = 14;
                    else
                        size = 32;

                    count = 0;
                    hasError = false;
                    error = "The filename can be at most " + size + " characters long.\r\n\r\nThe filename(s) for the follow file(s) are too large:\r\n";                    

                    foreach (string filename in addFileDialog.FileNames)
                    {
                        if (Path.GetFileName(filename).Length <= size)
                        {
                            _document.Add(filename);
                            count++;
                        }
                        else
                        {
                            hasError = true;
                            error += "\t\"" + filename + "\"\r\n";
                        }
                    }

                    UpdateBlobUI();

                    if (hasError)
                    {
                        MessageBox.Show(this, error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }                    

                    documentStatusLabel.Text = count + " file(s) added";
                }
                catch (Exception e)
                {
#if DEBUG
                    Trace.WriteLine("Exception thrown when trying to AddOperation(): " + e, "Error");
#endif

                    MessageBox.Show(this, "Unable to add file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }            
        }

        /// <summary>
        /// Extracts the selected items or all items from the blob file.
        /// </summary>
        private void ExtractOperation()
        {
            DialogResult result;
            int count = 0;

            result = extractBrowserDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                try
                {
                    // If none are selected, extract all
                    if (listView.SelectedItems.Count == 0)
                    {
                        foreach (ListViewItem item in listView.Items)
                        {
                            _document.Extract(item.Text, extractBrowserDialog.SelectedPath);
                            count++;
                        }
                    }
                    else // Extract only the selected items
                    {
                        foreach (ListViewItem item in listView.SelectedItems)
                        {
                            _document.Extract(item.Text, extractBrowserDialog.SelectedPath);
                            count++;
                        }
                    }

                    documentStatusLabel.Text = count + " file(s) extracted";
                }
                catch (Exception e)
                {
#if DEBUG
                    Trace.WriteLine("Exception thrown when trying ExtractOperation(): " + e, "Error");
#endif

                    MessageBox.Show(this, "Unable to extract files.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        /// <summary>
        /// Shows a dialog with information on the blob file.
        /// </summary>
        private void InfoOperation()
        {
            _infoDialog.BlobFile = _document;

            _infoDialog.ShowDialog(this);
        }

        /// <summary>
        /// Converts a file from version 1 to version 2.
        /// </summary>
        private void ConvertOperation()
        {
            try
            {
                _document.Convert();

                convertToolStripMenuItem.Enabled = false;
                convertToolStripButton.Enabled = false;

                documentStatusLabel.Text = "File converted";
            }
            catch (Exception e)
            {
#if DEBUG
                Trace.WriteLine("Exception thrown when trying ConvertOperation(): " + e, "Error");
#endif

                MessageBox.Show(this, "Unable to convert.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewDocumentOperation();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDocumentOperation();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _aboutBox.ShowDialog(this);
        }

        private void addToolStripButton_Click(object sender, EventArgs e)
        {
            AddOperation();
        }

        private void extractStripButton_Click(object sender, EventArgs e)
        {
            ExtractOperation();
        }

        private void deleteStripButton_Click(object sender, EventArgs e)
        {
            RemoveOperation();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            NewDocumentOperation();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            LoadDocumentOperation();
        }

        private void extractToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ExtractOperation();
        }

        private void removeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RemoveOperation();
        }

        private void extractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExtractOperation();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddOperation();
        }

        private void convertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConvertOperation();
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InfoOperation();
        }

        private void infoToolStripButton_Click(object sender, EventArgs e)
        {
            InfoOperation();
        }

        private void convertToolStripButton_Click(object sender, EventArgs e)
        {
            ConvertOperation();
        }        
    }
}