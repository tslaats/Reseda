using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class GtOp : BinOp
    {
        public GtOp(DataExpression l, DataExpression r) : base(l, r)
        {
        }

        public override string Symbol => ">";

        public override DataType Eval(Event context)
        {
            var l = left.Eval(context);
            var r = right.Eval(context);

            var lt = l.GetType();
            var rt = r.GetType();

            // note: I could get around having a switch for each type if I add a valuetype parent.
            if (rt == typeof(BoolType) && lt == typeof(BoolType))
            {
                throw new Exception("GtOp on booleans.");
            }
            else if (rt == typeof(IntType) && lt == typeof(IntType))
            {
                var lc = (IntType)l;
                var rc = (IntType)r;
                return new BoolType(lc.value > rc.value);
            }
            if (rt != lt)
                throw new Exception("GtOp Types mismatch: " + lt.ToString() + " - " + rt.ToString());
            else
                throw new NotImplementedException("GtOp incomplete: " + lt.ToString() + " - " + rt.ToString());
        }
    }
}
