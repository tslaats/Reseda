﻿using System;
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
        public Grammar pathgrammar = new ResedaPathGrammar();

        public ParseTreeNode getRoot(string sourceCode, Grammar grammar)
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
            var root = getRoot(sourceCode, grammar);
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
            var root = getRoot(sourceCode, grammar);
            if (root == null)
                throw new Exception("No Root!");
            dispTree(root, 0);
        }

        public Event Generate(string sourceCode)
        {
            var root = getRoot(sourceCode, grammar);
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
            if (node.ChildNodes.Count > 1)
                foreach (var r in GenerateRelations(node.ChildNodes[1]))
                    ev.AddRelation(r);
        }

        public Process GenerateProcess(ParseTreeNode node)
        {
            Process result = new Process(null);            
            foreach (var e in GenerateEvents(node.ChildNodes[0]))
                result.structuredData.Add(e);
            if (node.ChildNodes.Count > 1)
                foreach (var r in GenerateRelations(node.ChildNodes[1]))
                    result.relations.Add(r);

            return result;
        }

        public void AddInitialValue(ref Event e, ParseTreeNode exp)
        {
            if (exp.Term.Name == "Const")
                e.marking.value = GenerateConst(exp.ChildNodes[0]);
            else
                e.InitialValue = GenerateExpression(exp);
        }

        public HashSet<Event> GenerateEvents(ParseTreeNode node)
        {
            HashSet<Event> result = new HashSet<Event>();
            foreach (ParseTreeNode child in node.ChildNodes)
            {
                Event e = null;
                if ((child.ChildNodes[0].Term.Name == "Pending") 
                    || (child.ChildNodes[0].Term.Name == "Excluded") 
                    || (child.ChildNodes[0].Term.Name == "PendingExcluded")
                    || (child.ChildNodes[0].Term.Name == "Executed")
                    || (child.ChildNodes[0].Term.Name == "ExecutedPending")
                    || (child.ChildNodes[0].Term.Name == "ExecutedExcluded")
                    || (child.ChildNodes[0].Term.Name == "ExecutedPendingExcluded"))
                {                    
                    if (child.Term.Name == "OutputEvent")
                        e = GenerateOutputEvent(child, 1);
                    else if (child.Term.Name == "InputEvent")
                        e = GenerateInputEvent(child, 1);
                    else if (child.Term.Name == "TypedInputEvent")
                        e = (GenerateTypedInputEvent(child, 1));

                    if ((child.ChildNodes[0].Term.Name == "Pending") || (child.ChildNodes[0].Term.Name == "PendingExcluded") || 
                        (child.ChildNodes[0].Term.Name == "ExecutedPending") || (child.ChildNodes[0].Term.Name == "ExecutedPendingExcluded"))
                        e.marking.pending = true;
                    if ((child.ChildNodes[0].Term.Name == "Excluded") || (child.ChildNodes[0].Term.Name == "PendingExcluded") || 
                        (child.ChildNodes[0].Term.Name == "ExecutedExcluded") || (child.ChildNodes[0].Term.Name == "ExecutedPendingExcluded"))
                        e.marking.included = false;
                    if ((child.ChildNodes[0].Term.Name == "Executed") || (child.ChildNodes[0].Term.Name == "ExecutedPending") ||
                        (child.ChildNodes[0].Term.Name == "ExecutedExcluded") || (child.ChildNodes[0].Term.Name == "ExecutedPendingExcluded"))
                        e.marking.happened = true;

                    // TODO: check if const (set initial), or data expression, if data expression then store in a new field.
                    if (child.ChildNodes[2].ChildNodes.Count > 0 && child.ChildNodes[2].ChildNodes[0] != null)
                        AddInitialValue(ref e, child.ChildNodes[2].ChildNodes[0].ChildNodes[0]);


                }
                else
                {
                    if (child.Term.Name == "OutputEvent")
                        e = (GenerateOutputEvent(child, 0));
                    else if (child.Term.Name == "InputEvent")
                        e = (GenerateInputEvent(child, 0));
                    else if (child.Term.Name == "TypedInputEvent")
                        e = (GenerateTypedInputEvent(child, 0));
                    

                    // TODO: check if const (set initial), or data expression, if data expression then store in a new field.
                    if (child.ChildNodes[1].ChildNodes.Count > 0 && child.ChildNodes[1].ChildNodes[0] != null)
                        AddInitialValue(ref e, child.ChildNodes[1].ChildNodes[0].ChildNodes[0]);
                }
                result.Add(e);
            }  
            return result;
        }

        private Event GenerateInputEvent(ParseTreeNode child, int i)
        {
            i++;
            var result = new InputEvent(child.ChildNodes[i - 1].Token.Text);
            if (child.ChildNodes.Count > i + 1 && child.ChildNodes[i + 1] != null && child.ChildNodes[i + 1].Term.Name == "Process")
                AddProcess(result, child.ChildNodes[i+ 1]);
            return result;
        }

        private Event GenerateTypedInputEvent(ParseTreeNode child, int i)
        {
            i++;
            var result = new InputEvent(child.ChildNodes[i - 1].Token.Text, GenerateExpression(child.ChildNodes[i + 1]));
            if (child.ChildNodes.Count > i + 2 && child.ChildNodes[i + 2] != null && child.ChildNodes[i + 2].Term.Name == "Process")
                AddProcess(result, child.ChildNodes[i + 2]);
            return result;
        }

        private Event GenerateOutputEvent(ParseTreeNode child, int i)
        {
            i++;
            var result = new OutputEvent(child.ChildNodes[i - 1].Token.Text, GenerateExpression(child.ChildNodes[i + 1]));
            if (child.ChildNodes.Count > i + 2 && child.ChildNodes[i + 2].Term.Name == "Process")
                AddProcess(result, child.ChildNodes[i + 2]);
            return result;
        }

        private DataType GenerateConst(ParseTreeNode node)
        {
            switch (node.Term.Name)
            {
                case "number":
                    return new IntType((int)node.Token.Value);
                case "string":
                    return new StrType((string)node.Token.Value);
                case "Unit":
                    return new Unit();
                default:
                    throw new NotImplementedException(node.ToString());
            }
        }

        private DataExpression GenerateExpression(ParseTreeNode node)
        {            
            switch (node.Term.Name)
            {
                case "AndOp":
                    return new AndOp(GenerateExpression(node.ChildNodes[0]), GenerateExpression(node.ChildNodes[1]));
                case "OrOp":
                    return new OrOp(GenerateExpression(node.ChildNodes[0]), GenerateExpression(node.ChildNodes[1]));
                case "EqOp":
                    return new EqOp(GenerateExpression(node.ChildNodes[0]), GenerateExpression(node.ChildNodes[1]));
                case "GtOp":
                    return new GtOp(GenerateExpression(node.ChildNodes[0]), GenerateExpression(node.ChildNodes[1]));
                case "PlusOp":
                    return new PlusOp(GenerateExpression(node.ChildNodes[0]), GenerateExpression(node.ChildNodes[1]));                    
                case "MinOp":
                    return new MinOp(GenerateExpression(node.ChildNodes[0]), GenerateExpression(node.ChildNodes[1]));
                case "TimesOp":
                    return new TimesOp(GenerateExpression(node.ChildNodes[0]), GenerateExpression(node.ChildNodes[1]));
                case "DivOp":
                    return new DivOp(GenerateExpression(node.ChildNodes[0]), GenerateExpression(node.ChildNodes[1]));
                case "Const":
                    return GenerateConst(node.ChildNodes[0]);
                case "identifier":
                    return new Unit();
                case "DPathValue":
                    return new ValueOf(GenerateExpression(node.ChildNodes[0]));
                case "DPathIncluded":
                    return new IsIncluded(GenerateExpression(node.ChildNodes[0]));
                case "DPathExecuted":
                    return new IsExecuted(GenerateExpression(node.ChildNodes[0]));
                case "DPathPending":
                    return new IsPending(GenerateExpression(node.ChildNodes[0]));
                case "DPath":
                    return new Path(GeneratePath(node.ChildNodes[0]));
                case "NegOp":
                    return new NegOp(GenerateExpression(node.ChildNodes[0]));
                case "Function0Args":
                    return new Function(node.ChildNodes[0].Token.Text);
                case "Function1Args":
                    var arg1 = GenerateExpression(node.ChildNodes[1]);
                    var func = new Function(node.ChildNodes[0].Token.Text);
                    func.AddArgument(arg1);
                    return func;
                default:
                    throw new NotImplementedException(node.ToString());
            }            
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
                else if (child.Term.Name == "SpawnIterator")
                    result.Add(new Spawn(GeneratePath(child.ChildNodes[0]), child.ChildNodes[1].Token.Text, GeneratePath(child.ChildNodes[2]), GenerateProcess(child.ChildNodes[3])));
            }
            return result;
        }

        public PathExpression GeneratePath(string sourceCode)
        {
            var root = getRoot(sourceCode, pathgrammar);
            if (root == null)
                throw new Exception("No Root!");
            return GeneratePath(root);
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

            var j = 0;
            if (node.Term.Name == "PathName")
                j = 1;
            if (node.Term.Name != "PathRoot" && node.ChildNodes[j].ChildNodes.Count > 0) //.Term.Name == "Filter" 
                result.AddFilter(GenerateExpression(node.ChildNodes[j].ChildNodes[0]));

            var i = 1;
            if (node.Term.Name == "PathName")
                i = 2;
            else if (node.Term.Name == "PathRoot")
                i = 0;
            if (node.ChildNodes.Count > i)
                result.Extend(GeneratePath(node.ChildNodes[i]));
            return result;
        }
        
        
    }
}
