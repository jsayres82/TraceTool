using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.Xml;
using TraceFileReader.TraceTypes;
using NorthStateFramework;
using TraceFileReader.EnumTypes;

namespace TraceFileReader
{
    public class TraceFactory
    {

        private const string TraceTag = "Trace";
        private const string TraceLogTag = "TraceLog";
        private const string TimeTag = "Time";
        private const string TraceSaveString = "TraceSave";
        private const string TraceSaveCompleteString = "TraceSaveComplete";
        private const string FillRequestTraceType = "FillRequest";
        private const string FillStatusTraceType = "FillStatus";
        private const string IOUpdateTraceType = "IOUpdate";
        private const string TraceEventString = "TraceEvent";


        private Dictionary<string, List<Trace>> traceList = new Dictionary<string, List<Trace>>();
        

        public static Dictionary<string, List<Trace>> BuildTraceList(String filename)
        {
            XPathNavigator docNavigator = getNavigatorForFile(filename);
            if (docNavigator != null)
                return TraverseTraceFile(docNavigator);
            else
                return null;
        }


        /// <summary>
        /// Creates a new XmlDocument for the file, 
        /// loads the file ignoring namespaces, 
        /// closes the file and returns a navigator to the document.
        /// </summary>
        /// <param name="filename">Name of file containing XML data.</param>
        /// <returns>A navigator to the xml contained in the file.</returns>
        public static XPathNavigator getNavigatorForFile(String filename)
        {
            // Create a document and navagator for this data file
            XmlDocument document = new XmlDocument();
            XPathNavigator xml;
            using (XmlTextReader reader = new XmlTextReader(filename))
            {
                reader.Namespaces = false;
                document.Load(reader);
                reader.Close();
            }
            xml = document.CreateNavigator();
            xml.MoveToFirstChild();
            if (xml.Name == TraceLogTag)
                return xml;
            else
                return null;

        }


        /// <summary>
        /// Iterates through a trace file
        /// creating a Dictionary of trace objects
        /// keyed on trace type and returns the dictionary.
        /// </summary>
        /// <param name="docNavigator">A navigator to the xml contained in the file.</param>
        /// <param name="currentFilename">Name of file containing XML data.</param>
        /// <returns>A dictionary of trace objects.</returns>
        public static Dictionary<string, List<Trace>> TraverseTraceFile(XPathNavigator docNavigator)
        {
            int lastTraceTime = 0;
            int concurrentTraceTimeCount = 0;
            Trace t = null;
            FillStatusTrace fillStatus = null;
            FillRequestTrace fillRequest = null;
            XPathNavigator xml;
            XPathNodeIterator traceElementIterator;
            Dictionary<string, List<Trace>> list = new Dictionary<string, List<Trace>>();
            
            traceElementIterator = docNavigator.SelectDescendants(TraceTag, String.Empty, false);

            foreach (XPathNavigator traceEntry in traceElementIterator)
            {
                xml = traceEntry.Clone();
                xml.MoveToChild(TimeTag, String.Empty);

                if (xml.ValueAsInt == lastTraceTime)
                {
                    if (concurrentTraceTimeCount < 100)
                        ++concurrentTraceTimeCount;
                }
                else
                {
                    lastTraceTime = xml.ValueAsInt;
                    concurrentTraceTimeCount = 0;
                }

                xml.MoveToNext();

                switch (xml.Name)
                {
                    case NSFTraceTags.eventQueuedTag:
                        t = new StateMachineTrace(traceEntry);
                        t.ConcurrentValue = concurrentTraceTimeCount;
                        break;
                    case NSFTraceTags.stateEnteredTag:
                        t = new StateMachineTrace(traceEntry);
                        t.ConcurrentValue = concurrentTraceTimeCount;
                        break;
                    case NSFTraceTags.messageSentTag:
                        if (!xml.Value.Contains("CellNetwork"))
                        {
                            t = new CanMessageTrace(traceEntry);
                        }
                        else
                        {
                            t = new CellNetworkMessageTrace(traceEntry);
                        }
                        t.ConcurrentValue = concurrentTraceTimeCount;
                        break;
                    case NSFTraceTags.messageReceivedTag:
                        if (!xml.Value.Contains("CellNetwork"))
                        {
                            t = new CanMessageTrace(traceEntry);

                        }
                        else
                        {
                            t = new CellNetworkMessageTrace(traceEntry);
                        }
                        t.ConcurrentValue = concurrentTraceTimeCount;
                        break;
                    case NSFTraceTags.variableTag:
                        xml.MoveToFirstChild();
                        if (xml.Value.Contains("FillStatus"))
                        {
                            xml.MoveToNext();
                            if (fillStatus == null)
                            {
                                fillStatus = new FillStatusTrace(traceEntry);
                                fillStatus.ConcurrentValue = concurrentTraceTimeCount;
                            }
                            else if (!fillStatus.TagAndData.ContainsKey(xml.Value))
                            {
                                fillStatus.AddStatusSection(xml);
                            }
                            else if (fillStatus.TagAndData.ContainsKey(xml.Value))
                            {
                                if (!list.ContainsKey(fillStatus.TraceType))
                                    list.Add(fillStatus.TraceType, new List<Trace>());

                                list[fillStatus.TraceType].Add(fillStatus);
                                fillStatus = new FillStatusTrace(traceEntry);
                                fillStatus.ConcurrentValue = concurrentTraceTimeCount;
                            }
                        }
                        else if (xml.Value.Contains("Request"))
                        {
                            xml.MoveToNext();
                            if (fillRequest == null)
                            {
                                fillRequest = new FillRequestTrace(traceEntry);
                                fillRequest.ConcurrentValue = concurrentTraceTimeCount;
                            }
                            else if (!fillRequest.TagAndData.ContainsKey(xml.Value))
                            {
                                fillRequest.AddStatusSection(xml);
                            }
                            else if (fillRequest.TagAndData.ContainsKey(xml.Value))
                            {
                                fillRequest.FillRequestComplete = true;
                                if (!list.ContainsKey(fillRequest.TraceType))
                                    list.Add(fillRequest.TraceType, new List<Trace>());

                                list[fillRequest.TraceType].Add(fillRequest);
                                fillRequest = new FillRequestTrace(traceEntry);
                                fillRequest.ConcurrentValue = concurrentTraceTimeCount;

                            }
                        }
                        else if (xml.Value.Contains("Process"))
                        {
                            t = new StateMachineTrace(traceEntry);
                            if (t.TagAndData.ContainsKey(NSFTraceTags.sourceTag))
                            {
                                t.TagAndData.Add(NSFTraceTags.objectTag, t.TagAndData[NSFTraceTags.sourceTag]);
                                t.TagAndData.Remove(NSFTraceTags.sourceTag);
                            }
                            t.ConcurrentValue = concurrentTraceTimeCount;
                        }
                        else if (xml.Value.Contains("Motor"))
                        {
                            MotorTrace mT;
                            t = new MotorTrace(traceEntry);
                            mT = t as MotorTrace;
                            if (!TraceFileData.MotorTraceDic.ContainsKey(mT.MotorName))
                                TraceFileData.MotorTraceDic.Add(mT.MotorName, new List<MotorTrace>());
                            TraceFileData.MotorTraceDic[mT.MotorName].Add(mT);
                            t.ConcurrentValue = concurrentTraceTimeCount;
                        }
                        else
                        {
                            t = new StateMachineTrace(traceEntry);
                            if (t.TagAndData.ContainsKey(NSFTraceTags.sourceTag))
                            {
                                t.TagAndData.Add(NSFTraceTags.objectTag, t.TagAndData[NSFTraceTags.sourceTag]);
                                t.TagAndData.Remove(NSFTraceTags.sourceTag);
                            }
                            if (t.TagAndData[NSFTraceTags.nameTag].Equals("SampledData"))
                                t.TagAndData[NSFTraceTags.nameTag] = t.TagAndData[NSFTraceTags.objectTag] + "-SampledData";
                            t.ConcurrentValue = concurrentTraceTimeCount;
                        }
                        break;
                    case NSFTraceTags.informationalTag:
                        t = new StateMachineTrace(traceEntry);
                        if (t.TagAndData.ContainsKey(NSFTraceTags.nameTag))
                            if (t.TagAndData[NSFTraceTags.nameTag].Equals("MachineSerialNumber"))
                                if (TraceFileData.MachineModel == MachineModels.Unknown)
                                    TraceFileData.SetMachineType(t.TagAndData[NSFTraceTags.valueTag]);

                        t.ConcurrentValue = concurrentTraceTimeCount;
                        break;
                    case NSFTraceTags.errorTag:
                        t = new StateMachineTrace(traceEntry);
                        t.ConcurrentValue = concurrentTraceTimeCount;
                        break;
                    case IOUpdateTraceType:
                        SensorTrace sT;
                        t = new SensorTrace(traceEntry);
                        sT = t as SensorTrace;
                        if (!TraceFileData.SensorTraceDic.ContainsKey(sT.SensorName))
                            TraceFileData.SensorTraceDic.Add(sT.SensorName, new List<SensorTrace>());
                        TraceFileData.SensorTraceDic[sT.SensorName].Add(sT);
                        t.ConcurrentValue = concurrentTraceTimeCount;
                        break;
                    default:
                        t = new Trace(traceEntry);
                        t.ConcurrentValue = concurrentTraceTimeCount;
                        break;
                }

                xml.MoveToNext();
                if (t != null)
                {
                    if (!list.ContainsKey(t.TraceType))
                        list.Add(t.TraceType, new List<Trace>());

                    list[t.TraceType].Add(t);
                    t = null;
                }
                else if (fillRequest != null)
                {
                    if (fillRequest.FillRequestComplete)
                    {
                        if (!list.ContainsKey(fillRequest.TraceType))
                            list.Add(fillRequest.TraceType, new List<Trace>());

                        list[fillRequest.TraceType].Add(fillRequest);
                        fillRequest = null;
                    }
                }
            }
            return list;
        }

       
    }
}
