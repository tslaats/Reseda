using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    abstract public class BinOp : DataExpression
    {
        public DataExpression left;
        public DataExpression right;

        public BinOp(DataExpression l, DataExpression r)
        {
            left = l;
            right = r;
        }

        public abstract String Symbol { get; }

        public override String ToSource()
        {
            return left.ToSource() + " " + Symbol + " " + right.ToSource();
        }


        internal override bool ContainsNames(ISet<string> set)
        {
            return left.ContainsNames(set) || right.ContainsNames(set);
        }

        internal override void PathReplace(string iteratorName, Event e)
        {
            this.left.PathReplace(iteratorName, e);
            this.right.PathReplace(iteratorName, e);
        }
    }
}
