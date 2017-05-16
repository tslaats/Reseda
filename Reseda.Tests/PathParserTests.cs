using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reseda.Parser;
using Reseda.Core;

namespace Reseda.Tests
{
    [TestClass]
    public class PathParserTests
    {
        [TestMethod]
        public void BasicPathParserTest()
        {
            string input = "A/B";
            var p = new ResedaParser();
            var path = p.GeneratePath(input);            
            var path2 = p.GeneratePath(path.ToSource());

            Assert.AreEqual(path.ToSource(), "A/B");
            Assert.AreEqual(path.ToSource(), path2.ToSource());            
            System.Diagnostics.Debug.WriteLine(path.ToSource());
        }
    }
}
