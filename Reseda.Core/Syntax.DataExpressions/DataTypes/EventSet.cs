using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class EventSet : DataType
    {
        public HashSet<Event> value;
        //private IEnumerable<Event> enumerable;

        public EventSet(HashSet<Event> v)
        {
            value = v;
        }

        public EventSet(IEnumerable<Event> s)
        {
            this.value = new HashSet<Event>(s);
        }

        public override string ToSource()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            if (value.Count == 0)
                return "{}";

            String result = "{";
            ;
            foreach (var e in value)
                result += e.ToString() + ",";
            return result.Substring(0,result.Length - 1) + "}";
        }

        internal override DataExpression Clone()
        {
            return new EventSet(this.value);
        }
    }
}
