export const examples = 
{ 'Choose an example process': '' 
,'[Fig 4] Data, path expressions': `itemInCart[]{
  !name[?],
  !price[?], 
  !quantity[?],
  amount[@price:value * @quantity:value],
  invoiceRow[@name:value + ';' + @price:value + ';' + 
             @quantity:value + ';' + @amount:value]
  ~
  * --><> ../checkout      
},
checkout[?],
ship[?],
printInvoice[@itemInCart/invoiceRow:value]
~
checkout -->%  itemInCart,
checkout -->* ship,
checkout *--> ship,
ship -->% checkout,
ship -->* printInvoice,
ship *--> printInvoice
`
,'[Step-3]-Order-collection-new': `orders[]{
    create_order[?:@/customers/customer/customer_id:value]
    ;
    create_order -->>{
        order[]{
            !order_id[freshid()],
            !customer_id[@trigger:value],
            !pick_item[?:@/products/product/product_id:value]
            ;             
            pick_item -->>{
                order_line[]{
                    !order_line_id[freshid()],
                    !item[@trigger:value],
                    !wrap_item[?:@/deliveries/
                                   delivery[not(deliver_items:executed)]/
                                   delivery_id:value]
                    ;
                    wrap_item -->* order_line_id,
                    item -->* order_line_id,        
                    wrap_item -->% wrap_item 
                }
            }
        }
    }
}`
,'[Fig 2] Input and computational events': `!name[?],
!price[?],
!quantity[?],
amount[@price:value * @quantity:value],
invoiceRow[@name:value + ';' + @price:value + ';' + 
           @quantity:value + ';' + @amount:value]
`
,'[Step 4] Delivery collection': `deliveries[]{
   ;   
   (../orders/create_order OR 
    delivery/deliver_items    ) -->> { 
     delivery[]{
       delivery_id[freshid()],
       deliver_items[?:@trigger:value],
       customer_id[@trigger:value],
       items_to_deliver[]{
         ;
         /orders/order[@customer_id:value==@trigger:value]
           /order_line/wrap_item[@/deliver_items:included] 
             -->> {
                 !item_id[@trigger:value]
             }
       }
       ;
       deliver_items -->% deliver_items,
       deliver_items[count(@items_to_deliver/*)==0] -->* deliver_items 
    }
  }
}`
,'[Fig 5] Iteration, spawn': `addItem[?],
checkout[?],
ship[?],
printInvoice[@invoiceRow]
~
addItem -->> {
  itemInCart[]{  
    !name[?],
    !price[?], 
    !quantity[?],
    amount[@price:value * @quantity:value]        
    ~
    * --><> ../checkout
  }
},
checkout -->%  itemInCart,
checkout -->* ship,
checkout *--> ship,
ship -->% checkout,
ship -->* printInvoice,
ship *--> printInvoice,
checkout -(p in itemInCart)->> {  
    invoiceRow[@p/name:value + ';' + @p/price:value + ';' + 
               @p/quantity:value + ';' + @p/amount:value]
    ~  
}
`
,'[Fig 3] Relations': `!name[?],
!price[?],
!quantity[?],
amount[@price:value * @quantity:value],
invoiceRow[@name:value + ';' + @price:value + ';' + 
           @quantity:value + ';' + @amount:value],
checkout[?],
deliver_items[?]
~
name --><> checkout,
price --><> checkout,
quantity --><> checkout, 
checkout -->% name,
checkout -->% price,
checkout -->% quantity,
checkout -->* ship,
checkout *--> ship,
ship -->% checkout
`
,'[Step 3] Order collection': `orders[]{
    create_order[?:@../customers/customer/customer_id:value]
    ;
    create_order -->>{
        order[]{
            !order_id[freshid()],
            !customer_id[@trigger:value],
            !pick_item[?:@../../products/product/product_id:value]
            ;
            pick_item -->>{
                order_line[]{
                    !order_line_id[freshid()],
                    !product_id[@trigger:value],
                    !wrap_item[?:@order_line_id:value]
                    ;
                    wrap_item -->% wrap_item
                }
            }
        }
    }
}`
,'[Step 2] Product collection': `products[]{
    create_product[?:string]
    ;
    create_product -->>{
        product[]{
            !product_id[freshid()],
            !product_name[@trigger:value]
        }
    }
}`
,'[Step 0] Events': `customer[] {
  customer_id(0)[],
  customer_name('John')[]  
},
customer[]{
  customer_id(1)[],
  customer_name('Mary')[]
}
`
,'[Step 1] Customer collection': `customers[]{
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
}`
,'[Step-4]-Delivery-collection-new': `deliveries[]{
create_delivery[?:@/customers/customer/customer_id:value]
   ;   
  create_delivery -->> { 
     delivery[]{
       !delivery_id[freshid()],
       !customer_id[@trigger:value],
       !%deliver_items[?]
         ;
         /orders/order[@customer_id:value==@trigger:value]
           /order_line[@wrap_item:value==@rule/delivery_id:value]/order_line_id
             -->> {
                 !item_id[@trigger:value]
                 },
         item_id -->+ deliver_items,
         deliver_items -->% deliver_items
    }
  }
}`
,'[Step 4] Delivery collection - Working Syntax': `deliveries[]{
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

`
}
