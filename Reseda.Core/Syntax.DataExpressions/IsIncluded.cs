using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class IsIncluded : DataExpression
    {
        private DataExpression child;

        public IsIncluded(DataExpression dataExpression)
        {
            this.child = dataExpression;
        }

        public override DataType Eval(Event context)
        {
            var c = child.Eval(context);
            if (!(c.GetType() == typeof(EventSet)))
                throw new Exception("IsIncluded of not an event set.");
            EventSet s = (EventSet)c;

            if (s.value.Count > 1) throw new Exception("IsIncluded of multiple events.");

            if (s.value.ElementAt(0).marking.value == null)
                return new Unit();
            else
                return new BoolType(s.value.ElementAt(0).marking.included);
        }

        public override string ToSource()
        {
            return child.ToSource() + ":i";
        }


        internal override bool ContainsNames(ISet<string> set)
        {
            return child.ContainsNames(set);
        }

        internal override void PathReplace(string iteratorName, Event e)
        {
            this.child.PathReplace(iteratorName, e);
        }

        internal override DataExpression Clone()
        {
            return new IsIncluded(this.child.Clone());
        }
    }
}
