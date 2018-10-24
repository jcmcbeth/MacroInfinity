using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

using Infantry.Cfs;
using Infantry.Blob;

namespace JoelMcbeth.TestSuite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string directory;
            BlobFile blobFile;
            CfsFile cfsFile;    
            int blobFileSuccess;
            int cfsFileSuccess;
            int cfsTotal, blobTotal;
            string[] files;
            double totalBlobTime, averageBlobTime;
            double totalCfsTime, averageCfsTime;
            byte[] buffer;
            string ext;
            PerformanceTimer timer1 = new PerformanceTimer();

            //directory = @"C:\Program Files\Infantry";
            directory = @"C:\Users\Joel McBeth\Documents\Visual Studio 2008\Projects\MacroInfinity\Data";

            blobFileSuccess = 0;
            totalBlobTime = 0;
            totalCfsTime = 0;
            cfsFileSuccess = 0;
            cfsTotal = blobTotal = 0;
            files = Directory.GetFiles(directory);
            foreach (string filename in files)
            {
                ext = Path.GetExtension(filename);
                if (!ext.Equals(".blo", StringComparison.CurrentCultureIgnoreCase) &&
                    !ext.Equals(".lvb", StringComparison.CurrentCultureIgnoreCase))
                    continue;

                blobTotal++;

                try
                {
                    timer1.Start();
                    blobFile = new BlobFile(filename);
                    timer1.Stop();

                    totalBlobTime += timer1.Duration;

                    foreach (BlobEntry entry in blobFile)
                    {                        
                        buffer = blobFile.ExtractBytes(entry);

                        try
                        {
                            ext = Path.GetExtension(entry.Filename);
                            if (ext.Equals(".cfs", StringComparison.CurrentCultureIgnoreCase))
                            {
                                cfsTotal++;

                                timer1.Start();
                                //cfsFile = CfsFile.Read(buffer, 0, buffer.Length);
                                cfsFile = null;
                                timer1.Stop();

                                totalCfsTime += timer1.Duration;

                                cfsFileSuccess++;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("CFS File Exception\r\nFile:\t\t{0}\r\nBlob File:\t{1}\r\n{2}\r\n", entry.Filename, filename, ex.Message);
                        }
                    }

                    blobFileSuccess++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Blob File Exception\r\nFile:\t\t{0}\r\n{1}", filename, ex.Message);
                }                
            }

            averageBlobTime = totalBlobTime / (double)blobFileSuccess;
            averageCfsTime = totalCfsTime / (double)cfsFileSuccess;

            Console.WriteLine("Blob File Results\r\nSuccessful:\t{0}\r\nTotal:\t\t{1}\r\nTotal Time:\t{2}\r\n" +
                              "Average Time:\t{3}\r\n",
                              blobFileSuccess, blobTotal, totalBlobTime, averageBlobTime);

            Console.WriteLine("CFS File Results\r\nSuccessful:\t{0}\r\nTotal:\t\t{1}\r\n" +
                              "Total Time:\t{2}\r\nAverage Time:\t{3}\r\n",
                              cfsFileSuccess, cfsTotal, totalCfsTime, averageCfsTime);
            Console.ReadKey();
        }
    }
}
