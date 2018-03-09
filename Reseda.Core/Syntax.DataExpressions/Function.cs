using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class Function : DataExpression
    {
        private String name;
        private List<DataExpression> arguments;
        
        public Function(string name)
        {
            this.name = name;
            arguments = new List<DataExpression>();
        }

        public void AddArgument(DataExpression e)
        {
            arguments.Add(e);
        }

        public override DataType Eval(Event context)
        {
            switch (name)
            {
                case "freshid":
                     int max = -1;
                     var events = context.Root().Descendants();
                     foreach (Event e in events)
                        if (e.marking.value != null && e.marking.value.GetType() == typeof(IntType))
                            max = Math.Max(max, ((IntType)e.marking.value).value);
                    return new IntType(max + 1);
                case "count":
                    var a1 = arguments[0].Eval(context);
                    if (!(a1.GetType() == typeof(EventSet)))
                        throw new Exception("Value of not an event set.");
                    var arg1 = (EventSet)a1;
                    return new IntType(arg1.value.Count);
                default:
                    throw new NotImplementedException(name);
            }               

            throw new Exception("bla");
            DataExpression c = null;
            //var c = child.Eval(context);            
            if (!(c.GetType() == typeof(EventSet)))
                throw new Exception("Value of not an event set.");
            EventSet s = (EventSet)c;

            if (s.value.Count > 1) throw new Exception("Value of multiple events.");

            if (s.value.ElementAt(0).marking.value == null)
                return new Unit();
            else
                return s.value.ElementAt(0).marking.value;
        }

        public override string ToSource()
        {
            string res = name + "(";
            if (arguments.Count > 0)
                res += arguments[0].ToSource();
            res += ")";
            return res;
        }


        internal override bool ContainsNames(ISet<string> set)
        {
            foreach (var a in arguments)
                if (a.ContainsNames(set)) return true;            
            return false;            
        }

        internal override void PathReplace(string iteratorName, Event e)
        {
            foreach (var a in arguments)
                a.PathReplace(iteratorName, e);
        }

        internal override DataExpression Clone()
        {
            var result = new Function(name);
            foreach (var a in arguments)
                result.AddArgument(a);
            return result;
            //return new ValueOf(this.child.Clone());
        }
    }
}
