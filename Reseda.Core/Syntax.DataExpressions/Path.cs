using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core.Syntax.DataExpressions
{
    public class Path : DataExpression
    {
        public PathExpression value;        

        public Path(PathExpression v)
        {
            this.value = v;
        }
    }
}
