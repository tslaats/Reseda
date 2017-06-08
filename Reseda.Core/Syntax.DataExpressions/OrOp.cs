using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class OrOp : BinOp
    {
        public OrOp(DataExpression l, DataExpression r) : base(l, r)
        {
        }

        public override string Symbol => "||";

        public override DataType Eval(Event context)
        {
            var l = left.Eval(context);
            var r = right.Eval(context);

            var lt = l.GetType();
            var rt = r.GetType();

            if (rt == typeof(BoolType) && lt == typeof(BoolType))
            {
                var lc = (BoolType)l;
                var rc = (BoolType)r;
                return new BoolType(lc.value || rc.value);
            }
            if (rt != lt)
                throw new Exception("OrOp Types mismatch");
            else
                throw new NotImplementedException("OrOp incomplete");
        }
    }
}
