using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class StrType : DataType
    {
        public string value;
        public StrType(string v)
        {
            value = v;
        }

        public override string ToString()
        {
            return this.value.ToString();
        }

        public override string ToSource()
        {
            return value.ToString();
        }
    }
}
