using System;
using System.Windows.Forms;

namespace TimeBeam 
{
  partial class Timeline 
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose( bool disposing )
    {
      if( disposing && ( components != null ) ) 
      {
        components.Dispose();
      }
      base.Dispose( disposing );
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() 
    {
            this.components = new System.ComponentModel.Container();
            this.ScrollbarV = new TimeBeam.Scrollbar.VerticalScrollbar();
            this.ScrollbarH = new TimeBeam.Scrollbar.HorizontalScrollbar();
            this.contextMenuStripTimeline = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.fitAllTracksMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripTrackLabels = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.hideTrackMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripTimeline.SuspendLayout();
            this.contextMenuStripTrackLabels.SuspendLayout();
            this.SuspendLayout();
            // 
            // ScrollbarV
            // 
            this.ScrollbarV.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ScrollbarV.BackgroundColor = System.Drawing.Color.Black;
            this.ScrollbarV.ForegroundColor = System.Drawing.Color.Gray;
            this.ScrollbarV.Location = new System.Drawing.Point(791, 0);
            this.ScrollbarV.Max = 100;
            this.ScrollbarV.Min = 0;
            this.ScrollbarV.Name = "ScrollbarV";
            this.ScrollbarV.Size = new System.Drawing.Size(10, 180);
            this.ScrollbarV.TabIndex = 1;
            this.ScrollbarV.Value = 0;
            this.ScrollbarV.Scroll += new System.EventHandler<System.Windows.Forms.ScrollEventArgs>(this.ScrollbarVScroll);
            // 
            // ScrollbarH
            // 
            this.ScrollbarH.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ScrollbarH.BackgroundColor = System.Drawing.Color.Black;
            this.ScrollbarH.ForegroundColor = System.Drawing.Color.Gray;
            this.ScrollbarH.Location = new System.Drawing.Point(0, 190);
            this.ScrollbarH.Max = 100;
            this.ScrollbarH.Min = 0;
            this.ScrollbarH.Name = "ScrollbarH";
            this.ScrollbarH.Size = new System.Drawing.Size(780, 10);
            this.ScrollbarH.TabIndex = 0;
            this.ScrollbarH.Value = 0;
            this.ScrollbarH.Scroll += new System.EventHandler<System.Windows.Forms.ScrollEventArgs>(this.ScrollbarHScroll);
            // 
            // contextMenuStripTimeline
            // 
            this.contextMenuStripTimeline.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fitAllTracksMenuItem});
            this.contextMenuStripTimeline.Name = "contextMenuStripTimeline";
            this.contextMenuStripTimeline.Size = new System.Drawing.Size(105, 26);
            // 
            // fitAllTracksMenuItem
            // 
            this.fitAllTracksMenuItem.Name = "fitAllTracksMenuItem";
            this.fitAllTracksMenuItem.Size = new System.Drawing.Size(104, 22);
            this.fitAllTracksMenuItem.Text = "Fit All";
            // 
            // contextMenuStripTrackLabels
            // 
            this.contextMenuStripTrackLabels.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideTrackMenuItem,
            this.showAllMenuItem});
            this.contextMenuStripTrackLabels.Name = "contextMenuStripTrackLabels";
            this.contextMenuStripTrackLabels.Size = new System.Drawing.Size(131, 48);
            // 
            // hideTrackMenuItem
            // 
            this.hideTrackMenuItem.Name = "hideTrackMenuItem";
            this.hideTrackMenuItem.Size = new System.Drawing.Size(130, 22);
            this.hideTrackMenuItem.Text = "Hide Label";
            // 
            // showAllMenuItem
            // 
            this.showAllMenuItem.Name = "showAllMenuItem";
            this.showAllMenuItem.Size = new System.Drawing.Size(130, 22);
            this.showAllMenuItem.Text = "Show All";
            // 
            // Timeline
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(this.ScrollbarV);
            this.Controls.Add(this.ScrollbarH);
            this.Size = new System.Drawing.Size(800, 200);
            this.contextMenuStripTimeline.ResumeLayout(false);
            this.contextMenuStripTrackLabels.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion

    private Scrollbar.HorizontalScrollbar ScrollbarH;
    private Scrollbar.VerticalScrollbar ScrollbarV;
        private ContextMenuStrip contextMenuStripTimeline;
        private ContextMenuStrip contextMenuStripTrackLabels;
        private ToolStripMenuItem fitAllTracksMenuItem;
        private ToolStripMenuItem hideTrackMenuItem;
        private ToolStripMenuItem showAllMenuItem;
    }
}
