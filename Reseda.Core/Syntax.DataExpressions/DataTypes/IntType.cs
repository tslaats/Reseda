﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core.Syntax.DataExpressions
{
    public class IntType : DataType
    {
        public int value;
        public IntType(int v)
        {
            value = v;
        }        
    }
}
