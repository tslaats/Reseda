export const examples = 
{ 'Choose an example process': '' 
,'[Example 4] Orders collection definition': `orders[]{
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
                 delivery[!@deliver_items:included]/delivery_id:value
          ]
          ;
          wrap_item -->* order_line_id,
          item -->* order_line_id,        
          wrap_item -->% wrap_item 
        }
      }
    }
  }
}
`
,'[Example 1] Modelling structured data using constant values': `customer[] {
  customer_id(0)[],
  customer_name('John')[]  
},
customer[]{
  customer_id(1)[],
  customer_name('Mary')[]
}
`
,'[Example 2a] Dynamically created customers collection': `customers[]{
    create_customer[?]
    ;
    create_customer -->> {
        customer[]{
            !customer_id[freshid()],
            !customer_name[?:string]
        }
    }
}`
,'[Example 5] Deliveries collection definition': `deliveries[]{
  create_delivery[?:
    @/customers/customer/customer_id:value
  ]
  ;   
  create_delivery -->> { 
   delivery[]{
     !delivery_id[freshid()],
     !customer_id[@trigger:value],
     !%deliver_items[?]
     ;
     /orders/order[
        @customer_id:value==@trigger:value
     ]/order_line[
        @wrap_item:value==@rule/delivery_id:value
     ]/order_line_id
         -->> { !item_id[@trigger:value] },
     item_id -->+ deliver_items,
     deliver_items -->% deliver_items
    }
  }
}

`
,'[Example 2c] Computation sequence': `customers[]{
    create_customer[?],
    customer[]{
      customer_id(0)[freshid()],
      !customer_name[?:string]
    }
    ;
    create_customer -->> {
        customer[]{
            !customer_id[freshid()],
            !customer_name[?:string]
        }
    }
}`
,'[Example 2d] Computation Sequence': `customers[]{
    create_customer[?],
    customer[]{
      customer_id(0)[freshid()],
      customer_name('John')[?:string]
    }
    ;
    create_customer -->> {
        customer[]{
            !customer_id[freshid()],
            !customer_name[?:string]
        }
    }
}`
,'[Example 2b] Computation Sequence': `customers[]{
    create_customer[?],
    customer[]{
      !customer_id[freshid()],
      !customer_name[?:string]
    }
    ;
    create_customer -->> {
        customer[]{
            !customer_id[freshid()],
            !customer_name[?:string]
        }
    }
}`
,'[Example 3b] Dynamically created products and trigger path expression': `products[]{
    create_product[?:string],
    product[]{
            !product_id[freshid()],
            !product_name['iPhone X']
        }
    ;
    create_product -->>{
        product[]{
            !product_id[freshid()],
            !product_name[@trigger:value]
        }
    }
}`
,'[Example 3] Dynamically created products and trigger path expression': `products[]{
    create_product[?:string]
    ;
    create_product -->>{
        product[]{
            !product_id[freshid()],
            !product_name[@trigger:value]
        }
    }
}`
,'[Example 2] Dynamically created customers collection': `customers[]{
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
}
