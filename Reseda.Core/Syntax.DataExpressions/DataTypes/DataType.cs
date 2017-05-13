using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core.Syntax.DataExpressions
{
    public abstract class DataType : DataExpression
    {
        public override DataType Eval()
        {
            return this;
        }
    }
}
