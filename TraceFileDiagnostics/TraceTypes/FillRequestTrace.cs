using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.Collections;
using NorthStateFramework;
using System.Windows.Forms;

namespace TraceFileReader.TraceTypes
{
    class FillRequestTrace : Trace
    {
        private bool fillRequestComplete = false;
        private string requestType;

        public bool FillRequestComplete { get { return fillRequestComplete; } set { fillRequestComplete = value; } }
        public string RequestType { get { return requestType; } set { requestType = value; } }

        public FillRequestTrace()
            : base()
        {
            fillRequestComplete = false;
        }

        public FillRequestTrace(XPathNavigator trace)
            : base(trace)
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
            if (name.Equals("AllowedCellLocation"))
                name = "CellLocation";
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
            int requestTypeColumn = 0;

            while (item.SubItems.Count < lvCol.Count)
                item.SubItems.Add("");

            if (!lv.Columns.ContainsKey("Request Type"))
            {
                if(!lv.Columns.ContainsKey("StatusCode"))
                {
                    lv.Columns.Add("Request Type", "Request Type");
                    item.SubItems.Add("");
                    requestTypeColumn = lv.Columns.IndexOfKey("Request Type");
                }
                else
                {
                    requestTypeColumn = lv.Columns.IndexOfKey("StatusCode");
                    item.SubItems.Add("");
                }
            }
            else
            {
                requestTypeColumn = lv.Columns.IndexOfKey("Request Type");
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

            item.SubItems[requestTypeColumn].Text = requestType;

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
