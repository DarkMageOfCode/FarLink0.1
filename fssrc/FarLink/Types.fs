namespace FarLink
open System
open System.Net.Mime
open System.Threading
open System.Threading.Tasks
open Microsoft.Extensions.DependencyInjection

type MessageContext = {
    ContentType : Option<ContentType>
    TypeCode : Option<string>
    MessageId : Option<string>
    CorrelationId : Option<string>
    MessageTtl : Option<TimeSpan>
}

type Message<'t> =
    {
        Context : MessageContext
        Payload : 't
    }

type MetadataProvider =
    internal | MetadataProvider of IServiceProvider 
    member __.Get typ =
        let (MetadataProvider sp) = __ in sp.GetService typ |> Option.ofObj
    member __.GetAll typ =
        let (MetadataProvider sp) = __ in sp.GetServices typ 
            |> Seq.collect (fun p -> p |> Option.ofObj |> Option.map (fun t -> [t]) |> Option.defaultValue [])
            
type ITypeCodeMeta<'t> =
    abstract Code : string
    
type ISerialize<'t> =
    abstract Serialize : 't -> byte[]
    
type IDeserialize<'t> =
    abstract Deserialize : byte[] -> 't
    
type ISerializeProvider =
    abstract GetSerialize<'t> : ContentType -> ISerialize<'t> option
    
type IDeserailizeProvider =
    abstract GetDeserialize<'t> : ContentType -> IDeserialize<'t> option
    
type IRawEventPublisher<'t> = 
    abstract PublishAsync : msg: Message<byte[]> * payload: 't * cancellation: CancellationToken -> Task
                
type IRawEventPublisherProvider =
    abstract GetPublisher<'t> : root : MetadataProvider -> IRawEventPublisher<'t> option
    
type EventPublisher<'t> = Message<'t> * CancellationToken -> Task

           