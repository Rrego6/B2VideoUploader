using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace B2VideoUploader
{
    public partial class CredentialConfigForm : Form
    {
        public CredentialConfigForm()
        {
            InitializeComponent();
            this.Text = "Edit Credentials";
        }

        public delegate void HandleSaveCredentials(string applicationId, string applicationKey, string bucketId);
        public HandleSaveCredentials handleSaveCredentials;

        public void ShowEditCredentialsForm(string applicationId, string applicationKey, string bucketId, HandleSaveCredentials handleSaveCredentials)
        {
            this.application_id_field.Text = applicationId;
            this.application_key_field.Text = applicationKey;
            this.bucket_id_field.Text = bucketId;
            this.handleSaveCredentials = handleSaveCredentials;
            this.ShowDialog();
        }

        private void cancel_btn_Click(object sender, EventArgs e)
        {
            handleSaveCredentials = null;
            this.Close();
        }

        private void save_btn_Click(object sender, EventArgs e)
        {
            string applicationId = this.application_id_field.Text;
            string applicationKey = this.application_key_field.Text;
            string bucketId = this.bucket_id_field.Text;
            handleSaveCredentials(applicationId, applicationKey, bucketId);
            handleSaveCredentials = null;
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
