using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reseda.Parser;
using Reseda.Core;

namespace Reseda.Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void BasicParserTest()
        {
            string input = "A[ 2 * 2 + 4 ]," +
                           "B[3 + 4 + 5 * 6 + 7 + 8 + 9]{C[1 + (3 * 5) - @c/d];}" +
                           "; A -->* /B," +
                           " A/*/C -->* /B/.././F,"
                           + "B -->% *";
            var p = new ResedaParser();
            p.dispTree(input);

            var term = p.Generate(input);
            var term2 = p.Generate(term.ToSource());

            Assert.AreEqual(term.ToSource(), "A[2 * 2 + 4],B[3 + 4 + 5 * 6 + 7 + 8 + 9]{C[1 + 3 * 5 - @c/d];};A -->* /B,A/*/C -->* /B/.././F,B -->% *");
            Assert.AreEqual(term.ToSource(), term2.ToSource());



            //System.Diagnostics.Debug.WriteLine(p.Generate(input).PrintTree());

            /*
            var term = p.Generate(input);
            System.Diagnostics.Debug.WriteLine(term.PrintTree());
            System.Diagnostics.Debug.WriteLine(term.PrintTree(true));

            var c = (Condition)term.subProcess.relations[0];
            System.Diagnostics.Debug.WriteLine(c.source.ToString());
            System.Diagnostics.Debug.WriteLine(c.target.ToString());

            c = (Condition)term.subProcess.relations[1];
            System.Diagnostics.Debug.WriteLine(c.source.ToString());
            System.Diagnostics.Debug.WriteLine(c.target.ToString());

            term.subProcess.structuredData[0].Execute();
            System.Diagnostics.Debug.WriteLine(term.PrintTree(true));

            term.subProcess.structuredData[1].Execute();
            System.Diagnostics.Debug.WriteLine(term.PrintTree(true));
            */
        }

        [TestMethod]
        public void CurrentHackingTest()
        {
            string input = "A[?]," +
                           "B[?]," +
                           "C[@A:v + @B:v * 10]," +
                           "D[]," +
                           "E[]" +
                           "; A -->* B," +
                           " D -->> {N[?];}," +
                           "E -->% *";
            var p = new ResedaParser();
            p.dispTree(input);

            //System.Diagnostics.Debug.WriteLine(p.Generate(input).PrintTree());

            var term = p.Generate(input);

            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());

            //var term2 = term.CloneJson();
            var term2 = p.Generate(term.subProcess.ToSource());

            InputEvent a = (InputEvent)term.subProcess.structuredData[0];
            InputEvent b = (InputEvent)term.subProcess.structuredData[1];
            OutputEvent c = (OutputEvent)term.subProcess.structuredData[2];
            OutputEvent d = (OutputEvent)term.subProcess.structuredData[3];
            OutputEvent e = (OutputEvent)term.subProcess.structuredData[4];

            a.Execute(1);
            System.Diagnostics.Debug.WriteLine(term.PrintTree(true));

            b.Execute(5);
            System.Diagnostics.Debug.WriteLine(term.PrintTree(true));

            c.Execute();
            System.Diagnostics.Debug.WriteLine(term.PrintTree(true));

            d.Execute();
            System.Diagnostics.Debug.WriteLine(term.PrintTree(true));

            d.Execute();
            System.Diagnostics.Debug.WriteLine(term.PrintTree(true));

            System.Diagnostics.Debug.WriteLine("TERM 2");

            a = (InputEvent)term2.subProcess.structuredData[0];
            b = (InputEvent)term2.subProcess.structuredData[1];
            c = (OutputEvent)term2.subProcess.structuredData[2];
            d = (OutputEvent)term2.subProcess.structuredData[3];
            e = (OutputEvent)term2.subProcess.structuredData[4];

            System.Diagnostics.Debug.WriteLine(term2.PrintTree(true));
            a.Execute(1);
            System.Diagnostics.Debug.WriteLine(term2.PrintTree(true));

            b.Execute(5);
            System.Diagnostics.Debug.WriteLine(term2.PrintTree(true));

            c.Execute();
            System.Diagnostics.Debug.WriteLine(term2.PrintTree(true));

            d.Execute();
            System.Diagnostics.Debug.WriteLine(term2.PrintTree(true));

            d.Execute();
            System.Diagnostics.Debug.WriteLine(term2.PrintTree(true));
        }


        [TestMethod]
        public void BooleanExpresionTest()
        {
            string input = "A[?]," +
                           "B[?]," +
                           "C[@A:v + @B:v > 5 && !(@A:v - @B:v > 10)]" +
                           "; ";

            var p = new ResedaParser();
            p.dispTree(input);

            var term = p.Generate(input);
            var term2 = p.Generate(term.subProcess.ToSource());

            Assert.AreEqual(term.ToSource(), term2.ToSource());



            InputEvent a = (InputEvent)term.subProcess.structuredData[0];
            InputEvent b = (InputEvent)term.subProcess.structuredData[1];
            OutputEvent c = (OutputEvent)term.subProcess.structuredData[2];



            a.Execute(3);
            b.Execute(5);
            System.Diagnostics.Debug.WriteLine(term.PrintTree(true));

            c.Execute();
            var val = (BoolType)c.marking.value;
            Assert.AreEqual(val.value, true);
            System.Diagnostics.Debug.WriteLine(term.PrintTree(true));


            a.Execute(20);
            c.Execute();
            val = (BoolType)c.marking.value;
            Assert.AreEqual(val.value, false);
            System.Diagnostics.Debug.WriteLine(term.PrintTree(true));

        }



        [TestMethod]
        public void FilterTest()
        {
            string input = "A[]," +
                           "B[]," +
                           "B[]," +
                           "C[]" +
                           "; A -->* B[1]";

            var p = new ResedaParser();
            p.dispTree(input);

            var term = p.Generate(input);
            var term2 = p.Generate(term.subProcess.ToSource());

            Assert.AreEqual(term.ToSource(), term2.ToSource());



            OutputEvent a = (OutputEvent)term.subProcess.structuredData[0];
            OutputEvent b0 = (OutputEvent)term.subProcess.structuredData[1];
            OutputEvent b1 = (OutputEvent)term.subProcess.structuredData[2];
            OutputEvent c = (OutputEvent)term.subProcess.structuredData[3];


            b0.Execute();
            System.Diagnostics.Debug.WriteLine(term.PrintTree(true));

            try
            {
                b1.Execute();
                Assert.Fail();
            }
            catch
            { }

            System.Diagnostics.Debug.WriteLine(term.PrintTree(true));

        }



        [TestMethod]
        public void BooleanFilterTest()
        {
            string input = "A[?]," +
                           "A[?]," +
                           "A[?]," +
                           "B[]" +
                           "; A[@.:v == 5] *--> B";

            var p = new ResedaParser();
            p.dispTree(input);

            var term = p.Generate(input);
            var term2 = p.Generate(term.subProcess.ToSource());

            Assert.AreEqual(term.ToSource(), term2.ToSource());



            InputEvent a0 = (InputEvent)term.subProcess.structuredData[0];
            InputEvent a1 = (InputEvent)term.subProcess.structuredData[1];
            InputEvent a2 = (InputEvent)term.subProcess.structuredData[2];
            OutputEvent b = (OutputEvent)term.subProcess.structuredData[3];


            a0.Execute(1);
            System.Diagnostics.Debug.WriteLine(term.PrintTree(true));

            a1.Execute(5);
            System.Diagnostics.Debug.WriteLine(term.PrintTree(true));

            a2.Execute(3);
            System.Diagnostics.Debug.WriteLine(term.PrintTree(true));

        }


        [TestMethod]
        public void CloneTest()
        {
            string input = "A[?]{D[?];}" +
                           ";" +
                           " A -->> {B[?]{C[?];};}";
            var p = new ResedaParser();
            p.dispTree(input);

            //System.Diagnostics.Debug.WriteLine(p.Generate(input).PrintTree());

            var term = p.Generate(input);

            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            InputEvent d = (InputEvent)term.subProcess.structuredData[0].subProcess.structuredData[0];

            //var term2 = term.CloneJson();
            var term2 = p.Generate(term.subProcess.ToSource());

            Assert.AreEqual(term.ToSource(), term2.ToSource());


            //Assert.AreEqual(term.ToSource(), term.shallow().ToSource());


        }


        [TestMethod]
        public void CloneTestDataExpression()
        {
            string input = "A[5 * 6]" +
                           ";";
            var p = new ResedaParser();
            p.dispTree(input);

            //System.Diagnostics.Debug.WriteLine(p.Generate(input).PrintTree());

            var term = p.Generate(input);
            var term2 = term.Clone(null);

            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            System.Diagnostics.Debug.WriteLine(term2.subProcess.ToSource());
            
            Assert.AreEqual(term.ToSource(), term2.ToSource());


            //Assert.AreEqual(term.ToSource(), term.shallow().ToSource());


        }


        [TestMethod]
        public void MarkingTest()
        {
            string input = "%!A[?]," +
                           "!B[?]," +
                           "%C[?]," +
                           "!%D[?]" +
                           ";" +
                           "";
            var p = new ResedaParser();
            p.dispTree(input);

            //System.Diagnostics.Debug.WriteLine(p.Generate(input).PrintTree());

            var term = p.Generate(input);

            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());            

            //var term2 = term.CloneJson();
            var term2 = p.Generate(term.subProcess.ToSource());

            Assert.AreEqual(term.ToSource(), term2.ToSource());


            //Assert.AreEqual(term.ToSource(), term.shallow().ToSource());


        }

        [TestMethod]
        public void SpawnIterator()
        {
            string input = @"A[?],
B[?]
~
A -->> {C[!]},
B -(p in C)->> {D[!]}
";
            var p = new ResedaParser();
            p.dispTree(input);
            var term = p.Generate(input);
            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            var term2 = p.Generate(term.subProcess.ToSource());
            Assert.AreEqual(term.ToSource(), term2.ToSource());
        }


        [TestMethod]
        public void FunctionTest()
        {
            string input = @"
!name[?],
!price[?],
!quantity[freshid()],
amount[afuntion(@price:value * @quantity:value)],
invoiceRow[@name:value + ';' + @price:value + ';' + @quantity:value + ';' + @amount:value]";
            var p = new ResedaParser();
            p.dispTree(input);
            var term = p.Generate(input);
            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            var term2 = p.Generate(term.subProcess.ToSource());
            Assert.AreEqual(term.ToSource(), term2.ToSource());
        }


        [TestMethod]
        public void CountTest()
        {
            string input = @"
!A[?],
!A[?],
!C[count(@A)];";
            var p = new ResedaParser();
            p.dispTree(input);
            var term = p.Generate(input);
            OutputEvent c = (OutputEvent)term.subProcess.structuredData[2];
            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            var term2 = p.Generate(term.subProcess.ToSource());
            Assert.AreEqual(term.ToSource(), term2.ToSource());
            c.Execute();
            System.Diagnostics.Debug.WriteLine(c.ToSource());
            System.Diagnostics.Debug.WriteLine(c.marking.value);
            Assert.AreEqual(c.marking.value.ToString(), "2");

        }


        [TestMethod]
        public void FreshIdTest()
        {
            string input = @"
!A[?],
!B[?],
!C[freshid()];";
            var p = new ResedaParser();
            p.dispTree(input);
            var term = p.Generate(input);
            InputEvent a = (InputEvent)term.subProcess.structuredData[0];
            InputEvent b = (InputEvent)term.subProcess.structuredData[1];
            OutputEvent c = (OutputEvent)term.subProcess.structuredData[2];
            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            var term2 = p.Generate(term.subProcess.ToSource());
            Assert.AreEqual(term.ToSource(), term2.ToSource());
            c.Execute();
            System.Diagnostics.Debug.WriteLine(c.ToSource());
            System.Diagnostics.Debug.WriteLine(c.marking.value);
            Assert.AreEqual(c.marking.value.ToString(), "0");
            c.Execute();
            Assert.AreEqual(c.marking.value.ToString(), "1");
            a.Execute(3);
            c.Execute();
            Assert.AreEqual(c.marking.value.ToString(), "4");

            b.Execute(1);
            c.Execute();
            Assert.AreEqual(c.marking.value.ToString(), "5");

            b.Execute(10);
            c.Execute();
            Assert.AreEqual(c.marking.value.ToString(), "11");

        }



        [TestMethod]
        public void InitialValueTest()
        {
            string input = @"
!A(3)[?],
!B[?],
!C(1)[freshid()];";
            var p = new ResedaParser();
            p.dispTree(input);
            var term = p.Generate(input);
            InputEvent a = (InputEvent)term.subProcess.structuredData[0];
            InputEvent b = (InputEvent)term.subProcess.structuredData[1];
            OutputEvent c = (OutputEvent)term.subProcess.structuredData[2];
            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            var term2 = p.Generate(term.subProcess.ToSource());
            Assert.AreEqual(term.ToSource(), term2.ToSource());

            Assert.AreEqual(c.marking.value.ToString(), "1");
            Assert.AreEqual(a.marking.value.ToString(), "3");
            Assert.IsNull(b.marking.value);
            c.Execute();
            Assert.AreEqual(c.marking.value.ToString(), "4");
            b.Execute(1);
            Assert.AreEqual(c.marking.value.ToString(), "4");
            a.Execute(5);
            Assert.AreEqual(a.marking.value.ToString(), "5");
            c.Execute();
            Assert.AreEqual(c.marking.value.ToString(), "6");

            /*
            c.Execute();
            System.Diagnostics.Debug.WriteLine(c.ToSource());
            System.Diagnostics.Debug.WriteLine(c.marking.value);
            Assert.AreEqual(c.marking.value.ToString(), "0");
            c.Execute();
            Assert.AreEqual(c.marking.value.ToString(), "1");
            a.Execute(3);
            c.Execute();
            Assert.AreEqual(c.marking.value.ToString(), "4");

            b.Execute(1);
            c.Execute();
            Assert.AreEqual(c.marking.value.ToString(), "5");

            b.Execute(10);
            c.Execute();
            Assert.AreEqual(c.marking.value.ToString(), "11");
            */
        }

        [TestMethod]
        public void SpawnTrigger()
        {
            string input = @"A[?],
B[?]
;
A -->> {C[@trigger:v];}";
            var p = new ResedaParser();
            p.dispTree(input);
            var term = p.Generate(input);
            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            var term2 = p.Generate(term.subProcess.ToSource());
            Assert.AreEqual(term.ToSource(), term2.ToSource());
            InputEvent a = (InputEvent)term.subProcess.structuredData[0];
            InputEvent b = (InputEvent)term.subProcess.structuredData[1];
            a.Execute(15);
            OutputEvent c = (OutputEvent)term.subProcess.structuredData[2];
            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
        }


        [TestMethod]
        public void SpawnTriggerInitialValue()
        {
            string input = @"A(3)[?],
B[?]
;
A -->> {C(@trigger:v)[?];}, 
B -->> {C(@trigger:v)[?];}";
            var p = new ResedaParser();
            p.dispTree(input);
            var term = p.Generate(input);
            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            var term2 = p.Generate(term.subProcess.ToSource());
            Assert.AreEqual(term.ToSource(), term2.ToSource());
            InputEvent a = (InputEvent)term.subProcess.structuredData[0];
            InputEvent b = (InputEvent)term.subProcess.structuredData[1];            
            Assert.AreEqual(a.marking.value.ToString(), "3");
            Assert.IsNull(b.marking.value);
            a.Execute(15);
            InputEvent c = (InputEvent)term.subProcess.structuredData[2];
            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            Assert.AreEqual(c.marking.value.ToString(), "15");
            Assert.IsNull(b.marking.value);
            b.Execute(45);
            InputEvent c2 = (InputEvent)term.subProcess.structuredData[3];
            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            Assert.AreEqual(c2.marking.value.ToString(), "45");
            Assert.AreEqual(b.marking.value.ToString(), "45");
            Assert.AreEqual(a.marking.value.ToString(), "15");
            Assert.AreEqual(c.marking.value.ToString(), "15");
        }


        [TestMethod]
        public void GuardedCondition()
        {
            string input = @"A[],
B[],
D[],
C[];
A[(count(@/C)==0)] -->* B,
D -->* C
";
            var p = new ResedaParser();
            p.dispTree(input);
            var term = p.Generate(input);
            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            var term2 = p.Generate(term.subProcess.ToSource());
            Assert.AreEqual(term.ToSource(), term2.ToSource());
            OutputEvent a = (OutputEvent)term.subProcess.structuredData[0];
            OutputEvent b = (OutputEvent)term.subProcess.structuredData[1];
            OutputEvent d = (OutputEvent)term.subProcess.structuredData[2];
            OutputEvent c = (OutputEvent)term.subProcess.structuredData[3];
            Assert.IsTrue(b.IsEnabled());
            b.Execute();

            Assert.IsFalse(c.IsEnabled());
            Assert.ThrowsException<Exception>(() =>
            {
                c.Execute();
            });                
        }


        [TestMethod]
        public void CloneFilterTest()
        {
            string input = @"A[],
B[],
D[],
C[];
A[(count(@/C)==0)] -->* B,
D -->* C
";
            var p = new ResedaParser();
            p.dispTree(input);
            var term = p.Generate(input);
            var term3 = term.Clone(null);
            Assert.AreEqual(term.ToSource(), term3.ToSource());
            term = term3;

            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            var term2 = p.Generate(term.subProcess.ToSource());
            Assert.AreEqual(term.ToSource(), term2.ToSource());
            OutputEvent a = (OutputEvent)term.subProcess.structuredData[0];
            OutputEvent b = (OutputEvent)term.subProcess.structuredData[1];
            OutputEvent d = (OutputEvent)term.subProcess.structuredData[2];
            OutputEvent c = (OutputEvent)term.subProcess.structuredData[3];
            Assert.IsTrue(b.IsEnabled());
            b.Execute();

            Assert.IsFalse(c.IsEnabled());
            Assert.ThrowsException<Exception>(() =>
            {
                c.Execute();
            });
        }



    }
}
