


open System.Text
open System.IO
open System.Runtime.Serialization
open System.Runtime.Serialization.Json

open System.Collections.Concurrent

open Suave
open Suave.Filters
open Suave.Successful
open Suave.Operators
open Suave.Json
open Suave.Writers
open Suave.Utils


(* DATABASE, HELPERS *)


let internal to_json<'t> (myObj:'t) =   
        use ms = new MemoryStream() 
        (new DataContractJsonSerializer(typeof<'t>)).WriteObject(ms, myObj) 
        Encoding.Default.GetString(ms.ToArray()) 


let db = 
  ConcurrentDictionary<int, Reseda.Core.RootEvent>()


let clip src = 
  sprintf "%d bytes '%s...'"
    (String.length src) 
    (src.Substring(0, min (String.length src) 32).Replace('\n', ' ')) 


let store (P : Reseda.Core.RootEvent) classifier = 
  let src = P.ToSource()
  // C# implementation does not have useful (=recursive) GetHashCode(), likely
  // because of mutability. 
  let h = src.GetHashCode() + P.PrintTree(true).GetHashCode()
  db.AddOrUpdate(h, P, fun _ _ -> P)  |> ignore
  printfn "[%s] %s -> %u" classifier (clip src) h
  OK (sprintf "%u" h)


let with_term f id = 
  printfn "[API] Access at #%u" id
  match db.TryGetValue id with
  | false, _ -> 
      printfn "[API] Unknown #%u" id
      RequestErrors.NOT_FOUND <| string id
  | true, P -> 
      request (fun req -> 
        try
          f id P req
        with 
        | x -> 
            printfn "[ERR] %s" x.Message
            RequestErrors.BAD_REQUEST x.Message)
 

let parser = 
  Reseda.Parser.ResedaParser()


store (parser.Generate "A[?], B[@A:v+2]" :?> Reseda.Core.RootEvent) "INIT" |> ignore
// Hash of default program is 2714325035


(* API ENDPOINT IMPLEMENTATIONS *)


let parse src =  
  try 
    let P = (parser.Generate src) :?> Reseda.Core.RootEvent
    store P "PARSER" 
  with 
    | x -> 
        RequestErrors.BAD_REQUEST x.Message


let eval_path _ P (req : HttpRequest) = 
  match req.queryParamOpt "expr" with
  | Some (_, Some expr) -> 
      printfn "[PATH] '%s'" expr
      parser.GeneratePath(expr).Eval(P)
        |> Seq.map (fun e -> e.ToString())
        |> Array.ofSeq
        |> to_json
        |> OK
  | _ -> 
      printfn "[PATH] ? '%s'" req.rawQuery
      RequestErrors.BAD_REQUEST req.rawQuery
        

type Show = 
  { src : string
    tree : string 
  }

let show _ (P : Reseda.Core.RootEvent) _ = 
  { src = P.ToSource()
    tree = P.PrintTree(true) 
  } 
  |> to_json
  |> OK


type Analysis = 
  { bounded : bool
    live : bool
  }

let analyse _ (P : Reseda.Core.RootEvent) _ =
  let P = P :> Reseda.Core.Event
  { 
    bounded = P.Bounded()
    live = P.ProcessIsLive()
  } |> to_json |> OK


let antiGlitch _ (P : Reseda.Core.RootEvent) _ = 
  store <| P.MakeGlitchFree() <| "GLITCH" 


let antiPar _ (P : Reseda.Core.RootEvent) _ = 
  store <| P.MakeInSeq() <| "SEQ"


let exec id (Q : Reseda.Core.RootEvent) (req : HttpRequest) = 
  try
    let P = Q.Clone(null) :?> Reseda.Core.RootEvent
    // Figure out which event we're executing
    let path = (req.queryParamOpt "event" |> Option.bind snd).Value
    printfn "'%s'" path
    let matches = parser.GeneratePath(path).Eval(P)
    if matches.Count <> 1 then
      failwithf "Path identifies %d event, I can execute only one." <| matches.Count
    let evt = Seq.head <| matches
    // Figure out what type this event has, provide a value if necessary, and
    // execute it
    match evt with 
    | :? Reseda.Core.OutputEvent as oevt -> 
      oevt.Execute()
    | :? Reseda.Core.InputEvent as ievt -> 
      match req.queryParamOpt "value" |> Option.bind snd with
      | None -> failwithf "Event '%s' requires an input value to execute" path
      | Some x -> 
          match bool.TryParse x with 
          | true, b -> ievt.Execute(b)
          | false, _ -> ievt.Execute(int x)
    | _ -> 
      failwithf "The root event can never be executed"
    // Store the event
    store P "EXEC" 
  with
  | x -> 
      printfn "[EXEC] ? '%s'" x.Message
      RequestErrors.BAD_REQUEST x.Message
           
    
(* SERVER *)

let asset = 
    sprintf "../../../frontend/build/%s" >> Files.file

let app =
  choose [ 
    POST >=> path "/reseda/api/parse"
         >=> request (fun req -> parse (ASCII.toString req.rawForm))
    GET >=> pathScan "/reseda/api/%u/path" (with_term eval_path)
    GET >=> pathScan "/reseda/api/%u/term" (with_term show)
    GET >=> pathScan "/reseda/api/%u/exec" (with_term exec)
    GET >=> pathScan "/reseda/api/%u/antiglitch" (with_term antiGlitch)
    GET >=> pathScan "/reseda/api/%u/antipar" (with_term antiPar)
    GET >=> pathScan "/reseda/api/%u/analysis" (with_term analyse)
    // Static assets (for development without npm). Note that static 
    // assets must be available at the relative path specified in 
    // `asset` above. 
    GET >=> pathScan "/reseda/%s" asset
    GET >=> path "/index.html" >=> asset "index.html"
    GET >=> path "/" >=> asset "index.html"
    RequestErrors.NOT_FOUND "Page not found." 
  ] 

startWebServer defaultConfig app
