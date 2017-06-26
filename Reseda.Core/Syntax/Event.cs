using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public abstract class Event
    {
        public String name;
        public Marking marking;
        public Process subProcess;
        public Process parentProcess;

        public PathExpression Path {
            get
            {
                if (parentProcess != null)
                    return parentProcess.parent.Path.Extend(new Name(this.name));
                else
                    return new Root();
            }

        }

        public Event()
        {
            this.marking = new Marking();
            this.subProcess = new Process(this);
        }


        public Event(Marking m)
        {
            this.marking = m;
            this.subProcess = new Process(this);
        }

        public Event AddChildEvent(Event e)
        {
            this.subProcess.structuredData.Add(e);
            e.parentProcess = this.subProcess;
            return this;
        }


        public Event AddRelation(Relation r)
        {
            this.subProcess.relations.Add(r);
            return this;
        }

        internal abstract Event ShallowClone();        

        public ISet<Event> Descendants()
        {
            HashSet<Event> result = new HashSet<Event>(this.subProcess.structuredData);

            foreach (Event e in this.subProcess.structuredData)
                result.UnionWith(e.Descendants());
            return result;
        }



        public ISet<Event> Ancestors()
        {
            if (this.parentProcess != null)
            {
                HashSet<Event> result = new HashSet<Event>(this.parentProcess.parent.Ancestors());
                result.Add(this.parentProcess.parent);
                return result;
            }
            else
                return new HashSet<Event>();
        }


        public ISet<Event> DescendantLeaves()
        {
            HashSet<Event> result = new HashSet<Event>();

            if (this.subProcess.structuredData.Count == 0)
                result.Add(this);
            else
                foreach (Event e in this.subProcess.structuredData)
                    result.UnionWith(e.DescendantLeaves());

            return result;
        }

        public String Location()
        {
            String result = this.name;
            if (parentProcess != null)
                result = parentProcess.parent.Location() + result + "/";
            return result;
        }

        public String PrintTree(bool incMarking = false)
        {
            String result = Location();
            if (incMarking)
                result += "(" + marking.happened + "," + marking.included + "," + marking.pending + ",[" + (marking.value?.GetType().ToString() ?? "null") + "]" + marking.value + ")";
            result += Environment.NewLine;
            foreach (Event e in subProcess.structuredData)
                result += e.PrintTree(incMarking);
            return result;
        }

        public abstract String TypeToSource();

        public virtual String ToSource()
        {
            var result = this.name;
            result += TypeToSource();
            if (this.subProcess.structuredData.Count > 0 || this.subProcess.relations.Count > 0)
                result += "{" + subProcess.ToSource() + "}";
            return result;
        }



        public override String ToString()
        {
            return "Event: " + Location();
        }

        public Event Root()
        {
            if (this.parentProcess != null)
                return this.parentProcess.parent.Root();
            else
                return this;
        }

        public Boolean IsEnabled()
        {
            var result = true;

            result = result && this.marking.included;

            if (this.parentProcess != null)
                result = result && this.parentProcess.parent.IsEnabled();

            result = result && Root().subProcess.CheckEnabled(this);

            return result;
        }


        public virtual void Execute()
        {
            /*
            if (!this.IsEnabled())
            {
                throw new Exception("Trying to execute disabled event!");                
            }*/

            this.marking.happened = true;
            this.marking.pending = false;

            SideEffects se = Root().subProcess.GetSideEffects(this);

            foreach (var e in se.exclude)
                e.marking.included = false;

            foreach (var e in se.include)
                e.marking.included = true;

            foreach (var e in se.respond)
                e.marking.pending = true;

            foreach (var s in se.spawn)
            {
                foreach (var e in s.process.structuredData)
                {
                    s.context.AddChildEvent(e);
                }
                foreach (var r in s.process.relations)
                {
                    s.context.AddRelation(r);
                }
            }

        }

        public Boolean Bounded()
        {
            return subProcess.Bounded();
        }

        public HashSet<Event> StaticInhibitors()
        {
            return Root().subProcess.GetStaticInhibitors(this);
        }

        public Dictionary<Event, HashSet<Event>> StaticInhibitorGraph()
        {
            var result = new Dictionary<Event, HashSet<Event>>();

            if (this.StaticInhibitors().Count == 0)
                return result;

            result.Add(this, this.StaticInhibitors());
            foreach (var e in this.StaticInhibitors())
            {
                if (!result.ContainsKey(e))
                    e.StaticInhibitorGraph(ref result);
            }
            return result;
        }

        private void StaticInhibitorGraph(ref Dictionary<Event, HashSet<Event>> current)
        {
            current.Add(this, this.StaticInhibitors());
            foreach (var e in this.StaticInhibitors())
            {
                if (!current.ContainsKey(e))
                    e.StaticInhibitorGraph(ref current);
            }
        }

        public void CloseForResponses(ref Dictionary<Event, HashSet<Event>> g)
        {
            Root().subProcess.CloseForResponses(this, ref g);
        }

        public Dictionary<Event, HashSet<Event>> DependencyGraph()
        {
            var g = StaticInhibitorGraph();
            CloseForResponses(ref g);
            return g;
        }

        public Boolean IsLive()
        {            
            var g = DependencyGraph();


            System.Diagnostics.Debug.WriteLine("Dependence Graph for: " + this.ToString());
            foreach (var kvp in g)
            {
                var e = kvp.Key;
                System.Diagnostics.Debug.WriteLine(e.ToString());
                foreach (var e2 in kvp.Value)
                {
                    System.Diagnostics.Debug.WriteLine("---> " + e2.ToString());                    
                }
            }

            return ((g.Count == 0) || (!Cycle(g, new HashSet<Event>())));
        }

        private bool Cycle(Dictionary<Event, HashSet<Event>> g, HashSet<Event> s)
        {
            if (s.Contains(this))
                return true;
            var t = new HashSet<Event>();
            t.UnionWith(s);
            t.Add(this);
            foreach (Event e in g[this])
            {
                if (e.Cycle(g, t))
                    return true;
            }
            return false;
        }

        public Boolean ProcessIsLive()
        {
            System.Diagnostics.Debug.WriteLine("Checking process : " + this.Root().ToSource());
            var result = true;
            var term = this.Root().ShallowClone();
            if (!term.Bounded())
                return false;
            term.Root().subProcess.UnFold();
            // for all events in the term...
            foreach (Event e in this.Root().Descendants())
            {
                //check if they are live
                result = result && e.IsLive();
            }
            return result;

        }



        /*
        public Boolean IsEnabled(Event e)
        {
            var result = true;
            if (e != this)
                result = result && IsEnabled(this.parentProcess.parent);

            return result;
        }
        */
    }
}
