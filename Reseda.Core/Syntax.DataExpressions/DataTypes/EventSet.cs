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
        private IEnumerable<Event> enumerable;

        public EventSet(HashSet<Event> v)
        {
            value = v;
        }

        public EventSet(IEnumerable<Event> s)
        {
            this.value = new HashSet<Event>(s);
        }
    }
}
