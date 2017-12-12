namespace TraceFileReader
{
    partial class TraceFileReaderUI
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
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lvTraceDetails = new System.Windows.Forms.ListView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblDetectedModel = new System.Windows.Forms.Label();
            this.tbDetectedModel = new System.Windows.Forms.TextBox();
            this.gbOpenTraceFile = new System.Windows.Forms.GroupBox();
            this.btnBrowseFiles = new System.Windows.Forms.Button();
            this.tbSelectedTraceFile = new System.Windows.Forms.TextBox();
            this.lblFilterBy = new System.Windows.Forms.Label();
            this.cbTraceFilter = new System.Windows.Forms.ComboBox();
            this.btnPlotValues = new System.Windows.Forms.Button();
            this.btnPlot3D = new System.Windows.Forms.Button();
            this.lbTraceTypes = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblSelectedTrack = new System.Windows.Forms.Label();
            this.tbTimeLine = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbStateData2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbStartTime = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbEndTime = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lvConcurrentItems = new System.Windows.Forms.ListView();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.nudConcurrencyWindow = new System.Windows.Forms.NumericUpDown();
            this.timeline1 = new TimeBeam.Timeline();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.conTextMenulvTraceDetailsColumnClick = new System.Windows.Forms.ContextMenuStrip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.gbOpenTraceFile.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudConcurrencyWindow)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.MinimumSize = new System.Drawing.Size(800, 400);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel2.Controls.Add(this.groupBox3);
            this.splitContainer1.Size = new System.Drawing.Size(876, 612);
            this.splitContainer1.SplitterDistance = 200;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 8;
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSize = true;
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Controls.Add(this.lbTraceTypes);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.MinimumSize = new System.Drawing.Size(400, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(876, 200);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Trace Data View";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lvTraceDetails);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(745, 181);
            this.panel1.TabIndex = 7;
            // 
            // lvTraceDetails
            // 
            this.lvTraceDetails.AllowColumnReorder = true;
            this.lvTraceDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvTraceDetails.FullRowSelect = true;
            this.lvTraceDetails.GridLines = true;
            this.lvTraceDetails.Location = new System.Drawing.Point(0, 51);
            this.lvTraceDetails.Name = "lvTraceDetails";
            this.lvTraceDetails.Size = new System.Drawing.Size(745, 130);
            this.lvTraceDetails.TabIndex = 1;
            this.lvTraceDetails.UseCompatibleStateImageBehavior = false;
            this.lvTraceDetails.View = System.Windows.Forms.View.Details;
            this.lvTraceDetails.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvTraceDetails_ColumnClick);
            this.lvTraceDetails.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvTraceDetails_KeyUp);
            this.lvTraceDetails.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lvTraceDetails_MouseDown);
            // 
            // panel3
            // 
            this.panel3.AutoSize = true;
            this.panel3.Controls.Add(this.lblDetectedModel);
            this.panel3.Controls.Add(this.tbDetectedModel);
            this.panel3.Controls.Add(this.gbOpenTraceFile);
            this.panel3.Controls.Add(this.lblFilterBy);
            this.panel3.Controls.Add(this.cbTraceFilter);
            this.panel3.Controls.Add(this.btnPlotValues);
            this.panel3.Controls.Add(this.btnPlot3D);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(745, 51);
            this.panel3.TabIndex = 8;
            // 
            // lblDetectedModel
            // 
            this.lblDetectedModel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblDetectedModel.AutoSize = true;
            this.lblDetectedModel.Location = new System.Drawing.Point(521, 6);
            this.lblDetectedModel.Name = "lblDetectedModel";
            this.lblDetectedModel.Size = new System.Drawing.Size(86, 13);
            this.lblDetectedModel.TabIndex = 8;
            this.lblDetectedModel.Text = "Detected Model:";
            // 
            // tbDetectedModel
            // 
            this.tbDetectedModel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDetectedModel.Location = new System.Drawing.Point(613, 3);
            this.tbDetectedModel.Name = "tbDetectedModel";
            this.tbDetectedModel.Size = new System.Drawing.Size(126, 20);
            this.tbDetectedModel.TabIndex = 7;
            this.tbDetectedModel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // gbOpenTraceFile
            // 
            this.gbOpenTraceFile.Controls.Add(this.btnBrowseFiles);
            this.gbOpenTraceFile.Controls.Add(this.tbSelectedTraceFile);
            this.gbOpenTraceFile.Location = new System.Drawing.Point(12, 3);
            this.gbOpenTraceFile.MaximumSize = new System.Drawing.Size(0, 45);
            this.gbOpenTraceFile.MinimumSize = new System.Drawing.Size(272, 45);
            this.gbOpenTraceFile.Name = "gbOpenTraceFile";
            this.gbOpenTraceFile.Size = new System.Drawing.Size(272, 45);
            this.gbOpenTraceFile.TabIndex = 0;
            this.gbOpenTraceFile.TabStop = false;
            this.gbOpenTraceFile.Text = "Open Trace File";
            // 
            // btnBrowseFiles
            // 
            this.btnBrowseFiles.Location = new System.Drawing.Point(191, 16);
            this.btnBrowseFiles.Name = "btnBrowseFiles";
            this.btnBrowseFiles.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseFiles.TabIndex = 1;
            this.btnBrowseFiles.Text = "Browse";
            this.btnBrowseFiles.UseVisualStyleBackColor = true;
            this.btnBrowseFiles.Click += new System.EventHandler(this.btnBrowseFiles_Click);
            // 
            // tbSelectedTraceFile
            // 
            this.tbSelectedTraceFile.Location = new System.Drawing.Point(9, 16);
            this.tbSelectedTraceFile.Name = "tbSelectedTraceFile";
            this.tbSelectedTraceFile.Size = new System.Drawing.Size(176, 20);
            this.tbSelectedTraceFile.TabIndex = 0;
            // 
            // lblFilterBy
            // 
            this.lblFilterBy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFilterBy.AutoSize = true;
            this.lblFilterBy.Location = new System.Drawing.Point(456, 30);
            this.lblFilterBy.Name = "lblFilterBy";
            this.lblFilterBy.Size = new System.Drawing.Size(47, 13);
            this.lblFilterBy.TabIndex = 5;
            this.lblFilterBy.Text = "Filter By:";
            // 
            // cbTraceFilter
            // 
            this.cbTraceFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTraceFilter.FormattingEnabled = true;
            this.cbTraceFilter.Location = new System.Drawing.Point(509, 27);
            this.cbTraceFilter.Name = "cbTraceFilter";
            this.cbTraceFilter.Size = new System.Drawing.Size(230, 21);
            this.cbTraceFilter.TabIndex = 4;
            this.cbTraceFilter.SelectedIndexChanged += new System.EventHandler(this.cbTraceFilter_SelectedIndexChanged);
            // 
            // btnPlotValues
            // 
            this.btnPlotValues.Location = new System.Drawing.Point(369, 16);
            this.btnPlotValues.Name = "btnPlotValues";
            this.btnPlotValues.Size = new System.Drawing.Size(75, 23);
            this.btnPlotValues.TabIndex = 6;
            this.btnPlotValues.Text = "Plot Values";
            this.btnPlotValues.UseVisualStyleBackColor = true;
            this.btnPlotValues.Click += new System.EventHandler(this.btnPlotValues_Click);
            // 
            // btnPlot3D
            // 
            this.btnPlot3D.Enabled = false;
            this.btnPlot3D.Location = new System.Drawing.Point(290, 17);
            this.btnPlot3D.Name = "btnPlot3D";
            this.btnPlot3D.Size = new System.Drawing.Size(75, 23);
            this.btnPlot3D.TabIndex = 5;
            this.btnPlot3D.Text = "Plot3D";
            this.btnPlot3D.UseVisualStyleBackColor = true;
            this.btnPlot3D.Click += new System.EventHandler(this.btnPlot3D_Click);
            // 
            // lbTraceTypes
            // 
            this.lbTraceTypes.Dock = System.Windows.Forms.DockStyle.Right;
            this.lbTraceTypes.FormattingEnabled = true;
            this.lbTraceTypes.Location = new System.Drawing.Point(748, 16);
            this.lbTraceTypes.Name = "lbTraceTypes";
            this.lbTraceTypes.Size = new System.Drawing.Size(125, 181);
            this.lbTraceTypes.TabIndex = 2;
            this.lbTraceTypes.SelectedIndexChanged += new System.EventHandler(this.lbTraceTypes_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.AutoSize = true;
            this.groupBox3.Controls.Add(this.splitContainer2);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.MinimumSize = new System.Drawing.Size(800, 400);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(876, 406);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Time Line View";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 16);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            this.splitContainer2.Panel1MinSize = 100;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.timeline1);
            this.splitContainer2.Size = new System.Drawing.Size(870, 387);
            this.splitContainer2.SplitterDistance = 100;
            this.splitContainer2.TabIndex = 2;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.flowLayoutPanel1);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.groupBox4);
            this.splitContainer3.Panel2.Controls.Add(this.flowLayoutPanel2);
            this.splitContainer3.Size = new System.Drawing.Size(870, 100);
            this.splitContainer3.SplitterDistance = 167;
            this.splitContainer3.TabIndex = 6;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.lblSelectedTrack);
            this.flowLayoutPanel1.Controls.Add(this.tbTimeLine);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.tbStateData2);
            this.flowLayoutPanel1.Controls.Add(this.label5);
            this.flowLayoutPanel1.Controls.Add(this.tbStartTime);
            this.flowLayoutPanel1.Controls.Add(this.label6);
            this.flowLayoutPanel1.Controls.Add(this.tbEndTime);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(167, 100);
            this.flowLayoutPanel1.TabIndex = 16;
            // 
            // lblSelectedTrack
            // 
            this.lblSelectedTrack.AutoSize = true;
            this.lblSelectedTrack.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSelectedTrack.Location = new System.Drawing.Point(3, 0);
            this.lblSelectedTrack.Name = "lblSelectedTrack";
            this.lblSelectedTrack.Size = new System.Drawing.Size(153, 13);
            this.lblSelectedTrack.TabIndex = 13;
            this.lblSelectedTrack.Text = "State Machine:";
            // 
            // tbTimeLine
            // 
            this.tbTimeLine.Dock = System.Windows.Forms.DockStyle.Right;
            this.tbTimeLine.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTimeLine.Location = new System.Drawing.Point(3, 16);
            this.tbTimeLine.Name = "tbTimeLine";
            this.tbTimeLine.ReadOnly = true;
            this.tbTimeLine.Size = new System.Drawing.Size(153, 20);
            this.tbTimeLine.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "State Entered";
            // 
            // tbStateData2
            // 
            this.tbStateData2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbStateData2.Location = new System.Drawing.Point(3, 55);
            this.tbStateData2.Name = "tbStateData2";
            this.tbStateData2.Size = new System.Drawing.Size(153, 20);
            this.tbStateData2.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 78);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Start Time:";
            // 
            // tbStartTime
            // 
            this.tbStartTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbStartTime.Location = new System.Drawing.Point(162, 3);
            this.tbStartTime.Name = "tbStartTime";
            this.tbStartTime.ReadOnly = true;
            this.tbStartTime.Size = new System.Drawing.Size(101, 20);
            this.tbStartTime.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(269, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "End Time:";
            // 
            // tbEndTime
            // 
            this.tbEndTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbEndTime.Location = new System.Drawing.Point(269, 16);
            this.tbEndTime.Name = "tbEndTime";
            this.tbEndTime.ReadOnly = true;
            this.tbEndTime.Size = new System.Drawing.Size(101, 20);
            this.tbEndTime.TabIndex = 14;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lvConcurrentItems);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(0, 26);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(699, 74);
            this.groupBox4.TabIndex = 16;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Concurrent Events";
            // 
            // lvConcurrentItems
            // 
            this.lvConcurrentItems.AllowColumnReorder = true;
            this.lvConcurrentItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvConcurrentItems.FullRowSelect = true;
            this.lvConcurrentItems.GridLines = true;
            this.lvConcurrentItems.Location = new System.Drawing.Point(3, 16);
            this.lvConcurrentItems.MinimumSize = new System.Drawing.Size(100, 4);
            this.lvConcurrentItems.Name = "lvConcurrentItems";
            this.lvConcurrentItems.Size = new System.Drawing.Size(693, 55);
            this.lvConcurrentItems.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvConcurrentItems.TabIndex = 9;
            this.lvConcurrentItems.UseCompatibleStateImageBehavior = false;
            this.lvConcurrentItems.View = System.Windows.Forms.View.Details;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.Controls.Add(this.label3);
            this.flowLayoutPanel2.Controls.Add(this.nudConcurrencyWindow);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(699, 26);
            this.flowLayoutPanel2.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(133, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Concurrency Window (mS)";
            // 
            // nudConcurrencyWindow
            // 
            this.nudConcurrencyWindow.Location = new System.Drawing.Point(142, 3);
            this.nudConcurrencyWindow.Name = "nudConcurrencyWindow";
            this.nudConcurrencyWindow.Size = new System.Drawing.Size(133, 20);
            this.nudConcurrencyWindow.TabIndex = 1;
            this.nudConcurrencyWindow.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // timeline1
            // 
            this.timeline1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.timeline1.BackgroundColor = System.Drawing.Color.Black;
            this.timeline1.Clock = null;
            this.timeline1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeline1.GridAlpha = 40;
            this.timeline1.Location = new System.Drawing.Point(0, 0);
            this.timeline1.MinimumSize = new System.Drawing.Size(800, 400);
            this.timeline1.Name = "timeline1";
            this.timeline1.Size = new System.Drawing.Size(870, 400);
            this.timeline1.TabIndex = 1;
            this.timeline1.Text = "timeline1";
            this.timeline1.TrackBorderSize = 2;
            this.timeline1.TrackHeight = 20;
            this.timeline1.TrackLabelWidth = 100;
            this.timeline1.TrackSpacing = 1;
            // 
            // conTextMenulvTraceDetailsColumnClick
            // 
            this.conTextMenulvTraceDetailsColumnClick.Name = "conTextMenulvTraceDetailsColumnClick";
            this.conTextMenulvTraceDetailsColumnClick.Size = new System.Drawing.Size(61, 4);
            this.conTextMenulvTraceDetailsColumnClick.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.conTextMenulvTraceDetailsColumnClick_Closed);
            this.conTextMenulvTraceDetailsColumnClick.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.conTextMenulvTraceDetailsColumnClick_Closing);
            // 
            // TraceFileReaderUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 612);
            this.Controls.Add(this.splitContainer1);
            this.Name = "TraceFileReaderUI";
            this.Text = "Trace File Reader";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.gbOpenTraceFile.ResumeLayout(false);
            this.gbOpenTraceFile.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudConcurrencyWindow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnPlot3D;
        private System.Windows.Forms.GroupBox gbOpenTraceFile;
        private System.Windows.Forms.Button btnBrowseFiles;
        private System.Windows.Forms.TextBox tbSelectedTraceFile;
        private System.Windows.Forms.Label lblFilterBy;
        private System.Windows.Forms.ComboBox cbTraceFilter;
        private System.Windows.Forms.ListBox lbTraceTypes;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private TimeBeam.Timeline timeline1;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudConcurrencyWindow;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnPlotValues;
        private System.Windows.Forms.ContextMenuStrip conTextMenulvTraceDetailsColumnClick;
        private System.Windows.Forms.TextBox tbStartTime;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListView lvConcurrentItems;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lblSelectedTrack;
        private System.Windows.Forms.TextBox tbTimeLine;
        private System.Windows.Forms.TextBox tbEndTime;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbStateData2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListView lvTraceDetails;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lblDetectedModel;
        private System.Windows.Forms.TextBox tbDetectedModel;

    }
}

