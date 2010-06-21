namespace TreeViewPanelExtension
{
    partial class AboutForm
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
            this.lnkUrl = new System.Windows.Forms.LinkLabel();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblInstalledVersion = new System.Windows.Forms.Label();
            this.lblGetLatest = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lnkUrl
            // 
            this.lnkUrl.AutoSize = true;
            this.lnkUrl.Location = new System.Drawing.Point(12, 165);
            this.lnkUrl.Name = "lnkUrl";
            this.lnkUrl.Size = new System.Drawing.Size(260, 17);
            this.lnkUrl.TabIndex = 0;
            this.lnkUrl.TabStop = true;
            this.lnkUrl.Text = "http://fiddlertreeviewpanel.codeplex.com";
            this.lnkUrl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkUrl_LinkClicked);
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(132, 64);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(53, 17);
            this.lblVersion.TabIndex = 1;
            this.lblVersion.Text = "XXXXX";
            // 
            // lblInstalledVersion
            // 
            this.lblInstalledVersion.AutoSize = true;
            this.lblInstalledVersion.Location = new System.Drawing.Point(12, 64);
            this.lblInstalledVersion.Name = "lblInstalledVersion";
            this.lblInstalledVersion.Size = new System.Drawing.Size(114, 17);
            this.lblInstalledVersion.TabIndex = 2;
            this.lblInstalledVersion.Text = "Installed version:";
            // 
            // lblGetLatest
            // 
            this.lblGetLatest.AutoSize = true;
            this.lblGetLatest.Location = new System.Drawing.Point(12, 148);
            this.lblGetLatest.Name = "lblGetLatest";
            this.lblGetLatest.Size = new System.Drawing.Size(252, 17);
            this.lblGetLatest.TabIndex = 3;
            this.lblGetLatest.Text = "To get the latest version please check:";
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(101, 206);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 37);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(282, 255);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblGetLatest);
            this.Controls.Add(this.lblInstalledVersion);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lnkUrl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fiddler TreeView Panel Extension";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel lnkUrl;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblInstalledVersion;
        private System.Windows.Forms.Label lblGetLatest;
        private System.Windows.Forms.Button btnClose;
    }
}