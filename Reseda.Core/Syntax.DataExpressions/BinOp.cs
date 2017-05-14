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
    }
}
