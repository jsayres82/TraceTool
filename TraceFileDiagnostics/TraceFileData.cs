using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraceFileReader.EnumTypes;
using TraceFileReader.PlotTypes;
using TraceFileReader.TraceTypes;
using NorthStateFramework;
using TimeBeam;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace TraceFileReader
{
    public class TraceFileData
    {
        private const string TraceSaveCompleteString = "TraceSaveComplete";
        private const string FillRequestTraceType = "FillRequest";
        private const string FillStatusTraceType = "FillStatus";
        private const string IOUpdateTraceType = "IOUpdate";
        private const string TraceEventString = "TraceEvent";
        //private List<ServoMotor> servoList = new List<ServoMotor>();
        //private List<ServoMotorMove> servoMotorMoveList = new List<ServoMotorMove>();
        //private Dictionary<string, CANOpenNode> nodes = new Dictionary<string, CANOpenNode>();
        private List<Trace> allTraces = new List<Trace>();
        private Dictionary<string, List<Trace>> stateMachine = new Dictionary<string, List<Trace>>();
        private Dictionary<string, List<Trace>> traceList = new Dictionary<string, List<Trace>>();
        private Dictionary<string, List<DataPlotBase>> plotList = new Dictionary<string, List<DataPlotBase>>();
        static private Dictionary<string, List<MotorTrace>> motorTraceDic = new Dictionary<string, List<MotorTrace>>();
        static private Dictionary<string, List<SensorTrace>> sensorTraceDic = new Dictionary<string, List<SensorTrace>>();
        static private Dictionary<SubsystemType, List<Trace>> subSystemTraceDic = new Dictionary<SubsystemType, List<Trace>>();

        static private DateTime timeSyncDate;
        static private int timeSyncMilliSec;
        private int initialTimeStamp;
        private int longestNameLength;

        static private MachineModels model = MachineModels.Unknown;
        static public MachineModels MachineModel { get { return model; } set { model = value; } }
        static public DateTime TimeSyncDate { get { return timeSyncDate; } }
        static public int TimeSyncMilliSec { get { return timeSyncMilliSec; } }
        public Dictionary<string, List<DataPlotBase>> PlotList { get { return plotList; } set { plotList = value; } }
        public Dictionary<string, List<Trace>> StateMachine { get { return stateMachine; } set { stateMachine = value; } }
        public Dictionary<string, List<Trace>> TraceList { get { return traceList; } set { traceList = value; } }
        public static Dictionary<string, List<MotorTrace>> MotorTraceDic { get { return motorTraceDic; } set { motorTraceDic = value; } }
        public static Dictionary<string, List<SensorTrace>> SensorTraceDic { get { return sensorTraceDic; } set { sensorTraceDic = value; } }
        public static Dictionary<SubsystemType, List<Trace>> SubSystemTraceDic { get { return subSystemTraceDic; } set { subSystemTraceDic = value; } }


        public TraceFileData(string fileName)
        {
            traceList = TraceFactory.BuildTraceList(fileName);
            if(traceList != null)
                ConvertTimeStampData();
            sortTraceList();
            //buildDeviceList("..\\..\\MachineConfiguration\\" + modelType.ToString() + "\\SystemConfiguration.xml");
        }

        public void ConvertTimeStampData()
        {
            int finalTimeStamp = 0;
            initialTimeStamp = int.MaxValue;
            longestNameLength = 0;

            foreach (Trace t in traceList[NSFTraceTags.informationalTag])
            {
                allTraces.Add(t);
                if (t.TagAndData.ContainsValue("Time"))
                {
                    string date = t.TagAndData["Value"].Split(' ')[0];
                    string time = t.TagAndData["Value"].Split(' ')[1];
                    string amPm = t.TagAndData["Value"].Split(' ')[2];
                    string mSec = time.Split('.')[1];
                    int timeHour = Convert.ToInt32(time.Split(':')[0]);
                    time = time.Split('.')[0];


                    DateTime d = new DateTime(Convert.ToInt32(date.Split('/')[2]), Convert.ToInt32(date.Split('/')[0]), Convert.ToInt32(date.Split('/')[1]),
                        timeHour, Convert.ToInt32(time.Split(':')[1]), Convert.ToInt32(time.Split(':')[2]), Convert.ToInt32(mSec), DateTimeKind.Utc);

                    if (amPm.Equals("PM"))
                        d.AddHours(12);

                    timeSyncDate = d;
                    timeSyncMilliSec = t.TraceTime;
                    break;
                }
            }

            foreach (List<Trace> tL in traceList.Values)
            {
                foreach (Trace t in tL)
                {
                    t.setDateTimeFromTimeStamp(timeSyncDate, timeSyncMilliSec);
                    if (t.TraceTime > finalTimeStamp)
                        finalTimeStamp = t.TraceTime;
                    else if (t.TraceTime < initialTimeStamp)
                        initialTimeStamp = t.TraceTime;
                }
            }

            foreach (List<Trace> tL in traceList.Values)
            {
                foreach (Trace t in tL)
                {
                    t.setTraceTimeOffset(initialTimeStamp);
                }
            }
        }


        /// <summary>
        /// Creates a dictionary keyed on tag data type
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private void sortTraceList()
        {

            plotList.Add("Motor", new List<DataPlotBase>());
            foreach (KeyValuePair<string, List<MotorTrace>> kvp in MotorTraceDic)
            {
                LinearMotorType lMT;
                DataPlotBase dP = new DataPlotBase();
                dP.Key = string.Empty;
                foreach (MotorTrace mT in kvp.Value.Where(m => m.IsPlottable && m.DataType == MotorDataType.ActualPosition))
                {
                    if (dP.Key.Equals(string.Empty))
                    {
                        dP.Key = kvp.Key + mT.DataType.ToString();

                        if (Enum.TryParse<LinearMotorType>(mT.MotorName, out lMT))
                            dP.YAxis = new LinearAxis();
                        else
                            dP.YAxis = new AngleAxis();
                    }

                    //tD[kvp.Key].Add(mT);
                }


                dP.AxisPosition = AxisPosition.Left;
                dP.dataSeries = new LineSeries();

                if (Enum.TryParse<LinearMotorType>(kvp.Value.First().MotorName, out lMT))
                    dP.YAxis = new LinearAxis();
                else
                    dP.YAxis = new AngleAxis();

                plotList["Motor"].Add(dP);
            }


            plotList.Add("Sensor", new List<DataPlotBase>());
            foreach (KeyValuePair<string, List<SensorTrace>> kvp in SensorTraceDic)
            {
                DataPlotBase dP = new DataPlotBase();
                dP.Key = kvp.Key;
                dP.AxisPosition = OxyPlot.Axes.AxisPosition.Left;

                //tD.Add(kvp.Key, new List<Trace>());

                foreach (SensorTrace sT in kvp.Value)
                {
                    //tD[kvp.Key].Add(sT);
                }
                plotList["Motor"].Add(dP);
            }
            foreach (KeyValuePair<string, List<Trace>> traceType in traceList)
            {

                foreach (Trace t in traceType.Value)
                {
                    if (t.TraceType.Equals(NSFTraceTags.variableTag))
                    {

                    }
                    else if (t.TraceType.Equals(NSFTraceTags.stateEnteredTag))
                    {
                        if (!stateMachine.ContainsKey(t.TagAndData[NSFTraceTags.stateMachineTag]))
                        {
                            stateMachine.Add(t.TagAndData[NSFTraceTags.stateMachineTag], new List<Trace>());
                            //traceData.Add(t.TagAndData[NSFTraceTags.stateMachineTag], new List<Trace>());
                        }
                        stateMachine[t.TagAndData[NSFTraceTags.stateMachineTag]].Add(t);
                    }
                    else if (t.TraceType.Equals(NSFTraceTags.eventQueuedTag))
                    {

                    }
                    else if (t.TraceType.Equals(NSFTraceTags.informationalTag))
                    {

                    }
                    else if (t.TraceType.Equals(IOUpdateTraceType))
                    {

                    }
                    else if (t.TraceType.Equals(NSFTraceTags.errorTag))
                    {

                    }
                    else if (t.TraceType.Equals(FillStatusTraceType))
                    {

                    }
                    else if (t.TraceType.Equals(FillRequestTraceType))
                    {

                    }
                    else if (t.TraceType.Equals("CanMessaging"))
                    {

                    }
                    else if (t.TraceType.Equals("CellMessaging"))
                    {

                    }
                }

            }
        }


        public void LoadTimeline(Timeline timeLine)
        {
            timeLine.Tracks.Clear();
            foreach (KeyValuePair<string, List<Trace>> tD in traceList)
            {
                if (tD.Key.Length >= longestNameLength)
                    longestNameLength = tD.Key.Length;
                if (!tD.Key.Equals(NSFTraceTags.stateEnteredTag))
                    timeLine.AddTrack(new TraceTrack(tD.Value, initialTimeStamp) { Name = tD.Key });
            }

            foreach (KeyValuePair<string, List<Trace>> tL in stateMachine)
            {
                if (tL.Key.Length >= longestNameLength)
                    longestNameLength = tL.Key.Length;
                timeLine.AddTrack(new TraceTrack(tL.Value, initialTimeStamp) { Name = tL.Key });
            }

            timeLine.TrackLabelWidth = longestNameLength * Convert.ToInt32(timeLine.Font.Size);
        }


        public static void SetMachineType(string serialNum)
        {
            MachineModels m = (MachineModels)Convert.ToInt32(serialNum.Substring(0, 2));
            switch (m)
            {
                case MachineModels.Max:
                    model = MachineModels.Max;
                    break;
                case MachineModels.Mini:
                    model = MachineModels.Mini;
                    break;
                case MachineModels.Express:
                    model = MachineModels.Express;
                    break;
                case MachineModels.CalibrationStation:
                    model = MachineModels.CalibrationStation;
                    break;
                case MachineModels.MaxPlus:
                    model = MachineModels.MaxPlus;
                    break;
                case MachineModels.Uber:
                    model = MachineModels.Uber;
                    break;
                case MachineModels.MaxComplete:
                    model = MachineModels.MaxComplete;
                    break;
                case MachineModels.MaxChute:
                    model = MachineModels.MaxChute;
                    break;
                case MachineModels.ExpressCountAhead:
                    model = MachineModels.ExpressCountAhead;
                    break;
                default:
                    model = MachineModels.Unknown;
                    break;
            }
        }


    }
}
