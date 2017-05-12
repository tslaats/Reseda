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
            // 1. Terminals
            var number = new NumberLiteral("number");
            //Let's allow big integers (with unlimited number of digits):
            number.DefaultIntTypes = new TypeCode[] { TypeCode.Int32, TypeCode.Int64, NumberLiteral.TypeCodeBigInt };
            var identifier = new IdentifierTerminal("identifier");
            var comment = new CommentTerminal("comment", "#", "\n", "\r");
            //comment must to be added to NonGrammarTerminals list; it is not used directly in grammar rules,
            // so we add it to this list to let Scanner know that it is also a valid terminal. 
            base.NonGrammarTerminals.Add(comment);

            // 2. Non-terminals
            var Process = new NonTerminal("Process");
            var SubProcess = new NonTerminal("SubProcess");
            var SubProcess_opt = new NonTerminal("SubProcess_opt");
            var StructuredData = new NonTerminal("StructuredData");
            var Relations = new NonTerminal("Relations");
            //var RelationsLine = new NonTerminal("RelationsLine");
            var Relation = new NonTerminal("Relation");
            var Include = new NonTerminal("Include");
            var Exclude = new NonTerminal("Exclude");
            var Response = new NonTerminal("Response");
            var Condition = new NonTerminal("Condition");
            var Spawn = new NonTerminal("Spawn");
            //var StructuredDataLine = new NonTerminal("StructuredDataLine");
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
            //var PathTerminal = new NonTerminal("PathTerminal");
            //var PathName = new NonTerminal("PathName");

            /*
            IdentifierTerminal PathName = new IdentifierTerminal("PathName");
            KeyTerm PathLocal = ToTerm(".", "PathLocal");
            KeyTerm PathParent = ToTerm("..", "PathParent");
            KeyTerm PathAll = ToTerm("*", "PathAll");
            */

            var PathName = new NonTerminal("PathName");
            var PathLocal = new NonTerminal("PathLocal");
            var PathParent = new NonTerminal("PathParent");
            var PathAll = new NonTerminal("PathAll");


            /*
            var PathLocal = new NonTerminal("PathLocal");
            var PathParent = new NonTerminal("PathParent");            
            var PathAll = new NonTerminal("PathAll");
            */
            //PathTerminal.Rule = PathName | PathLocal | PathParent | PathAll;
            //PathExpression.Rule = PathTerminal | (PathTerminal + "/" + PathExpression);
            //PathExpression.Rule = (PathExpression + "/" + PathExpression) | PathName | PathLocal | PathParent | PathAll;
            //PathExpression.Rule = identifier;

            PathName.Rule = identifier + PathExpressionCont;
            PathLocal.Rule = ToTerm(".") + PathExpressionCont;
            PathParent.Rule = ToTerm("..") + PathExpressionCont;
            PathAll.Rule = ToTerm("*") + PathExpressionCont;
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
            //var PlusOp = new KeyTerm("+","PlusOp");
            //var MinOp = new KeyTerm("-", "MinOp");
            //var TimesOp = new KeyTerm("*", "TimesOp");
            //var DivOp = new KeyTerm("/", "DivOp");
            

            var PlusOp = new NonTerminal("PlusOp");
            var MinOp = new NonTerminal("MinOp");
            var TimesOp = new NonTerminal("TimesOp");
            var DivOp = new NonTerminal("DivOp");


            //.Rule = ;
            //Unit.Rule = ToTerm("!");
            Expression.Rule = Term | BinExpr;
            Term.Rule = number | ParExpr | identifier | Unit | (ToTerm("@") + Path);
            ParExpr.Rule = "(" + Expression + ")";
            //BinExpr.Rule = Expression + BinOp + Expression;
            //BinOp.Rule = ToTerm("+") | "-" | "*" | "/" | "**";
            BinExpr.Rule = PlusOp | MinOp | BinExpr2;
            PlusOp.Rule = BinExpr2 + ToTerm("+") + BinExpr2;
            MinOp.Rule = BinExpr2 + ToTerm("-") + BinExpr2;

            BinExpr2.Rule = TimesOp | DivOp | Expression;
            TimesOp.Rule = Expression + ToTerm("*") + Expression;
            DivOp.Rule = Expression + ToTerm("/") + Expression;

            Include.Rule = Path + ToTerm("-->+") + Path;
            Exclude.Rule = Path + ToTerm("-->%") + Path;
            Spawn.Rule = Path + ToTerm("-->>") + Process;
            Condition.Rule = Path + ToTerm("-->*") + Path;
            Response.Rule = Path + ToTerm("*-->") + Path;
            Relation.Rule = Include | Exclude | Response | Condition | Spawn;
            //RelationsLine.Rule = ToTerm(",") + Relation;
            //Relations.Rule = Relation + MakeStarRule(Relations, RelationsLine);
            Relations.Rule = MakeListRule(Relations, ToTerm(","), Relation) | Empty;            
            //InputEvent.Rule = identifier + ToTerm("<?>") + ToTerm("{") + Process + ToTerm("}");
            //OutputEvent.Rule = identifier + ToTerm("<") + Expression + ToTerm(">") + ToTerm("{") + Process + ToTerm("}"));
            InputEvent.Rule = identifier + ToTerm("<?>") +SubProcess;
            OutputEvent.Rule = identifier + ToTerm("<") + Expression + ToTerm(">") +SubProcess;
            //InputEventP.Rule = identifier + ToTerm("<?>") + SubProcess;
            //OutputEventP.Rule = identifier + ToTerm("<") + Expression + ToTerm(">") + SubProcess;
            SubProcess.Rule = (ToTerm("{") + Process + ToTerm("}")) | Empty;
            //SubProcess_opt.Rule = SubProcess.Q();
            Event.Rule = InputEvent | OutputEvent;
            //Event.Rule = (InputEvent | OutputEvent) + SubProcess.Q();
            //Event.Rule = InputEvent | OutputEvent;            
            //Event.Rule = InputEvent | OutputEvent ;
            //Event.Rule = (InputEvent | OutputEvent) + ToTerm("{") + Process + ToTerm("}");
            //StructuredDataLine.Rule = Event + ToTerm(",");
            //StructuredData.Rule = MakeStarRule(StructuredData, StructuredDataLine);
            StructuredData.Rule = MakeListRule(StructuredData, ToTerm(","), Event) | Empty;            
            Process.Rule = StructuredData + ToTerm(";") + Relations;
            this.Root = Process;

            
            MarkPunctuation("-->+", ",", ";", "-->%", "-->*", "*-->",
                "-->>", "<", ">", "?", "!", "{", "}", "/", ".", "..", "*", "+", "-", "(", ")");

            //MarkPunctuation(Empty);

            //MarkTransient(Empty);

            //MarkTransient(PathExpression);
            MarkTransient(Relation, Event, SubProcess, PathExpressionCont,
                PathExpression, Path, BinExpr, Expression, Term, ParExpr, BinExpr2);
            


            /*
            var Expr = new NonTerminal("Expr");
            var Term = new NonTerminal("Term");
            var BinExpr = new NonTerminal("BinExpr", typeof(BinExprNode));
            var ParExpr = new NonTerminal("ParExpr");
            var UnExpr = new NonTerminal("UnExpr", typeof(UnExprNode));
            var UnOp = new NonTerminal("UnOp");
            var BinOp = new NonTerminal("BinOp", "operator");
            var PostFixExpr = new NonTerminal("PostFixExpr", typeof(UnExprNode));
            var PostFixOp = new NonTerminal("PostFixOp");
            var AssignmentStmt = new NonTerminal("AssignmentStmt", typeof(AssigmentNode));
            var AssignmentOp = new NonTerminal("AssignmentOp", "assignment operator");
            var Statement = new NonTerminal("Statement");
            var ProgramLine = new NonTerminal("ProgramLine");
            var Program = new NonTerminal("Program", typeof(StatementListNode));

            // 3. BNF rules
            Expr.Rule = Term | UnExpr | BinExpr | PostFixExpr;
            Term.Rule = number | ParExpr | identifier;
            ParExpr.Rule = "(" + Expr + ")";
            UnExpr.Rule = UnOp + Term;
            UnOp.Rule = ToTerm("+") | "-" | "++" | "--";
            BinExpr.Rule = Expr + BinOp + Expr;
            BinOp.Rule = ToTerm("+") | "-" | "*" | "/" | "**";
            PostFixExpr.Rule = Term + PostFixOp;
            PostFixOp.Rule = ToTerm("++") | "--";
            AssignmentStmt.Rule = identifier + AssignmentOp + Expr;
            AssignmentOp.Rule = ToTerm("=") | "+=" | "-=" | "*=" | "/=";
            Statement.Rule = AssignmentStmt | Expr | Empty;
            ProgramLine.Rule = Statement + NewLine;
            Program.Rule = MakeStarRule(Program, ProgramLine);
            this.Root = Program;       // Set grammar root
            */
        }
    }
}
