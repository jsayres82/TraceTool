using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Xml.XPath;
using System.Xml;
using NorthStateFramework;
using TimeBeam;
using TimeBeam.Events;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ServiceTool.TraceTypes;
using ServiceTool.PlotTypes;
using ServiceTool.EnumTypes;
using Parata.Controls.CANBus;
using Parata.Controls;

namespace ServiceTool
{
    //TODO:  Add ability to load and read machine configuratino files 
    public partial class Form1 : Form
    {
        private const string TimeTag = "Time";
        private const string TraceTag = "Trace";
        private const string TraceLogTag = "TraceLog";
        private const string TraceSaveString = "TraceSave";
        private const string TraceSaveCompleteString = "TraceSaveComplete";
        private const string FillRequestTraceType = "FillRequest";
        private const string FillStatusTraceType = "FillStatus";
        private const string IOUpdateTraceType = "IOUpdate";
        private const string TraceEventString = "TraceEvent";
        private string traceFileName = string.Empty;
        private DateTime timeSyncDate;
        private int timeSyncMilliSec;
        private int initialTimeStamp;
        private int finalTimeStamp;
        private int longestNameLength;
        private List<Trace> allTraces = new List<Trace>();
        private Dictionary<string, List<Trace>> stateMachine = new Dictionary<string, List<Trace>>();
        private Dictionary<string, List<Trace>> traceList = new Dictionary<string, List<Trace>>();
        private Dictionary<string, List<DataPlotBase>> plotList = new Dictionary<string, List<DataPlotBase>>();
        static private Dictionary<string, List<MotorTrace>> motorTraceDic = new Dictionary<string, List<MotorTrace>>();
        static private Dictionary<string, List<SensorTrace>> sensorTraceDic = new Dictionary<string, List<SensorTrace>>();
        static private Dictionary<SubsystemType, List<Trace>> subSystemTraceDic = new Dictionary<SubsystemType, List<Trace>>();
        static private MachineModels modelType = MachineModels.Unknown;
        private Dictionary<string, List<object>> objDictionary = new Dictionary<string, List<object>>();
        private Dictionary<string, List<object>> nodeDictionary = new Dictionary<string,List<object>>();

        private TimeBeamClock _clock = new TimeBeamClock();
        //private SciLabUtility sciLab;
        private ListView selectedListView;
        private ListViewHitTestInfo currentHitTestInfo;
        private Dictionary<string, bool> ColumnItemState = new Dictionary<string, bool>();
        private Dictionary<string, List<ListViewItem>> listView1RemovedRows = new Dictionary<string, List<ListViewItem>>();

        private List<string> listView1ItemsToAdd = new List<string>();
        private List<string> listView1ItemsToRemove = new List<string>();
        private string[] objectTypes;
        private List<ServoMotor> servoList = new List<ServoMotor>();
        private List<ServoMotorMove> servoMotorMoveList = new List<ServoMotorMove>();

        private Dictionary<string, CANOpenNode> nodes = new Dictionary<string, CANOpenNode>();


        public System.Windows.Forms.Form MyChild;
        PopupForm f2;
        private ListViewColumnSorter lvwColumnSorter;
        private ICollection devices;
        private Dictionary<string, List<Object>> deviceObjects = new Dictionary<string, List<Object>>();

        public DateTime TimeSyncDate { get { return timeSyncDate; } }
        public int TimeSyncMilliSec { get { return timeSyncMilliSec; } }
        public ListView ListView1 { get { return listView1; } set { listView1 = value; } }
        public ListView LvConncurrentItems { get { return lvConcurrentItems; } set { lvConcurrentItems = value; } }
        public Dictionary<string, List<MotorTrace>> MotorTraceDic { get { return motorTraceDic; } set { motorTraceDic = value; } }
        public Dictionary<string, List<SensorTrace>> SensorTraceDic { get { return sensorTraceDic; } set { sensorTraceDic = value; } }
        public Dictionary<SubsystemType, List<Trace>> SubSystemTraceDic { get { return subSystemTraceDic; } set { subSystemTraceDic = value; } }
        private const String LocalDirectoryString = "LocalDirectory";
        private const String FilenameString = "Filename";
        private const String VariablesDefinitionsString = "VariablesDefinitions";
        private const String HandledString = "Handled";
        private const String FeaturesFileString = "ConfigurationTemplateFile";
        private const String ConfigurationItemsFileString = "ConfigurationItemsFile";
        private const String ConfigurationItemsTemplateFileString = "ConfigurationItemsTemplateFile";
        private const String ConfigurationItemsFilesString = "ConfigurationItemsFiles";
        private const String TrueString = "True";
        private static List<String> XMLFactoryKeywords = new List<string>(new String[] { VariablesDefinitionsString, LocalDirectoryString, FilenameString, ConfigurationItemsFileString, ConfigurationItemsTemplateFileString, ConfigurationItemsFilesString, TrueString });
        private static OrderedDictionary globalVariables = new OrderedDictionary();
        
        private static XPathNavigator traverseNode(XPathNavigator docNavigator, string currentFilename, OrderedDictionary parentVariables)
        {
            if (docNavigator.Name == ConfigurationItemsFilesString)
            {
                docNavigator = loadConfigurationItems(docNavigator, currentFilename);
            }
            else if (docNavigator.Name == VariablesDefinitionsString)
            {
                docNavigator = loadVariables(docNavigator, currentFilename, null);
            }
            else if (docNavigator.Name == FilenameString)
            {
                docNavigator = expandDataFile(docNavigator, currentFilename, null);
            }
            else
            {
                docNavigator = substituteVariables(docNavigator, null);
            }

            // Note that you cannot use an iterator here because the 
            // members of the collection can be changed out via variable substitution.
            if (docNavigator.HasChildren)
            {
                XPathNavigator childNode = docNavigator.Clone();
                childNode.MoveToFirstChild();

                do
                {
                    childNode = traverseNode(childNode, currentFilename, null);
                } while (childNode.MoveToNext());
            }

            return docNavigator;
        }

        private static XPathNavigator substituteVariables(XPathNavigator docNavigator, OrderedDictionary variables)
        {
            if (globalVariables.Count == 0)
            {
                return docNavigator;
            }

            String doc = docNavigator.OuterXml.ToString();
            foreach (String variable in globalVariables.Keys)
            {
                String variableString = "_" + variable + "_";
                doc = doc.Replace(variableString, (String)globalVariables[variable]);
            }

            docNavigator.ReplaceSelf(doc);
            return docNavigator;
        }

        private static XPathNavigator loadConfigurationItems(XPathNavigator parentFileNavigator, string parentFilename)
        {
            string currentPath = Path.GetDirectoryName(parentFilename);

            bool fileModified = false;
            XPathNavigator docNav = parentFileNavigator.Clone();
            docNav.MoveToChild(XPathNodeType.Element);
            XPathNodeIterator childIterator = docNav.SelectChildren(XPathNodeType.Element);

            foreach (XPathNavigator childNode in childIterator)
            {
                new ConfigurationItem(childNode, @"mytest.xml");
            }
            return parentFileNavigator;
            //String configurationItemsTemplateFilename = Path.Combine(currentPath, Read<String>(parentFileNavigator, ConfigurationItemsTemplateFileString));
            //String configurationItemsFilename = Path.Combine(currentPath, Read<String>(parentFileNavigator, ConfigurationItemsFileString));

            //XPathNavigator templateNavigator = getNavigatorForFile(configurationItemsTemplateFilename);
            //XPathNavigator configNavigator;
            //try
            //{
            //    configNavigator = getNavigatorForFile(configurationItemsFilename);
            //}
            //catch (System.IO.FileNotFoundException)
            //{
            //    fileModified = true;
            //    configNavigator = templateNavigator.Clone();
            //}

            // Move into the root element
            //templateNavigator.MoveToFirstChild();
            //configNavigator.MoveToFirstChild();

            // Make sure all the template nodes are in the config file
            //XPathNodeIterator templateNodes = templateNavigator.SelectChildren(XPathNodeType.Element);
            //foreach (XPathNavigator templateNode in templateNodes)
            //{
            //    if (!configNavigator.MoveToChild(templateNode.Name, String.Empty))
            //    {
            //        fileModified = true;
            //        configNavigator.AppendChild(templateNode);
            //    }
            //    else
            //    {
            //        configNavigator.MoveToParent();
            //    }

            //    new ConfigurationItem(templateNode, configurationItemsFilename);
            //}

            //// Make sure only the template node are in the config file
            //XPathNodeIterator configNodes = configNavigator.SelectChildren(XPathNodeType.Element);
            //foreach (XPathNavigator configNode in configNodes)
            //{
            //    if (!templateNavigator.MoveToChild(configNode.Name, String.Empty))
            //    {
            //        fileModified = true;
            //        configNode.DeleteSelf();
            //    }
            //    else
            //    {
            //        templateNavigator.MoveToParent();
            //    }
            //}

            //if (fileModified)
            //{
            //    saveNavigatorToFile(configNavigator, configurationItemsFilename);
            //}
        }


        private static XPathNavigator expandDataFile(XPathNavigator docNavigator, string currentFilename, OrderedDictionary variables)
        {
            string currentPath = Path.GetDirectoryName(currentFilename);
            String dataFileName = Path.Combine(currentPath, docNavigator.Value);
            XPathNavigator dataFileNavigator = docNavigator;

            if (docNavigator.GetAttribute(HandledString, String.Empty) != TrueString)
            {
                docNavigator.CreateAttribute(String.Empty, HandledString, String.Empty, TrueString);
            }
            else
            {
                return docNavigator;
            }


            // Move to the child of the mandatory root node of the document and select the children of that node.  
            // This is done so that all the sub nodes of the child will be inserted without an extraneous parent node.
            dataFileNavigator.MoveToChild(XPathNodeType.Element);

            dataFileNavigator = traverseNode(dataFileNavigator, dataFileName, null);

            XPathNodeIterator dataFileNodes = dataFileNavigator.SelectChildren(XPathNodeType.Element);
            docNavigator.InsertElementBefore(String.Empty, LocalDirectoryString, String.Empty, Path.GetDirectoryName(dataFileName));

            // Interate through all the nodes in the data file adding each of them to the document
            foreach (XPathNavigator dataFileNode in dataFileNodes)
            {
                docNavigator.InsertBefore(dataFileNode.ReadSubtree());
            }

            return docNavigator;
        }

        private static XPathNavigator loadVariables(XPathNavigator docNavigator, string currentFilename, OrderedDictionary variables)
        {
            docNavigator.MoveToChild(XPathNodeType.Element);
            XPathNodeIterator childIterator = docNavigator.SelectChildren(XPathNodeType.Element);

            foreach (XPathNavigator childNode in childIterator)
            {
                traverseNode(childNode, currentFilename, null);
            }

            foreach (XPathNavigator childNode in childIterator)
            {
                // Ignore this keywords, they are not variables.
                if (!XMLFactoryKeywords.Contains(childNode.Name))
                {
                    // If the varialbe already exist then write the new value.
                    if (globalVariables.Contains(childNode.Name))
                    {
                        globalVariables[childNode.Name] = childNode.Value;
                    }
                    // Otherwise just add it to the list of variables.
                    else
                    {
                        globalVariables.Insert(0, childNode.Name, childNode.Value);
                    }

                    // Update any configuration item that may be associated with the variable.
                    ConfigurationItem.updateConfigurationItem(childNode.Name, globalVariables[childNode.Name].ToString());
                }
            }

            return docNavigator;
        }

        public Form1(string traceFile)
        {
            InitializeComponent();
            //this.KeyPreview = true;
            string fileName = @"mytest.xml";
            string[] ArrLines;
            int n, i;

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            n = 10;
            ArrLines = new string[n];
            Console.Write(" Input {0} strings below :\n", n);
            for (i = 0; i < n; i++)
            {
                if(i==0 )
                    ArrLines[i] = "<RootElement>";
                else if(i == (n-1))
                    ArrLines[i] = "</RootElement>";
                else
                    ArrLines[i] = "<" + "TestElement_" + i + " Options=\"Test,Test12345\">Test12345" + "</" + "TestElement_" + i + ">";
            }
            System.IO.File.WriteAllLines(fileName, ArrLines);

            XPathNavigator testNavigator;
            try
            {
                testNavigator = getNavigatorForFile(fileName);
            }
            catch (Exception ex)
            {
                throw;
            }

            string file = @"mytest.xml";

            loadConfigurationItems(testNavigator, file);
            loadVariables(testNavigator, file, null);

            //Console.Write(" Input number of lines to write in the file  :");
            //n = Convert.ToInt32(Console.ReadLine());
            //ArrLines = new string[n];
            //Console.Write(" Input {0} strings below :\n", n);
            //for (i = 0; i < n; i++)
            //{
            //    Console.Write(" Value line {0} : ", i + 1);
            
            //    ArrLines[i] = Console.ReadLine();
            //}
            //System.IO.File.WriteAllLines(fileName, ArrLines);

            lvwColumnSorter = new ListViewColumnSorter();
            this.listView1.ListViewItemSorter = lvwColumnSorter;
            //sciLab = new SciLabUtility();
            // Register the clock with the timeline
            timeline1.Clock = _clock;
            // Activate the timer that invokes the clock to update.
            timer1.Enabled = true;

            objectTypes = new string[] {
                "CANOpenNode",
                "CANSIODigitalInput",
                "CANSIODigitalOutput",
                "CANSIOServoMotor",
                "CANSIOAnalogInput",
                "CANSIOAnalogOutput",
                "LockingSolenoid",
                
                "ServoMotor",
                "ServoMotorPositionControlParameters",
                "ServoMotorCurrentControlParameters",        
                "ServoMotorTrajectoryControlParameters",

                "ServoMotorVelocityMove",
                "ServoMotorRelativePositionMove",
                "ServoMotorAbsolutePositionMove"
           };

            if (traceFile != string.Empty)
            {
                if (string.IsNullOrEmpty(Path.GetFileName(traceFile)))
                    return;

                traceFileName = traceFile;
                textBox1.Text = traceFileName;
                LoadTraceFile(traceFile);
            }
            f2 = new PopupForm(this);
            f2.MyParent = this;
            this.MyChild = f2;
        }

        public Form1()
        {
            InitializeComponent();

            objectTypes = new string[] {
                "CANOpenNode",
                "CANSIODigitalInput",
                "CANSIODigitalOutput",
                "CANSIOServoMotor",
                "CANSIOAnalogInput",
                "CANSIOAnalogOutput",
                "LockingSolenoid",

                "ServoMotor",
                "ServoMotorPositionControlParameters",
                "ServoMotorCurrentControlParameters",        
                "ServoMotorTrajectoryControlParameters",

                "ServoMotorVelocityMove",
                "ServoMotorRelativePositionMove",
                "ServoMotorAbsolutePositionMove"
           };

            //this.KeyPreview = true;
            lvwColumnSorter = new ListViewColumnSorter();
            this.listView1.ListViewItemSorter = lvwColumnSorter;
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
                listView1.Columns.Clear();
                listView1.Items.Clear();
                comboBox1.SelectedIndex = -1;
                comboBox1.Items.Clear();
                listBox1.Items.Clear();
                plotList.Clear();
            }

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                traceFileName = openFileDialog.FileName.ToString();
                textBox1.Text = traceFileName;

                LoadTraceFile(traceFileName);
            }

        }

        public void LoadTraceFile(string filename)
        {
            buildTraceList(filename);
            sortTraceList();
            LoadTimeline();
            if(modelType == MachineModels.Unknown)
                tbDetectedModel.Text = modelType.ToString();
            else
                tbDetectedModel.Text = modelType.ToString();
        }

        public void buildTraceList(String filename)
        {
            XPathNavigator docNavigator = getNavigatorForFile(filename);
            docNavigator.MoveToFirstChild();
            traceList = traverseTraceFile(docNavigator, this);
            ConvertTimeStampData();
            //buildDeviceList("..\\..\\MachineConfiguration\\" + modelType.ToString() + "\\SystemConfiguration.xml");
        }

        //public void builddevicelist(string filename)
        //{
        //    xmlfactory.buildobjects(filename, objecttypes, deviceobjects);
        //    foreach (object o in deviceobjects["canopennode"])
        //    {
        //        canopennode cannode = o as canopennode;
        //        nodedictionary.add(cannode.name, new list<object>());
        //        nodes.add(cannode.name, cannode);
        //    }
        //    foreach (keyvaluepair<string, list<object>> kvp in deviceobjects)
        //    {
        //        if(kvp.key != "canopennode")
        //        {
        //            foreach (object ob in kvp.value)
        //            {

        //                if (ob is cansioanaloginput)
        //                {
        //                    cansioanaloginput o = ob as cansioanaloginput;
        //                    if (o.node != null)
        //                    nodedictionary[o.node.name].add(ob);
        //                }
        //                else if (ob is cansioanalogoutput)
        //                {
        //                    cansioanalogoutput o = ob as cansioanalogoutput;
        //                    if (o.node != null)
        //                    nodedictionary[o.node.name].add(ob);
        //                }
        //                else if (ob is cansiodigitalinput)
        //                {
        //                    cansiodigitalinput o = ob as cansiodigitalinput;
        //                    if(o.node != null)
        //                    nodedictionary[o.node.name].add(ob);
        //                }
        //                else if (ob is cansiodigitaloutput)
        //                {
        //                    cansiodigitaloutput o = ob as cansiodigitaloutput;
        //                    cansionumericio a = o as cansionumericio;
        //                    //if (a.node.name != null)
        //                    //nodedictionary[o.node.name].add(ob);
        //                }
        //                else if (ob is cansiofilteredanaloginput)
        //                {
        //                    cansiofilteredanaloginput o = ob as cansiofilteredanaloginput;
        //                    if(o.node != null)
        //                    nodedictionary[o.node.name].add(ob);
        //                }
        //                else if (ob is cansioservomotor)
        //                {
        //                    cansioservomotor o = ob as cansioservomotor;
        //                    if (o.node != null)
        //                    nodedictionary[o.node.name].add(ob);
        //                }
        //                else if (ob is servomotor)
        //                {
        //                    servomotor o = ob as servomotor;
        //                    servolist.add(o);
        //                }
        //                else if (ob is servomotorabsolutepositionmove)
        //                {
        //                    servomotorabsolutepositionmove o = ob as servomotorabsolutepositionmove;
        //                    servomotormovelist.add(o);
        //                }
        //                else if (ob is servomotorcurrentcontrolparameters)
        //                {
        //                    servomotorcurrentcontrolparameters o = ob as servomotorcurrentcontrolparameters;
        //                }
        //                else if (ob is servomotorpositioncontrolparameters)
        //                {
        //                    servomotorpositioncontrolparameters o = ob as servomotorpositioncontrolparameters;
        //                }
        //                else if (ob is servomotortrajectorycontrolparameters)
        //                {
        //                    servomotortrajectorycontrolparameters o = ob as servomotortrajectorycontrolparameters;
        //                }
        //                else if (ob is servomotorrelativepositionmove)
        //                {
        //                    servomotorrelativepositionmove o = ob as servomotorrelativepositionmove; 
        //                    servomotormovelist.add(o);
        //                }
        //                else if (ob is servomotorvelocitymove)
        //                {
        //                    servomotorvelocitymove o = ob as servomotorvelocitymove;
        //                    servomotormovelist.add(o);
        //                }
        //                else if (ob is lockingsolenoid)
        //                {
        //                    lockingsolenoid o = ob as lockingsolenoid;
        //                    string s = o.name;
        //                }
      
        //            }

        //        }

        //    }

        //}
        
        public void ConvertTimeStampData()
        {
            initialTimeStamp = int.MaxValue;
            finalTimeStamp = 0;
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

        public void LoadTimeline()
        {
            foreach (KeyValuePair<string, List<Trace>> tD in traceList)
            {
                if (tD.Key.Length >= longestNameLength)
                    longestNameLength = tD.Key.Length;
                if (!tD.Key.Equals(NSFTraceTags.stateEnteredTag))
                    timeline1.AddTrack(new TraceTrack(tD.Value, initialTimeStamp) { Name = tD.Key });
            }

            foreach (KeyValuePair<string, List<Trace>> tL in stateMachine)
            {
                if (tL.Key.Length >= longestNameLength)
                    longestNameLength = tL.Key.Length;
                timeline1.AddTrack(new TraceTrack(tL.Value, initialTimeStamp) { Name = tL.Key });
            }

            timeline1.TrackLabelWidth = longestNameLength * Convert.ToInt32(timeline1.Font.Size);
            timeline1.SelectionChanged += TimelineSelectionChanged;
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
            using (XmlTextReader reader = new XmlTextReader(filename))
            {
                reader.Namespaces = false;
                document.Load(reader);
                reader.Close();
            }
            return document.CreateNavigator();
        }


        /// <summary>
        /// Iterates through a trace file
        /// creating a Dictionary of trace objects
        /// keyed on trace type and returns the dictionary.
        /// </summary>
        /// <param name="docNavigator">A navigator to the xml contained in the file.</param>
        /// <param name="currentFilename">Name of file containing XML data.</param>
        /// <returns>A dictionary of trace objects.</returns>
        private static Dictionary<string, List<Trace>> traverseTraceFile(XPathNavigator docNavigator, Form1 parent)
        {
            int lastTraceTime = 0;
            int concurrentTraceTimeCount = 0;
            Trace t = null;
            FillStatusTrace fillStatus = null;
            FillRequestTrace fillRequest = null;
            XPathNavigator xml;
            XPathNodeIterator traceElementIterator;
            Dictionary<string, List<Trace>> list = new Dictionary<string, List<Trace>>();

            if (docNavigator.Name == TraceLogTag)
                traceElementIterator = docNavigator.SelectDescendants(TraceTag, String.Empty, false);
            else
                return list;

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
                        t = new StateMachineTrace(traceEntry, parent);
                        t.ConcurrentValue = concurrentTraceTimeCount;
                        break;
                    case NSFTraceTags.stateEnteredTag:
                        t = new StateMachineTrace(traceEntry, parent);
                        t.ConcurrentValue = concurrentTraceTimeCount;
                        break;
                    case NSFTraceTags.messageSentTag:
                        if (!xml.Value.Contains("CellNetwork"))
                        {
                            t = new CanMessageTrace(traceEntry, parent);
                        }
                        else
                        {
                            t = new CellNetworkMessageTrace(traceEntry, parent);
                        }
                        t.ConcurrentValue = concurrentTraceTimeCount;
                        break;
                    case NSFTraceTags.messageReceivedTag:
                        if (!xml.Value.Contains("CellNetwork"))
                        {
                            t = new CanMessageTrace(traceEntry, parent);

                        }
                        else
                        {
                            t = new CellNetworkMessageTrace(traceEntry, parent);
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
                                fillStatus = new FillStatusTrace(traceEntry, parent);
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
                                fillStatus = new FillStatusTrace(traceEntry, parent);
                                fillStatus.ConcurrentValue = concurrentTraceTimeCount;
                            }
                        }
                        else if (xml.Value.Contains("Request"))
                        {
                            xml.MoveToNext();
                            if (fillRequest == null)
                            {
                                fillRequest = new FillRequestTrace(traceEntry, parent);
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
                                fillRequest = new FillRequestTrace(traceEntry, parent);
                                fillRequest.ConcurrentValue = concurrentTraceTimeCount;

                            }
                        }
                        else if (xml.Value.Contains("Process"))
                        {
                            t = new StateMachineTrace(traceEntry, parent);              
                            if(t.TagAndData.ContainsKey(NSFTraceTags.sourceTag))
                            {
                                t.TagAndData.Add(NSFTraceTags.objectTag, t.TagAndData[NSFTraceTags.sourceTag]);
                                t.TagAndData.Remove(NSFTraceTags.sourceTag);       
                            }
                            t.ConcurrentValue = concurrentTraceTimeCount;
                        }
                        else if( xml.Value.Contains("Motor"))
                        {
                            MotorTrace mT;
                            t = new MotorTrace(traceEntry, parent);
                            mT = t as MotorTrace;
                            if (!motorTraceDic.ContainsKey(mT.MotorName))
                                motorTraceDic.Add(mT.MotorName, new List<MotorTrace>());
                            motorTraceDic[mT.MotorName].Add(mT);
                            t.ConcurrentValue = concurrentTraceTimeCount;
                        }
                        else
                        {
                            t = new StateMachineTrace(traceEntry, parent);
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
                        t = new StateMachineTrace(traceEntry, parent);
                        if (t.TagAndData.ContainsKey(NSFTraceTags.nameTag))
                            if (t.TagAndData[NSFTraceTags.nameTag].Equals("MachineSerialNumber"))
                                if (modelType == MachineModels.Unknown)
                                    setMachineType(t.TagAndData[NSFTraceTags.valueTag]);

                        t.ConcurrentValue = concurrentTraceTimeCount;
                        break;
                    case NSFTraceTags.errorTag:
                        t = new StateMachineTrace(traceEntry, parent);
                        t.ConcurrentValue = concurrentTraceTimeCount;
                        break;
                    case IOUpdateTraceType:
                        SensorTrace sT;
                        t = new SensorTrace(traceEntry, parent);
                        sT = t as SensorTrace;
                        if (!sensorTraceDic.ContainsKey(sT.SensorName))
                            sensorTraceDic.Add(sT.SensorName, new List<SensorTrace>());
                        sensorTraceDic[sT.SensorName].Add(sT);
                        t.ConcurrentValue = concurrentTraceTimeCount;
                        break;
                    default:
                        t = new Trace(traceEntry, parent);
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
                AngularMotorType aMT;
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
               
                
                dP.AxisPosition = OxyPlot.Axes.AxisPosition.Left;
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
                listBox1.Items.Add(traceType.Key);

                foreach (Trace t in traceType.Value)
                {
                    if (t.TraceType.Equals(NSFTraceTags.variableTag))
                    {
                        
                    }
                    else if (t.TraceType.Equals(NSFTraceTags.stateEnteredTag))
                    {
                        
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
                    else if(t.TraceType.Equals("CanMessaging"))
                    {
                       
                    }
                    else if (t.TraceType.Equals("CellMessaging"))
                    {

                    }
                }

            }
        }

        private static void setMachineType(string serialNum)
        {
            MachineModels m = (MachineModels)Convert.ToInt32(serialNum.Substring(0,2));
            switch (m)
            {
                case MachineModels.Max:
                    modelType = MachineModels.Max;
                    break;
                case MachineModels.Mini:
                    modelType = MachineModels.Mini;
                    break;
                case MachineModels.Express:
                    modelType = MachineModels.Express;
                    break;
                case MachineModels.CalibrationStation:
                    modelType = MachineModels.CalibrationStation;
                    break;
                case MachineModels.MaxPlus:
                    modelType = MachineModels.MaxPlus;
                    break;
                case MachineModels.Uber:
                    modelType = MachineModels.Uber;
                    break;
                case MachineModels.MaxComplete:
                    modelType = MachineModels.MaxComplete;
                    break;
                case MachineModels.MaxChute:
                    modelType = MachineModels.MaxChute;
                    break;
                case MachineModels.ExpressCountAhead:
                    modelType = MachineModels.ExpressCountAhead;
                    break;
                default:
                    modelType = MachineModels.Unknown;
                    break;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView1.Columns.Clear();
            listView1.Items.Clear();
            
            listView1.Columns.Add("Date", "Date");
            listView1.Columns.Add("Time", "Time");

            foreach (Trace t in traceList[listBox1.SelectedItem.ToString()])
            {
                List<int> tempTime = new List<int>();
                List<string> tempList = new List<string>();
                ListViewItem item = new ListViewItem();

                foreach (KeyValuePair<string, string> d in t.TagAndData)
                {
                    if (d.Value.Equals(comboBox1.SelectedItem))
                    {
                        t.AddListViewItem(listView1);
                        break;
                    }
                }
            }

            UpdateListViewItemSubItems(listView1);
            AdjustListViewColumnWidth(listView1);
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> dropDownItems = new List<string>();
            comboBox1.SelectedIndex = -1;
            comboBox1.Items.Clear();
            listView1.Columns.Clear();
            listView1.Items.Clear();
            ColumnItemState.Clear();
            listView1RemovedRows.Clear();

            listView1.Columns.Add("Date", "Date");
            listView1.Columns.Add("Time", "Time");

            foreach (Trace t in traceList[listBox1.SelectedItem.ToString()])
            {
                t.AddListViewItem(listView1);
                if (!comboBox1.Items.Contains(t.TagAndData.First().Value))
                    comboBox1.Items.Add(t.TagAndData.First().Value);
            }

            UpdateListViewItemSubItems(listView1);
            AdjustListViewColumnWidth(listView1);

        }

        private void btnPlotValues_Click(object sender, EventArgs e)
        {
            f2 = new PopupForm(this);
            f2.MyParent = this;
            this.MyChild = f2;
            f2.Show();
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

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.listView1.Sort();
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                currentHitTestInfo = listView1.HitTest(e.X, e.Y);
                selectedListView = listView1;
                if (currentHitTestInfo.SubItem != null)
                {
                    int col = currentHitTestInfo.Item.SubItems.IndexOf(currentHitTestInfo.SubItem);
                    string val = currentHitTestInfo.Item.SubItems[col].Text;

                    conMenuColumnClick.Tag = col;
                    ListView lv = listView1;
                    int rowIndex = 0;

                    if (!ColumnItemState.ContainsKey(col + "-" + val))
                    {
                        Dictionary<string, bool> dic = new Dictionary<string, bool>();
                        ColumnItemState.Add(col + "-" + val, true);
                        while (rowIndex < lv.Items.Count)
                        {
                            if (!ColumnItemState.ContainsKey(col + "-" + lv.Items[rowIndex].SubItems[col].Text))
                                ColumnItemState.Add(col + "-" + lv.Items[rowIndex].SubItems[col].Text, true);
                            rowIndex++;
                        }
                    }
                    conMenuColumnClick.Items.Clear();
                    BuildMenuItems();
                    conMenuColumnClick.Show(Cursor.Position);
                }
            }
        }

        private void BuildMenuItems()
        {
            List<ToolStripMenuItem> itemList = new List<ToolStripMenuItem>();
            IEnumerable<string> fullMatchingKeys =
            ColumnItemState.Keys.Where(currentKey => currentKey.StartsWith(conMenuColumnClick.Tag.ToString() + "-"));

            ToolStripMenuItem tsmiFISelectAll = new System.Windows.Forms.ToolStripMenuItem();
            tsmiFISelectAll.Text = "Select All";
            tsmiFISelectAll.Name = tsmiFISelectAll.Text;
            tsmiFISelectAll.CheckState = System.Windows.Forms.CheckState.Checked;
            tsmiFISelectAll.Click += tsmiFISelectAll_Click;
            conMenuColumnClick.Items.Add(tsmiFISelectAll);
            
            foreach (string currentKey in fullMatchingKeys)
            {
                ToolStripMenuItem newItem = new System.Windows.Forms.ToolStripMenuItem();
                newItem.Checked = ColumnItemState[currentKey];
                if (ColumnItemState[currentKey])
                {
                    newItem.CheckState = System.Windows.Forms.CheckState.Checked;
                }
                else
                {
                    newItem.CheckState = System.Windows.Forms.CheckState.Unchecked;
                    if (tsmiFISelectAll.CheckState == System.Windows.Forms.CheckState.Checked)
                        tsmiFISelectAll.CheckState = System.Windows.Forms.CheckState.Unchecked;
                }

                newItem.Name = currentKey;
                newItem.Size = new System.Drawing.Size(152, 22);
                newItem.Text = newItem.Name.Remove(0, newItem.Name.Split('-')[0].Length + 1);
                newItem.Click += new System.EventHandler(this.contextMenuItem_Click);
                conMenuColumnClick.Items.Add(newItem);
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
                AddListViewItemsToAdd(ListView1, item.Text, column);
                ColumnItemState[item.Name] = true;
            }
            else
            {
                AddListViewItemsToRemove(ListView1, item.Text, column);
                ColumnItemState[item.Name] = false;
            }
            
        }

        private void AddListViewItemsToRemove(ListView listView, string item, int column)
        {
            ListView lv = listView;
            string itemVal = item;
            int col = column;
            string key = col.ToString() + "-" + itemVal;

            if (listView1ItemsToAdd.Contains(key))
                listView1ItemsToAdd.Remove(key);

            listView1ItemsToRemove.Add(key);
        }

        private void AddListViewItemsToAdd(ListView listView, string item, int column)
        {
            ListView lv = listView;
            string itemVal = item;
            int col = column;
            string key = col.ToString() + "-" + itemVal;

            if (listView1ItemsToRemove.Contains(key))
                listView1ItemsToRemove.Remove(key);

            listView1ItemsToAdd.Add(key);
        }

        private void tsmiFISelectAll_Click(object sender, EventArgs e)
        {
            IEnumerable<string> fullMatchingKeys =
            ColumnItemState.Keys.Where(currentKey => currentKey.StartsWith(conMenuColumnClick.Tag.ToString() + "-"));

            
            ((ToolStripMenuItem)sender).Checked ^= true;
            foreach (ToolStripMenuItem i in conMenuColumnClick.Items)
            {
                if (i != ((ToolStripMenuItem)sender))
                {
                    if (i.Checked != ((ToolStripMenuItem)sender).Checked)
                    {
                        if (i.Checked)
                        {
                            AddListViewItemsToRemove(ListView1, i.Text, Convert.ToInt32(conMenuColumnClick.Tag));
                            ColumnItemState[i.Name] = false;                           
                        }
                        else
                        {
                            AddListViewItemsToAdd(ListView1, i.Text, Convert.ToInt32(conMenuColumnClick.Tag));
                            ColumnItemState[i.Name] = true;
                        }
                        i.Checked ^= true;//= CheckState.Checked;
                    }
                }
            }
            

        }

        private void conMenuColumnClick_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
            {
                e.Cancel = true;
            }
        }

        private void conMenuColumnClick_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            List<string> CurrentlyVisisbleItems = new List<string>();

            foreach (KeyValuePair<string, bool> kvp in ColumnItemState)
            {
                if (kvp.Value == true)
                    CurrentlyVisisbleItems.Add(kvp.Key);
            }

            foreach (string s in listView1ItemsToAdd)
            {
                if(listView1RemovedRows.ContainsKey(s))
                {
                    foreach (ListViewItem i in listView1RemovedRows[s])
                    {
                        listView1.Items.Add(i);
                    }
                    listView1RemovedRows.Remove(s);
                }
            }

            if (CurrentlyVisisbleItems.Count > 0)
            {
                foreach (string s in listView1ItemsToRemove)
                {
                    ListView lv = listView1;
                    string itemVal = s.Split('-')[1];
                    int col = Convert.ToInt32(conMenuColumnClick.Tag);
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
                    if (!listView1RemovedRows.ContainsKey(s))
                        listView1RemovedRows.Add(s, itemCopies);
                }

                if (listView1ItemsToRemove.Count != 0)
                    ((ToolStripMenuItem)conMenuColumnClick.Items[0]).Checked = false;
            }
            else
            {
                foreach (string s in listView1ItemsToRemove)
                {
                    foreach (ToolStripMenuItem i in conMenuColumnClick.Items)
                    {
                        if (i.Name == s)
                            i.Checked = true;
                    }
                    ColumnItemState[s] = true;
                }
            }


            listView1ItemsToRemove.Clear();
            listView1ItemsToAdd.Clear();

        }

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender != listView1) return;

            if (e.Control && e.KeyCode == Keys.C)
                CopySelectedValuesToClipboard();
        }

        private void CopySelectedValuesToClipboard()
        {
            var builder = new StringBuilder();
            string line = string.Empty;

            foreach (ListViewItem item in listView1.SelectedItems)
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
