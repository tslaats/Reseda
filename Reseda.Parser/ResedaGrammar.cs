using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Irony.Ast;

namespace Reseda.Parser
{
    public class ResedaGrammar : Grammar
    {
        public ResedaGrammar()
        {
            var number = new NumberLiteral("number");
            number.DefaultIntTypes = new TypeCode[] { TypeCode.Int32, TypeCode.Int64, NumberLiteral.TypeCodeBigInt };
            var identifier = new IdentifierTerminal("identifier");
            var comment = new CommentTerminal("comment", "#", "\n", "\r");
            base.NonGrammarTerminals.Add(comment);

            var Process = new NonTerminal("Process");
            var SubProcess = new NonTerminal("SubProcess");
            var SubProcess_opt = new NonTerminal("SubProcess_opt");
            var StructuredData = new NonTerminal("StructuredData");
            var Relations = new NonTerminal("Relations");
            var Relation = new NonTerminal("Relation");
            var Include = new NonTerminal("Include");
            var Exclude = new NonTerminal("Exclude");
            var Response = new NonTerminal("Response");
            var Condition = new NonTerminal("Condition");
            var Spawn = new NonTerminal("Spawn");
            var Event = new NonTerminal("Event");
            var InputEvent = new NonTerminal("InputEvent");
            var OutputEvent = new NonTerminal("OutputEvent");
            var InputEventP = new NonTerminal("InputEventP");
            var OutputEventP = new NonTerminal("OutputEventP");
            var Expression = new NonTerminal("Expression");
            var Path = new NonTerminal("Path");
            var PathRoot = new NonTerminal("PathRoot");
            var PathExpression = new NonTerminal("PathExpression");
            var PathExpressionCont = new NonTerminal("PathExpressionCont");

            var PathName = new NonTerminal("PathName");
            var PathLocal = new NonTerminal("PathLocal");
            var PathParent = new NonTerminal("PathParent");
            var PathAll = new NonTerminal("PathAll");

            var Filter = new NonTerminal("Filter");
            Filter.Rule = (ToTerm("[") + Expression + ToTerm("]")) | Empty;
            PathName.Rule = identifier + Filter + PathExpressionCont;
            PathLocal.Rule = ToTerm(".") + Filter + PathExpressionCont;
            PathParent.Rule = ToTerm("..") + Filter + PathExpressionCont;
            PathAll.Rule = ToTerm("*") + Filter + PathExpressionCont;
            PathExpressionCont.Rule = (ToTerm("/") + PathExpression) | Empty;
            PathExpression.Rule = PathName | PathLocal | PathParent | PathAll;
            PathRoot.Rule = ToTerm("/") + PathExpression;
            Path.Rule = PathExpression | PathRoot;

            var Term = new NonTerminal("Term");
            var ParExpr = new NonTerminal("ParExpr");
            var BinExpr = new NonTerminal("BinExpr");
            var BinExpr2 = new NonTerminal("BinExpr2");
            var BinOp = new NonTerminal("BinOp");            
            var Unit = new KeyTerm("!", "Unit");

            var PlusOp = new NonTerminal("PlusOp");
            var MinOp = new NonTerminal("MinOp");
            var TimesOp = new NonTerminal("TimesOp");
            var DivOp = new NonTerminal("DivOp");
            var DPath = new NonTerminal("DPath");
            
            var DPathValue = new NonTerminal("DPathValue");
            var DDPath = new NonTerminal("DDPath");


            DDPath.Rule = DPathValue | DPath;
            DPathValue.Rule = DPath + ToTerm(":v");

            DPath.Rule = ToTerm("@") + Path;
            Expression.Rule = Term | BinExpr;
            Term.Rule = number | ParExpr | identifier | Unit | DDPath;
            ParExpr.Rule = "(" + Expression + ")";
            BinExpr.Rule = PlusOp | MinOp | BinExpr2;
            PlusOp.Rule = BinExpr2 + ToTerm("+") + BinExpr2;
            MinOp.Rule = BinExpr2 + ToTerm("-") + BinExpr2;

            BinExpr2.Rule = TimesOp | DivOp | Expression;
            TimesOp.Rule = Expression + ToTerm("*") + Expression;
            DivOp.Rule = Expression + ToTerm("/") + Expression;

            Include.Rule = Path + ToTerm("-->+") + Path;
            Exclude.Rule = Path + ToTerm("-->%") + Path;
            Spawn.Rule = Path + ToTerm("-->>") + ToTerm("{") + Process + ToTerm("}");
            Condition.Rule = Path + ToTerm("-->*") + Path;
            Response.Rule = Path + ToTerm("*-->") + Path;
            Relation.Rule = Include | Exclude | Response | Condition | Spawn;
            Relations.Rule = MakeListRule(Relations, ToTerm(","), Relation) | Empty;            
            InputEvent.Rule = identifier + ToTerm("<?>") +SubProcess;
            OutputEvent.Rule = identifier + ToTerm("<") + Expression + ToTerm(">") +SubProcess;
            SubProcess.Rule = (ToTerm("{") + Process + ToTerm("}")) | Empty;
            Event.Rule = InputEvent | OutputEvent;
            StructuredData.Rule = MakeListRule(StructuredData, ToTerm(","), Event) | Empty;            
            Process.Rule = StructuredData + ToTerm(";") + Relations;
            this.Root = Process;

            
            MarkPunctuation("-->+", ",", ";", "-->%", "-->*", "*-->",
                "-->>", "<", ">", "?", "!", "{", "}", "/", ".", "..", "*", "+", "-", "(", ")", "@", "[","]");

            MarkTransient(Relation, Event, SubProcess, PathExpressionCont,
                PathExpression, Path, BinExpr, Expression, Term, ParExpr, BinExpr2,
                DDPath);

        }
    }
}
