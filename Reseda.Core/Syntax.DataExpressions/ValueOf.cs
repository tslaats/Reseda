using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class ValueOf : DataExpression
    {
        private DataExpression child;

        public ValueOf(DataExpression dataExpression)
        {
            this.child = dataExpression;
        }

        public override DataType Eval(Event context)
        {
            var c = child.Eval(context);
            if (!(c.GetType() == typeof(EventSet)))
                throw new Exception("Value of not an event set.");
            EventSet s = (EventSet)c;

            if (s.value.Count > 1) throw new Exception("Value of multiple events.");
            return s.value.ElementAt(0).marking.value;
        }
    }
}
