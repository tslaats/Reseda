using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public abstract class PathExpression
    {        
        public override String ToString()
        {
            if (child == null)
                return Symbol + FilterToString();
            else
                return Symbol + FilterToString() + "/" + child.ToString();
        }

        private string FilterToString()
        {
            if (this.filter == null)
                return "";
            else
                return "[" + filter.ToSource() + "]";
        }

        public String ToSource()
        {
            return this.ToString();
        }

        PathExpression child;
        DataExpression filter;

        //Saet<Event> eval(Event context, Process Root);

        public PathExpression Extend(PathExpression p)
        {
            this.child = p;
            return this;
        }


        PathExpression Next
        {
            get
            {
                return child;
            }
        }

        public ISet<Event> Eval(Event context)
        {
            return Eval(context, context.Root());
        }

        ISet<Event> ApplyFilter(ISet<Event> s, Event context)
        {
            if (this.filter == null)
                return s;

            var result = new HashSet<Event>();
            var val = this.filter.Eval(context);
            if (val.GetType() == typeof(IntType))
            {
                IntType v = (IntType)val;
                if (s.Count > v.value)
                    result.Add(s.ElementAt(v.value));
                return result;
            }

            throw new Exception("Bad filter type: " + val.GetType());
        }

        public ISet<Event> Eval(Event context, Event root)
        {
            //System.out.println("Checking '" + this.ToString() + "' in context: " + context.location());
            if (Next== null)
            {
                //System.out.println("1");
                return ApplyFilter(Current(context, root),context);
            }
            else
            {
                //System.out.println("2");
                //System.out.println(current(context,root));
                HashSet<Event> result = new HashSet<Event>();
                foreach (Event e in ApplyFilter(Current(context, root), context))
                {
                    result.UnionWith(Next.Eval(e, root));
                }
                return result;
            }
        }


        public PathExpression()
        {            
            this.child = null;
        }



        public PathExpression(PathExpression child)
        {         
            this.child = child;
        }

        public PathExpression(PathExpression child, DataExpression filter)
        {            
            this.child = child;
            this.filter = filter;
        }


        public abstract ISet<Event> Current(Event context, Event root);

        public abstract String Symbol { get; }

        public void AddFilter(DataExpression dataExpression)
        {
            this.filter = dataExpression;
        }
    }
}
