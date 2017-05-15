using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reseda.Core;

namespace Reseda.Tests
{
    [TestClass]
    public class CoreTests
    {
        [TestMethod]
        public void BasicTerm()
        {
            Event root = new RootEvent()
                        .AddChildEvent(new OutputEvent("A")
                            .AddChildEvent(new OutputEvent("B")))
                        .AddChildEvent(new OutputEvent("C"))
                        .AddChildEvent(new OutputEvent("A")
                            .AddChildEvent(new OutputEvent("F"))
                            .AddChildEvent(new OutputEvent("B"))
                        );

            PathExpression p = new Root().Extend(new Name("A").Extend(new All()));


            String expected = @"/
/A/
/A/B/
/C/
/A/
/A/F/
/A/B/
";

            Assert.AreEqual(expected, root.PrintTree());

            //int i = 0;
            Assert.AreEqual("/A/*", p.ToString());

            String s = "";
            foreach (Event e in p.Eval(root, root))
            {
                s += e.name;
            }
            Assert.AreEqual("BFB", s);
        }

        [TestMethod]
        public void BasicEnablement()
        {
            Event root = new RootEvent()
                        .AddChildEvent(new OutputEvent("A")
                            .AddChildEvent(new OutputEvent("B"))
                            .AddRelation(new Condition(new Root().Extend(new Name("A").Extend(new All())), new Here()))
                            )
                        .AddChildEvent(new OutputEvent("C", new Marking(false, false, false)))
                        .AddChildEvent(new OutputEvent("A")
                            .AddChildEvent(new OutputEvent("F"))
                            .AddChildEvent(new OutputEvent("B"))
                        );
            
            Assert.AreEqual(root.subProcess.structuredData[0].IsEnabled(), false);
            Assert.AreEqual(root.subProcess.structuredData[0].subProcess.structuredData[0].IsEnabled(), false);
            Assert.AreEqual(root.subProcess.structuredData[1].IsEnabled(), false);
            Assert.AreEqual(root.subProcess.structuredData[2].IsEnabled(), true);
        }

        [TestMethod]
        public void BasicExecution()
        {
            Event root = new RootEvent()
                        .AddChildEvent(new OutputEvent("A")
                            .AddChildEvent(new OutputEvent("B"))
                            .AddRelation(new Condition(new Root().Extend(new Name("A").Extend(new Name("F"))), new Here()))
                            )
                        .AddChildEvent(new OutputEvent("C", new Marking(false, false, false)))
                        .AddChildEvent(new OutputEvent("A")
                            .AddChildEvent(new OutputEvent("F"))
                            .AddChildEvent(new OutputEvent("B"))
                        );



            //PathExpression p = new Root().Extend(new Name("A").Extend(new All()));

            Assert.AreEqual(root.subProcess.structuredData[0].IsEnabled(), false);
            Assert.AreEqual(root.subProcess.structuredData[0].subProcess.structuredData[0].IsEnabled(), false);
            Assert.AreEqual(root.subProcess.structuredData[1].IsEnabled(), false);
            Assert.AreEqual(root.subProcess.structuredData[2].IsEnabled(), true);

            root.subProcess.structuredData[2].subProcess.structuredData[0].Execute();
            root.subProcess.structuredData[2].subProcess.structuredData[1].Execute();
            root.subProcess.structuredData[0].subProcess.structuredData[0].Execute();

            Assert.AreEqual(root.subProcess.structuredData[0].IsEnabled(), true);
            Assert.AreEqual(root.subProcess.structuredData[0].subProcess.structuredData[0].IsEnabled(), true);
            Assert.AreEqual(root.subProcess.structuredData[1].IsEnabled(), false);
            Assert.AreEqual(root.subProcess.structuredData[2].IsEnabled(), true);
            
        }


    }
}
