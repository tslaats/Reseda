using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reseda.Core;

namespace Reseda.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
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
            //PathExpression p = new Root(new Name("A", new All()));

            System.Diagnostics.Debug.WriteLine(root.PrintTree());

            //int i = 0;
            System.Diagnostics.Debug.WriteLine(p.ToString());
            System.Diagnostics.Debug.WriteLine(p.Eval(root, root));
            System.Diagnostics.Debug.WriteLine(p.Eval(root, root).Count);
            foreach (Event e in p.Eval(root, root))
            {
                System.Diagnostics.Debug.WriteLine(e.name);
            }
            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void TestMethod2()
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



            //PathExpression p = new Root().Extend(new Name("A").Extend(new All()));

            System.Diagnostics.Debug.WriteLine(root.PrintTree());

            System.Diagnostics.Debug.WriteLine(root.subProcess.structuredData[0].IsEnabled());
            System.Diagnostics.Debug.WriteLine(root.subProcess.structuredData[0].subProcess.structuredData[0].IsEnabled());
            System.Diagnostics.Debug.WriteLine(root.subProcess.structuredData[1].IsEnabled());
            System.Diagnostics.Debug.WriteLine(root.subProcess.structuredData[2].IsEnabled());

            foreach (var e in new Here().Eval(root.subProcess.structuredData[0]))
                System.Diagnostics.Debug.WriteLine(e);
            
            //int i = 0;

            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void TestMethod3()
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

            System.Diagnostics.Debug.WriteLine(root.PrintTree());
            System.Diagnostics.Debug.WriteLine(new Root().Extend(new Name("A").Extend(new All())).ToString());
            

            System.Diagnostics.Debug.WriteLine(root.subProcess.structuredData[0].IsEnabled());
            System.Diagnostics.Debug.WriteLine(root.subProcess.structuredData[0].subProcess.structuredData[0].IsEnabled());
            System.Diagnostics.Debug.WriteLine(root.subProcess.structuredData[1].IsEnabled());
            System.Diagnostics.Debug.WriteLine(root.subProcess.structuredData[2].IsEnabled());
                        
            root.subProcess.structuredData[2].subProcess.structuredData[0].Execute();
            root.subProcess.structuredData[2].subProcess.structuredData[1].Execute();
            root.subProcess.structuredData[0].subProcess.structuredData[0].Execute();

            System.Diagnostics.Debug.WriteLine("------------------------------------");

            System.Diagnostics.Debug.WriteLine(root.subProcess.structuredData[0].IsEnabled());
            System.Diagnostics.Debug.WriteLine(root.subProcess.structuredData[0].subProcess.structuredData[0].IsEnabled());
            System.Diagnostics.Debug.WriteLine(root.subProcess.structuredData[1].IsEnabled());
            System.Diagnostics.Debug.WriteLine(root.subProcess.structuredData[2].IsEnabled());

            foreach (var e in new Here().Eval(root.subProcess.structuredData[0]))
                System.Diagnostics.Debug.WriteLine(e);

            //int i = 0;

            Assert.AreEqual(true, true);
        }


    }
}
