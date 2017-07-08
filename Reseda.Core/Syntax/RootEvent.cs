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

        public override Event Clone(Process parent) 
        {
            return this.CloneInto(new RootEvent(), parent);
        }

        public RootEvent MakeInSeq()
        {
            var result = this.Clone(null);
            var temp = this.Clone(null);
            temp.subProcess.UnFold();


            HashSet<InputEvent> input = new HashSet<InputEvent>();
            HashSet<OutputEvent> output = new HashSet<OutputEvent>();

            foreach (Event e in temp.Descendants())
            {
                if (e.GetType() == typeof(InputEvent))
                    input.Add((InputEvent)e);
                else if(e.GetType() == typeof(OutputEvent))
                    output.Add((OutputEvent)e);
            }

            foreach(var i in input)
            {
                foreach (var o in output)
                {
                    result.AddRelation(new Milestone(o.GeneralPath, i.GeneralPath));
                }
            }

            return (RootEvent)result;
        }

        public RootEvent MakeGlitchFree()
        {
            var result = this.Clone(null);
            var temp = this.Clone(null);
            temp.subProcess.UnFold();


            HashSet<InputEvent> input = new HashSet<InputEvent>();
            HashSet<OutputEvent> output = new HashSet<OutputEvent>();

            foreach (Event e in temp.Descendants())
            {
                if (e.GetType() == typeof(InputEvent))
                    input.Add((InputEvent)e);
                else if (e.GetType() == typeof(OutputEvent))
                    output.Add((OutputEvent)e);
            }
            foreach (var o in output)
            {
                foreach (var i in input)
                {
                    var x = new HashSet<string>();
                    x.Add(i.name);
                    if (o.expression.ContainsNames(x))
                    {
                        result.AddRelation(new Response(i.GeneralPath, o.GeneralPath));
                        result.AddRelation(new Milestone(i.GeneralPath, o.GeneralPath));
                    }
                }
                foreach (var o2 in output)
                {
                    var x = new HashSet<string>();
                    x.Add(o2.name);
                    if (o.expression.ContainsNames(x))
                    {
                        result.AddRelation(new Response(o2.GeneralPath, o.GeneralPath));
                        result.AddRelation(new Milestone(o2.GeneralPath, o.GeneralPath));
                    }
                }
            }

            return (RootEvent)result;
        }

        public List<PathExpression> GetAllEnabledEvents()
        {
            List<PathExpression> result = new List<PathExpression>();
            foreach (var e in this.Descendants())
            {
                if (e.IsEnabled())
                    result.Add(e.Path);
            }

            return result;
        }

        public List<Event> GetAllEnabledAndPendingComputationEvents()
        {
            List<Event> result = new List<Event>();
            foreach (var e in this.Descendants())
            {
                if (e.IsEnabled() && e.GetType() == typeof(OutputEvent) && e.marking.pending)
                    result.Add(e);
            }

            return result;
        }


        public String AutoComputeToString()
        {
            var result = "";
            foreach (var p in AutoCompute())
                result += p.ToString() + System.Environment.NewLine;
            return result;
        }


        public List<PathExpression> AutoCompute()
        {
            if (GetAllEnabledAndPendingComputationEvents().Count == 0)
                return new List<PathExpression>();
            var e = GetAllEnabledAndPendingComputationEvents()[0];
            e.Execute();
            var result = AutoCompute();
            result.Add(e.Path);
            return result; 
        }
    }
}
