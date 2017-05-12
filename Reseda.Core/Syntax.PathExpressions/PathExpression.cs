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
                return Symbol;
            else
                return Symbol+ "/" + child.ToString();
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

        public ISet<Event> Eval(Event context, Event root)
        {
            //System.out.println("Checking '" + this.ToString() + "' in context: " + context.location());
            if (Next== null)
            {
                //System.out.println("1");
                return Current(context, root);
            }
            else
            {
                //System.out.println("2");
                //System.out.println(current(context,root));
                HashSet<Event> result = new HashSet<Event>();
                foreach (Event e in Current(context, root))
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
    }
}
