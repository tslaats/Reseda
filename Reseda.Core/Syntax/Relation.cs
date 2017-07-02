using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public abstract class Relation
    {
        public abstract String ToSource();
        internal abstract void PathReplace(string iteratorName, Event e);

        internal abstract Relation Clone();
    }
}
