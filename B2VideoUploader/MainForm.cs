using B2VideoUploader.Model;
using B2VideoUploader.Services;
using Microsoft.Extensions.Logging;
using static System.Windows.Forms.ListViewItem;
using ListView = System.Windows.Forms.ListView;

namespace B2VideoUploader
{
    public partial class MainForm : Form
    {
        private readonly BlackBlazeUploadService b2UploadService;
        private readonly CustomLogger logger;
        private readonly ConnectionSettingsValidator connectionSettingsValidator;
        private FfmpegVideoConversionService ffmpegVideoConversionService;

        public MainForm(BlackBlazeUploadService b2UploadService, FfmpegVideoConversionService ffmpegVideoConversionService, CustomLogger logger, Config config, ConnectionSettingsValidator connectionSettingsValidator)
        {
            this.b2UploadService = b2UploadService;
            this.logger = logger;
            this.connectionSettingsValidator = connectionSettingsValidator;
            this.ffmpegVideoConversionService = ffmpegVideoConversionService;
            InitializeComponent();
            InitApplication();
            this.Text = "B2VideoUploader";
        }


        private void OnConnectionStatusUpdated(bool isValid, string errMsg)
        {
            this.Invoke(() =>
            {
                if (isValid)
                {
                    this.connection_status_string.Text = "Connected ✅";
                    this.connection_status_string.ForeColor = Color.Green;
                    this.add_videos_btn.Enabled = true;
                    this.start_upload_btn.Enabled = true;
                }
                else
                {
                    this.connection_status_string.ForeColor = Color.Red;
                    this.connection_status_string.Text = "Failed To Connect ❌";
                    this.add_videos_btn.Enabled = false;
                    this.start_upload_btn.Enabled = false;
                }
            });
        }

        private async void InitApplication()
        {
            logger.addLoggingSubscriber(
                (logMessage) => {
                    loggingListView.Invoke(() => { loggingListView.Items.Add(logMessage); });
                    }
                );
            (bool isLoginValid, string errLoginString) = await connectionSettingsValidator.ValidateLoginConfiguration(this.OnConnectionStatusUpdated);
            (bool isBucketValid, string errBucket) = await connectionSettingsValidator.ValidateBucketConfiguration();
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
            Task[] tasks = new Task[uploadInfos.Count()];
            int taskCount = 0;
            foreach(VideoUploadInformationContainer uploadInfo in uploadInfos )
            {
                try
                {
                    Task task = handleVideoTask(uploadInfo.FilePath, uploadInfo.InProgressListViewItem, uploadInfo.CompletedListView);
                    tasks[taskCount++] = task;
                    await task;
                    
                } catch(Exception e)
                {
                    logger.LogError(e.ToString());
                    throw;
                }
            }
        }

        //todo: Move to VideoUploadService
        private async Task handleVideoTask(string filePath, ListViewItem inProgressListViewItem, ListView completedListView)
        {
            inProgressListViewItem.SubItems[1] = new ListViewSubItem(inProgressListViewItem, "Started");
            inProgressListViewItem.SubItems[2] = new ListViewSubItem(inProgressListViewItem, "Not Started");
            var (outputVideoFilePath, subtitleFilePathTemp) = await ffmpegVideoConversionService.convertVideoToWebFormat(filePath,
                (double percentageUpdate) =>
                {
                    string statusMessage = $"{percentageUpdate}% encoded";
                    logger.LogInformation(statusMessage);
                    inProgressListView.Invoke(() =>
                    {
                        inProgressListViewItem.SubItems[3] = new ListViewSubItem(inProgressListViewItem, statusMessage);
                        inProgressListView.Invoke(inProgressListView.Update);
                    });
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

        private void connectionStatusStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void edit_connection_status_btn_click(object sender, EventArgs e)
        {
            connectionSettingsValidator.EditConnectionSettingsPrompt(OnConnectionStatusUpdated);
        }

        private void connection_status_string_Click(object sender, EventArgs e)
        {

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