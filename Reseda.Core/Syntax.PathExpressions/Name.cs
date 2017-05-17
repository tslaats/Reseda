using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class Name : PathExpression
    {
        public Name(String name) :base()
        {            
            this.name = name;
        }


        public Name(String name, PathExpression child) : base(child)
        {            
            this.name = name;
        }

        public Name(String name, PathExpression child, DataExpression filter) : base (child, filter)
        {            
            this.name = name;
        }


        public override String Symbol
        {
            get
            {
                return this.name;
            }
        }

        public String name;
        
        public override ISet<Event> EvalCurrentNode(Event context, Event root) {
            HashSet<Event> result = new HashSet<Event>(context.subProcess.structuredData);
            result.RemoveWhere(p => p.name != name);
            return result;
        }

    }
}
