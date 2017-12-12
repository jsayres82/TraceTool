using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using TimeBeam;
using TimeBeam.Events;
using TraceFileReader.TraceTypes;
using TraceFileReader.PlotTypes;

namespace TraceFileReader
{
    //TODO:  Add ability to load and read machine configuratino files 
    public partial class TraceFileReaderUI : Form
    {

        private string traceFileName = string.Empty;

        private TraceFileData traceData;
        
        private Dictionary<string, List<DataPlotBase>> plotList = new Dictionary<string, List<DataPlotBase>>();

        
        private TimeBeamClock _clock = new TimeBeamClock();
        //private SciLabUtility sciLab;

        private Dictionary<string, bool> lvTraceDetailsColumnItemState = new Dictionary<string, bool>();
        private Dictionary<string, List<ListViewItem>> lvTraceDetailsRemovedRows = new Dictionary<string, List<ListViewItem>>();
        private List<string> lvTraceDetailsItemsToAdd = new List<string>();
        private List<string> lvTraceDetailsItemsToRemove = new List<string>();
        private ListViewColumnSorter lvTraceDetailsColumnSorter;
        
        public Form MyChild;
        PopupForm plotForm;

        public TraceFileData TraceData;
        public ListView LVTraceDetails { get { return lvTraceDetails; } set { lvTraceDetails = value; } }
        public ListView LvConncurrentItems { get { return lvConcurrentItems; } set { lvConcurrentItems = value; } }

        public TraceFileReaderUI(string traceFile)
        {
            InitializeComponent();
           
            lvTraceDetailsColumnSorter = new ListViewColumnSorter();
            lvTraceDetails.ListViewItemSorter = lvTraceDetailsColumnSorter;
            //sciLab = new SciLabUtility();
            // Register the clock with the timeline
            timeline1.Clock = _clock;
            // Activate the timer that invokes the clock to update.
            timer1.Enabled = true;
            
            if (string.IsNullOrEmpty(Path.GetFileName(traceFile)))
                return;

            traceFileName = traceFile;
            tbSelectedTraceFile.Text = traceFileName;
            LoadTraceFile(traceFile);

            plotForm = new PopupForm(this);
            plotForm.MyParent = this;
            this.MyChild = plotForm;
        }

        public TraceFileReaderUI()
        {
            InitializeComponent();
            
            //this.KeyPreview = true;
            lvTraceDetailsColumnSorter = new ListViewColumnSorter();
            lvTraceDetails.ListViewItemSorter = lvTraceDetailsColumnSorter;
            //sciLab = new SciLabUtility();
            // Register the clock with the timeline
            timeline1.Clock = _clock;
            // Activate the timer that invokes the clock to update.
            timer1.Enabled = true;

        }

        private void btnBrowseFiles_Click(object sender, EventArgs e)
        {
            // If we already opened a trace file
            if (traceFileName != string.Empty)
            {
                lvTraceDetails.Columns.Clear();
                lvTraceDetails.Items.Clear();
                cbTraceFilter.SelectedIndex = -1;
                cbTraceFilter.Items.Clear();
                lbTraceTypes.Items.Clear();
                plotList.Clear();
            }

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                traceFileName = openFileDialog.FileName.ToString();
                tbSelectedTraceFile.Text = traceFileName;

                LoadTraceFile(traceFileName);
            }

        }

        public void LoadTraceFile(string filename)
        {
            traceData = new TraceFileData(filename);
            updateControls();
            traceData.LoadTimeline(timeline1);
            timeline1.SelectionChanged += TimelineSelectionChanged;
        }

        private void updateControls()
        {
            foreach (KeyValuePair<string, List<Trace>> traceType in traceData.TraceList)
            {
                lbTraceTypes.Items.Add(traceType.Key);
            }
            tbDetectedModel.Text = TraceFileData.MachineModel.ToString();
        }
        private void cbTraceFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            lvTraceDetails.Columns.Clear();
            lvTraceDetails.Items.Clear();
            
            lvTraceDetails.Columns.Add("Date", "Date");
            lvTraceDetails.Columns.Add("Time", "Time");
            // Set the column number that is to be sorted; default to ascending.
            lvTraceDetailsColumnSorter.SortColumn = lvTraceDetails.Columns.IndexOfKey("Time");
            lvTraceDetailsColumnSorter.Order = SortOrder.Ascending;

            // Perform the sort with these new sort options.
            lvTraceDetails.Sort();

            using (new WaitCursor())
            {
                foreach (Trace t in traceData.TraceList[lbTraceTypes.SelectedItem.ToString()])
                {
                    List<int> tempTime = new List<int>();
                    List<string> tempList = new List<string>();
                    ListViewItem item = new ListViewItem();

                    foreach (KeyValuePair<string, string> d in t.TagAndData)
                    {
                        if (d.Value.Equals(cbTraceFilter.SelectedItem))
                        {
                            t.AddListViewItem(lvTraceDetails);
                            break;
                        }
                    }
                }

                UpdateListViewItemSubItems(lvTraceDetails);
                AdjustListViewColumnWidth(lvTraceDetails);
            }
        }

        private void lbTraceTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> dropDownItems = new List<string>();
            cbTraceFilter.SelectedIndex = -1;
            cbTraceFilter.Items.Clear();
            lvTraceDetails.Columns.Clear();
            lvTraceDetails.Items.Clear();
            lvTraceDetailsColumnItemState.Clear();
            lvTraceDetailsRemovedRows.Clear();

            lvTraceDetails.Columns.Add("Date", "Date");
            lvTraceDetails.Columns.Add("Time", "Time");
            // Set the column number that is to be sorted; default to ascending.
            lvTraceDetailsColumnSorter.SortColumn = lvTraceDetails.Columns.IndexOfKey("Time");
            lvTraceDetailsColumnSorter.Order = SortOrder.Ascending;

            // Perform the sort with these new sort options.
            this.lvTraceDetails.Sort();
            using (new WaitCursor())
            {
                foreach (Trace t in traceData.TraceList[lbTraceTypes.SelectedItem.ToString()])
                {
                    t.AddListViewItem(lvTraceDetails);
                    if (!cbTraceFilter.Items.Contains(t.TagAndData.First().Value))
                        cbTraceFilter.Items.Add(t.TagAndData.First().Value);
                }

                UpdateListViewItemSubItems(lvTraceDetails);
                AdjustListViewColumnWidth(lvTraceDetails);
            }

        }

        private void btnPlotValues_Click(object sender, EventArgs e)
        {
            plotForm = new PopupForm(this);
            plotForm.MyParent = this;
            this.MyChild = plotForm;
            plotForm.Show();
        }

        private void btnPlot3D_Click(object sender, EventArgs e)
        {
           // sciLab.plot3DMotion();
        }

        private void TimelineSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if (null != selectionChangedEventArgs.Deselected)
            {
                foreach (ITimelineTrackBase track in selectionChangedEventArgs.Deselected)
                {
                    
                }
            }
            if (null != selectionChangedEventArgs.Selected)
            {
                if (selectionChangedEventArgs.Selected is IEnumerable<TimeBeam.IMultiPartTimelineTrack>)
                    return;
                lvConcurrentItems.Columns.Clear();
                lvConcurrentItems.Items.Clear();

                lvConcurrentItems.Columns.Add("TimeStamp", "TimeStamp");
                lvConcurrentItems.Columns.Add("Type", "Type");

                int windowSize = Convert.ToInt32(nudConcurrencyWindow.Value);

                foreach (ITimelineTrackBase track in selectionChangedEventArgs.Selected)
                {

                    TraceTrackItem trackItem = track as TraceTrackItem;
                    tbTimeLine.Text = "";// track.Name + "  ";
                    List<string> keys = trackItem.Trace.TagAndData.Keys.ToList<string>();
                    lblSelectedTrack.Text = keys[0];
                    List<string> values = trackItem.Trace.TagAndData.Values.ToList<string>();
                    tbTimeLine.Text = values[0];
                    tbStartTime.Text = trackItem.Start.ToString();
                    tbEndTime.Text = trackItem.End.ToString();
                    List<string> items = new List<string>();
                    foreach (KeyValuePair<string, string> tagData in trackItem.Trace.TagAndData)
                    {
                        items.Add(tagData.Value);
                        lblSelectedTrack.Text = tagData.Key + ": ";
                    }

                    tbTimeLine.Text = items[0];
                    if (items.Count > 1)
                        tbStateData2.Text = items[1];


                    foreach (TraceTrack tT in timeline1.Tracks)
                    {
                        foreach (TraceTrackItem tI in tT.TrackElements)
                        {
                            ListViewItem item = new ListViewItem();
                            if ((trackItem.Start) > (tI.Start - windowSize) &&
                                (trackItem.Start) < (tI.Start + windowSize))
                            {
                                if (!tI.Name.Equals(track.Name))
                                {
                                    Trace t = tI.Trace;

                                    ListViewItem.ListViewSubItem subItemDate = new ListViewItem.ListViewSubItem();
                                    ListViewItem.ListViewSubItem subItemTime = new ListViewItem.ListViewSubItem();
                                    ListViewItem.ListViewSubItem subItemType = new ListViewItem.ListViewSubItem();
                                    subItemTime.Text = tI.Start.ToString();
                                    item.SubItems.Insert(0, subItemTime);
                                    subItemType.Text = tI.Trace.TraceType;
                                    item.SubItems.Insert(1, subItemType);

                                    foreach (KeyValuePair<string, string> d in t.TagAndData)
                                    {
                                        if (!lvConcurrentItems.Columns.ContainsKey(d.Key))
                                            lvConcurrentItems.Columns.Add(d.Key, d.Key);
                                        int index = lvConcurrentItems.Columns.IndexOfKey(d.Key);
                                        ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem();
                                        subItem.Text = d.Value;
                                        while (item.SubItems.Count < index)
                                            item.SubItems.Add("");
                                        item.SubItems.Insert(index, subItem);
                                    }
                                    lvConcurrentItems.Items.Add(item);
                                }
                            }
                        }
                    }

                    lvConcurrentItems.Sort();

                    AdjustListViewColumnWidth(lvConcurrentItems);
                }
            }
        }

        public void UpdateListViewItemSubItems(ListView lv)
        {
            int index = 0;
            int columnCount = lv.Columns.Count;
            while (index < lv.Items.Count)
            {
                while (lv.Items[index].SubItems.Count < columnCount)
                    lv.Items[index].SubItems.Add("");
                index++;
            }
        }

        private void AdjustListViewColumnWidth(ListView lv)
        {
            foreach (ColumnHeader cH in lv.Columns)
            {
                if (cH.Text.Length * lv.Font.Size > cH.Width)
                    cH.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                else
                    cH.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                if (cH.Text.Length * lv.Font.Size > cH.Width)
                    cH.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }

        private void lvTraceDetails_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvTraceDetailsColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvTraceDetailsColumnSorter.Order == SortOrder.Ascending)
                {
                    lvTraceDetailsColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvTraceDetailsColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvTraceDetailsColumnSorter.SortColumn = e.Column;
                lvTraceDetailsColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.lvTraceDetails.Sort();
        }

        private void lvTraceDetails_MouseDown(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo currentHitTestInfo;
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                currentHitTestInfo = lvTraceDetails.HitTest(e.X, e.Y);
                if (currentHitTestInfo.SubItem != null)
                {
                    int col = currentHitTestInfo.Item.SubItems.IndexOf(currentHitTestInfo.SubItem);
                    string val = currentHitTestInfo.Item.SubItems[col].Text;

                    conTextMenulvTraceDetailsColumnClick.Tag = col;
                    ListView lv = lvTraceDetails;
                    int rowIndex = 0;

                    if (!lvTraceDetailsColumnItemState.ContainsKey(col + "-" + val))
                    {
                        Dictionary<string, bool> dic = new Dictionary<string, bool>();
                        lvTraceDetailsColumnItemState.Add(col + "-" + val, true);
                        while (rowIndex < lv.Items.Count)
                        {
                            if (!lvTraceDetailsColumnItemState.ContainsKey(col + "-" + lv.Items[rowIndex].SubItems[col].Text))
                                lvTraceDetailsColumnItemState.Add(col + "-" + lv.Items[rowIndex].SubItems[col].Text, true);
                            rowIndex++;
                        }
                    }
                    conTextMenulvTraceDetailsColumnClick.Items.Clear();
                    BuildMenuItems();
                    conTextMenulvTraceDetailsColumnClick.Show(Cursor.Position);
                }
            }
        }

        private void BuildMenuItems()
        {
            List<ToolStripMenuItem> itemList = new List<ToolStripMenuItem>();
            IEnumerable<string> fullMatchingKeys =
            lvTraceDetailsColumnItemState.Keys.Where(currentKey => currentKey.StartsWith(conTextMenulvTraceDetailsColumnClick.Tag.ToString() + "-"));

            ToolStripMenuItem tsmiFISelectAll = new ToolStripMenuItem();
            tsmiFISelectAll.Text = "Select All";
            tsmiFISelectAll.Name = tsmiFISelectAll.Text;
            tsmiFISelectAll.CheckState = CheckState.Checked;
            tsmiFISelectAll.Click += tsmiFISelectAll_Click;
            conTextMenulvTraceDetailsColumnClick.Items.Add(tsmiFISelectAll);
            
            foreach (string currentKey in fullMatchingKeys)
            {
                ToolStripMenuItem newItem = new ToolStripMenuItem();
                newItem.Checked = lvTraceDetailsColumnItemState[currentKey];
                if (lvTraceDetailsColumnItemState[currentKey])
                {
                    newItem.CheckState = CheckState.Checked;
                }
                else
                {
                    newItem.CheckState = CheckState.Unchecked;
                    if (tsmiFISelectAll.CheckState == CheckState.Checked)
                        tsmiFISelectAll.CheckState = CheckState.Unchecked;
                }

                newItem.Name = currentKey;
                newItem.Size = new System.Drawing.Size(152, 22);
                newItem.Text = newItem.Name.Remove(0, newItem.Name.Split('-')[0].Length + 1);
                newItem.Click += new EventHandler(contextMenuItem_Click);
                conTextMenulvTraceDetailsColumnClick.Items.Add(newItem);
                itemList.Add(newItem);

            }
        }

        private void contextMenuItem_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            int column = Convert.ToInt32(item.Name.Split('-')[0]);

            ((ToolStripMenuItem)sender).Checked ^= true;
            if (item.Checked)
            {
                AddListViewItemsToAdd(LVTraceDetails, item.Text, column);
                lvTraceDetailsColumnItemState[item.Name] = true;
            }
            else
            {
                AddListViewItemsToRemove(LVTraceDetails, item.Text, column);
                lvTraceDetailsColumnItemState[item.Name] = false;
            }
            
        }

        private void AddListViewItemsToRemove(ListView listView, string item, int column)
        {
            ListView lv = listView;
            string itemVal = item;
            int col = column;
            string key = col.ToString() + "-" + itemVal;

            if (lvTraceDetailsItemsToAdd.Contains(key))
                lvTraceDetailsItemsToAdd.Remove(key);

            lvTraceDetailsItemsToRemove.Add(key);
        }

        private void AddListViewItemsToAdd(ListView listView, string item, int column)
        {
            ListView lv = listView;
            string itemVal = item;
            int col = column;
            string key = col.ToString() + "-" + itemVal;

            if (lvTraceDetailsItemsToRemove.Contains(key))
                lvTraceDetailsItemsToRemove.Remove(key);

            lvTraceDetailsItemsToAdd.Add(key);
        }

        private void tsmiFISelectAll_Click(object sender, EventArgs e)
        {
            IEnumerable<string> fullMatchingKeys =
            lvTraceDetailsColumnItemState.Keys.Where(currentKey => currentKey.StartsWith(conTextMenulvTraceDetailsColumnClick.Tag.ToString() + "-"));

            
            ((ToolStripMenuItem)sender).Checked ^= true;
            foreach (ToolStripMenuItem i in conTextMenulvTraceDetailsColumnClick.Items)
            {
                if (i != ((ToolStripMenuItem)sender))
                {
                    if (i.Checked != ((ToolStripMenuItem)sender).Checked)
                    {
                        if (i.Checked)
                        {
                            AddListViewItemsToRemove(LVTraceDetails, i.Text, Convert.ToInt32(conTextMenulvTraceDetailsColumnClick.Tag));
                            lvTraceDetailsColumnItemState[i.Name] = false;                           
                        }
                        else
                        {
                            AddListViewItemsToAdd(LVTraceDetails, i.Text, Convert.ToInt32(conTextMenulvTraceDetailsColumnClick.Tag));
                            lvTraceDetailsColumnItemState[i.Name] = true;
                        }
                        i.Checked ^= true;//= CheckState.Checked;
                    }
                }
            }
            

        }

        private void conTextMenulvTraceDetailsColumnClick_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
            {
                e.Cancel = true;
            }
        }

        private void conTextMenulvTraceDetailsColumnClick_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            List<string> CurrentlyVisisbleItems = new List<string>();

            foreach (KeyValuePair<string, bool> kvp in lvTraceDetailsColumnItemState)
            {
                if (kvp.Value == true)
                    CurrentlyVisisbleItems.Add(kvp.Key);
            }

            foreach (string s in lvTraceDetailsItemsToAdd)
            {
                if(lvTraceDetailsRemovedRows.ContainsKey(s))
                {
                    foreach (ListViewItem i in lvTraceDetailsRemovedRows[s])
                    {
                        lvTraceDetails.Items.Add(i);
                    }
                    lvTraceDetailsRemovedRows.Remove(s);
                }
            }

            if (CurrentlyVisisbleItems.Count > 0)
            {
                foreach (string s in lvTraceDetailsItemsToRemove)
                {
                    ListView lv = lvTraceDetails;
                    string itemVal = s.Split('-')[1];
                    int col = Convert.ToInt32(conTextMenulvTraceDetailsColumnClick.Tag);
                    int rowIndex = 0;
                    List<ListViewItem> itemCopies = new List<ListViewItem>();

                    while (rowIndex < lv.Items.Count)
                    {
                        if (lv.Items[rowIndex].SubItems[col].Text == itemVal)
                        {
                            itemCopies.Add(lv.Items[rowIndex]);
                        }

                        rowIndex++;
                    }

                    int itemIndex = 0;
                    while (itemIndex < itemCopies.Count)
                    {
                        lv.Items.Remove(itemCopies[itemIndex]);
                        itemIndex++;
                    }
                    if (!lvTraceDetailsRemovedRows.ContainsKey(s))
                        lvTraceDetailsRemovedRows.Add(s, itemCopies);
                }

                if (lvTraceDetailsItemsToRemove.Count != 0)
                    ((ToolStripMenuItem)conTextMenulvTraceDetailsColumnClick.Items[0]).Checked = false;
            }
            else
            {
                foreach (string s in lvTraceDetailsItemsToRemove)
                {
                    foreach (ToolStripMenuItem i in conTextMenulvTraceDetailsColumnClick.Items)
                    {
                        if (i.Name == s)
                            i.Checked = true;
                    }
                    lvTraceDetailsColumnItemState[s] = true;
                }
            }


            lvTraceDetailsItemsToRemove.Clear();
            lvTraceDetailsItemsToAdd.Clear();

        }

        private void lvTraceDetails_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender != lvTraceDetails) return;

            if (e.Control && e.KeyCode == Keys.C)
                CopySelectedValuesToClipboard();
        }

        private void CopySelectedValuesToClipboard()
        {
            var builder = new StringBuilder();
            string line = string.Empty;

            foreach (ListViewItem item in lvTraceDetails.SelectedItems)
            {
                line = string.Empty;
                foreach (ListViewItem.ListViewSubItem s in item.SubItems)
                {
                    if(s.Text != "")
                        line += s.Text + ",";
                }
                builder.AppendLine(line);
            }
            Clipboard.SetText(builder.ToString());
        } 
    }
}
