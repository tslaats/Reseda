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
        public Process context;

        public SpawnEffect(Process p, Process c)
        {
            //this.process = p.Copy();
            this.process = p;
            this.context = c;
        }
    }
}
