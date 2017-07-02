using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class Parent : PathExpression
    {
        public override String Symbol
        {
            get
            {
                return "..";
            }
        }

        public override ISet<Event> EvalCurrentNode(Event context, Event root) {
            HashSet<Event> result = new HashSet<Event>();
            result.Add(context.parentProcess.parent);
            return result;
        }

        internal override PathExpression Clone()
        {
            return new Parent().Extend(this.child);
        }
    }
}
