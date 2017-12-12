using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.Collections;
using NorthStateFramework;
using OxyPlot;
using TraceFileReader.EnumTypes;
using System.Windows.Forms;

namespace TraceFileReader.TraceTypes
{
    //TODO: Fix the target position of relative moves so the can be plotted.
    //TODO: Break down motor events so that things like ServoOn an Target reached can be graphed
    public class MotorTrace : Trace
    {
        private const string actualPositionStringString = "ActualPosition";
        private const string averageStagingCurrentString = "AverageStagingCurrent";
        private const string AverageStagingTimeString = "AverageStagingTime";
        private const string commandedPositionString = "CommandedPosition";
        private const string controlWordString = "ControlWord";
        private const string errorCodeString = "ErrorCode";
        private const string maxActualPositionErrorString = "MaxActualPositionError";
        private const string maxActualAbsolutePositionErrorString = "MaxActualAbsolutePositionError";        
        private const string moveString = "Move";
        private const string moveAverageCurrentString = "MoveAverageCurrent";
        private const string moveMaxPositionErrorString = "MoveMaxPositionError";
        private const string statusWordString = "StatusWord";
        private const string targetPositionString = "TargetPosition";
           
        private string motorName = string.Empty;
        private int xValue;
        private double yValue;
        private bool isPlottable = false;
        private MotorDataType dataType = MotorDataType.Undefined;
        private DataPoint dataPoint;

        public string MotorName { get { return motorName; } set { motorName = value; } }
        public int XValue { get { return xValue; } set { xValue = value; } }
        public double YValue { get { return yValue; } set { yValue = value; } }
        public bool IsPlottable { get{ return isPlottable;} set { isPlottable = value;}}
        public MotorDataType DataType { get{ return dataType;} set { dataType = value;}}
        public DataPoint DataPoint { get { return dataPoint; } set { dataPoint = value; } }
        

        public MotorTrace()
            : base()
        {
            
        }

        public MotorTrace(XPathNavigator trace)
            : base(trace)
        {
            base.DerivedType = this.GetType();
            if(TagAndData.ContainsKey(NSFTraceTags.objectTag))
                motorName = TagAndData[NSFTraceTags.objectTag].Remove(TagAndData[NSFTraceTags.objectTag].Length - "motor".Length);
            else if(TagAndData.ContainsKey(NSFTraceTags.sourceTag))
                motorName = TagAndData[NSFTraceTags.sourceTag].Remove(TagAndData[NSFTraceTags.nameTag].Length - "motor".Length);
            
            System.Enum.TryParse<MotorDataType>(TagAndData[NSFTraceTags.nameTag], out dataType);
            if (dataType != MotorDataType.Undefined && dataType < MotorDataType.ControlWord)
            {
                isPlottable = true;
                xValue = base.TraceTime;
                yValue = Convert.ToDouble(this.TagAndData[NSFTraceTags.valueTag]);
            }
            else
            {
                if (dataType == MotorDataType.Undefined)
                    return;
            }
        }
        
        public override DataPoint GetDataPoint()
        {
            return dataPoint;
        }


        public override void AddListViewItem(ListView lv)
        {
            DateTime nT;
            ListViewItem item = new ListViewItem();
            ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem();
            ListView.ColumnHeaderCollection lvCol = lv.Columns;

            if (!lv.Columns.ContainsKey(NSFTraceTags.objectTag))
            {
                lv.Columns.Add(NSFTraceTags.objectTag, NSFTraceTags.objectTag);
                item.SubItems.Add("");
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
    }
}
