﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public abstract class BasicRelation : Relation
    {
        public PathExpression source;
        public PathExpression target;     
        
        public BasicRelation(PathExpression s, PathExpression t)
        {
            source = s;
            target = t;
        }

        public abstract String Symbol { get; }

        public override String ToSource()
        {
            return source.ToSource() + " " + Symbol + " " + target.ToSource();            
        }

        internal override void PathReplace(string iteratorName, Event e)
        {
            source = source.PathReplace(iteratorName, e);
            target = target.PathReplace(iteratorName, e);
        }
    }
}
