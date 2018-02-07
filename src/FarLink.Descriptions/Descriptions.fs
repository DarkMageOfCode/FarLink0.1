namespace rec FarLink.Descriptions

open System
open System.Collections.Immutable
open System.Text.RegularExpressions

type Identifier = private Identifier of string
type QualifiedIdentifier = 
    private {
        ns : QualifiedIdentifier option
        name : Identifier
    }
    member __.Namespace = __.ns
    member __.Name = __.name

module Identifier =
    let private idRegExp = Regex("^[A-Za-z][A-Za-z0-9_]*$");
    let create name = 
        if (idRegExp.IsMatch name) then 
            Identifier name
        else
            invalidArg "name" "Invalid identifier"
    let createQualified (name : string) = 
        let rec fromList (arr : string list) =
            match  arr with
            | [] -> invalidArg "name" "Empty qualified identifier"
            | [a] -> {ns = None; name = create a }
            | h :: r -> {ns = fromList r |> Some; name = create h }
        let splitted = name.Split('.') |> List.ofArray |> List.rev
        fromList splitted
        


type IServiceDesc<'service 
        when 'service :> IServiceDesc<'service> 
            and 'service : (new : unit -> 'service)
            and 'service : struct > = interface end
    
    
type IEventDesc<'service 
        when 'service :> IServiceDesc<'service> 
        and 'service : (new : unit -> 'service)
        and 'service : struct>  = interface end
    
    
    

type IEventDesc<'ed, 'service, 'msg 
    when 'service :> IServiceDesc<'service> 
        and 'service : (new : unit -> 'service)
        and 'service : struct
        and 'ed :> IEventDesc<'ed, 'service, 'msg> 
        and 'ed : (new: unit -> 'ed)
        and 'ed : struct >   = 
        inherit IEventDesc<'service>

    
type ICallDesc<'service 
    when 'service :> IServiceDesc<'service> 
        and 'service : (new : unit -> 'service)
        and 'service : struct>  = interface end
    
    
    

type ICallDesc<'cd, 'service, 'req, 'resp 
    when 'service :> IServiceDesc<'service>
        and 'service : (new : unit -> 'service)
        and 'service : struct
        and 'cd :> ICallDesc<'cd, 'service, 'req, 'resp>
        and 'cd : (new : unit -> 'cd)
        and 'cd : struct >  =
    inherit ICallDesc<'service>
    
    
type Endpoint<'service 
        when 'service :> IServiceDesc<'service> 
            and 'service : (new : unit -> 'service)
            and 'service : struct > =
    | Event of  IEventDesc<'service>
    | Call of ICallDesc<'service>
    (*member __.Name =
        match __ with
        | Event ev -> ev.Name
        | Call cv -> cv.Name*)
    
module Descriptions =

    //let eventName<'ev> 
    //     =


    let private memoise<'k, 'v> (f : 'k -> 'v) =
        let mutable cashe = ImmutableDictionary<'k, 'v>.Empty
        fun t ->
            match cashe.TryGetValue(t) with
            | true, v -> v
            | false, _ ->
                cashe <- cashe.SetItem(t, f t)
                cashe.[t]
    let endpoints<'service when 'service :> IServiceDesc<'service> and 'service : (new : unit -> 'service) and 'service : struct>  =
        let ep (t : Type) =
            let assembly = t.Assembly
            let createEndpoint (ep : obj) =
                match ep with
                | :? IEventDesc<'service> as ev -> Event ev
                | :? ICallDesc<'service> as c -> Call c
                | _ -> invalidOp (sprintf "Unknown endpoint type %A" ep)
            assembly.GetTypes() 
                |> Seq.filter 
                    (fun p -> 
                        typeof<IEventDesc<'service>>.IsAssignableFrom p ||
                        typeof<ICallDesc<'service>>.IsAssignableFrom p)
                |> Seq.map (Activator.CreateInstance >> createEndpoint)
                |> Seq.toList
        (memoise ep) (typeof<'service>) 
                        
                    
                     
        
        
    
    
