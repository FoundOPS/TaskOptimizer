namespace TaskOptimizerDevKit
{
    partial class Form1
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbOsrmAddress = new System.Windows.Forms.ComboBox();
            this.btnOsrmPing = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lnlOsrmInsertCoord = new System.Windows.Forms.LinkLabel();
            this.txtOsrmResponse = new System.Windows.Forms.TextBox();
            this.btnOsrmSubmit = new System.Windows.Forms.Button();
            this.lbOsrmArgs = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lbOsrmMethod = new System.Windows.Forms.ListBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(710, 437);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.btnOsrmSubmit);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.btnOsrmPing);
            this.tabPage1.Controls.Add(this.cmbOsrmAddress);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(702, 411);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "OSRM API Call";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(702, 411);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Redis Cache Viewer";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server Address";
            // 
            // cmbOsrmAddress
            // 
            this.cmbOsrmAddress.FormattingEnabled = true;
            this.cmbOsrmAddress.Location = new System.Drawing.Point(91, 9);
            this.cmbOsrmAddress.Name = "cmbOsrmAddress";
            this.cmbOsrmAddress.Size = new System.Drawing.Size(508, 21);
            this.cmbOsrmAddress.TabIndex = 1;
            // 
            // btnOsrmPing
            // 
            this.btnOsrmPing.Location = new System.Drawing.Point(605, 9);
            this.btnOsrmPing.Name = "btnOsrmPing";
            this.btnOsrmPing.Size = new System.Drawing.Size(91, 21);
            this.btnOsrmPing.TabIndex = 2;
            this.btnOsrmPing.Text = "Ping";
            this.btnOsrmPing.UseVisualStyleBackColor = true;
            this.btnOsrmPing.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lbOsrmMethod);
            this.groupBox1.Location = new System.Drawing.Point(9, 36);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(284, 146);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Request Data";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.txtOsrmResponse);
            this.groupBox2.Location = new System.Drawing.Point(9, 188);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(687, 179);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Response Data";
            // 
            // lnlOsrmInsertCoord
            // 
            this.lnlOsrmInsertCoord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnlOsrmInsertCoord.AutoSize = true;
            this.lnlOsrmInsertCoord.Location = new System.Drawing.Point(309, 21);
            this.lnlOsrmInsertCoord.Name = "lnlOsrmInsertCoord";
            this.lnlOsrmInsertCoord.Size = new System.Drawing.Size(80, 13);
            this.lnlOsrmInsertCoord.TabIndex = 4;
            this.lnlOsrmInsertCoord.TabStop = true;
            this.lnlOsrmInsertCoord.Text = "Add Coordinate";
            this.lnlOsrmInsertCoord.VisitedLinkColor = System.Drawing.Color.Blue;
            this.lnlOsrmInsertCoord.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnlOsrmInsertCoord_LinkClicked);
            // 
            // txtOsrmResponse
            // 
            this.txtOsrmResponse.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOsrmResponse.Location = new System.Drawing.Point(6, 19);
            this.txtOsrmResponse.Multiline = true;
            this.txtOsrmResponse.Name = "txtOsrmResponse";
            this.txtOsrmResponse.Size = new System.Drawing.Size(674, 154);
            this.txtOsrmResponse.TabIndex = 0;
            // 
            // btnOsrmSubmit
            // 
            this.btnOsrmSubmit.Location = new System.Drawing.Point(582, 373);
            this.btnOsrmSubmit.Name = "btnOsrmSubmit";
            this.btnOsrmSubmit.Size = new System.Drawing.Size(114, 32);
            this.btnOsrmSubmit.TabIndex = 5;
            this.btnOsrmSubmit.Text = "Submit Request";
            this.btnOsrmSubmit.UseVisualStyleBackColor = true;
            this.btnOsrmSubmit.Click += new System.EventHandler(this.btnOsrmSubmit_Click);
            // 
            // lbOsrmArgs
            // 
            this.lbOsrmArgs.FormattingEnabled = true;
            this.lbOsrmArgs.Location = new System.Drawing.Point(6, 17);
            this.lbOsrmArgs.Name = "lbOsrmArgs";
            this.lbOsrmArgs.Size = new System.Drawing.Size(294, 121);
            this.lbOsrmArgs.TabIndex = 5;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lnlOsrmInsertCoord);
            this.groupBox3.Controls.Add(this.lbOsrmArgs);
            this.groupBox3.Location = new System.Drawing.Point(299, 36);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(397, 146);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Arguments";
            // 
            // lbOsrmMethod
            // 
            this.lbOsrmMethod.FormattingEnabled = true;
            this.lbOsrmMethod.Items.AddRange(new object[] {
            "FindNearestNode",
            "GetDistanceTime",
            "CalculateRoute"});
            this.lbOsrmMethod.Location = new System.Drawing.Point(6, 17);
            this.lbOsrmMethod.Name = "lbOsrmMethod";
            this.lbOsrmMethod.Size = new System.Drawing.Size(270, 121);
            this.lbOsrmMethod.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 461);
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(750, 500);
            this.Name = "Form1";
            this.Text = "TaskOptimizer Developer Tools";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ComboBox cmbOsrmAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOsrmPing;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.LinkLabel lnlOsrmInsertCoord;
        private System.Windows.Forms.Button btnOsrmSubmit;
        private System.Windows.Forms.TextBox txtOsrmResponse;
        private System.Windows.Forms.ListBox lbOsrmArgs;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListBox lbOsrmMethod;
    }
}

