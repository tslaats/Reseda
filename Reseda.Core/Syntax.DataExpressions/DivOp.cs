using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class DivOp : BinOp
    {
        public DivOp(DataExpression l, DataExpression r) : base(l, r)
        {
        }

        public override string Symbol => "/";

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
                return new IntType(lc.value / rc.value);
            }
            if (rt != lt)
                throw new Exception("DivOp Types mismatch");            
            else
                throw new NotImplementedException("DivOp incomplete");
        }
    }
}
