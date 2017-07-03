using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class AndOp : BinOp
    {
        public AndOp(DataExpression l, DataExpression r) : base(l, r)
        {
        }

        public override string Symbol => Symbols.And;

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
                return new BoolType(lc.value && rc.value);
            }
            if (rt != lt)
                throw new Exception("AndOp Types mismatch");
            else
                throw new NotImplementedException("AndOp incomplete");
        }

        //internal override DataExpression Clone()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
