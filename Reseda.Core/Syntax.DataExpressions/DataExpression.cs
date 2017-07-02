using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    abstract public class DataExpression
    {
        public abstract DataType Eval(Event context);

        public abstract String ToSource();

        internal abstract bool ContainsNames(ISet<string> set);

        internal virtual void PathReplace(string iteratorName, Event e)
        {            
        }
    }
}
