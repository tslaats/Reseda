﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class Inclusion : BasicRelation
    {
        public Inclusion(PathExpression s, PathExpression t) : base(s, t)
        {
        }


        public override String Symbol
        {
            get
            {
                return "-->+";
            }
        }

        internal override Relation Clone()
        {
            return new Inclusion(source.Clone(), target.Clone());
        }

    }
}
