using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace ServiceTool.TraceTypes
{
    class StateMachineTrace : Trace
    {

        public StateMachineTrace(XPathNavigator trace, Form1 parentForm)
            : base(trace, parentForm)
        {
        }

    }
}
