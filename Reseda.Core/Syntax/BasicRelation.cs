using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public abstract class BasicRelation : Relation
    {
        public PathExpression source;
        public PathExpression target;     
        
        public BasicRelation(PathExpression s, PathExpression t)
        {
            source = s;
            target = t;
        }
    }
}
