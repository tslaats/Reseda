﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Irony.Ast;
using Reseda.Core;

namespace Reseda.Parser
{
    public class ResedaGrammar : Grammar
    {
        internal NonTerminal path;
        public ResedaGrammar()
        {

            var str = new StringLiteral("string", "\'");
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
            var RelationsOpt = new NonTerminal("RelationsOpt");
            var Relation = new NonTerminal("Relation");
            var Include = new NonTerminal("Include");
            var Exclude = new NonTerminal("Exclude");
            var Response = new NonTerminal("Response");
            var Condition = new NonTerminal("Condition");
            var Milestone = new NonTerminal("Milestone");
            var Spawn = new NonTerminal("Spawn");
            var SpawnIterator = new NonTerminal("SpawnIterator");
            var Event = new NonTerminal("Event");
            var InputEvent = new NonTerminal("InputEvent");
            var TypedInputEvent = new NonTerminal("TypedInputEvent");            
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
            var NegOp = new NonTerminal("NegOp");
            var BinExpr = new NonTerminal("BinExpr");
            var BinExpr2 = new NonTerminal("BinExpr2");
            var BinOp = new NonTerminal("BinOp");
            //var Unit = new KeyTerm("", "Unit");
            var Unit = new NonTerminal("Unit");

            var Const = new NonTerminal("Const");


            var PlusOp = new NonTerminal("PlusOp");
            var MinOp = new NonTerminal("MinOp");
            var TimesOp = new NonTerminal("TimesOp");
            var DivOp = new NonTerminal("DivOp");
            var DPath = new NonTerminal("DPath");

            var DPathValue = new NonTerminal("DPathValue");
            var DPathIncluded = new NonTerminal("DPathIncluded");
            var DPathExecuted = new NonTerminal("DPathExecuted");
            var DPathPending = new NonTerminal("DPathPending");            
            var DDPath = new NonTerminal("DDPath");


            var RelExpr = new NonTerminal("RelExpr");
            var BoolExpr = new NonTerminal("BoolExpr");
            var AndOp = new NonTerminal("AndOp");
            var OrOp = new NonTerminal("OrOp");
            var EqOp = new NonTerminal("EqOp");
            var GtOp = new NonTerminal("GtOp");

            var InitialValue = new NonTerminal("InitialValue");
            var InitialValueC = new NonTerminal("InitialValueC");
            //var Trigger = new NonTerminal("Trigger");

            var Marking = new NonTerminal("Marking");
            var Executed = new NonTerminal("Executed");
            var ExecutedPending = new NonTerminal("ExecutedPending");
            var ExecutedExcluded = new NonTerminal("ExecutedExcluded");
            var ExecutedPendingExcluded = new NonTerminal("ExecutedPendingExcluded");
            var Pending = new NonTerminal("Pending");
            var Excluded = new NonTerminal("Excluded");
            var PendingExcluded = new NonTerminal("PendingExcluded");
            
            var Function0Args = new NonTerminal("Function0Args");
            var Function1Args = new NonTerminal("Function1Args");

            DDPath.Rule = DPathValue | DPathIncluded | DPathExecuted | DPathPending | DPath;
            DPathValue.Rule = DPath + (ToTerm(Symbols.ValueShort) | ToTerm(Symbols.Value));
            DPathIncluded.Rule = DPath + (ToTerm(Symbols.IncludedShort) | ToTerm(Symbols.Included));
            DPathExecuted.Rule = DPath + (ToTerm(Symbols.ExecutedShort) | ToTerm(Symbols.Executed));
            DPathPending.Rule = DPath + (ToTerm(Symbols.PendingShort) | ToTerm(Symbols.Pending));

            DPath.Rule = ToTerm("@") + Path;
            Expression.Rule = Term | BoolExpr;

            BoolExpr.Rule = AndOp | OrOp | RelExpr;
            AndOp.Rule = RelExpr + ToTerm(Symbols.And) + RelExpr;
            OrOp.Rule = RelExpr + ToTerm(Symbols.Or) + RelExpr;

            RelExpr.Rule = EqOp | GtOp | BinExpr;
            EqOp.Rule = BinExpr + ToTerm(Symbols.Eq) + BinExpr;
            GtOp.Rule = BinExpr + ToTerm(Symbols.Gt) + BinExpr;

            Term.Rule = Const | ParExpr | NegOp | identifier | DDPath | Function0Args | Function1Args;
            Const.Rule = str | number | Unit;
            Unit.Rule = Empty;
            NegOp.Rule = ToTerm("!") + Expression;
            ParExpr.Rule = "(" + Expression + ")";
            BinExpr.Rule = PlusOp | MinOp | BinExpr2;
            PlusOp.Rule = BinExpr2 + ToTerm("+") + BinExpr2;
            MinOp.Rule = BinExpr2 + ToTerm("-") + BinExpr2;


            Function0Args.Rule = identifier + ToTerm("()");
            Function1Args.Rule = identifier + ToTerm("(") + Expression + ToTerm(")");

            //Function0Args.Rule = ToTerm("abba") + identifier + ToTerm("()");
            //Function0Args.Rule = ToTerm("abba") + identifier + ToTerm("(") + Expression + ToTerm(")");


            BinExpr2.Rule = TimesOp | DivOp | Expression;
            TimesOp.Rule = Expression + ToTerm("*") + Expression;
            DivOp.Rule = Expression + ToTerm("/") + Expression;

            Include.Rule = Path + ToTerm("-->+") + Path;
            Exclude.Rule = Path + ToTerm("-->%") + Path;
            Spawn.Rule = Path + ToTerm("-->>") + ToTerm("{") + Process + ToTerm("}");
            SpawnIterator.Rule = Path + ToTerm("-(") + identifier + ToTerm("in") + Path  + ToTerm(")->>") + ToTerm("{") + Process + ToTerm("}");
            Condition.Rule = Path + ToTerm("-->*") + Path;
            Milestone.Rule = Path + ToTerm("--><>") + Path;
            Response.Rule = Path + ToTerm("*-->") + Path;
            Relation.Rule = Include | Exclude | Response | Condition | Milestone | Spawn | SpawnIterator;
            Relations.Rule = MakeListRule(Relations, ToTerm(","), Relation) | Empty;
            InputEvent.Rule = Marking + identifier + InitialValue + ToTerm("[?]") + SubProcess;
            TypedInputEvent.Rule = Marking + identifier + InitialValue + ToTerm("[?:") + Expression + ToTerm("]") + SubProcess;
            OutputEvent.Rule = Marking + identifier + InitialValue + ToTerm("[") + Expression + ToTerm("]") + SubProcess;
            SubProcess.Rule = (ToTerm("{") + Process + ToTerm("}")) | Empty;
            Event.Rule = InputEvent | OutputEvent | TypedInputEvent;
            StructuredData.Rule = MakeListRule(StructuredData, ToTerm(","), Event) | Empty;
            Process.Rule = StructuredData + RelationsOpt;
            RelationsOpt.Rule = ((ToTerm(";") | ToTerm("~")) + Relations) | Empty;
            Marking.Rule = Pending | Excluded | PendingExcluded | Executed | ExecutedPending | ExecutedExcluded | ExecutedPendingExcluded | Empty;
            InitialValue.Rule = (ToTerm("(") + InitialValueC + ToTerm(")")) | Empty;
            InitialValueC.Rule = Expression;
            //InitialValueC.Rule = Const | Trigger;
            //Trigger.Rule = ToTerm("src") + Path + (ToTerm(Symbols.ValueShort) | ToTerm(Symbols.Value)) ;
            Pending.Rule = ToTerm("!");
            Excluded.Rule = ToTerm("%");
            PendingExcluded.Rule = ToTerm("!%") | ToTerm("%!");
            Executed.Rule = ToTerm("✓");
            ExecutedPending.Rule = ToTerm("!✓") | ToTerm("✓!");
            ExecutedExcluded.Rule = ToTerm("✓%") | ToTerm("%✓");
            ExecutedPendingExcluded.Rule = ToTerm("!✓%") | ToTerm("!%✓") | ToTerm("✓!%") | ToTerm("✓%!") | ToTerm("%!✓") | ToTerm("%✓!");            
            this.Root = Process;
            this.path = Path;
            
            MarkPunctuation("-->+", ",", ";", "-->%", "-->*", "--><>", "*-->",
                "-->>", "<", ">", "?", "!", "{", "}", "/", ".", "..", "*", "+", "-", "(", ")", "@", "[","]", Symbols.And, Symbols.Eq, Symbols.Gt, Symbols.Neg, Symbols.Or, "[?]", "[?:", "-(", "in", ")->>");

            MarkTransient(Relation, Event, SubProcess, PathExpressionCont,
                PathExpression, Path, BinExpr, Expression, Term, ParExpr, BinExpr2,
                DDPath, BoolExpr, RelExpr, Marking, RelationsOpt, InitialValueC);

        }
    }
}
