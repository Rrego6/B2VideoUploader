﻿namespace B2VideoUploader
{
    partial class CredentialConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.application_id_label = new System.Windows.Forms.Label();
            this.application_key_label = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.application_id_field = new System.Windows.Forms.TextBox();
            this.application_key_field = new System.Windows.Forms.TextBox();
            this.bucket_id_field = new System.Windows.Forms.TextBox();
            this.cancel_btn = new System.Windows.Forms.Button();
            this.save_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // application_id_label
            // 
            this.application_id_label.AutoSize = true;
            this.application_id_label.Location = new System.Drawing.Point(32, 19);
            this.application_id_label.Margin = new System.Windows.Forms.Padding(0, 0, 0, 20);
            this.application_id_label.Name = "application_id_label";
            this.application_id_label.Padding = new System.Windows.Forms.Padding(0, 0, 0, 20);
            this.application_id_label.Size = new System.Drawing.Size(85, 35);
            this.application_id_label.TabIndex = 0;
            this.application_id_label.Text = "Application ID:";
            this.application_id_label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // application_key_label
            // 
            this.application_key_label.AutoSize = true;
            this.application_key_label.Location = new System.Drawing.Point(24, 51);
            this.application_key_label.Margin = new System.Windows.Forms.Padding(0);
            this.application_key_label.Name = "application_key_label";
            this.application_key_label.Size = new System.Drawing.Size(93, 15);
            this.application_key_label.TabIndex = 1;
            this.application_key_label.Text = "Application Key:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(57, 86);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Bucket ID:";
            // 
            // application_id_field
            // 
            this.application_id_field.Location = new System.Drawing.Point(127, 19);
            this.application_id_field.Margin = new System.Windows.Forms.Padding(10);
            this.application_id_field.Name = "application_id_field";
            this.application_id_field.Size = new System.Drawing.Size(323, 23);
            this.application_id_field.TabIndex = 3;
            // 
            // application_key_field
            // 
            this.application_key_field.Location = new System.Drawing.Point(127, 51);
            this.application_key_field.Margin = new System.Windows.Forms.Padding(10);
            this.application_key_field.Name = "application_key_field";
            this.application_key_field.Size = new System.Drawing.Size(323, 23);
            this.application_key_field.TabIndex = 4;
            // 
            // bucket_id_field
            // 
            this.bucket_id_field.Location = new System.Drawing.Point(127, 86);
            this.bucket_id_field.Margin = new System.Windows.Forms.Padding(10);
            this.bucket_id_field.Name = "bucket_id_field";
            this.bucket_id_field.Size = new System.Drawing.Size(323, 23);
            this.bucket_id_field.TabIndex = 5;
            // 
            // cancel_btn
            // 
            this.cancel_btn.BackColor = System.Drawing.Color.RosyBrown;
            this.cancel_btn.Location = new System.Drawing.Point(24, 130);
            this.cancel_btn.Name = "cancel_btn";
            this.cancel_btn.Size = new System.Drawing.Size(75, 23);
            this.cancel_btn.TabIndex = 6;
            this.cancel_btn.Text = "Cancel";
            this.cancel_btn.UseVisualStyleBackColor = false;
            this.cancel_btn.Click += new System.EventHandler(this.cancel_btn_Click);
            // 
            // save_btn
            // 
            this.save_btn.BackColor = System.Drawing.Color.LightGreen;
            this.save_btn.Location = new System.Drawing.Point(488, 130);
            this.save_btn.Name = "save_btn";
            this.save_btn.Size = new System.Drawing.Size(75, 23);
            this.save_btn.TabIndex = 7;
            this.save_btn.Text = "Save";
            this.save_btn.UseVisualStyleBackColor = false;
            // 
            // CredentialConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 165);
            this.Controls.Add(this.save_btn);
            this.Controls.Add(this.cancel_btn);
            this.Controls.Add(this.bucket_id_field);
            this.Controls.Add(this.application_key_field);
            this.Controls.Add(this.application_id_field);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.application_key_label);
            this.Controls.Add(this.application_id_label);
            this.Name = "CredentialConfigForm";
            this.Text = "Form2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label application_id_label;
        private Label application_key_label;
        private Label label3;
        private TextBox application_id_field;
        private TextBox application_key_field;
        private TextBox bucket_id_field;
        private Button cancel_btn;
        private Button save_btn;
    }
}