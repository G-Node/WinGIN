using Ionic.Zip;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace GinClientApp.Dialogs
{
    /// <summary>
    /// Dialog for showing gin-cli download and extraction progress
    /// </summary>
    public partial class GinCliDownloadDlg : Form
    {
        /// <summary>
        /// path to gin-cli folder
        /// </summary>
        public string path;
        /// <summary>
        /// path to gin-cli archive
        /// </summary>
        private string gincli;
        /// <summary>
        /// gin-cli for x86 Windows
        /// </summary>
        private static readonly string _ginURL =
          "https://github.com/G-Node/gin-cli/releases/download/v1.11/gin-cli-1.11-windows32.zip";
        /// <summary>
        /// gin-cli for windows 64 bit that was tested
        /// </summary>
        private static readonly string _gin64URL =
           "https://github.com/G-Node/gin-cli/releases/download/v1.11/gin-cli-1.11-windows64.zip";
        public GinCliDownloadDlg()
        {
            InitializeComponent();
            var appDataPath = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\g-node\WinGIN\gin-cli\");
            path = appDataPath.FullName;
            DownloadGinCli();
        }

        /// <summary>
        /// When finishes async download, start bg worker for extraction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Wb_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            label1.Text = "Download Complete. Extracting... Please wait...";
            label1.Refresh();
            this.backgroundWorker1.RunWorkerAsync();
        }
        /// <summary>
        /// Extraction of gin-cli zip in backroung
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (var archive = new ZipFile(gincli))
            {
                archive.ExtractProgress += ZipProgressChanged;
                archive.ExtractAll(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\g-node\WinGIN\gin-cli\", ExtractExistingFileAction.OverwriteSilently);
            }
        }
        /// <summary>
        /// when extraction finishes, show error, or delete archive and close dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                File.Delete(gincli);
                Close();
            }
        }
        /// <summary>
        /// show extraction progress in progress bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// show download progress in progress bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="downloadProgressChangedEventArgs"></param>
        private void WbOnDownloadProgressChanged(object sender,
            DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
        {
            progressBar1.Value = downloadProgressChangedEventArgs.ProgressPercentage;
        }

        /// <summary>
        /// calculate extraction progress (number of files) and send them to bg worker progress
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="zipProgressChangedEventArgs"></param>
        private void ZipProgressChanged(object sender,
                ExtractProgressEventArgs zipProgressChangedEventArgs)
        {
            float percent;
            float total = zipProgressChangedEventArgs.EntriesTotal;
            if (zipProgressChangedEventArgs.EntriesExtracted != 0)
            {
                percent = (zipProgressChangedEventArgs.EntriesExtracted / total) * 100;
                int done = Convert.ToInt32(Math.Floor(percent));
                this.backgroundWorker1.ReportProgress(done);
            }
        }
        /// <summary>
        /// starting method, download gin-cli
        /// </summary>
        public void DownloadGinCli()
        {
            try
            {
                ///clear existing directory
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                Directory.CreateDirectory(path);

                var web = new WebClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                web.Headers.Add("user-agent", "archive_download");
                web.DownloadFileCompleted += new AsyncCompletedEventHandler(Wb_DownloadFileCompleted);
                web.DownloadProgressChanged += new DownloadProgressChangedEventHandler(WbOnDownloadProgressChanged);
                //Download the current gin-cli release and run bg worker to unpack it into program data directory
                if (Environment.Is64BitOperatingSystem)
                {
                    ///64bit
                    gincli = path + "gin-cli-latest-windows-amd64.zip";
                    web.DownloadFileAsync(new Uri(_gin64URL), gincli);
                }
                else
                {
                    ///32 bit                  
                    gincli = path + "gin-cli-latest-windows-386.zip";
                    web.DownloadFileAsync(new Uri(_ginURL), gincli);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}







