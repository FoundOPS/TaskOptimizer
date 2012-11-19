namespace TaskOptimizerDevKit
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.llnOsrmClear = new System.Windows.Forms.LinkLabel();
            this.lnlOsrmInsertCoord = new System.Windows.Forms.LinkLabel();
            this.lbOsrmArgs = new System.Windows.Forms.ListBox();
            this.btnOsrmSubmit = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtOsrmResponse = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbOsrmMethod = new System.Windows.Forms.ListBox();
            this.btnOsrmPing = new System.Windows.Forms.Button();
            this.cmbOsrmAddress = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.wsPprocConnect = new Nightingale.WizardState();
            this.button4 = new System.Windows.Forms.Button();
            this.txtPpConsole = new System.Windows.Forms.RichTextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.wsPprocReview = new Nightingale.WizardState();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.label10 = new System.Windows.Forms.Label();
            this.wsPprocConfig = new Nightingale.WizardState();
            this.nudPpCfgPprocC = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.lblPpCfgOPingResult = new System.Windows.Forms.Label();
            this.lblPpCfgRPingResult = new System.Windows.Forms.Label();
            this.btnPpCgfNext = new System.Windows.Forms.Button();
            this.nudPpCfgTaskC = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.nudPpCfgWorkerC = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.txtPpCfgRnd = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnPpCfgOsrmPing = new System.Windows.Forms.Button();
            this.btnPpCfgRedisPing = new System.Windows.Forms.Button();
            this.txtPpCfgOsrmAddr = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPpCfgRedisAddr = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnPpCfgGenId = new System.Windows.Forms.Button();
            this.txtPpCfgId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.erp = new System.Windows.Forms.ErrorProvider(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.wsPprocConnect.SuspendLayout();
            this.wsPprocReview.SuspendLayout();
            this.wsPprocConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPpCfgPprocC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPpCfgTaskC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPpCfgWorkerC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.erp)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
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
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.llnOsrmClear);
            this.groupBox3.Controls.Add(this.lnlOsrmInsertCoord);
            this.groupBox3.Controls.Add(this.lbOsrmArgs);
            this.groupBox3.Location = new System.Drawing.Point(299, 36);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(397, 146);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Arguments";
            // 
            // llnOsrmClear
            // 
            this.llnOsrmClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llnOsrmClear.AutoSize = true;
            this.llnOsrmClear.Location = new System.Drawing.Point(309, 46);
            this.llnOsrmClear.Name = "llnOsrmClear";
            this.llnOsrmClear.Size = new System.Drawing.Size(50, 13);
            this.llnOsrmClear.TabIndex = 6;
            this.llnOsrmClear.TabStop = true;
            this.llnOsrmClear.Text = "Clear List";
            this.llnOsrmClear.VisitedLinkColor = System.Drawing.Color.Blue;
            this.llnOsrmClear.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llnOsrmClear_LinkClicked);
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
            // lbOsrmArgs
            // 
            this.lbOsrmArgs.FormattingEnabled = true;
            this.lbOsrmArgs.Location = new System.Drawing.Point(6, 17);
            this.lbOsrmArgs.Name = "lbOsrmArgs";
            this.lbOsrmArgs.Size = new System.Drawing.Size(294, 121);
            this.lbOsrmArgs.TabIndex = 5;
            // 
            // btnOsrmSubmit
            // 
            this.btnOsrmSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOsrmSubmit.Location = new System.Drawing.Point(582, 373);
            this.btnOsrmSubmit.Name = "btnOsrmSubmit";
            this.btnOsrmSubmit.Size = new System.Drawing.Size(114, 32);
            this.btnOsrmSubmit.TabIndex = 5;
            this.btnOsrmSubmit.Text = "Submit Request";
            this.btnOsrmSubmit.UseVisualStyleBackColor = true;
            this.btnOsrmSubmit.Click += new System.EventHandler(this.btnOsrmSubmit_Click);
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
            // cmbOsrmAddress
            // 
            this.cmbOsrmAddress.FormattingEnabled = true;
            this.cmbOsrmAddress.Location = new System.Drawing.Point(91, 9);
            this.cmbOsrmAddress.Name = "cmbOsrmAddress";
            this.cmbOsrmAddress.Size = new System.Drawing.Size(508, 21);
            this.cmbOsrmAddress.TabIndex = 1;
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
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.wsPprocConnect);
            this.tabPage3.Controls.Add(this.wsPprocReview);
            this.tabPage3.Controls.Add(this.wsPprocConfig);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(702, 411);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Preprocessing";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // wsPprocConnect
            // 
            this.wsPprocConnect.Controls.Add(this.button4);
            this.wsPprocConnect.Controls.Add(this.txtPpConsole);
            this.wsPprocConnect.Controls.Add(this.button3);
            this.wsPprocConnect.Controls.Add(this.textBox1);
            this.wsPprocConnect.Controls.Add(this.label12);
            this.wsPprocConnect.Controls.Add(this.label11);
            this.wsPprocConnect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wsPprocConnect.Location = new System.Drawing.Point(3, 3);
            this.wsPprocConnect.Name = "wsPprocConnect";
            this.wsPprocConnect.Size = new System.Drawing.Size(696, 405);
            this.wsPprocConnect.StateManager = null;
            this.wsPprocConnect.TabIndex = 2;
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(596, 371);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(97, 31);
            this.button4.TabIndex = 18;
            this.button4.Text = "Reset";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // txtPpConsole
            // 
            this.txtPpConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPpConsole.BackColor = System.Drawing.Color.Black;
            this.txtPpConsole.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPpConsole.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPpConsole.ForeColor = System.Drawing.SystemColors.GrayText;
            this.txtPpConsole.Location = new System.Drawing.Point(3, 82);
            this.txtPpConsole.Name = "txtPpConsole";
            this.txtPpConsole.Size = new System.Drawing.Size(690, 275);
            this.txtPpConsole.TabIndex = 10;
            this.txtPpConsole.Text = "DistributionController Mockup..";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(454, 53);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(69, 20);
            this.button3.TabIndex = 9;
            this.button3.Text = "Connect";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(132, 53);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(316, 20);
            this.textBox1.TabIndex = 4;
            this.textBox1.Text = "127.0.0.1";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(20, 56);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(106, 13);
            this.label12.TabIndex = 3;
            this.label12.Text = "Distribution Server IP";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(16, 16);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(332, 26);
            this.label11.TabIndex = 2;
            this.label11.Text = "Connect to Distribution Server";
            // 
            // wsPprocReview
            // 
            this.wsPprocReview.Controls.Add(this.button2);
            this.wsPprocReview.Controls.Add(this.button1);
            this.wsPprocReview.Controls.Add(this.treeView1);
            this.wsPprocReview.Controls.Add(this.label10);
            this.wsPprocReview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wsPprocReview.Location = new System.Drawing.Point(3, 3);
            this.wsPprocReview.Name = "wsPprocReview";
            this.wsPprocReview.Size = new System.Drawing.Size(696, 405);
            this.wsPprocReview.StateManager = null;
            this.wsPprocReview.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(493, 371);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(97, 31);
            this.button2.TabIndex = 18;
            this.button2.Text = "Back";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(596, 371);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 31);
            this.button1.TabIndex = 17;
            this.button1.Text = "Next";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.Location = new System.Drawing.Point(3, 45);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(690, 312);
            this.treeView1.TabIndex = 2;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(16, 16);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(379, 26);
            this.label10.TabIndex = 1;
            this.label10.Text = "Review Preprocessing Parameters";
            // 
            // wsPprocConfig
            // 
            this.wsPprocConfig.Controls.Add(this.nudPpCfgPprocC);
            this.wsPprocConfig.Controls.Add(this.label9);
            this.wsPprocConfig.Controls.Add(this.lblPpCfgOPingResult);
            this.wsPprocConfig.Controls.Add(this.lblPpCfgRPingResult);
            this.wsPprocConfig.Controls.Add(this.btnPpCgfNext);
            this.wsPprocConfig.Controls.Add(this.nudPpCfgTaskC);
            this.wsPprocConfig.Controls.Add(this.label8);
            this.wsPprocConfig.Controls.Add(this.nudPpCfgWorkerC);
            this.wsPprocConfig.Controls.Add(this.label7);
            this.wsPprocConfig.Controls.Add(this.txtPpCfgRnd);
            this.wsPprocConfig.Controls.Add(this.label6);
            this.wsPprocConfig.Controls.Add(this.btnPpCfgOsrmPing);
            this.wsPprocConfig.Controls.Add(this.btnPpCfgRedisPing);
            this.wsPprocConfig.Controls.Add(this.txtPpCfgOsrmAddr);
            this.wsPprocConfig.Controls.Add(this.label5);
            this.wsPprocConfig.Controls.Add(this.txtPpCfgRedisAddr);
            this.wsPprocConfig.Controls.Add(this.label4);
            this.wsPprocConfig.Controls.Add(this.btnPpCfgGenId);
            this.wsPprocConfig.Controls.Add(this.txtPpCfgId);
            this.wsPprocConfig.Controls.Add(this.label3);
            this.wsPprocConfig.Controls.Add(this.label2);
            this.wsPprocConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wsPprocConfig.Location = new System.Drawing.Point(3, 3);
            this.wsPprocConfig.Name = "wsPprocConfig";
            this.wsPprocConfig.Size = new System.Drawing.Size(696, 405);
            this.wsPprocConfig.StateManager = null;
            this.wsPprocConfig.TabIndex = 0;
            // 
            // nudPpCfgPprocC
            // 
            this.nudPpCfgPprocC.Location = new System.Drawing.Point(95, 260);
            this.nudPpCfgPprocC.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.nudPpCfgPprocC.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudPpCfgPprocC.Name = "nudPpCfgPprocC";
            this.nudPpCfgPprocC.Size = new System.Drawing.Size(91, 20);
            this.nudPpCfgPprocC.TabIndex = 20;
            this.nudPpCfgPprocC.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(20, 262);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(69, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Distribution #";
            // 
            // lblPpCfgOPingResult
            // 
            this.lblPpCfgOPingResult.AutoSize = true;
            this.lblPpCfgOPingResult.Location = new System.Drawing.Point(529, 121);
            this.lblPpCfgOPingResult.Name = "lblPpCfgOPingResult";
            this.lblPpCfgOPingResult.Size = new System.Drawing.Size(0, 13);
            this.lblPpCfgOPingResult.TabIndex = 18;
            // 
            // lblPpCfgRPingResult
            // 
            this.lblPpCfgRPingResult.AutoSize = true;
            this.lblPpCfgRPingResult.Location = new System.Drawing.Point(529, 95);
            this.lblPpCfgRPingResult.Name = "lblPpCfgRPingResult";
            this.lblPpCfgRPingResult.Size = new System.Drawing.Size(0, 13);
            this.lblPpCfgRPingResult.TabIndex = 17;
            // 
            // btnPpCgfNext
            // 
            this.btnPpCgfNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPpCgfNext.Location = new System.Drawing.Point(596, 371);
            this.btnPpCgfNext.Name = "btnPpCgfNext";
            this.btnPpCgfNext.Size = new System.Drawing.Size(97, 31);
            this.btnPpCgfNext.TabIndex = 16;
            this.btnPpCgfNext.Text = "Next";
            this.btnPpCgfNext.UseVisualStyleBackColor = true;
            this.btnPpCgfNext.Click += new System.EventHandler(this.btnPpCgfNext_Click);
            // 
            // nudPpCfgTaskC
            // 
            this.nudPpCfgTaskC.Location = new System.Drawing.Point(95, 234);
            this.nudPpCfgTaskC.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudPpCfgTaskC.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudPpCfgTaskC.Name = "nudPpCfgTaskC";
            this.nudPpCfgTaskC.Size = new System.Drawing.Size(91, 20);
            this.nudPpCfgTaskC.TabIndex = 15;
            this.nudPpCfgTaskC.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(27, 236);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(62, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Task Count";
            // 
            // nudPpCfgWorkerC
            // 
            this.nudPpCfgWorkerC.Location = new System.Drawing.Point(95, 208);
            this.nudPpCfgWorkerC.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudPpCfgWorkerC.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudPpCfgWorkerC.Name = "nudPpCfgWorkerC";
            this.nudPpCfgWorkerC.Size = new System.Drawing.Size(91, 20);
            this.nudPpCfgWorkerC.TabIndex = 13;
            this.nudPpCfgWorkerC.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 210);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Worker Count";
            // 
            // txtPpCfgRnd
            // 
            this.txtPpCfgRnd.Location = new System.Drawing.Point(95, 157);
            this.txtPpCfgRnd.Name = "txtPpCfgRnd";
            this.txtPpCfgRnd.Size = new System.Drawing.Size(353, 20);
            this.txtPpCfgRnd.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 160);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Random Seed";
            // 
            // btnPpCfgOsrmPing
            // 
            this.btnPpCfgOsrmPing.Location = new System.Drawing.Point(454, 117);
            this.btnPpCfgOsrmPing.Name = "btnPpCfgOsrmPing";
            this.btnPpCfgOsrmPing.Size = new System.Drawing.Size(69, 20);
            this.btnPpCfgOsrmPing.TabIndex = 9;
            this.btnPpCfgOsrmPing.Text = "Ping";
            this.btnPpCfgOsrmPing.UseVisualStyleBackColor = true;
            this.btnPpCfgOsrmPing.Click += new System.EventHandler(this.btnPpCfgOsrmPing_Click);
            // 
            // btnPpCfgRedisPing
            // 
            this.btnPpCfgRedisPing.Location = new System.Drawing.Point(454, 91);
            this.btnPpCfgRedisPing.Name = "btnPpCfgRedisPing";
            this.btnPpCfgRedisPing.Size = new System.Drawing.Size(69, 20);
            this.btnPpCfgRedisPing.TabIndex = 8;
            this.btnPpCfgRedisPing.Text = "Ping";
            this.btnPpCfgRedisPing.UseVisualStyleBackColor = true;
            this.btnPpCfgRedisPing.Click += new System.EventHandler(this.btnPpCfgRedisPing_Click);
            // 
            // txtPpCfgOsrmAddr
            // 
            this.txtPpCfgOsrmAddr.Location = new System.Drawing.Point(95, 118);
            this.txtPpCfgOsrmAddr.Name = "txtPpCfgOsrmAddr";
            this.txtPpCfgOsrmAddr.Size = new System.Drawing.Size(353, 20);
            this.txtPpCfgOsrmAddr.TabIndex = 7;
            this.txtPpCfgOsrmAddr.Text = "192.168.2.31";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 121);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "OSRM Server";
            // 
            // txtPpCfgRedisAddr
            // 
            this.txtPpCfgRedisAddr.Location = new System.Drawing.Point(95, 92);
            this.txtPpCfgRedisAddr.Name = "txtPpCfgRedisAddr";
            this.txtPpCfgRedisAddr.Size = new System.Drawing.Size(353, 20);
            this.txtPpCfgRedisAddr.TabIndex = 5;
            this.txtPpCfgRedisAddr.Text = "192.168.2.31";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Redis Server";
            // 
            // btnPpCfgGenId
            // 
            this.btnPpCfgGenId.Location = new System.Drawing.Point(454, 56);
            this.btnPpCfgGenId.Name = "btnPpCfgGenId";
            this.btnPpCfgGenId.Size = new System.Drawing.Size(69, 20);
            this.btnPpCfgGenId.TabIndex = 3;
            this.btnPpCfgGenId.Text = "Generate";
            this.btnPpCfgGenId.UseVisualStyleBackColor = true;
            this.btnPpCfgGenId.Click += new System.EventHandler(this.btnPpCfgGenId_Click);
            // 
            // txtPpCfgId
            // 
            this.txtPpCfgId.Location = new System.Drawing.Point(83, 56);
            this.txtPpCfgId.Name = "txtPpCfgId";
            this.txtPpCfgId.Size = new System.Drawing.Size(365, 20);
            this.txtPpCfgId.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Problem ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(319, 26);
            this.label2.TabIndex = 0;
            this.label2.Text = "Configure Preprocessing Job";
            // 
            // erp
            // 
            this.erp.ContainerControl = this;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 461);
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(750, 500);
            this.Name = "MainForm";
            this.Text = "TaskOptimizer Developer Tools";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.wsPprocConnect.ResumeLayout(false);
            this.wsPprocConnect.PerformLayout();
            this.wsPprocReview.ResumeLayout(false);
            this.wsPprocReview.PerformLayout();
            this.wsPprocConfig.ResumeLayout(false);
            this.wsPprocConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPpCfgPprocC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPpCfgTaskC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPpCfgWorkerC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.erp)).EndInit();
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
        private System.Windows.Forms.LinkLabel llnOsrmClear;
        private System.Windows.Forms.TabPage tabPage3;
        private Nightingale.WizardState wsPprocConfig;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnPpCfgOsrmPing;
        private System.Windows.Forms.Button btnPpCfgRedisPing;
        private System.Windows.Forms.TextBox txtPpCfgOsrmAddr;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPpCfgRedisAddr;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnPpCfgGenId;
        private System.Windows.Forms.TextBox txtPpCfgId;
        private System.Windows.Forms.NumericUpDown nudPpCfgTaskC;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown nudPpCfgWorkerC;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtPpCfgRnd;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnPpCgfNext;
        private System.Windows.Forms.Label lblPpCfgOPingResult;
        private System.Windows.Forms.Label lblPpCfgRPingResult;
        private System.Windows.Forms.ErrorProvider erp;
        private System.Windows.Forms.NumericUpDown nudPpCfgPprocC;
        private System.Windows.Forms.Label label9;
        private Nightingale.WizardState wsPprocReview;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TreeView treeView1;
        private Nightingale.WizardState wsPprocConnect;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.RichTextBox txtPpConsole;
        private System.Windows.Forms.Button button4;
    }
}

