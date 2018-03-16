using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class Here : PathExpression
    {

        public override ISet<Event> EvalCurrentNode(Event context, Event root) {
            HashSet<Event> result = new HashSet<Event>();
            result.Add(context);
            return result;
        }


        public override String Symbol
        {
            get
            {
                return ".";
            }
        }


        internal override PathExpression Clone()
        {
            return ExtendClone(new Here().Extend(this.child));
        }
    }
}
