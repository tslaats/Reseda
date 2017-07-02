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

        internal Process Clone(Event parent) 
        {
            var p = new Process(null);
            foreach (var r in relations)
                p.relations.Add(r.Clone());
            foreach (var e in structuredData)
                p.structuredData.Add(e.Clone(p));
            p.parent = parent;
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
                        // handle iterator...
                        if (s.iterateOver != null)
                        {
                            var over = s.iterateOver.Eval(this.parent);
                            foreach (Event x in over)
                            {                                
                                result.spawn.Add(new SpawnEffect(s.target.Clone(null).PathReplace(s.iteratorName, x), this.parent));
                                //result.spawn.Add(new SpawnEffect(s.target.Clone(null), this.parent));
                            }
                        }
                        else
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

        internal Process PathReplace(string iteratorName, Event e)
        {   
            foreach(var f in this.structuredData)
            {
                f.PathReplace(iteratorName, e);
            }
            foreach (Relation r in this.relations)
            {
                r.PathReplace(iteratorName, e);
            }
            return this;            
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
            foreach (var e in structuredData)
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
        public Boolean OldBounded()
        {
            var result = true;

            foreach (var r in relations)
            {
                if (r.GetType() == typeof(Spawn))
                {
                    Spawn c = (Spawn)r;

                    result = result && !c.source.ContainsNamesOrStar(c.target.Names());

                    // not so clear from paper that this is also needed:
                    result = result && c.target.OldBounded();
                }
            }

            foreach (Event e in structuredData)
            {
                result = result && e.subProcess.OldBounded();
            }
            return result;
        }



        List<Spawn> DescendantSpawns()
        {
            var result = new List<Spawn>();
            foreach (var r in relations)
            {
                if (r.GetType() == typeof(Spawn))
                {
                    var s = (Spawn)r;
                    result.Add(s);
                    result.AddRange(s.target.DescendantSpawns());
                }
            }

            foreach (Event e in structuredData)
            {
                result.AddRange(e.subProcess.DescendantSpawns());
            }

            return result;
        }


        public Boolean SpawnCycle(Spawn current, List<Spawn> trace, Process root)
        {
            List<Spawn> newTrace = new List<Spawn>(trace);            
            newTrace.Add(current);
            foreach (var rel in root.DescendantSpawns())
            {
                if (current.source.ContainsNames(rel.target.Names()))
                {
                    if (newTrace.Contains(rel))
                        return true;

                    if (SpawnCycle(rel, newTrace, root))
                        return true;
                }
            }
            return false;
        }


        public Boolean Bounded()
        {
            return Bounded(this.parent.Root().subProcess);
        }

        public Boolean Bounded(Process root)
        {            
            foreach (var r in relations)
            {
                if (r.GetType() == typeof(Spawn))
                {
                    Spawn c = (Spawn)r;

                    if (c.source.ContainsStar())
                        return false;

                    if (SpawnCycle(c, new List<Spawn>(), root))
                        return false;
                    // check for spawn cycle here...

                    // build dpeendency graph
                    // just do it here, not most efficient way to do it, but meh...

                    // not so clear from paper that this is also needed:
                    if (!c.target.Bounded(root)) return false;
                }
            }

            foreach (Event e in structuredData)
            {
                if (!e.subProcess.Bounded(root)) return false;
            }
            return true;
        }





        public void UnFold()
        {
            UnFold(this.relations);
        }

        private void UnFold(List<Relation> relations)
        {
            var newRels = new List<Relation>();
            foreach (var r in relations)
            {
                if (r.GetType() == typeof(Spawn))
                {
                    Spawn sr = (Spawn)r;
                    var s = sr.target;
                    foreach (var e in s.structuredData)
                    {
                        this.parent.AddChildEvent(e);
                        // Adding a condition to each spawned event.
                        //this.parent.AddRelation(new Condition(this.parent.Path, e.Path));
                        newRels.Add(new Condition(this.parent.Path, e.Path));
                    }
                    foreach (var nr in s.relations)
                    {
                        //this.parent.AddRelation(nr);
                        newRels.Add(nr);
                    }
                    UnFold(s.relations);
                }
            }
            foreach (var r in newRels)
            {
                this.parent.AddRelation(r);
            }
            foreach (var e in this.structuredData)
            {
                System.Diagnostics.Debug.WriteLine(e.Root().ToSource());
                e.subProcess.UnFold();
            }
        }

        public HashSet<Event> GetStaticInhibitors(Event e)
        {            
            var result = new HashSet<Event>();
            foreach (var r in this.relations)
            {
                if (r.GetType() == typeof(Condition))
                {
                    Condition c = (Condition)r;
                    var src = c.source.Eval(this.parent);
                    var trg = c.target.Eval(this.parent);
                    if (trg.Contains(e))
                        result.UnionWith(src);
                    //System.Diagnostics.Debug.WriteLine("Checking ancestors of : " + e.ToString());
                    //System.Diagnostics.Debug.WriteLine(">");
                    foreach (var e2 in e.Ancestors())
                    {

                        //System.Diagnostics.Debug.WriteLine("     " + e2.ToString());
                        if (trg.Contains(e2))
                            result.UnionWith(src);
                    }
                    //System.Diagnostics.Debug.WriteLine("<");
                }
                else if (r.GetType() == typeof(Milestone))
                {
                    Milestone c = (Milestone)r;
                    var src = c.source.Eval(this.parent);
                    var trg = c.target.Eval(this.parent);
                    if (trg.Contains(e))
                        result.UnionWith(src);
                    foreach (var e2 in e.Ancestors())
                    {
                        if (trg.Contains(e2))
                            result.UnionWith(src);
                    }
                }
            }
            foreach (var f in this.structuredData)
            {
                result.UnionWith(f.subProcess.GetStaticInhibitors(e));
            }
            return result;
        }


        // skipping inclusions for now.
        public void CloseForResponses(Event e, ref Dictionary<Event, HashSet<Event>> g)
        {            
            foreach (var r in this.relations)
            {
                if (r.GetType() == typeof(Response))
                {
                    Response c = (Response)r;
                    var src = c.source.Eval(this.parent);
                    var trg = c.target.Eval(this.parent);
                    foreach (var a in src)
                    {
                        foreach (var b in trg)
                        {
                            if (g.ContainsKey(a) && g.ContainsKey(b))
                                g[b].Add(a);
                        }
                    }
                    /*
                    if (trg.Contains(e))
                        result.UnionWith(src);
                        */
                }
            }
            foreach (var f in this.structuredData)
            {
                f.subProcess.CloseForResponses(e, ref g);
            }
            
        }        
    }
}
