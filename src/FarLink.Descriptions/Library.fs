namespace FarLink.Descriptions

open System
open System.Collections.Immutable

type Identifier = Identifier
type QualifiedIdentifier = QualifiedIdentifier


type IServiceDesc<'service when 'service :> IServiceDesc<'service> >  =
    abstract Name : QualifiedIdentifier
    
type IEventDesc<'service when 'service :> IServiceDesc<'service> >  =
    abstract Name : Identifier
    abstract MsgType : Type
    
    
type IEventDesc<'service, 'msg when 'service :> IServiceDesc<'service> >  =
    inherit IEventDesc<'service>

    
type ICallDesc<'service when 'service :> IServiceDesc<'service> >  =
    abstract Name : Type
    
    
    
type ICallDesc<'service, 'req, 'resp when 'service :> IServiceDesc<'service> >  =
    inherit ICallDesc<'service>
    
    
type Endpoint<'service when 'service :> IServiceDesc<'service>> =
    | Event of  IEventDesc<'service>
    | Call of ICallDesc<'service>
    
module Discriptions =

    let private memoise<'k, 'v> (f : 'k -> 'v) =
        let mutable cashe = ImmutableDictionary<'k, 'v>.Empty
        fun t ->
            match cashe.TryGetValue(t) with
            | true, v -> v
            | false, _ ->
                cashe <- cashe.SetItem(t, f t)
                cashe.[t]
    let endpoints<'service when 'service :> IServiceDesc<'service>>  =
        let ep (t : Type) =
            let assembly = t.Assembly
            assembly.GetTypes() 
                |> Seq.filter 
                    (fun p -> 
                        typeof<IEventDesc<'service>>.IsAssignableFrom p ||
                        typeof<ICallDesc<'service>>.IsAssignableFrom p)
                |> Seq.map (fun p -> Activator.CreateInstance p)
                |> Seq.map  
                    (fun p ->
                        match p with
                        | :? IEventDesc<'service> as ev -> Event ev
                        | :? ICallDesc<'service> as c -> Call c)
                |> Seq.toList
        (memoise ep) (typeof<'service>) 
                        
                    
                     
        
        
    
    
