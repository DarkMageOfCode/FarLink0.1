namespace FarLink
open System
open System.Collections.Generic
open System.Linq
open System.Net.Mime
open Microsoft.Extensions.DependencyInjection

type MetadataStore =
    private | MetadataStore of IServiceProvider 
    member __.Get typ =
        let (MetadataStore sp) = __ in sp.GetService typ |> Option.ofObj
    member __.GetAll typ =
        let (MetadataStore sp) = __ in sp.GetServices typ 
            |> Seq.collect (fun p -> p |> Option.ofObj |> Option.map (fun t -> [t]) |> Option.defaultValue [])
            
type IContentTypeProvider<'t> =
     abstract ContentType : 't -> ContentType
    
module MetadataStore =
    open System.Collections.Generic
    module Unchecked =
        let get typ (store : MetadataStore) =
            store.Get typ
        let getAll typ (store : MetadataStore) = store.GetAll typ
    let get<'t> store = Unchecked.get typeof<'t> store |> Option.bind (fun p -> match p with | :? 't as t -> Some t | _ -> None)              
    let getAll<'t> store = Unchecked.getAll typeof<'t> store |> Enumerable.OfType<'t>      
 