using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class Spawn : Relation
    {
        public PathExpression source;
        public Process target;
        public PathExpression iterateOver;
        public String iteratorName;

        public Spawn(PathExpression pathExpression, Process process)
        {
            source = pathExpression;
            target = process;
        }

        public Spawn(PathExpression pathExpression, String name, PathExpression iterate, Process process)
        {
            source = pathExpression;
            target = process;
            iteratorName = name;
            iterateOver = iterate;
        }


        public String Symbol
        {
            get
            {
                return "-->>";
            }
        }

        public override string ToSource()
        {
            return source.ToSource() + " " + Symbol + " {" + target.ToSource() + "}";
        }


        internal override void PathReplace(string iteratorName, Event e)
        {
            source = source.PathReplace(iteratorName, e);
            target.PathReplace(iteratorName, e);
        }


        internal override Relation Clone()
        {
            if (iterateOver != null)
                return new Spawn(source.Clone(), iteratorName, iterateOver.Clone(), target.Clone(target.parent));
            else
                return new Spawn(source.Clone(), target.Clone(target.parent));
        }
    }
}
