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

        public override void Execute()
        {
            this.Execute(new Unit());
        }

        public void Execute(int i)
        {
            this.Execute(new IntType(i));
        }


        public void Execute(bool i)
        {
            this.Execute(new BoolType(i));
        }

        public void Execute(string i)
        {
            this.Execute(new StrType(i));
        }

        public override string TypeToSource()
        {
            return "[?]";
        }

        private void Execute(DataType d)
        {
            if (!this.IsEnabled())
            {
                throw new Exception("Trying to execute disabled event!");
            }
            this.marking.value = d;
            base.Execute();
        }

        public override Event Clone(Process parent)
        {
            return (Event)this.CloneInto(new InputEvent(this.name), parent);
        }
    }
}
