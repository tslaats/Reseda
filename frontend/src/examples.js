export const examples = 
{ 'Choose an example process': '' 
,'[Tutorial 1] Input, computation': `A[?], B[@A:v+1]
`
,'[Tutorial 2] Constraint': `A[], B[]; A -->* B
`
,'[Tutorial 3] Nesting': `A[] { B[] }, 
C[] { 
    B[]
    ; 
    B *--> ../A 
}
; 
A/B -->* C/B
`
,'[Fig 2] Input and computational events': `!name[?],
!price[?],
!quantity[?],
amount[@price:value * @quantity:value],
invoiceRow[@name:value + ';' + @price:value + ';' + 
           @quantity:value + ';' + @amount:value]
`
,'[Fig 3] Relations': `!name[?],
!price[?],
!quantity[?],
amount[@price:value * @quantity:value],
invoiceRow[@name:value + ';' + @price:value + ';' + 
           @quantity:value + ';' + @amount:value],
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
checkout *--> ship,
ship -->% checkout
`
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
}
