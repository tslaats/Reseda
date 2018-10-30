using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reseda.Parser;
using Reseda.Core;

namespace Reseda.Tests
{
    [TestClass]
    public class EDOCPaperTests
    {
        /*
        [TestMethod]
        public void BPMExample1()
        {
            string input = @"customer[] {
  customer_id(0)[0],
  customer_name['John']  
},
customer[]{
  customer_id[1],
  customer_name['Mary']
    }";
            var p = new ResedaParser();
            p.dispTree(input);
            var term = p.Generate(input);
            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            var term2 = p.Generate(term.subProcess.ToSource());
            Assert.AreEqual(term.ToSource(), term2.ToSource());
        }*/


        // General tests related to additional functionality included in the EDOC 2018 paper:

        [TestMethod]
        public void InputTypeTest()
        {
            string input = @"customers[]{
    create_customer[?],
    customer[] {
      customer_id(0)[?],
      customer_name('John')[?]  
    },
    customer[]{
      customer_id(1)[?],
      customer_name('Mary')[?]
    }
    ~
    create_customer -->> {
        customer[]{
            !customer_id[freshid()],
            !customer_name[?]
}
    }
},
viewcustomer[?:@/customers/customer/customer_id:value]
";
            var p = new ResedaParser();
            p.dispTree(input);
            var term = p.Generate(input);
            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            var term2 = p.Generate(term.subProcess.ToSource());
            //Assert.AreEqual(term.ToSource(), term2.ToSource());

            InputEvent viewcustomer = (InputEvent)term.subProcess.structuredData[1];

            System.Diagnostics.Debug.WriteLine(viewcustomer.ValidInputs().ToString());
            viewcustomer.Execute(1);
        }

    }
}


