using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.Collections;
using NorthStateFramework;
using System.Windows.Forms;
using OxyPlot;

namespace TraceFileReader.TraceTypes
{
    public class Trace : IDataPointProvider
    {
        private const int timeScalar = 100;
        private object derivedType;
        private string traceType;        
        private int traceTimeStampOffset;
        private int concurrentValue;
        private DateTime time;
        private Dictionary<string, string> tagAndData = new Dictionary<string, string>();
        
        protected int traceTimeStamp;

        public int TimeScalar { get { return timeScalar; } }
        public object DerivedType { get { return derivedType; } set { derivedType = value; } }
        public string TraceType { get { return traceType; } set { traceType = value; } }
        public int TraceTime { get { return traceTimeStamp; } set { traceTimeStamp = value; } }
        public Dictionary<string, string> TagAndData { get { return tagAndData; } set { tagAndData = value; } }
        public DateTime Time { get { return time; } set { time = value; } }
        public int OffSetTime { get { return traceTimeStampOffset; } set { traceTimeStampOffset = value; } }
        public int ConcurrentValue { get { return concurrentValue; } set { concurrentValue = value; } }

        public Trace()
        {
            derivedType = this.GetType();
            traceType = string.Empty;
            traceTimeStamp = 0;
        }

        public Trace(XPathNavigator trace)
        {
            trace.MoveToFirstChild();
            setTraceTime(trace);

            if (trace.MoveToNext())
                TraceType = trace.Name;

            trace.MoveToFirstChild();
            do
            {
                tagAndData.Add(trace.Name, trace.Value);
            } while (trace.MoveToNext());

            switch (traceType)
            {
                case NSFTraceTags.eventQueuedTag:
                    break;
                case NSFTraceTags.stateEnteredTag:
                    break;
                default:
                    break;
            }
        }

        public void setTraceTime(XPathNavigator xmlTime)
        {
            int t = xmlTime.ValueAsInt;
            traceTimeStamp = t;
        }

        public void setTraceTimeOffset(int offSet)
        {
            traceTimeStampOffset = traceTimeStamp - offSet;
        }

        public void setDateTimeFromTimeStamp(DateTime dT, int milliSecSync)
        {
            time = dT.AddMilliseconds(traceTimeStamp - milliSecSync);
        }

        public string getTagData(int index)
        {
            List<string> data = new List<string>();
            foreach (KeyValuePair<string, string> d in tagAndData)
                data.Add(d.Value);
            return data[index];
        }

        public virtual void AddListViewItem(ListView lv)
        {
            DateTime nT;
            ListViewItem item = new ListViewItem();
            ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem();
            ListView.ColumnHeaderCollection lvCol = lv.Columns;

            if(tagAndData.ContainsKey(NSFTraceTags.objectTag))
            {
                if (!lv.Columns.ContainsKey(NSFTraceTags.objectTag))
                {
                    lv.Columns.Add(NSFTraceTags.objectTag, NSFTraceTags.objectTag);
                    item.SubItems.Add("");
                }
            }

            while (item.SubItems.Count < lvCol.Count)
                item.SubItems.Add("");

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


        public virtual DataPoint GetDataPoint()
        {
            return new DataPoint();
        }

    }
}
