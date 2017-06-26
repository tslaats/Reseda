using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class RootEvent : Event
    {
        public RootEvent()
        {
            this.name = "/";
        }

        public override void Execute()
        {
            throw new Exception("Don't execute a root event!");
        }

        public override string TypeToSource()
        {
            return subProcess.ToSource();
        }

        public override string ToSource()
        {
            return subProcess.ToSource();
        }

        internal override Event ShallowClone()
        {
            var result = new RootEvent();
            result.subProcess = this.subProcess.ShallowClone();
            return result;
        }
    }
}
