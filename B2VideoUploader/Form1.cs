using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using B2VideoUploader.Helper;
using B2VideoUploader.Model;
using B2VideoUploader.Properties;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.ListViewItem;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ListView = System.Windows.Forms.ListView;

namespace B2VideoUploader
{
    public partial class Form1 : Form
    {
        private readonly BlackBlazeUploadService b2UploadService;
        private readonly CustomLogger logger;
        private FfmpegVideoConversionService ffmpegVideoConversionService;
        IEnumerable<ProgressListViewItem> progressListViewItems = new List<ProgressListViewItem>();

        public Form1(BlackBlazeUploadService b2UploadService, FfmpegVideoConversionService ffmpegVideoConversionService, CustomLogger logger)
        {
            this.b2UploadService = b2UploadService;
            this.logger = logger;
            this.ffmpegVideoConversionService = ffmpegVideoConversionService;
            InitApplication();
            InitializeComponent();
        }

        private void InitApplication()
        {
            logger.addLoggingSubscriber(
                (logMessage) => {
                    loggingListView.Invoke(() => { loggingListView.Items.Add(logMessage); });
                    }
                );
        }

        private void btnAddVideosClick(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            //add extension filter etc
            ofd.Filter = "All files (*.*)|*.*";

            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (var f in ofd.FileNames)
                {
                    //Transform the list to a better presentation if needed
                    //Below code just adds the full path to list
                    listView1.Items.Add(f);

                    //Or use below code to just add file names
                    //listView1.Items.Add (Path.GetFileName (f));
                }
            }
        }



        private async Task<string> handleVideoUpload(string filePath, Action<string> progressCallback)
        {
            return await this.b2UploadService.uploadVideo(filePath, progressCallback);
        }

        private record VideoUploadInformationContainer(string FilePath, ListViewItem InProgressListViewItem, ListView CompletedListView);

        private async void btnUploadVideosClick(object sender, EventArgs e)
        {
            List<VideoUploadInformationContainer> videoUploadToDo = new List<VideoUploadInformationContainer>();
            foreach (ListViewItem listViewItem in ((ListView)listView1).Items)
            {
                var fileName = Path.GetFileName(listViewItem.Text);
                ListViewItem inProgressListViewItem = new ListViewItem(new string[] { fileName, "Not Started", "Not Started", ""});
                inProgressListView.Items.Add(inProgressListViewItem);
                //fix tasks to execute sequentially
                videoUploadToDo.Add(new VideoUploadInformationContainer(listViewItem.Text, inProgressListViewItem, completedListView));
                listView1.Items.Remove(listViewItem);
            }
                handleUploadTasks(videoUploadToDo);
        }

        private async void handleUploadTasks(List<VideoUploadInformationContainer> uploadInfos)
        {
            foreach(VideoUploadInformationContainer uploadInfo in uploadInfos )
            {
                try
                {
                    await handleVideoTask(uploadInfo.FilePath, uploadInfo.InProgressListViewItem, uploadInfo.CompletedListView);
                } catch(Exception e)
                {
                    logger.LogError(e.ToString());
                    throw;
                }
            }
        }

        private async Task handleVideoTask(string filePath, ListViewItem inProgressListViewItem, ListView completedListView)
        {
            inProgressListViewItem.SubItems[1] = new ListViewSubItem(inProgressListViewItem, "Started");
            inProgressListViewItem.SubItems[2] = new ListViewSubItem(inProgressListViewItem, "Not Started");
            var (outputVideoFilePath, subtitleFilePathTemp) = await ffmpegVideoConversionService.convertVideoToWebFormat(filePath,
                (double percentageUpdate) =>
                {
                    string statusMessage = $"{percentageUpdate}% encoded";
                    logger.LogInformation(statusMessage);
                    inProgressListViewItem.SubItems[3] = new ListViewSubItem(inProgressListViewItem, statusMessage);
                    inProgressListView.Invoke(inProgressListView.Update);
                }
                );
            var subtitleFilePath = await ffmpegVideoConversionService.extractSubtitles(filePath);
            inProgressListViewItem.SubItems[1] = new ListViewSubItem(inProgressListViewItem, "Finished");
            inProgressListViewItem.SubItems[2] = new ListViewSubItem(inProgressListViewItem, "Started");
            var videoUploadUrl = await handleVideoUpload(outputVideoFilePath, 
                (string update) =>
                {
                    inProgressListViewItem.SubItems[3] = new ListViewSubItem(inProgressListViewItem, update);
                    inProgressListView.Update();
                }
                );

            string? subtitleUrl = null;
            if(subtitleFilePath != null)
            {
                subtitleUrl = await b2UploadService.uploadFile(subtitleFilePath, "text/vtt");
            }
            var jsonFilePath = await ffmpegVideoConversionService.createCytubeJson(outputVideoFilePath, videoUploadUrl, subtitleUrl);
            var jsonUrl = await b2UploadService.uploadFile(jsonFilePath, "application/json");

            inProgressListView.Items.Remove(inProgressListViewItem);
            completedListView.Items.Add(jsonUrl);
        }



        private void clear_btn_Click(object sender, EventArgs e)
        {
            var listview = (ListView)completedListView;
            foreach (ListViewItem completedListViewItem in listview.SelectedItems)
            {
                completedListViewItem.Remove();
            }
        }

        private void clear_all_btn_Click(object sender, EventArgs e)
        {
            var listview = (ListView)completedListView;
            foreach (ListViewItem completedListViewItem in listview.Items)
            {
                completedListViewItem.Remove();
            }
        }

        private void CopyListViewSelectedToClipboard(ListView listView)
        {
            var count = listView.SelectedItems.Count;
            if(count == 0)
            {
                return;
            }
            var selectedItemsArr = new ListViewItem[count];
            listView.SelectedItems.CopyTo(selectedItemsArr, 0);
            var clipboardStr = string.Join("\n", (from listViewItem in selectedItemsArr select listViewItem.Text));
            if (!string.IsNullOrEmpty(clipboardStr))
            {
                Clipboard.SetText(clipboardStr);
            }
        }

        private void completedListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                foreach (ListViewItem completedListViewItem in ((ListView)sender).Items)
                {
                    completedListViewItem.Selected = true;
                }
            }
            if (e.Control && e.KeyCode == Keys.C)
            {
                CopyListViewSelectedToClipboard((ListView)sender);
            }
        }
        private void loggingListView_KeyDown(object sender, KeyEventArgs e)
        {
            var listview = (ListView)sender;
            if (e.Control && e.KeyCode == Keys.A)
            {
                foreach (ListViewItem completedListViewItem in listview.Items)
                {
                    completedListViewItem.Selected = true;
                }
            }
            if (e.Control && e.KeyCode == Keys.C)
            {
                CopyListViewSelectedToClipboard((ListView)sender);
            }
            if (e.KeyCode == Keys.Delete)
            {
                foreach (ListViewItem completedListViewItem in listview.SelectedItems)
                {
                    completedListViewItem.Remove();
                }
            }

        }


        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Control && e.KeyCode == Keys.A)
            {
                foreach (ListViewItem listViewItem in ((ListView)sender).Items)
                {
                    listViewItem.Selected = true;
                }
            }
            if (Keys.Delete == e.KeyCode)
            {
                foreach (ListViewItem listViewItem in ((ListView)sender).SelectedItems)
                {
                    listViewItem.Remove();
                }
            }
        }
    }
}