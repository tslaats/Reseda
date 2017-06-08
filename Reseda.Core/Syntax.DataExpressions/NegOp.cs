using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class NegOp : DataExpression
    {
        private DataExpression child;

        public NegOp(DataExpression dataExpression)
        {
            this.child = dataExpression;
        }

        //public override string Symbol => "!";

        public override DataType Eval(Event context)
        {
            var c = child.Eval(context);            

            var ct = c.GetType();
            
            if (ct == typeof(BoolType))
            {
                var cc = (BoolType)c;                
                return new BoolType(!cc.value);
            }            
            else
                throw new NotImplementedException("NegOp on non-boolean.");
        }

        public override string ToSource()
        {
            return Symbols.Neg + child.ToSource();
        }
    }
}
