﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core.Syntax.DataExpressions
{
    abstract public class DataExpression
    {
        public abstract DataType Eval();
    }
}
