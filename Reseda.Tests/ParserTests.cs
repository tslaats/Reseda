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
            string input = "A< 2 * 2 + 4 >," +
                           "B<3 + 4 + 5 * 6 + 7 + 8 + 9>{C<1 + (3 * 5) - @c/d>;}" +
                           "; A -->* /B," +
                           " A/*/C -->* /B/.././F," 
                           + "B -->% *";
            var p = new ResedaParser();
            p.dispTree(input);

            var term = p.Generate(input);
            var term2 = p.Generate(term.ToSource());

            Assert.AreEqual(term.ToSource(), "A<2 * 2 + 4>,B<3 + 4 + 5 * 6 + 7 + 8 + 9>{C<1 + 3 * 5 - @c/d>;};A -->* /B,A/*/C -->* /B/.././F,B -->% *");
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
            string input = "A<?>," +
                           "B<?>," +
                           "C<@A:v + @B:v * 10>," +
                           "D<!>," +
                           "E<!>" +
                           "; A -->* B," +
                           " D -->> {N<?>;}," +
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
        public void CurrentHackingTest2()
        {
            string input = "A<?>," +
                           "B<?>," +
                           "C<@A:v + @B:v * 10>," +
                           "D<!>," +
                           "E<!>" +
                           "; A -->* B[1]," +
                           " D -->> {N<?>;}," +
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

    }
}
