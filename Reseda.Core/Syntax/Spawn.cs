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

        public Spawn(PathExpression pathExpression, Process process)
        {
            source = pathExpression;
            target = process;
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
    }
}
