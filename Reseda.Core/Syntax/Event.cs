using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class Event
    {
        public String name;
        public Marking marking;
        public Process subProcess;
        public Process parentProcess;

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

        public ISet<Event> Descendants()
        {
            HashSet<Event> result = new HashSet<Event>(this.subProcess.structuredData);

            foreach (Event e in this.subProcess.structuredData)
                result.UnionWith(e.Descendants());
            return result;
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
                e.marking.included = false;
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
