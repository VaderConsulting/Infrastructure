namespace TinyDHCP
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.txtStartIPAddress = new System.Windows.Forms.TextBox();
            this.lblStartingIPAddress = new System.Windows.Forms.Label();
            this.lblDHCPMACAddress = new System.Windows.Forms.Label();
            this.txtMACMask = new System.Windows.Forms.TextBox();
            this.btnSetServerState = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // txtStartIPAddress
            // 
            this.txtStartIPAddress.Location = new System.Drawing.Point(255, 32);
            this.txtStartIPAddress.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtStartIPAddress.Name = "txtStartIPAddress";
            this.txtStartIPAddress.Size = new System.Drawing.Size(121, 26);
            this.txtStartIPAddress.TabIndex = 2;
            // 
            // lblStartingIPAddress
            // 
            this.lblStartingIPAddress.AutoSize = true;
            this.lblStartingIPAddress.Location = new System.Drawing.Point(18, 37);
            this.lblStartingIPAddress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStartingIPAddress.Name = "lblStartingIPAddress";
            this.lblStartingIPAddress.Size = new System.Drawing.Size(139, 20);
            this.lblStartingIPAddress.TabIndex = 4;
            this.lblStartingIPAddress.Text = "Starting IP to Use:";
            // 
            // lblDHCPMACAddress
            // 
            this.lblDHCPMACAddress.AutoSize = true;
            this.lblDHCPMACAddress.Location = new System.Drawing.Point(18, 72);
            this.lblDHCPMACAddress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDHCPMACAddress.Name = "lblDHCPMACAddress";
            this.lblDHCPMACAddress.Size = new System.Drawing.Size(139, 20);
            this.lblDHCPMACAddress.TabIndex = 7;
            this.lblDHCPMACAddress.Text = "DHCP MAC Mask:";
            // 
            // txtMACMask
            // 
            this.txtMACMask.Location = new System.Drawing.Point(255, 72);
            this.txtMACMask.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtMACMask.Name = "txtMACMask";
            this.txtMACMask.Size = new System.Drawing.Size(180, 26);
            this.txtMACMask.TabIndex = 8;
            // 
            // btnSetServerState
            // 
            this.btnSetServerState.Location = new System.Drawing.Point(406, 129);
            this.btnSetServerState.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSetServerState.Name = "btnSetServerState";
            this.btnSetServerState.Size = new System.Drawing.Size(72, 43);
            this.btnSetServerState.TabIndex = 9;
            this.btnSetServerState.Text = "Start";
            this.btnSetServerState.UseVisualStyleBackColor = true;
            this.btnSetServerState.Click += new System.EventHandler(this.btnSetServerState_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 20;
            this.listBox1.Location = new System.Drawing.Point(18, 182);
            this.listBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.listBox1.Name = "listBox1";
            this.listBox1.ScrollAlwaysVisible = true;
            this.listBox1.Size = new System.Drawing.Size(460, 264);
            this.listBox1.TabIndex = 10;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 471);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.btnSetServerState);
            this.Controls.Add(this.txtMACMask);
            this.Controls.Add(this.lblDHCPMACAddress);
            this.Controls.Add(this.lblStartingIPAddress);
            this.Controls.Add(this.txtStartIPAddress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "frmMain";
            this.Text = "tiny DHCP Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtStartIPAddress;
        private System.Windows.Forms.Label lblStartingIPAddress;
        private System.Windows.Forms.Label lblDHCPMACAddress;
        private System.Windows.Forms.TextBox txtMACMask;
        private System.Windows.Forms.Button btnSetServerState;
        private System.Windows.Forms.ListBox listBox1;
    }
}

