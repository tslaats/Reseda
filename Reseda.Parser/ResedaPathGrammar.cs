using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Irony.Ast;

namespace Reseda.Parser
{
    public class ResedaPathGrammar : ResedaGrammar
    {
        public ResedaPathGrammar() : base()
        {
            this.Root = this.path;
        }
    }
}
