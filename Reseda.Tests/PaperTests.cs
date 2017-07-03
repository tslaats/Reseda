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
            string input = @"# First step:
# ** Input and computational events
!name[?],
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


        [TestMethod]
        public void Example2()
        {
            string input = @"!name[?],
!price[?],
!quantity[?],
amount[@price:value * @quantity:value],
invoiceRow[@name:value + ';' + @price:value + ';' + @quantity:value + ';' + @amount:value],
checkout[?],
ship[?]
~
name --><> checkout,
price --><> checkout,
quantity --><> checkout, 
checkout -->% name,
checkout -->% price,
checkout -->% quantity,
checkout -->* ship,
checkout*--> ship,
ship -->% checkout
";
            var p = new ResedaParser();
            p.dispTree(input);
            var term = p.Generate(input);
            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            var term2 = p.Generate(term.subProcess.ToSource());
            Assert.AreEqual(term.ToSource(), term2.ToSource());
        }

        [TestMethod]
        public void Example3()
        {
            string input = @"itemInCart[!]{
  data[!]{
        !name[?],
        !price[?], 
        !quantity[?],
        amount[@price:value * @quantity:value],
        invoiceRow[@name:value + ';' + @price:value + ';' + @quantity:value + ';' + @amount:value]
        ~
        * --><> ../../checkout
      }
},
checkout[?],
ship[?],
%printInvoice[@itemInCart/data/invoiceRow]
~
checkout -->%  itemInCart/data,
checkout -->* ship,
checkout *--> ship,
ship -->% checkout,
ship -->+ printInvoice,
ship *--> printInvoice";
            var p = new ResedaParser();
            p.dispTree(input);
            var term = p.Generate(input);
            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            var term2 = p.Generate(term.subProcess.ToSource());
            Assert.AreEqual(term.ToSource(), term2.ToSource());
        }


        [TestMethod]
        public void Example4()
        {
            string input = @"addItem[?],
checkout[?],
ship[?],
%printInvoice[@itemInCart/data/invoiceRow]
~
addItem -->> {itemInCart[]{
  data[]{
        !name[?],
        !price[?], 
        !quantity[?],
        amount[@price:value * @quantity:value]        
        ~
        * --><> ../../checkout
      }
}},
checkout -->%  itemInCart/data,
checkout -->* ship,
checkout *--> ship,
ship -->% checkout,
ship -->+ printInvoice,
ship *--> printInvoice,
checkout -(p in itemInCart)->> 
  {shipping[] { 
    invoiceRow[@p/name:value + ';' + @p/price:value + ';' + @p/quantity:value + ';' + @p/amount:value]
    ;    
  }}
";
            var p = new ResedaParser();
            p.dispTree(input);
            var term = p.Generate(input);
            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            var term2 = p.Generate(term.subProcess.ToSource());
            Assert.AreEqual(term.ToSource(), term2.ToSource());
        }

    }
}
