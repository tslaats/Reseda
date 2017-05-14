using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class SideEffects
    {
        public HashSet<Event> include = new HashSet<Event>();
        public HashSet<Event> exclude = new HashSet<Event>();
        public HashSet<Event> respond = new HashSet<Event>();
        public HashSet<Process> spawn = new HashSet<Process>();

    }
}
