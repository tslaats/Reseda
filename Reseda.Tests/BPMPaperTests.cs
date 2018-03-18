using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reseda.Parser;
using Reseda.Core;

namespace Reseda.Tests
{
    [TestClass]
    public class BPMPaperTests
    {
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
        }


        [TestMethod]
        public void BPMExample2()
        {
            string input = @"customers[]{
    create_customer[?],
    customer[] {
      customer_id[0],
      customer_name['John']  
    },
    customer[]{
      customer_id[1],
      customer_name['Mary']
    }
    ~
    create_customer -->> {
        customer[]{
            !customer_id[freshid()],
            !customer_name[?:string]
}
    }
}
";
            var p = new ResedaParser();
            p.dispTree(input);
            var term = p.Generate(input);
            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            var term2 = p.Generate(term.subProcess.ToSource());
            Assert.AreEqual(term.ToSource(), term2.ToSource());
        }

        [TestMethod]
        public void BPMExample3()
        {
            string input = @"products[]{
    create_product[?]
    ~
    create_product -->>{
        product[]{
            !product_id[freshid()],
            !product_name[?:string]
        }
    }
}";
            var p = new ResedaParser();
            p.dispTree(input);
            var term = p.Generate(input);
            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            var term2 = p.Generate(term.subProcess.ToSource());
            Assert.AreEqual(term.ToSource(), term2.ToSource());
        }


        [TestMethod]
        public void BPMExample4()
        {
            string input = @"orders[]{
    create_order[?:@../customers/customer/customer_id:value]
    ~
    create_order -->>{
        order[]{
            !order_id[freshid()],
            !customer_id[@trigger:value],
            !pick_item[?:@../../products/product/product_id:value]
            ~
            pick_item -->>{
                order_line[]{
                    !order_line_id[freshid()],
                    !product_id[@trigger:value],
                    !wrap_item[?:@order_line_id:value]
                    ~
                    wrap_item -->% wrap_item
                }
            }
        }
    }
}
";
            var p = new ResedaParser();
            p.dispTree(input);
            var term = p.Generate(input);
            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            var term2 = p.Generate(term.subProcess.ToSource());
            Assert.AreEqual(term.ToSource(), term2.ToSource());
        }

        [TestMethod]
        public void BPMExample5()
        {
            string input = @"deliveries[]{
   ~   
   ../orders/create_order -->> { 
     delivery[]{
       delivery_id[freshid()],
       deliver_items[?:@trigger:value],
       customer_id[@trigger:value],
       items_to_deliver[]{
         ~
         /orders/order[@customer_id:value==@trigger:value]/order_line/wrap_item[@/deliver_items:included] 
             -->> {
                 !item_id[@trigger:value]
             }
       }
       ~
       deliver_items -->% deliver_items,
       deliver_items[count(@items_to_deliver/*)==0] -->* deliver_items 
    }
  },
   delivery/deliver_items -->> { 
     delivery[]{
       delivery_id[freshid()],
       deliver_items[?:@trigger:value],
       customer_id[@trigger:value],
       items_to_deliver[]{
         ~
         /orders/order[@customer_id:value==@trigger:value]/order_line/wrap_item[@/deliver_items:included] 
             -->> {
                 !item_id[@trigger:value]
             }
       }
       ~
       deliver_items -->% deliver_items,
       deliver_items[count(@items_to_deliver/*)==0] -->* deliver_items 
    }
  }  
}



    ";
            var p = new ResedaParser();
            p.dispTree(input);
            var term = p.Generate(input);
            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            var term2 = p.Generate(term.subProcess.ToSource());
            Assert.AreEqual(term.ToSource(), term2.ToSource());
        }


        [TestMethod]
        public void BPMTestThomas()
        {
            string input = @"orders[]{
 create_order[?:@/customers/customer/customer_id:value]
 ;
 create_order -->>{
 order[]{
 !order_id[freshid()],
 !customer_id[@trigger:value]
 }
}
},
customers[]{
 create_customer[?],
 customer[] {
 customer_id(0)[],
 customer_name('John')[] 
 },
 customer[]{
 customer_id(1)[],
 customer_name('Mary')[]
 }
 ;
 create_customer -->> {
 customer[]{
 !customer_id[freshid()],
 !customer_name[?:string]
 }
 }
}";
            var p = new ResedaParser();
            p.dispTree(input);
            var term = p.Generate(input);
            System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            var term2 = p.Generate(term.subProcess.ToSource());

            //Assert.AreEqual(term.ToSource(), term2.ToSource());
            InputEvent create_order = (InputEvent)term.subProcess.structuredData[0].subProcess.structuredData[0];

            create_order.Execute(1);

            System.Diagnostics.Debug.WriteLine(term.ToSource());

            OutputEvent o1 = (OutputEvent)term.subProcess.structuredData[0].subProcess.structuredData[1].subProcess.structuredData[0];
            OutputEvent o2 = (OutputEvent)term.subProcess.structuredData[0].subProcess.structuredData[1].subProcess.structuredData[1];

            o1.Execute();
            o2.Execute();
            System.Diagnostics.Debug.WriteLine(term.ToSource());

        }


    }
}


