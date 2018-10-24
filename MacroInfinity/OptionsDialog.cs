using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Shell32;
using System.IO;

#if DEBUG
using System.Diagnostics;
#endif

namespace MacroInfinity
{
    public partial class OptionsDialog : Form
    {
        private string _directory;
        private bool _modified;

        public string InfantryDirectory
        {
            get { return _directory; }
            set { _directory = value; }
        }

        public OptionsDialog()
        {            
            InitializeComponent();

            _directory = Environment.CurrentDirectory;
            _modified = false;
        }

        private void browseButton_Click(object sender, EventArgs e)
        {                        
            ShellClass shell;
            Folder2 folder;

            // Call the shell folder browsing dialog
            shell = new Shell32.ShellClass();
            folder = (Folder2)shell.BrowseForFolder(
                0,
                "Browse for Infantry Folder",
                0,
                null
            );            

            if (folder != null)
            {                
                browseTextBox.Text = folder.Self.Path;

                OptionsModified();
            }
        }

        /// <summary>
        /// This sets the neccesary state if the data in the form has been modified.
        /// </summary>
        private void OptionsModified()
        {
            _modified = true;

            applyButton.Enabled = true;
        }

        /// <summary>
        /// Sets the neccesary state if the modified data has been commited.
        /// </summary>
        private void OptionsCommited()
        {
            _modified = false;

            applyButton.Enabled = false;
        }

        /// <summary>
        /// Checks if all the values in the form are valid.
        /// </summary>
        /// <returns>If the form is valid.</returns>
        private bool ValidateForm()
        {
            bool exist;

            // Infantry directory validation
            if (browseTextBox.Text == "")
            {
                MessageBox.Show(
                    this,
                    "You must choose the directory infantry is located.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            else
            {
                try
                {
                    exist = Directory.Exists(browseTextBox.Text);
                }
                catch (Exception e)
                {
#if DEBUG
                    Trace.WriteLine(e, "Exception");
#endif
                    exist = false;
                }

                if (!exist)
                {
                    MessageBox.Show(
                        this,
                        "That directory does not exist.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Performs the apply operation that validates and commits the form data to memory.
        /// </summary>
        private void ApplyOperation()
        {
            if (ValidateForm())
            {
                UpdateOptionsData();

                OptionsCommited();
            }
        }

        /// <summary>
        /// Performs the ok operation that validates and commits form data to memory,
        /// then returns the dialog result.
        /// </summary>
        private void OKOperation()
        {
            if (ValidateForm())
            {
                UpdateOptionsData();

                this.DialogResult = DialogResult.OK;
            }
        }

        /// <summary>
        /// Updates the data read from the form.
        /// Assumes that the data has been validated.
        /// </summary>
        private void UpdateOptionsData()
        {
            InfantryDirectory = browseTextBox.Text;
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            ApplyOperation();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            OKOperation();
        }

        private void TextBoxTextChanged(object sender, EventArgs e)
        {
            OptionsModified();
        }
    }
}