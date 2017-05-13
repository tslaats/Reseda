using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core.Syntax.DataExpressions
{
    public class PlusOp : BinOp
    {
        public PlusOp(DataExpression l, DataExpression r) : base(l, r)
        {
        }

        public override DataType Eval()
        {
            var l = left.Eval();
            var r = right.Eval();

            var lt = l.GetType();
            var rt = r.GetType();

            if (rt == typeof(IntType) && lt == typeof(IntType))
            {
                var lc = (IntType)l;
                var rc = (IntType)r;
                return new IntType(lc.value + rc.value);
            }
            if (rt != lt)
                throw new Exception("DivOp Types mismatch");
            else
                throw new NotImplementedException("DivOp incomplete");
        }
    }
}
