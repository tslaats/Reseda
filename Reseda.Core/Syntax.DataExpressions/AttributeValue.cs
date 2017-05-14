using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class AttributeValue : Attribute
    {
        public override DataType Eval(EventSet s)
        {
            if (s.value.Count > 0) throw new Exception("Value of multiple events.");
            return s.value.ElementAt(0).marking.value;
        }
    }
}
