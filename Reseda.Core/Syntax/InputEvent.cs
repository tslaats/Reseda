using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class InputEvent : Event
    {
        public InputEvent(String name)
        {
            this.name = name;
        }

        public InputEvent(String name, Marking m): base(m)
        {
            this.name = name;
        }
    }
}
