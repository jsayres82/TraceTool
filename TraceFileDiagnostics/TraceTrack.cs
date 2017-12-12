using System.Collections.Generic;
using TimeBeam;
using NorthStateFramework;
using TraceFileReader.TraceTypes;


namespace TraceFileReader
{
    public class TraceTrack : IMultiPartTimelineTrack
    {
        public IEnumerable<ITimelineTrack> TrackElements
        {
            get { return Parts; }
        }

        private List<TraceTrackItem> Parts = new List<TraceTrackItem>();

        public string Name { get; set; }

        //public TraceTrack( float distanceBetweenParts ) 
        //{
        //  for( int partIndex = 0; partIndex < 10; partIndex++ ) 
        //  {
        //    TraceTrackItem part = new TraceTrackItem 
        //    {
        //      Start = (20+ distanceBetweenParts) * partIndex ,
        //      Name = "Part " + partIndex 
        //    };
        //    part.End = part.Start + 1;
        //    Parts.Add( part );
        //  }
        //}

        public TraceTrack(List<Trace> traces, int startTime)
        {
            traces.Sort((a,b) => a.TraceTime.CompareTo(b.TraceTime));
            List<Trace>.Enumerator traceEnumerator;
            traceEnumerator = traces.GetEnumerator();
            traceEnumerator.MoveNext();

            foreach (Trace t in traces)
            {
                int index = 0;
                int scalarValue = t.TimeScalar;
                int concurrencyVal = t.ConcurrentValue;
                if (t.TagAndData.Count > 1)
                    index = 1;

                TraceTrackItem part = new TraceTrackItem
                {
                    Start = (t.TraceTime*scalarValue) + concurrencyVal - (startTime*scalarValue),
              
                    Name = t.TraceType + " " + t.getTagData(index)
                };

                part.Trace = t;
                part.End = part.Start + 1;

                if (t.TraceType.Equals(NSFTraceTags.stateEnteredTag))
                {
                    part.Name = t.getTagData(index);
                    if (traceEnumerator.MoveNext())
                        part.End = (traceEnumerator.Current.TraceTime * scalarValue) + concurrencyVal - (startTime * scalarValue);
                    else
                        part.End = part.Start + 10;
                }
                else if (t.TraceType.Equals("FillStatus"))
                {
                    FillStatusTrace fT = t as FillStatusTrace;
                    
                    part = fT.CreateTrackItem(part);

                    if (traceEnumerator.MoveNext())
                        part.End = (traceEnumerator.Current.TraceTime * scalarValue) + concurrencyVal - (startTime * scalarValue);
                    else
                        part.End = part.Start + 10;
                }
                else if (t.TraceType.Equals("FillRequest"))
                {
                    FillRequestTrace fR = t as FillRequestTrace;
                    part = fR.CreateTrackItem(part);

                    if (traceEnumerator.MoveNext())
                        part.End = (traceEnumerator.Current.TraceTime * scalarValue) + concurrencyVal - (startTime * scalarValue);
                    else
                        part.End = part.Start + 10;
                }

                Parts.Add(part);
            }
        }

        public override string ToString()
        {
            return string.Format("Name: {0}", Name);
        }
    }
}
