using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeBeam;
using ServiceTool.TraceTypes;

namespace ServiceTool
{
    class TraceTrackItem : ITimelineTrack
    {
        public float Start { get; set; }
        public float End { get; set; }
        public string Name { get; set; }
        public Trace Trace { get; set; }

        public override string ToString()
        {
            return string.Format("Name: {0}, End: {1}, Start: {2}", Name, End, Start);
        }
    }
}
