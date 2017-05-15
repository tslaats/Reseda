using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Reseda.Core;

namespace Reseda.Parser
{
    public class ResedaParser
    {
        public Grammar grammar = new ResedaGrammar();

        public ParseTreeNode getRoot(string sourceCode)
        {
            LanguageData language = new LanguageData(grammar);
            Irony.Parsing.Parser parser = new Irony.Parsing.Parser(language);
            ParseTree parseTree = parser.Parse(sourceCode);
            ParseTreeNode root = parseTree.Root;
            if (root == null)
                throw new Exception(parseTree.ParserMessages[0].ToString());
            return root;
        }


        public String stringTree(ParseTreeNode node, int level)
        {
            var result = "";
            for (int i = 0; i < level; i++)
                result += "  ";
            //System.Diagnostics.Debug.WriteLine(node);
            if (node != null && node.Token != null)
                result += node.Token.Value + Environment.NewLine;
            else
                result += node.Term.Name + Environment.NewLine;

            foreach (ParseTreeNode child in node.ChildNodes)
                result += stringTree(child, level + 1);

            return result;
        }

        public String stringTree(string sourceCode)
        {
            var root = getRoot(sourceCode);
            if (root == null)
                throw new Exception("No Root!");
            return stringTree(root, 0);
        }

        public void dispTree(ParseTreeNode node, int level)
        {
            for (int i = 0; i < level; i++)
                System.Diagnostics.Debug.Write("  ");
            //System.Diagnostics.Debug.WriteLine(node);
            if (node != null && node.Token != null)
                System.Diagnostics.Debug.WriteLine(node.Token.Value);
            else
                System.Diagnostics.Debug.WriteLine(node.Term.Name);

            foreach (ParseTreeNode child in node.ChildNodes)
                dispTree(child, level + 1);
        }

        public void dispTree(string sourceCode)
        {
            var root = getRoot(sourceCode);
            if (root == null)
                throw new Exception("No Root!");
            dispTree(root, 0);
        }

        public Event Generate(string sourceCode)
        {
            var root = getRoot(sourceCode);
            if (root == null)
                throw new Exception("No Root!");
            return GenerateRoot(root);
        }

        public Event GenerateRoot(ParseTreeNode node)
        {
            var result = new RootEvent();
            //result.subProcess
            AddProcess(result, node);
            return result;
        }

        public void AddProcess(Event ev, ParseTreeNode node)
        {
            foreach (var e in GenerateEvents(node.ChildNodes[0]))
                ev.AddChildEvent(e);
            foreach (var r in GenerateRelations(node.ChildNodes[1]))
                ev.AddRelation(r);
        }

        public Process GenerateProcess(ParseTreeNode node)
        {
            Process result = new Process(null);            
            foreach (var e in GenerateEvents(node.ChildNodes[0]))
                result.structuredData.Add(e);
            foreach (var r in GenerateRelations(node.ChildNodes[1]))
                result.relations.Add(r);

            return result;
        }


        public HashSet<Event> GenerateEvents(ParseTreeNode node)
        {
            HashSet<Event> result = new HashSet<Event>();
            foreach (ParseTreeNode child in node.ChildNodes)
            {
                if (child.Term.Name == "OutputEvent")
                    result.Add(GenerateOutputEvent(child));
                else if (child.Term.Name == "InputEvent")
                    result.Add(GenerateInputEvent(child));
            }  
            return result;
        }

        private Event GenerateInputEvent(ParseTreeNode child)
        {
            var result = new InputEvent(child.ChildNodes[0].Token.Text);
            if (child.ChildNodes[1] != null && child.ChildNodes[1].Term.Name == "Process")
                AddProcess(result, child.ChildNodes[1]);
            return result;
        }

        private Event GenerateOutputEvent(ParseTreeNode child)
        {
            var result = new OutputEvent(child.ChildNodes[0].Token.Text, GenerateExpression(child.ChildNodes[1]));
            if (child.ChildNodes.Count > 2 && child.ChildNodes[2].Term.Name == "Process")
                AddProcess(result, child.ChildNodes[2]);
            return result;
        }

        private DataExpression GenerateExpression(ParseTreeNode node)
        {            
            switch (node.Term.Name)
            {
                case "PlusOp":
                    return new PlusOp(GenerateExpression(node.ChildNodes[0]), GenerateExpression(node.ChildNodes[1]));                    
                case "MinOp":
                    return new MinOp(GenerateExpression(node.ChildNodes[0]), GenerateExpression(node.ChildNodes[1]));
                case "TimesOp":
                    return new TimesOp(GenerateExpression(node.ChildNodes[0]), GenerateExpression(node.ChildNodes[1]));
                case "DivOp":
                    return new DivOp(GenerateExpression(node.ChildNodes[0]), GenerateExpression(node.ChildNodes[1]));
                case "number":
                    return new IntType((int)node.Token.Value);
                case "identifier":
                    return new Unit();
                case "Unit":
                    return new Unit();
                case "DPathValue":
                    return new ValueOf(GenerateExpression(node.ChildNodes[0]));                    
                case "DPath":
                    return new Path(GeneratePath(node.ChildNodes[0]));

                /*case "PathAll":
                    result = new All();
                    break;*/
                default:
                    throw new NotImplementedException(node.ToString());
            }

            return null;
            /*
            PlusOp
  TimesOp
          2(number)
          2(number)
        4(number*/
        }

        public HashSet<Relation> GenerateRelations(ParseTreeNode node)
        {
            HashSet<Relation> result = new HashSet<Relation>();
            foreach (ParseTreeNode child in node.ChildNodes)
            {
                if (child.Term.Name == "Condition")
                    result.Add(new Condition(GeneratePath(child.ChildNodes[0]), GeneratePath(child.ChildNodes[1])));
                else if (child.Term.Name == "Response")
                    result.Add(new Response(GeneratePath(child.ChildNodes[0]), GeneratePath(child.ChildNodes[1])));
                else if (child.Term.Name == "Include")
                    result.Add(new Inclusion(GeneratePath(child.ChildNodes[0]), GeneratePath(child.ChildNodes[1])));
                else if (child.Term.Name == "Exclude")
                    result.Add(new Exclusion(GeneratePath(child.ChildNodes[0]), GeneratePath(child.ChildNodes[1])));
                else if (child.Term.Name == "Milestone")
                    result.Add(new Milestone(GeneratePath(child.ChildNodes[0]), GeneratePath(child.ChildNodes[1])));
                else if (child.Term.Name == "Spawn")
                    result.Add(new Spawn(GeneratePath(child.ChildNodes[0]), GenerateProcess(child.ChildNodes[1])));
            }
            return result;
        }

        private PathExpression GeneratePath(ParseTreeNode node)
        {
            PathExpression result;
            switch (node.Term.Name)
            {
                case "PathRoot":
                    result = new Root();
                    break;
                case "PathName":
                    result = new Name(node.ChildNodes[0].Token.Text);
                    break;
                case "PathLocal":
                    result = new Here();
                    break;
                case "PathParent":
                    result = new Parent();
                    break;
                case "PathAll":
                    result = new All();
                    break;
                default:
                    throw new NotImplementedException(node.ToString());
            }

            var i = 0;
            if (node.Term.Name == "PathName")
                i = 1;
            if (node.ChildNodes.Count > i)
                result.Extend(GeneratePath(node.ChildNodes[i]));
            return result;
        }




        /*
        public Process generateProcess(ParseTreeNode node)
        {

        }

        public Event generate(ParseTreeNode node)
        {
            

            switch (node.Term.Name)
            {
                case "TopLevel":
                case "Expr":
                case "ValueExpr":
                case "ParenExpr":
                    return generate(node.ChildNodes[0]);
                case "NumberExpr":
                    return ConstantFP.get(builder.getDoubleTy(), double.Parse(node.Token.Text));
                case "VariableExpr":

            }
        }*/
    }
}
