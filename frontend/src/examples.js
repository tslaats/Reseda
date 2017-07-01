export const examples = 
  { 'Paper examples': ""
  , 'Simple example': `A[?], B[@A:v+1]`
  , 'Arrows':         `A[], B[]; A -->* B`
    , 'Nesting':        
`A[] { B[] }, 
C[] { 
    B[]
    ; 
    B *--> ../A 
}
; 
A/B -->* C/B`
  }
