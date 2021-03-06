﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class Root : PathExpression
    {
        public override String Symbol
        {
            get
            {
                return "";
            }
        }

        public override ISet<Event> EvalCurrentNode(Event context, Event root) {
            HashSet<Event> result = new HashSet<Event>();
            result.Add(root);
            return result;
        }


        internal override PathExpression Clone()
        {
            return ExtendClone(new Root().Extend(this.child));
        }


        public override String ToSource()
        {
            if (this.child == null)
                return "/";
            else
                return this.ToString();
        }


    }
}
