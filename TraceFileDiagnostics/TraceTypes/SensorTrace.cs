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

namespace TraceFileReader.TraceTypes
{
    public class SensorTrace : Trace, IDataPointProvider
    {
        private const string InactiveString = "Inactive";
        private const string ActiveString = "Active";
        private const string DeadBandString = "DeadBand";

        private string sensorName = string.Empty;
        private int xValue;
        private double yValue;
        private bool isPlottable = false;
        private SensorStateType sensorState = SensorStateType.Undefined;
        private DataPoint dataPoint;

        public string SensorName { get { return sensorName; } set { sensorName = value; } }
        public int XValue { get { return xValue; } set { xValue = value; } }
        public double YValue { get { return yValue; } set { yValue = value; } }
        public bool IsPlottable { get { return isPlottable; } set { isPlottable = value; } }
        public SensorStateType SensorState { get { return sensorState; } set { sensorState = value; } }
        public DataPoint DataPoint { get { return dataPoint; } set { dataPoint = value; } }
    
        public SensorTrace()
            : base()
        {

        }

        public SensorTrace(XPathNavigator trace)
            : base(trace)
        {
            base.DerivedType = this.GetType();
            sensorName = TagAndData[NSFTraceTags.nameTag];
            System.Enum.TryParse<SensorStateType>(TagAndData[NSFTraceTags.stateTag], out sensorState);
            if (sensorState != SensorStateType.Undefined)
            {
                isPlottable = true;
                xValue = base.TraceTime;
                yValue = Convert.ToDouble(this.TagAndData[NSFTraceTags.valueTag]);
            }
        }

        public override DataPoint GetDataPoint()
        {
            return dataPoint;
        }
    }
}
