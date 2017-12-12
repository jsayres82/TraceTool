using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using TraceFileReader.TraceTypes;


namespace TraceFileReader.PlotTypes
{
    public class DataPlotBase
    {        
        /// <summary>
        /// Gets or sets the key of the axis. This can be used to specify an plot item if you have defined multiple series/axis in a plot. The default value is <c>null</c>.
        /// </summary>
        public string Key { get; set; }
        public AxisPosition AxisPosition { get; set; }
        public Axis YAxis { get; set; }
        public Series dataSeries { get; set; }

        public DataPlotBase()
        {

        }

        public void PopulataDataSeries(List<Trace> tL)
        {
            
        }

        public PlotModel AddDataPlot(PlotModel pM)
        {

            return pM;
        }

    }
}
