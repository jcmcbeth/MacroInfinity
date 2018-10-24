using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

#if DEBUG
using System.Diagnostics;
#endif

namespace MacroInfinity
{
    public partial class MacroInfinityForm : Form
    {
        private Macro[] _macros;
        private bool _saved;
        private bool _modified;
        private string _filename;
        private string _path;

        private AboutBox _aboutBox;
        private OptionsDialog _optionsDialog;

        public MacroInfinityForm()
        {
            InitializeComponent();

            _aboutBox = new AboutBox();
            _optionsDialog = new OptionsDialog();

            _macros = null;
            _filename = null;
            _modified = false;
            _saved = false;

            _path = openMacroDialog.InitialDirectory;

            SetTitle();

            NewDocumentOperation();
        }

        /// <summary>
        /// Updates the text in the macro text boxes to what
        /// their current values are.
        /// </summary>
        private void UpdateMacroText()
        {
            macroTextBox1.Text = _macros[0].Text;
            macroTextBox2.Text = _macros[1].Text;
            macroTextBox3.Text = _macros[2].Text;
            macroTextBox4.Text = _macros[3].Text;
            macroTextBox5.Text = _macros[4].Text;
            macroTextBox6.Text = _macros[5].Text;
            macroTextBox7.Text = _macros[6].Text;
            macroTextBox8.Text = _macros[7].Text;
            macroTextBox9.Text = _macros[8].Text;
            macroTextBox10.Text = _macros[9].Text;
            macroTextBox11.Text = _macros[10].Text;
            macroTextBox12.Text = _macros[11].Text;
        }

        /// <summary>
        /// Updates the macro data to the data in the form.
        /// </summary>
        private void UpdateMacroData()
        {
            _macros[0].Text = macroTextBox1.Text;
            _macros[1].Text = macroTextBox2.Text;
            _macros[2].Text = macroTextBox3.Text;
            _macros[3].Text = macroTextBox4.Text;
            _macros[4].Text = macroTextBox5.Text;
            _macros[5].Text = macroTextBox6.Text;
            _macros[6].Text = macroTextBox7.Text;
            _macros[7].Text = macroTextBox8.Text;
            _macros[8].Text = macroTextBox9.Text;
            _macros[9].Text = macroTextBox10.Text;
            _macros[10].Text = macroTextBox11.Text;
            _macros[11].Text = macroTextBox12.Text;
        }

        /// <summary>
        /// Sets the form's title to the current name and version.
        /// </summary>
        private void SetTitle()
        {
            Text = Application.ProductName + " " + Application.ProductVersion;
        }

        /// <summary>
        /// The operation of creating a new blank document.
        /// </summary>
        private void NewDocumentOperation()
        {
            if (CloseDocument())
            {
                _macros = new Macro[12];

                for (int index = 0; index < 12; index++)
                {
                    _macros[index] = new Macro();
                }

                _saved = false;
                _modified = false;

                UpdateMacroText();
            }
        }

        /// <summary>
        /// The operation of loading a document from file.
        /// </summary>
        private void LoadDocumentOperation()
        {
            DialogResult result;

            if (CloseDocument())
            {
                openMacroDialog.InitialDirectory = _path;

                result = openMacroDialog.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    try
                    {
                        _macros = Macro.LoadMacros(openMacroDialog.FileName);

                        UpdateMacroText();
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
        }        

        /// <summary>
        /// The operation of saving the new document to file.
        /// </summary>
        private void SaveDocumentOperation()
        {
            bool save = true;

            if (!_saved)
            {
                if (!GetSaveFilename())
                    save = false;
            }

            if (save)            
                SaveDocument();
        }

        /// <summary>
        /// Saves a document to file using the current filename.
        /// </summary>
        private void SaveDocument()
        {
            try
            {
                UpdateMacroData();

                _saved = true;
                _modified = false;

                
#if DEBUG
                Trace.WriteLine(_filename, "Filename");
#endif

                Macro.SaveMacros(_filename, _macros);
            }
            catch (Exception e)
            {
#if DEBUG
                Trace.WriteLine("Exception thrown when trying to SaveDocument(): " + e, "Error");
#endif
                MessageBox.Show(this, "Unable to save file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Updates the current filename using a save file dialog.
        /// </summary>
        /// <returns>If the filename was updated.</returns>
        private bool GetSaveFilename()
        {
            DialogResult result;

            saveMacroDialog.InitialDirectory = _path;

            result = saveMacroDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                _filename = saveMacroDialog.FileName;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Saves the document to file and shows the save file dialog.
        /// </summary>
        private void SaveAsDocumentOperation()
        {
            if (GetSaveFilename())
            {
                SaveDocument();
            }
        }

        /// <summary>
        /// Operation that opens a dialog to edit the options.
        /// </summary>
        private void EditOptionsOperation()
        {
            DialogResult result;

            result = _optionsDialog.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                _path = _optionsDialog.InfantryDirectory;
            }
        }

        /// <summary>
        /// Prompts the user to close the document, cancel or save.
        /// </summary>
        /// <returns>True if the document was closed.</returns>
        private bool CloseDocument()
        {
            DialogResult result;
            bool closed;

            closed = true;

            if (_modified)
            {
                result = MessageBox.Show(
                    this,
                    "The document has been modified. Would you like to save?",
                    "Document Modified",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button3
                );

                switch (result)
                {
                    case DialogResult.No:
                        closed = true;
                        break;
                    case DialogResult.Cancel:
                        closed = false;
                        break;
                    case DialogResult.Yes:
                        SaveDocument();

                        closed = true;                        
                        break;
                }                
            }

            return closed;
        }

        private void MacroModified()
        {
            _modified = true;            
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDocumentOperation();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAsDocumentOperation();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveDocumentOperation();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            NewDocumentOperation();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _aboutBox.ShowDialog(this);
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditOptionsOperation();
        }

        private void MacroTextBoxTextChanged(object sender, EventArgs e)
        {
            MacroModified();
        }
    }
}