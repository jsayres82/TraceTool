using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using TraceFileReader.TraceTypes;
using TraceFileReader.EnumTypes;

namespace TraceFileReader
{
    public partial class PopupForm : Form
    {
        private const string positionInchesAxis = "Position (Inches)";
        private const string sensorValueInchesAxis = "Value";
        private const string timeSecondsAxis = "Time (Seconds)";
        private const string motorString = "Motor";
        private const string sensorString = "Sensor";

        public System.Windows.Forms.Form MyParent;
        public TraceFileReaderUI ParentF1;

        public List<string> gbSelectChoices = new List<string>();
        public List<string> gbSelectNameChoices = new List<string>();
        public Dictionary<string, List<string>> categoryItemChoices = new Dictionary<string, List<string>>();

        private Dictionary<string, List<Trace>> tD = new Dictionary<string, List<Trace>>();
        public Dictionary<string, List<Trace>> TD { get { return tD; } set { tD = value; } }

        public Dictionary<CheckBox, List<string>> plottedItems = new Dictionary<CheckBox, List<string>>();

        private Dictionary<CheckBox, LineSeries> dataSeriesList = new Dictionary<CheckBox, LineSeries>();
        private Dictionary<string, LinearAxis> verticalAxes = new Dictionary<string, LinearAxis>();
        private Dictionary<string, LinearAxis> horizonatlAxes = new Dictionary<string, LinearAxis>();

        private PlotModel model = new PlotModel() { Title = "Trace Data", LegendSymbolLength = 24 };

        public PopupForm(TraceFileReaderUI parent)
        {
            InitializeComponent();

            TraceFileReaderUI f1 = (TraceFileReaderUI)this.MyParent;
            ParentF1 = parent;

            gbSelectChoices.Add(motorString);
            gbSelectChoices.Add(sensorString);

            foreach (string s in gbSelectChoices)
            {
                cbDataCategory.Items.Add(s);
                categoryItemChoices.Add(s, new List<string>());
            }

            foreach (KeyValuePair<string, List<MotorTrace>> kvp in TraceFileData.MotorTraceDic)
            {
                categoryItemChoices[motorString].Add(kvp.Key);
                tD.Add(kvp.Key, new List<Trace>());

                foreach (MotorTrace mT in kvp.Value)
                {
                    tD[kvp.Key].Add(mT);
                }
            }


            foreach (KeyValuePair<string, List<SensorTrace>> kvp in TraceFileData.SensorTraceDic)
            {
                categoryItemChoices[sensorString].Add(kvp.Key);
                tD.Add(kvp.Key, new List<Trace>());

                foreach (SensorTrace sT in kvp.Value)
                {
                    tD[kvp.Key].Add(sT);
                }
            }

            verticalAxes.Add(sensorValueInchesAxis, new LinearAxis { Key = sensorValueInchesAxis, Position = AxisPosition.Right, Title = "Value", Unit = "" });
            verticalAxes.Add(positionInchesAxis, new LinearAxis { Key = positionInchesAxis, Position = AxisPosition.Left, Title = "Position", Unit = "Inches" });
            horizonatlAxes.Add(timeSecondsAxis, new LinearAxis { Key = timeSecondsAxis, Position = AxisPosition.Bottom, Title = "Time", Unit = "Seconds" });

            model.Axes.Add(horizonatlAxes[timeSecondsAxis]);
            plotView1.Model = model;
        }

        public static PlotModel PositionTier()
        {
            var plotModel1 = new PlotModel();
            var linearAxis1 = new LinearAxis { Maximum = 1, Minimum = -1, Title = "PositionTier=0" };
            plotModel1.Axes.Add(linearAxis1);
            var linearAxis2 = new LinearAxis
            {
                AxislineStyle = LineStyle.Solid,
                Maximum = 2,
                Minimum = -2,
                PositionTier = 1,
                Title = "PositionTier=1"
            };
            plotModel1.Axes.Add(linearAxis2);
            var linearAxis3 = new LinearAxis
            {
                Maximum = 1,
                Minimum = -1,
                Position = AxisPosition.Right,
                Title = "PositionTier=0"
            };
            plotModel1.Axes.Add(linearAxis3);
            var linearAxis4 = new LinearAxis
            {
                AxislineStyle = LineStyle.Solid,
                Maximum = 2,
                Minimum = -2,
                Position = AxisPosition.Right,
                PositionTier = 1,
                Title = "PositionTier=1"
            };
            plotModel1.Axes.Add(linearAxis4);
            var linearAxis5 = new LinearAxis
            {
                Maximum = 1,
                Minimum = -1,
                Position = AxisPosition.Top,
                Title = "PositionTier=0"
            };
            plotModel1.Axes.Add(linearAxis5);
            var linearAxis6 = new LinearAxis
            {
                AxislineStyle = LineStyle.Solid,
                Maximum = 2,
                Minimum = -2,
                Position = AxisPosition.Top,
                PositionTier = 1,
                Title = "PositionTier=1"
            };
            plotModel1.Axes.Add(linearAxis6);
            var linearAxis7 = new LinearAxis
            {
                AxislineStyle = LineStyle.Solid,
                Maximum = 10,
                Minimum = -10,
                Position = AxisPosition.Top,
                PositionTier = 2,
                Title = "PositionTier=2"
            };
            plotModel1.Axes.Add(linearAxis7);
            var linearAxis8 = new LinearAxis
            {
                Maximum = 1,
                Minimum = -1,
                Position = AxisPosition.Bottom,
                Title = "PositionTier=0"
            };
            plotModel1.Axes.Add(linearAxis8);
            var linearAxis9 = new LinearAxis
            {
                AxislineStyle = LineStyle.Solid,
                Maximum = 2,
                Minimum = -2,
                Position = AxisPosition.Bottom,
                PositionTier = 1,
                Title = "PositionTier=1"
            };
            plotModel1.Axes.Add(linearAxis9);
            var linearAxis10 = new LinearAxis
            {
                AxislineStyle = LineStyle.Solid,
                Maximum = 10,
                Minimum = -10,
                Position = AxisPosition.Bottom,
                PositionTier = 2,
                Title = "PositionTier=2"
            };
            plotModel1.Axes.Add(linearAxis10);
            return plotModel1;
        }

        private void cbDataCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            flpUnselectedCheckBoxItems.Controls.Clear();
            gbSelectChoices.Clear();
            gbSelectNameChoices.Clear();
            cbCategoryItems.Items.Clear();
            string category = cbDataCategory.SelectedItem.ToString();
            foreach (string s in categoryItemChoices[category])
            {
                cbCategoryItems.Items.Add(s);
            }
        }

        private void cbCategoryItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            flpUnselectedCheckBoxItems.Controls.Clear();
            gbSelectChoices.Clear();
            gbSelectNameChoices.Clear();
            string category = cbDataCategory.SelectedItem.ToString();

            if (category.Equals(motorString))
            {
                foreach (MotorTrace mT in tD[cbCategoryItems.SelectedItem.ToString()])
                {
                    if (mT.IsPlottable && !gbSelectChoices.Contains(mT.DataType.ToString()))
                    {
                        gbSelectChoices.Add(mT.DataType.ToString());
                        gbSelectNameChoices.Add(cbCategoryItems.SelectedItem.ToString() + ": " + mT.DataType.ToString());
                    }
                }
            }
            else if (category.Equals(sensorString))
            {
                foreach (SensorTrace sT in tD[cbCategoryItems.SelectedItem.ToString()])
                {
                    if (sT.IsPlottable && !gbSelectChoices.Contains(sT.SensorName))
                    {
                        gbSelectChoices.Add(sT.SensorName);
                        gbSelectNameChoices.Add(sT.SensorName);
                    }
                }
            }
            foreach (string c in gbSelectChoices)
            {
                System.Windows.Forms.CheckBox cB = new CheckBox();

                cB.AutoSize = true;
                cB.Name = c.ToString();
                cB.Text = gbSelectNameChoices[gbSelectChoices.IndexOf(c)];
                cB.UseVisualStyleBackColor = true;
                cB.CheckedChanged += new System.EventHandler(this.gbPlottableItemsCheckBox_CheckedChanged);

                flpUnselectedCheckBoxItems.Controls.Add(cB);
            }
        }

        private void gbPlottableItemsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cB = sender as CheckBox;
            MotorDataType dT;
            string sN = string.Empty;
            bool isMotor = false;

            if (System.Enum.TryParse<MotorDataType>(cB.Name, out dT))
                isMotor = true;
            else
                sN = cB.Name;

            if (cB.Checked)
            {
                if (!plottedItems.ContainsKey(cB))
                {
                    plottedItems.Add(cB, new List<string>());
                }
                else
                {
                    if (isMotor)
                        plottedItems[cB].Add(dT.ToString());
                    else
                        plottedItems[cB].Add(sN);
                }

                if (isMotor)
                    dataSeriesList.Add(cB, AddMotorDataSeries(dT));
                else
                    dataSeriesList.Add(cB, AddSensorDataSeries(sN));
                flowLayoutPanel2.Controls.Add(cB);
            }
            else
            {
                if (isMotor)
                {
                    if (cB.Text.Split(':')[0] == cbCategoryItems.SelectedItem.ToString())
                        flpUnselectedCheckBoxItems.Controls.Add(cB);
                    else
                        flowLayoutPanel2.Controls.Remove(cB);

                    plottedItems[cB].Remove(dT.ToString());
                }
                else
                {
                    if (cB.Text == cbCategoryItems.SelectedItem.ToString())
                        flpUnselectedCheckBoxItems.Controls.Add(cB);
                    else
                        flowLayoutPanel2.Controls.Remove(cB);

                    plottedItems[cB].Remove(sN);
                }

                model.Series.Remove(dataSeriesList[cB]);
                dataSeriesList.Remove(cB);

                plotView1.Model = model;

                foreach (var ax in plotView1.Model.Axes)
                    ax.Maximum = ax.Minimum = Double.NaN;
                plotView1.InvalidatePlot(true);
                plotView1.Model.ResetAllAxes();
            }
        }

        public LineSeries AddMotorDataSeries(MotorDataType type)
        {
            double maxY = double.MinValue;
            double minY = double.MaxValue;


            if (!plotView1.Model.Axes.Contains(verticalAxes[positionInchesAxis]))
                plotView1.Model.Axes.Add(verticalAxes[positionInchesAxis]);

            var s1 = new LineSeries()
            {
                Title = cbCategoryItems.SelectedItem.ToString() + ": " + type.ToString(),
                YAxisKey = positionInchesAxis,
                MarkerType = MarkerType.Circle,
                MarkerSize = 6,
                MarkerStroke = OxyColors.White,
                MarkerStrokeThickness = 1.5
            };
            Dictionary<double,double> dP = new Dictionary<double,double>();
            foreach (MotorTrace mT in tD[cbCategoryItems.SelectedItem.ToString()])
            {
                if (mT.DataType == type)
                {
                    dP.Add(Convert.ToDouble(mT.OffSetTime) / 1000, mT.YValue);
                    //s1.Points.Add(new DataPoint(Convert.ToDouble(mT.OffSetTime) / 1000, mT.YValue));
                    if (mT.YValue >= maxY)
                        maxY = mT.YValue;
                    if (mT.YValue <= minY)
                        minY = mT.YValue;
                }
            }
            var pList = dP.ToList();
            pList.Sort((pair1, pair2) => pair1.Key.CompareTo(pair2.Key));
            foreach(KeyValuePair<double,double> kvp in pList)
                s1.Points.Add(new DataPoint(kvp.Key, kvp.Value));
            if (model.Series.Count > 0)
            {
                if (minY > verticalAxes[positionInchesAxis].ActualMaximum || maxY < verticalAxes[positionInchesAxis].ActualMinimum)
                {
                    LinearAxis a = new LinearAxis() { Key = (verticalAxes.Count + 1).ToString(), PositionTier = verticalAxes.Count};
                    verticalAxes.Add((verticalAxes.Count + 1).ToString(), a);
                    plotView1.Model.Axes.Add(a);
                    s1.YAxisKey = a.Key;
                }
            }
            model.Series.Add(s1);
            plotView1.Model = model;

            plotView1.InvalidatePlot(true);
            foreach (var ax in plotView1.Model.Axes)
                ax.Maximum = ax.Minimum = Double.NaN;

            plotView1.Model.ResetAllAxes();
            return s1;
        }

        public LineSeries AddSensorDataSeries(string name)
        {

            if (!plotView1.Model.Axes.Contains(verticalAxes[sensorValueInchesAxis]))
                plotView1.Model.Axes.Add(verticalAxes[sensorValueInchesAxis]);

            var s1 = new LineSeries()
            {
                Title = cbCategoryItems.SelectedItem.ToString(),
                YAxisKey = sensorValueInchesAxis,
                MarkerType = MarkerType.Circle,
                MarkerSize = 6,
                MarkerStroke = OxyColors.White,
                MarkerStrokeThickness = 1.5
            };
            Dictionary<double, double> dP = new Dictionary<double, double>();
            foreach (SensorTrace sT in tD[cbCategoryItems.SelectedItem.ToString()])
            {
                if (sT.SensorName == name)
                {
                    dP.Add(Convert.ToDouble(sT.OffSetTime) / 1000, sT.YValue);
                    //s1.Points.Add(new DataPoint(Convert.ToDouble(sT.OffSetTime) / 1000, sT.YValue));
                }
            }
            var pList = dP.ToList();
            pList.Sort((pair1, pair2) => pair1.Key.CompareTo(pair2.Key));
            foreach (KeyValuePair<double, double> kvp in pList)
                s1.Points.Add(new DataPoint(kvp.Key, kvp.Value));
            model.Series.Add(s1);
            plotView1.Model = model;

            plotView1.InvalidatePlot(true);
            foreach (var ax in plotView1.Model.Axes)
                ax.Maximum = ax.Minimum = Double.NaN;

            plotView1.Model.ResetAllAxes();
            return s1;
        }

    }
}
