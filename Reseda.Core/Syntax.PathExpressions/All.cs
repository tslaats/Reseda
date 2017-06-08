using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class All : PathExpression
    {
        public override String Symbol
        {
            get
            {
                return "*";
            }
        }
        
        public override ISet<Event> EvalCurrentNode(Event context, Event root) {
            return new HashSet<Event>(context.subProcess.structuredData);
        }

        override internal bool ContainsNamesOrStar(ISet<string> set)
        {
            return false;
        }

    }
}
