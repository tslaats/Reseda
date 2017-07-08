using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class Name : PathExpression
    {
        public Name(String name) :base()
        {            
            this.name = name;
        }


        public Name(String name, PathExpression child) : base(child)
        {            
            this.name = name;
        }

        public Name(String name, PathExpression child, DataExpression filter) : base (child, filter)
        {            
            this.name = name;
        }


        public override String Symbol
        {
            get
            {
                return this.name;
            }
        }

        public String name;
        
        public override ISet<Event> EvalCurrentNode(Event context, Event root) {
            HashSet<Event> result = new HashSet<Event>(context.subProcess.structuredData);
            result.RemoveWhere(p => p.name != name);
            return result;
        }


        override internal bool ContainsNamesOrStar(ISet<string> set)
        {
            if (set.Contains(name))
            {
                return true;
            }
            else
                return base.ContainsNamesOrStar(set);
        }


        override internal bool ContainsNames(ISet<string> set)
        {
            if (set.Contains(name))
            {
                return true;
            }
            else
                return base.ContainsNames(set);
        }


        internal override PathExpression PathReplace(string iteratorName, Event e)
        {
            if (this.name == iteratorName)
            {
                //System.Diagnostics.Debug.WriteLine(e.Path.ToSource());
                if (this.child != null)
                {
                    
                    //System.Diagnostics.Debug.WriteLine(e.Path.Extend(this.child.Clone()).ToSource());
                    //var nc = this.child.Clone();
                    //var p = e.Path.Clone();
                    //System.Diagnostics.Debug.WriteLine(p);
                    //System.Diagnostics.Debug.WriteLine(nc.ToSource());
                    //p.child = nc;
                    //System.Diagnostics.Debug.WriteLine(p);
                    return e.Path.ExtendFinal(this.child);
                }
                else
                    return e.Path;//.Extend(this.child);
            }
            else
            {
                if (child != null)
                    child.PathReplace(iteratorName, e);

                System.Diagnostics.Debug.WriteLine(this);
                return this;
            }
        }

        internal override PathExpression Clone()
        {
            return new Name(name).Extend(this.child);
        }


    }
}
