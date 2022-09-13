using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace B2VideoUploader
{
    public partial class CredentialConfigForm : Form
    {

        public CredentialConfigForm()
        {
            InitializeComponent();
        }

        public void ShowEditCredentialsForm(string applicationId, string applicationKey, string bucketId)
        {
            this.application_id_field.Text = applicationId;
            this.application_key_field.Text = applicationKey;
            this.bucket_id_field.Text = bucketId;
            this.ShowDialog();
        }

        private void cancel_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
