using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class IntType : DataType
    {
        public int value;
        public IntType(int v)
        {
            value = v;
        }

        public override bool Equals(Object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                IntType i = (IntType)obj;
                return (i.value == this.value);
            }
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }    

        public override string ToString()
        {
            return this.value.ToString();
        }

        public override string ToSource()
        {
            return value.ToString();
        }

        internal override DataExpression Clone()
        {
            return new IntType(this.value);
        }
    }
}
