using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.Collections;

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
    }
}
