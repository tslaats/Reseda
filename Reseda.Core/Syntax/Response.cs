﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class Response : BasicRelation
    {
        public Response(PathExpression s, PathExpression t) : base(s, t)
        {
        }


        public override String Symbol
        {
            get
            {
                return "*-->";
            }
        }


        internal override Relation Clone()
        {
            return new Response(source.Clone(), target.Clone());
        }

    }
}
