using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class Path : DataExpression
    {
        public PathExpression value;        

        public Path(PathExpression v)
        {
            this.value = v;
        }

        public override DataType Eval(Event context)
        {
            var result = value.Eval(context);

            return new EventSet(result);
        }

        public override string ToSource()
        {
            return "@" + value.ToSource();
        }


        internal override bool ContainsNames(ISet<string> set)
        {
            return value.ContainsNames(set);
        }
    }
}
