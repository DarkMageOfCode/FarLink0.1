namespace FarLink
open System
open System.Net.Mime
open System.Threading
open System.Threading.Tasks
open FarLink


type EventMessage<'t> =
    {
        Context : MessageContext
        Body : 't 
    }


type EventPublisher<'t> = 't -> MessageContext -> CancellationToken -> Task


    