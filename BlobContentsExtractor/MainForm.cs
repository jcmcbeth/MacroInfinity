using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using System.Xml.Serialization;

using BlobContentsExtractor.Properties;

using Infantry.Blob;
using Infantry.Cfs;

namespace BlobContentsExtractor {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();

            if (Settings.Default.LastDirectory.Equals("")) {
                folderBrowserDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            }
            else {
                folderBrowserDialog.SelectedPath = Settings.Default.LastDirectory;
                directoryTextBox.Text = Settings.Default.LastDirectory;
            }

            this.Text = "Blob Contents Extractor v" + Application.ProductVersion;
        }

        private void button2_Click(object sender, EventArgs e) {
            extractWorker.CancelAsync();
            this.Close();
        }

        private void directoryBrowseButton_Click(object sender, EventArgs e) {
            DialogResult result;

            result = folderBrowserDialog.ShowDialog(this);
            if (result == DialogResult.OK) {
                directoryTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void fileBrowseButton_Click(object sender, EventArgs e) {
            DialogResult result;

            result = saveFileDialog.ShowDialog(this);
            if (result == DialogResult.OK) {
                fileTextBox.Text = saveFileDialog.FileName;
            }
        }

        private void extractWorker_DoWork(object sender, DoWorkEventArgs e) {
            string[] files;
            string ext;
            string directory = (string)e.Argument;
            int count = 0;
            int percent;
            BlobFile blobFile;
            Blob blob;
            BlobContent content;
            byte[] buffer;
            CfsFile cfsFile;
            BackgroundWorker worker = (BackgroundWorker)sender;
            ExtractionResults results = new ExtractionResults();

            files = Directory.GetFiles(directory);
            foreach (string file in files) {
                count++;
                percent = (int)((count / (double)files.Length) * 100.0);
                

                ext = Path.GetExtension(file);
                if (!ext.Equals(".blo", StringComparison.CurrentCultureIgnoreCase) &&
                    !ext.Equals(".lvb", StringComparison.CurrentCultureIgnoreCase)) {                    
                    continue;
                }                

                try {
                    blobFile = new BlobFile(file);
                    blob = new Blob();
                    blob.Name = Path.GetFileNameWithoutExtension(file);
                    blob.Path = Path.GetDirectoryName(file);
                    blob.Version = blobFile.Version;
                    blob.FileSize = (int)new FileInfo(file).Length;
                    blob.Extention = Path.GetExtension(file);
                    results.AddBlob(blob);

                    foreach (BlobEntry entry in blobFile) {                        
                        ext = Path.GetExtension(entry.Filename);
                        results.ContentCount++;

                        content = new BlobContent();                        
                        content.Size = entry.Size;                        

                        if (ext.Equals(".cfs", StringComparison.CurrentCultureIgnoreCase)) {                            
                            content.Type = FileType.Cfs;
                            content.Name = Path.GetFileNameWithoutExtension(entry.Filename);                              
                            buffer = blobFile.ExtractBytes(entry);

                            try {
                                //cfsFile = CfsFile.Read(buffer, 0, buffer.Length);
                                cfsFile = null;

                                if (cfsFile.Version == 3)
                                    results.CfsVersion3Files++;
                                else if (cfsFile.Version == 4)
                                    results.CfsVersion4Files++;
                                else // should never happen, but why not
                                    results.CfsVersionUnknownFiles++;                                
                            }
                            catch (Exception) {                                
                                results.CfsVersionUnknownFiles++;                                
                            }

                            blob.AddFile(content);
                        }
                        else if (ext.Equals(".wav", StringComparison.CurrentCultureIgnoreCase)) {                            
                            content.Type = FileType.Wav;
                            content.Name = Path.GetFileNameWithoutExtension(entry.Filename);
                            results.WavFiles++;

                            blob.AddFile(content);
                        }
                        else {
                            results.UnknownFiles++;
                        }
                    }
                    
                }
                catch (Exception) {
                }

                if (worker.WorkerSupportsCancellation && worker.CancellationPending) {
                    e.Cancel = true;
                    return;
                }
                
                worker.ReportProgress(percent, results);
            }

            worker.ReportProgress(100, results);
            e.Result = results;
        }

        private void extractWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            ExtractionResults results = (ExtractionResults)e.UserState;
            BackgroundWorker worker = (BackgroundWorker)sender;

            if (worker.WorkerSupportsCancellation && !worker.CancellationPending) {
                progressBar.Value = e.ProgressPercentage;

                currentFileLabel.Text = results.LastBlobFile.Path + "\\" + results.LastBlobFile.Name + results.LastBlobFile.Extention;
                blobLabel.Text = results.BlobFileCount.ToString();
                cfsOtherCountLabel.Text = results.CfsVersionUnknownFiles.ToString();
                cfsV3CountLabel.Text = results.CfsVersion3Files.ToString();
                cfsV4CountLabel.Text = results.CfsVersion4Files.ToString();
                unknownCountLabel.Text = results.UnknownFiles.ToString();
                wavCountLabel.Text = results.WavFiles.ToString();
                contentCountLabel.Text = results.ContentCount.ToString();
            }
            
        }

        private void extractWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            ExtractionResults results;
            FileStream stream;
            Type[] types;
            XmlSerializer serializer;

            startButton.Enabled = true;
            stopButton.Enabled = false;
            directoryBrowseButton.Enabled = true;
            directoryTextBox.Enabled = true;
            fileBrowseButton.Enabled = true;
            fileTextBox.Enabled = true;

            if (e.Cancelled) {
                progressBar.Value = 0;

                currentFileLabel.Text = "";
                blobLabel.Text = "";
                cfsOtherCountLabel.Text = "";
                cfsV3CountLabel.Text = "";
                cfsV4CountLabel.Text = "";
                unknownCountLabel.Text = "";
                wavCountLabel.Text = "";
                contentCountLabel.Text = "";
            }
            else {
                results = (ExtractionResults)e.Result;
                try {
                    types = new Type[] { typeof(Blob) };
                    serializer = new XmlSerializer(typeof(List<Blob>), types);
                    stream = File.Open(fileTextBox.Text, FileMode.Create, FileAccess.Write);

                    serializer.Serialize(stream, results.BlobFiles);
                }
                catch (Exception ex) {
                    MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void startButton_Click(object sender, EventArgs e) {

            if (!Directory.Exists(directoryTextBox.Text)) {
                MessageBox.Show(this, "The entered directory does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (fileTextBox.Text.Equals("")) {
                MessageBox.Show(this, "You must provide a valid filename for the output file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Settings.Default.LastDirectory = directoryTextBox.Text;
            Settings.Default.Save();

            startButton.Enabled = false;
            stopButton.Enabled = true;
            directoryBrowseButton.Enabled = false;
            directoryTextBox.Enabled = false;
            fileBrowseButton.Enabled = false;
            fileTextBox.Enabled = false;

            extractWorker.RunWorkerAsync(Settings.Default.LastDirectory);
            
        }

        private void stopButton_Click(object sender, EventArgs e) {
            extractWorker.CancelAsync();
        }

        private void aboutButton_Click(object sender, EventArgs e) {
            MessageBox.Show(this,
                Application.ProductName + " " + Application.ProductVersion + "\n\r" +
                "©2009 " + Application.CompanyName + ". All Rights reserved.\n\r" +
                "joel.mcbeth@gmail.com\n\r" +
                "http://joelmcbeth.com",
                "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
