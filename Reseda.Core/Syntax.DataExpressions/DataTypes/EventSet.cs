using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core.Syntax.DataExpressions
{
    public class EventSet : DataType
    {
        public HashSet<Event> value;
        public EventSet(HashSet<Event> v)
        {
            value = v;
        }
    }
}
