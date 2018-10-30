using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class IsExecuted : DataExpression
    {
        private DataExpression child;

        public IsExecuted(DataExpression dataExpression)
        {
            this.child = dataExpression;
        }

        public override DataType Eval(Event context)
        {
            var c = child.Eval(context);
            if (!(c.GetType() == typeof(EventSet)))
                throw new Exception("IsExecuted of not an event set.");
            EventSet s = (EventSet)c;

            if (s.value.Count > 1) throw new Exception("IsExecuted of multiple events.");

            if (s.value.ElementAt(0).marking == null)
                return new Unit();
            else
                return new BoolType(s.value.ElementAt(0).marking.happened);
        }

        public override string ToSource()
        {
            return child.ToSource() + ":e";
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
            return new IsExecuted(this.child.Clone());
        }
    }
}
