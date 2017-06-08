using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class Process
    {
        public List<Event> structuredData;
        public List<Relation> relations;
        public Event parent;

        public Process(Event parent)
        {            
            this.structuredData = new List<Event>();
            this.relations = new List<Relation>();
            this.parent = parent;
        }

        internal Process ShallowClone()
        {
            var p = new Process(null);
            foreach (var r in relations)
                p.relations.Add(r);

            foreach (var e in structuredData)
                p.structuredData.Add(e.ShallowClone());

            return p;
        }

        public ISet<Event> allEvents()
        {
            return new HashSet<Event>(structuredData);
        }

        public ISet<Event> eventsByName(String name)
        {
            HashSet<Event> result = new HashSet<Event>(structuredData);
            result.RemoveWhere(p => p.name != name);
            return result;
        }


        public SideEffects GetSideEffects(Event e)
        {
            var result = new SideEffects();            
            foreach (var r in this.relations)
            {
                if (r.GetType() == typeof(Inclusion))
                {
                    Inclusion c = (Inclusion)r;
                    var src = c.source.Eval(this.parent);
                    var trg = c.target.Eval(this.parent);
                    if (src.Contains(e))
                    {
                        foreach (var f in trg)
                            result.include.Add(f);
                    }
                }
                else if (r.GetType() == typeof(Exclusion))
                {
                    Exclusion m = (Exclusion)r;
                    var src = m.source.Eval(this.parent);
                    var trg = m.target.Eval(this.parent);
                    if (src.Contains(e))
                    {
                        foreach (var f in trg)
                            result.exclude.Add(f);
                    }
                }
                else if (r.GetType() == typeof(Response))
                {
                    Response m = (Response)r;
                    var src = m.source.Eval(this.parent);
                    var trg = m.target.Eval(this.parent);
                    if (src.Contains(e))
                    {
                        foreach (var f in trg)
                            result.respond.Add(f);
                    }
                }
                else if (r.GetType() == typeof(Spawn))
                {
                    Spawn s = (Spawn)r;
                    var src = s.source.Eval(this.parent);
                    //var trg = s.target.Eval(this.parent);
                    if (src.Contains(e))
                    {                        
                        result.spawn.Add(new SpawnEffect(s.target, this.parent));
                    }
                }
            }

            foreach (var f in this.structuredData)
            {
                //System.Diagnostics.Debug.WriteLine("-->" + f);                
                var se2 = f.subProcess.GetSideEffects(e);
                result.include.UnionWith(se2.include);
                result.exclude.UnionWith(se2.exclude);
                result.respond.UnionWith(se2.respond);
                result.spawn.UnionWith(se2.spawn);
                //System.Diagnostics.Debug.WriteLine("<--" + result);
            }
            return result;
        }

        public Boolean CheckEnabled(Event e)
        {
            //System.Diagnostics.Debug.WriteLine(this.parent.ToString() + " checking for: " + e.ToString());

            var result = true;
            foreach (var r in this.relations)
            {
                if (r.GetType() == typeof(Condition))
                {
                    Condition c = (Condition)r;
                    var src = c.source.Eval(this.parent);
                    var trg = c.target.Eval(this.parent);
                    if (trg.Contains(e))
                    {
                        foreach (var f in src)
                            result = result && (!f.marking.included || f.marking.happened);
                    }                        
                }
                else if (r.GetType() == typeof(Milestone))
                {
                    Milestone m = (Milestone)r;
                    var src = m.source.Eval(this.parent);
                    var trg = m.target.Eval(this.parent);
                    if (trg.Contains(e))
                    {
                        foreach (var f in src)
                            result = result && (!f.marking.included || !f.marking.pending);
                    }
                }
            }

            foreach (var f in this.structuredData)
            {
                //System.Diagnostics.Debug.WriteLine("-->" + f);                
                result = result && f.subProcess.CheckEnabled(e);
                //System.Diagnostics.Debug.WriteLine("<--" + result);
            }

            return result;
        }


        public String ToSource()
        {
            var result = "";
            foreach(var e in structuredData)
            {
                result += e.ToSource() + ",";
            }
            result = result.TrimEnd(',');
            result += ";";
            foreach (var r in relations)
            {
                result += r.ToSource() + ",";
            }
            result = result.TrimEnd(',');
            return result;
        }


        public ISet<String> Names()
        {
            var result = new HashSet<String>();
            foreach (Event e in structuredData)
            {
                result.Add(e.name);
                result.UnionWith(e.subProcess.Names());
            }
            return result;
        }

        // Should also check for valueof? - actually not since its a dataexpression
        public Boolean Bounded()
        {
            var result = true;

            foreach (var r in relations)
            {
                if (r.GetType() == typeof(Spawn))
                {
                    Spawn c = (Spawn)r;
                    if (c.source.ContainsNamesOrStar(c.target.Names())) result = false;
                }
            }

            foreach (Event e in structuredData)
            {
                result = result && e.subProcess.Bounded();
            }
            return result;
            //throw new NotImplementedException("Bounded()");
        }



    }
}
