namespace FarLink
open System.Threading
open System.Threading.Tasks



type RawEventPublisher<'t> = 
    RawEventPublisher of (EventMessage<byte[]> -> 't -> CancellationToken -> Task)
     
type IRawEventPublisherProvider =
    abstract GetPublisher<'t> : rootMeta : MetadataStore -> (RawEventPublisher<'t> option)    

type Transport = {
    Metadata : MetadataStore
    
}

module Transport =
    let tryGetEventPublisher<'t> rootMeta transport  =
        transport.Metadata 
            |> MetadataStore.get<IRawEventPublisherProvider>
            |> Option.bind (fun p -> p.GetPublisher<'t> rootMeta)
      