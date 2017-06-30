using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reseda.Parser;
using Reseda.Core;

namespace Reseda.Tests
{
    [TestClass]
    public class PaperTests
    {
        [TestMethod]
        public void Example1()
        {
            string input = @"!name[?],
!price[?],
!quantity[?],
amount[@price:value * @quantity:value],
invoiceRow[@name:value + ';' + @price:value + ';' + @quantity:value + ';' + @amount:value]";
            var p = new ResedaParser();
            p.dispTree(input);
            var term = p.Generate(input);
            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            var term2 = p.Generate(term.subProcess.ToSource());
            Assert.AreEqual(term.ToSource(), term2.ToSource());
        }
    }
}
