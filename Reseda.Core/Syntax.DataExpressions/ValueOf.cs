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
            
            if (s.value.Count < 1) throw new Exception("Value of non-existent event.");

            if (s.value.Count > 1)
            {
                var result = new HashSet<DataType>();
                foreach (Event e in s.value)
                {
                    if (e.marking.value == null)
                        result.Add(new Unit());
                    else
                        result.Add(e.marking.value);
                }
                return new DataSet(result);
            }
            else
            {
                if (s.value.ElementAt(0).marking.value == null)
                    return new Unit();
                else
                    return s.value.ElementAt(0).marking.value;
            }
        }

        public override string ToSource()
        {
            return child.ToSource() + ":v";
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
            return new ValueOf(this.child.Clone());
        }
    }
}
