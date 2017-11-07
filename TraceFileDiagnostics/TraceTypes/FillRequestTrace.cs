using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.Collections;
using NorthStateFramework;
using System.Windows.Forms;

namespace ServiceTool.TraceTypes
{
    class FillRequestTrace : Trace
    {
        private bool fillRequestComplete = false;
        private string requestType;

        public bool FillRequestComplete { get { return fillRequestComplete; } set { fillRequestComplete = value; } }
        public string RequestType { get { return requestType; } set { requestType = value; } }

        public FillRequestTrace(Form1 parentForm)
            : base(parentForm)
        {
            fillRequestComplete = false;
        }

        public FillRequestTrace(XPathNavigator trace, Form1 parentForm)
            : base(trace, parentForm)
        {
            List<string> statusValues = new List<string>();

            this.TraceType = "Request";
            requestType = TagAndData[NSFTraceTags.objectTag];

            statusValues = TagAndData.Values.ToList<string>();
            
            TagAndData.Clear();

            TagAndData.Add(statusValues[1], statusValues[2]);
            fillRequestComplete = false;
        }

        public bool AddStatusSection(XPathNavigator trace)
        {
            string name =trace.Value;

            if (trace.Value == "SpillableDropOffLocation")
                fillRequestComplete = true;
            trace.MoveToNext();
            if (TagAndData.ContainsKey(name))
                TagAndData[name] += ", " + trace.Value;
            else
                TagAndData.Add(name, trace.Value);

            return fillRequestComplete;
        }

        public TraceTrackItem CreateTrackItem(TraceTrackItem item)
        {
            item.Name = "OrderId: " + this.TagAndData["OrderId"].ToString() + "  Cell: " + this.TagAndData["CellSerialNumber"].ToString();
            return item;
        }


        public override void AddListViewItem(ListView lv)
        {
            DateTime nT;
            ListViewItem item = new ListViewItem();
            ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem();
            ListView.ColumnHeaderCollection lvCol = lv.Columns;

            while (item.SubItems.Count < lvCol.Count)
                item.SubItems.Add("");

            if (!lv.Columns.ContainsKey("Request Type"))
            {
                lv.Columns.Add("Request Type", "Request Type");
                item.SubItems.Add("");
            }

            if (lvCol[0].Text == "TimeStamp")
                item.SubItems[0].Text = traceTimeStamp.ToString();
            else
            {
                nT = parent.TimeSyncDate.AddMilliseconds(traceTimeStamp - parent.TimeSyncMilliSec);
                subItem.Text = nT.ToShortDateString();
                item.SubItems[0].Text = nT.ToShortDateString();
                item.SubItems[1].Text = nT.ToString("hh:mm:ss.fff");
            }

            item.SubItems[2].Text = requestType;

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
