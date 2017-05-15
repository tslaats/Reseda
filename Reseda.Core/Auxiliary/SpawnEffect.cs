using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class SpawnEffect
    {
        public Process process;
        public Event context;

        public SpawnEffect(Process p, Event c)
        {
            this.process = p.ShallowClone();
            //this.process = p;
            this.context = c;
        }
    }
}
