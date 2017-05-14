using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class Marking
    {
        public Boolean happened;
        public Boolean included;
        public Boolean pending;
        public DataType value;

        public Marking()
        {
            happened = false;
            included = true;
            pending = false;
        }

        public Marking(Boolean h, Boolean i, Boolean p)
        {
            happened = h;
            included = i;
            pending = p;
        }
    }
}
