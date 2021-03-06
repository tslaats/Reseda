﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class OutputEvent : Event
    {
        public DataExpression expression;

        public OutputEvent(String name, DataExpression expression)
        {            
            this.name = name;
            this.expression = expression;
        }

        public OutputEvent(String name)
        {            
            this.name = name;
            this.expression = new Unit();
        }

        public OutputEvent(String name, Marking m) : base(m)
        {
            this.name = name;
            this.expression = new Unit();
        }

        public override Event Clone(Process parent) 
        {
            var other = new OutputEvent(this.name, this.expression.Clone());
            return this.CloneInto(other, parent);
        }

        public override void Execute()
        {
            if (!this.IsEnabled())
            {
                throw new Exception("Trying to execute disabled event!");
            }
            this.marking.value = this.Compute(this.parentProcess.parent);
            base.Execute();
        }


        private DataType Compute(Event e)
        {
            return this.expression.Eval(e);                
        }

        public override string TypeToSource()
        {
            return "[" + expression.ToSource() + "]";
        }

        internal override void PathReplace(string iteratorName, Event e)
        {
            this.expression.PathReplace(iteratorName, e);
            base.PathReplace(iteratorName, e);            
        }
    }
}
