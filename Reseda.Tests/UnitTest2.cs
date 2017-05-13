using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reseda.Parser;
using Reseda.Core;

namespace Reseda.Tests
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            string input = "A< 2 * 2 + 4 >," +
                           "B<3 + 4 + 5 * 6 + 7 + 8 + 9>{C<a + (3 * 5) - @c/d>;}" +
                           "; A -->* /B," +
                           " A/*/C -->* /B/.././F," 
                           + "B -->% *";
            var p = new ResedaParser();
            p.dispTree(input);

            //System.Diagnostics.Debug.WriteLine(p.Generate(input).PrintTree());
                        
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
        }
    }
}
