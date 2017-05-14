using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class PlusOp : BinOp
    {
        public PlusOp(DataExpression l, DataExpression r) : base(l, r)
        {
        }

        public override string Symbol => "+";

        public override DataType Eval(Event context)
        {
            var l = left.Eval(context);
            var r = right.Eval(context);

            var lt = l.GetType();
            var rt = r.GetType();

            if (rt == typeof(IntType) && lt == typeof(IntType))
            {
                var lc = (IntType)l;
                var rc = (IntType)r;
                return new IntType(lc.value + rc.value);
            }
            if (rt == typeof(EventSet) && lt == typeof(EventSet))
            {
                var lc = (EventSet)l;
                var rc = (EventSet)r;
                return new EventSet(lc.value.Union(rc.value));
            }
            if (rt != lt)
                throw new Exception("PlusOp Types mismatch");
            else
                throw new NotImplementedException("PlusOp incomplete: " + lt.ToString() + " - " + rt.ToString());
        }
    }
}
