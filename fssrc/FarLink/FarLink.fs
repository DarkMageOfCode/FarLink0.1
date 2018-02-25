namespace FarLink
open Microsoft.Extensions.Logging
open System.Net.Mime

type FarLink = private {
    Metadata : MetadataStore
    Transports : Transport list
    Logger : ILogger
    DefaultContentType : ContentType option 
}

module FarLink =
    let private findTransport accessor farLink =
        farLink.Transports
            |> Seq.map (fun p -> accessor farLink.Metadata p, p)
            |> Seq.tryFind (fun (svc, _) -> Option.isSome svc)
            |> Option.map (fun (svc, tr) -> Option.get svc, tr)
     
     (*          
    let publishAsync farLink (evt : EventMessage<'t>) cancellation =
        use ls = farLink.Logger.BeginScope("{EventType}:{MessageId}", typeof<'t>, evt.Context.MessageId)
        Log.trace  "Publishing Event {EventType}:{MessageId}" farLink.Logger
        match farLink |> findTransport Transport.tryGetEventPublisher<'t> with
        | None -> Log.warn "No publisher found for event" farLink.Logger; None
        | Some (publisher, transport) ->  
            let contentType =
                evt.Context.ContentType
                |> Option.orElseWith (MetadataStore.get<IContentTypeProvider<'T>> transport.Metadata >> Option.map (fun p -> p.ContentType))
                |> Option.orElseWith (MetadataStore.get<IContentTypeProvider<'T>> farLink.Metadata >> Option.map (fun p -> p.ContentType))
                |> Option.orElse farLink.DefaultContentType
            match contentType with
            | None -> Log.warn "Cannot determine content type" farLink.Logger; None
            | Some contentType ->
                None *)             
                       