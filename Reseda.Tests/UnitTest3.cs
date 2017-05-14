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
                           " D -->> N<?>;," +
                           "E -->% *";
            var p = new ResedaParser();
            p.dispTree(input);

            //System.Diagnostics.Debug.WriteLine(p.Generate(input).PrintTree());

            var term = p.Generate(input);

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
        }
    }
}
