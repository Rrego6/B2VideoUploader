namespace B2VideoUploader
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.edit_connection_status_btn = new System.Windows.Forms.Button();
            this.connection_status_string = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listView1 = new System.Windows.Forms.ListView();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.add_videos_btn = new System.Windows.Forms.Button();
            this.start_upload_btn = new System.Windows.Forms.Button();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.clear_btn = new System.Windows.Forms.Button();
            this.clear_all_btn = new System.Windows.Forms.Button();
            this.completedListView = new System.Windows.Forms.ListView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.inProgressStatusPage = new System.Windows.Forms.TabPage();
            this.inProgressListView = new System.Windows.Forms.ListView();
            this.fileHeader = new System.Windows.Forms.ColumnHeader();
            this.conversionStatusHeader = new System.Windows.Forms.ColumnHeader();
            this.uploadStatusHeader = new System.Windows.Forms.ColumnHeader();
            this.progressInfoHeader = new System.Windows.Forms.ColumnHeader();
            this.loggingPage = new System.Windows.Forms.TabPage();
            this.loggingListView = new System.Windows.Forms.ListView();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.inProgressStatusPage.SuspendLayout();
            this.loggingPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(23, 0, 23, 0);
            this.panel1.Size = new System.Drawing.Size(1172, 673);
            this.panel1.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(23, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.103727F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 91.89627F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 186F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1126, 673);
            this.tableLayoutPanel1.TabIndex = 3;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.edit_connection_status_btn);
            this.flowLayoutPanel3.Controls.Add(this.connection_status_string);
            this.flowLayoutPanel3.Controls.Add(this.label1);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(1120, 33);
            this.flowLayoutPanel3.TabIndex = 3;
            // 
            // edit_connection_status_btn
            // 
            this.edit_connection_status_btn.AutoSize = true;
            this.edit_connection_status_btn.Location = new System.Drawing.Point(975, 3);
            this.edit_connection_status_btn.Name = "edit_connection_status_btn";
            this.edit_connection_status_btn.Size = new System.Drawing.Size(142, 25);
            this.edit_connection_status_btn.TabIndex = 3;
            this.edit_connection_status_btn.Text = "Edit Connection Status";
            this.edit_connection_status_btn.UseVisualStyleBackColor = true;
            this.edit_connection_status_btn.Click += new System.EventHandler(this.edit_connection_status_btn_click);
            // 
            // connection_status_string
            // 
            this.connection_status_string.AutoSize = true;
            this.connection_status_string.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connection_status_string.Location = new System.Drawing.Point(931, 0);
            this.connection_status_string.Name = "connection_status_string";
            this.connection_status_string.Size = new System.Drawing.Size(38, 31);
            this.connection_status_string.TabIndex = 5;
            this.connection_status_string.Text = "label2";
            this.connection_status_string.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.connection_status_string.Click += new System.EventHandler(this.connection_status_string_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(818, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 31);
            this.label1.TabIndex = 4;
            this.label1.Text = "Connection Status:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(4, 42);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listView1);
            this.splitContainer1.Panel1.Controls.Add(this.flowLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.flowLayoutPanel2);
            this.splitContainer1.Panel2.Controls.Add(this.completedListView);
            this.splitContainer1.Size = new System.Drawing.Size(1118, 441);
            this.splitContainer1.SplitterDistance = 549;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // listView1
            // 
            this.listView1.BackColor = System.Drawing.SystemColors.Window;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(549, 389);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged_1);
            this.listView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyDown);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.add_videos_btn);
            this.flowLayoutPanel1.Controls.Add(this.start_upload_btn);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 389);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(549, 52);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // add_videos_btn
            // 
            this.add_videos_btn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.add_videos_btn.Location = new System.Drawing.Point(4, 3);
            this.add_videos_btn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.add_videos_btn.Name = "add_videos_btn";
            this.add_videos_btn.Size = new System.Drawing.Size(88, 27);
            this.add_videos_btn.TabIndex = 0;
            this.add_videos_btn.Text = "Add Videos";
            this.add_videos_btn.UseVisualStyleBackColor = true;
            this.add_videos_btn.Click += new System.EventHandler(this.btnAddVideosClick);
            // 
            // start_upload_btn
            // 
            this.start_upload_btn.Location = new System.Drawing.Point(100, 3);
            this.start_upload_btn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.start_upload_btn.Name = "start_upload_btn";
            this.start_upload_btn.Size = new System.Drawing.Size(88, 27);
            this.start_upload_btn.TabIndex = 1;
            this.start_upload_btn.Text = "Start Upload";
            this.start_upload_btn.UseVisualStyleBackColor = true;
            this.start_upload_btn.Click += new System.EventHandler(this.btnUploadVideosClick);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.clear_btn);
            this.flowLayoutPanel2.Controls.Add(this.clear_all_btn);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 389);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(564, 52);
            this.flowLayoutPanel2.TabIndex = 3;
            // 
            // clear_btn
            // 
            this.clear_btn.Location = new System.Drawing.Point(4, 3);
            this.clear_btn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.clear_btn.Name = "clear_btn";
            this.clear_btn.Size = new System.Drawing.Size(88, 27);
            this.clear_btn.TabIndex = 0;
            this.clear_btn.Text = "Clear";
            this.clear_btn.UseVisualStyleBackColor = true;
            this.clear_btn.Click += new System.EventHandler(this.clear_btn_Click);
            // 
            // clear_all_btn
            // 
            this.clear_all_btn.Location = new System.Drawing.Point(100, 3);
            this.clear_all_btn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.clear_all_btn.Name = "clear_all_btn";
            this.clear_all_btn.Size = new System.Drawing.Size(88, 27);
            this.clear_all_btn.TabIndex = 1;
            this.clear_all_btn.Text = "Clear All";
            this.clear_all_btn.UseVisualStyleBackColor = true;
            this.clear_all_btn.Click += new System.EventHandler(this.clear_all_btn_Click);
            // 
            // completedListView
            // 
            this.completedListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.completedListView.GridLines = true;
            this.completedListView.Location = new System.Drawing.Point(0, 0);
            this.completedListView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.completedListView.Name = "completedListView";
            this.completedListView.Size = new System.Drawing.Size(564, 441);
            this.completedListView.TabIndex = 2;
            this.completedListView.UseCompatibleStateImageBehavior = false;
            this.completedListView.View = System.Windows.Forms.View.List;
            this.completedListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.completedListView_KeyDown);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.inProgressStatusPage);
            this.tabControl1.Controls.Add(this.loggingPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 489);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1120, 181);
            this.tabControl1.TabIndex = 1;
            // 
            // inProgressStatusPage
            // 
            this.inProgressStatusPage.Controls.Add(this.inProgressListView);
            this.inProgressStatusPage.Location = new System.Drawing.Point(4, 24);
            this.inProgressStatusPage.Name = "inProgressStatusPage";
            this.inProgressStatusPage.Padding = new System.Windows.Forms.Padding(3);
            this.inProgressStatusPage.Size = new System.Drawing.Size(1112, 153);
            this.inProgressStatusPage.TabIndex = 0;
            this.inProgressStatusPage.Text = "In Progress";
            this.inProgressStatusPage.UseVisualStyleBackColor = true;
            // 
            // inProgressListView
            // 
            this.inProgressListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.fileHeader,
            this.conversionStatusHeader,
            this.uploadStatusHeader,
            this.progressInfoHeader});
            this.inProgressListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inProgressListView.FullRowSelect = true;
            this.inProgressListView.GridLines = true;
            this.inProgressListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.inProgressListView.Location = new System.Drawing.Point(3, 3);
            this.inProgressListView.Name = "inProgressListView";
            this.inProgressListView.Size = new System.Drawing.Size(1106, 147);
            this.inProgressListView.TabIndex = 0;
            this.inProgressListView.UseCompatibleStateImageBehavior = false;
            this.inProgressListView.View = System.Windows.Forms.View.Details;
            // 
            // fileHeader
            // 
            this.fileHeader.Text = "File";
            this.fileHeader.Width = 200;
            // 
            // conversionStatusHeader
            // 
            this.conversionStatusHeader.Text = "Conversion Status";
            this.conversionStatusHeader.Width = 200;
            // 
            // uploadStatusHeader
            // 
            this.uploadStatusHeader.Text = "Upload Status";
            this.uploadStatusHeader.Width = 200;
            // 
            // progressInfoHeader
            // 
            this.progressInfoHeader.Text = "Progress";
            this.progressInfoHeader.Width = 200;
            // 
            // loggingPage
            // 
            this.loggingPage.Controls.Add(this.loggingListView);
            this.loggingPage.Location = new System.Drawing.Point(4, 24);
            this.loggingPage.Name = "loggingPage";
            this.loggingPage.Padding = new System.Windows.Forms.Padding(3);
            this.loggingPage.Size = new System.Drawing.Size(1112, 153);
            this.loggingPage.TabIndex = 1;
            this.loggingPage.Text = "Log";
            this.loggingPage.UseVisualStyleBackColor = true;
            // 
            // loggingListView
            // 
            this.loggingListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loggingListView.Location = new System.Drawing.Point(3, 3);
            this.loggingListView.Name = "loggingListView";
            this.loggingListView.Size = new System.Drawing.Size(1106, 147);
            this.loggingListView.TabIndex = 0;
            this.loggingListView.UseCompatibleStateImageBehavior = false;
            this.loggingListView.View = System.Windows.Forms.View.List;
            this.loggingListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.loggingListView_KeyDown);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1172, 673);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.inProgressStatusPage.ResumeLayout(false);
            this.loggingPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button add_videos_btn;
        private System.Windows.Forms.Button start_upload_btn;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button clear_btn;
        private System.Windows.Forms.Button clear_all_btn;
        private System.Windows.Forms.ListView completedListView;
        private TableLayoutPanel tableLayoutPanel1;
        private TabControl tabControl1;
        private TabPage inProgressStatusPage;
        private ListView inProgressListView;
        private TabPage loggingPage;
        private ColumnHeader fileHeader;
        private ColumnHeader conversionStatusHeader;
        private ColumnHeader uploadStatusHeader;
        private ColumnHeader progressInfoHeader;
        private ListView loggingListView;
        private FlowLayoutPanel flowLayoutPanel3;
        private Label label1;
        private Button button1;
        private Label connection_status_string;
        private Button edit_connection_status_btn;
    }
}