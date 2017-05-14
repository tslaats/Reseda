using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reseda.Parser;
using Reseda.Core;

namespace Reseda.Tests
{
    [TestClass]
    public class UnitTest3
    {
        [TestMethod]
        public void ComputeTest()
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
    }
}
