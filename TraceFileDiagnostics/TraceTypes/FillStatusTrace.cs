using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.Collections;
using System.Windows.Forms;

namespace TraceFileReader.TraceTypes
{
    class FillStatusTrace : Trace
    {
        public FillStatusTrace()
            : base()
        {
        }

        public FillStatusTrace(XPathNavigator trace)
            : base(trace)
        {
            List<string> statusValues = new List<string>();

            this.TraceType = "FillStatus";

            statusValues = TagAndData.Values.ToList<string>();
            
            TagAndData.Clear();
            if (statusValues[1] == "OrderId")
                TagAndData.Add(statusValues[1], statusValues[2]);
            else
            {
                TagAndData.Add("OrderId", "0");
                TagAndData.Add(statusValues[1], statusValues[2]);
            }

        }

        public void AddStatusSection(XPathNavigator trace)
        {
            string name = trace.Value;
            
            trace.MoveToNext();
            TagAndData.Add(name, trace.Value);
        }

        public TraceTrackItem CreateTrackItem(TraceTrackItem item)
        {
            if(TagAndData.ContainsKey("StatusCode"))
                item.Name = "OrderId: " + this.TagAndData["OrderId"].ToString() + "  Status: " + this.TagAndData["StatusCode"].ToString();
            return item;
        }


        public override void AddListViewItem(ListView lv)
        {
            DateTime nT;
            ListViewItem item = new ListViewItem();
            ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem();
            ListView.ColumnHeaderCollection lvCol = lv.Columns;
            int statusColumn = 0;

            while (item.SubItems.Count < lvCol.Count)
                item.SubItems.Add("");

            if (!lv.Columns.ContainsKey("OrderId"))
            {
                lv.Columns.Add("OrderID", "OrderId");
                item.SubItems.Add("");
            }

            if (!lv.Columns.ContainsKey("StatusCode"))
            {
                lv.Columns.Add("StatusCode", "StatusCode");
                item.SubItems.Add("");
                statusColumn = lv.Columns.IndexOfKey("StatusCode");
            }
            else
            {
                statusColumn = lv.Columns.IndexOfKey("StatusCode");
                item.SubItems.Add("");
            }

            if (!lv.Columns.ContainsKey("CellSerialNumber"))
            {
                lv.Columns.Add("CellSerialNumber", "CellSerialNumber");
                item.SubItems.Add("");
            }

            if (!lv.Columns.ContainsKey("CellLocation"))
            {
                lv.Columns.Add("CellLocation", "CellLocation");
                item.SubItems.Add("");
            }

            if (lvCol[0].Text == "TimeStamp")
                item.SubItems[0].Text = traceTimeStamp.ToString();
            else
            {
                nT = TraceFileData.TimeSyncDate.AddMilliseconds(traceTimeStamp - TraceFileData.TimeSyncMilliSec);
                subItem.Text = nT.ToShortDateString();
                item.SubItems[0].Text = nT.ToShortDateString();
                item.SubItems[1].Text = nT.ToString("hh:mm:ss.fff");
            }
            

            foreach (KeyValuePair<string, string> d in TagAndData)
            {
                if (!lv.Columns.ContainsKey(d.Key))
                {
                    lv.Columns.Add(d.Key, d.Key);
                    item.SubItems.Add("");
                }

                item.SubItems[lv.Columns[d.Key].Index].Text = d.Value;
            }

            lv.Items.Add(item);
        }
    }
}
