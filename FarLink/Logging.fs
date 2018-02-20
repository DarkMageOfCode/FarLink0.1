namespace FarLink.Logging

open System
open System.Collections.Generic
open Microsoft.Extensions.Logging

type ILog =
    abstract With : parameters : KeyValuePair<string, obj> seq -> ILog
    abstract Write :  level : LogLevel * template : string  * ex : Exception -> unit
    
type ILog<'t> =
    inherit ILog

type ILogFactory =
    abstract CreateLog : category : string -> ILog

type LogFactory(factory : ILoggerFactory) =
    member private __.CreateLog category =
        let props = Map.empty
        let logger = factory.CreateLogger(category)
        {
            new ILog with
                member __.Write(level, template, ex) =
                    match level with
                    | LogLevel.Trace -> if isNull ex then logger.LogTrace(template) else logger.LogTrace(EventId(0), ex, template)
                    | LogLevel.Debug -> if isNull ex then logger.LogDebug(template) else logger.LogDebug(EventId(0), ex, template)
                    | LogLevel.Information -> if isNull ex then logger.LogInformation(template) else logger.LogInformation(EventId(0), ex, template)
                    | LogLevel.Warning -> if isNull ex then logger.LogWarning(template) else logger.LogWarning(EventId(0), ex, template)
                    | LogLevel.Error -> if isNull ex then logger.LogError(template) else logger.LogError(EventId(0), ex, template)
                    | LogLevel.Critical -> if isNull ex then logger.LogCritical(template) else logger.LogCritical(EventId(0), ex, template)
                    | _ -> invalidArg "level" "Unknown logging level"
                member __.With parameters =
                    let rec wrapLog param  us fn =
                        let innerProps = param |> Seq.map (fun (p : KeyValuePair<string, obj>) -> p.Key, p.Value) |> Map.ofSeq
                        {
                            new ILog with
                                member __.Write(level, template, ex) =
                                    use p = us()
                                    use v = logger.BeginScope innerProps
                                    fn(level, template, ex)
                                member __.With param =
                                    let combine () =
                                        let p1 = us()
                                        let p2 = logger.BeginScope innerProps
                                        {
                                            new IDisposable with
                                                member __.Dispose () =
                                                    p2.Dispose()
                                                    p1.Dispose() 
                                        } 
                                    wrapLog param combine fn 
                        }
                    wrapLog parameters (fun () -> { new IDisposable with member __.Dispose () = () }) (__.Write)                    
        }  
        
    interface ILogFactory with
        member __.CreateLog category = __.CreateLog category    
        
type Log<'t>(factory : ILogFactory) =
    let log = factory.CreateLog(typeof<'t>.FullName)
    interface ILog<'t> with
        member __.Write(level, template, ex) = log.Write(level, template, ex)
        member __.With parameters = log.With parameters

open System.Runtime.CompilerServices

[<Extension>]
type LoggingExtensions private() =
    static member CreateLog<'t> (factory : ILogFactory) = Log<'t>(factory)
    static member Trace (log : ILog , template , ?ex) =
        log.Write(LogLevel.Trace, template, defaultArg ex null);
    static member Trace(log : ILog, ex : Exception) =
        log.Write(LogLevel.Trace, ex.Message, ex);
    static member Debug (log : ILog , template , ?ex) =
        log.Write(LogLevel.Debug, template, defaultArg ex null);
    static member Debug(log : ILog, ex : Exception) =
        log.Write(LogLevel.Debug, ex.Message, ex);
    static member Info (log : ILog , template , ?ex) =
        log.Write(LogLevel.Information, template, defaultArg ex null);
    static member Info(log : ILog, ex : Exception) =
        log.Write(LogLevel.Information, ex.Message, ex);
    static member Warn (log : ILog , template , ?ex) =
        log.Write(LogLevel.Warning, template, defaultArg ex null);
    static member Warn(log : ILog, ex : Exception) =
        log.Write(LogLevel.Warning, ex.Message, ex);
    static member Error (log : ILog , template , ?ex) =
        log.Write(LogLevel.Error, template, defaultArg ex null);
    static member Error(log : ILog, ex : Exception) =
        log.Write(LogLevel.Error, ex.Message, ex);
    static member Fatal (log : ILog , template , ?ex) =
        log.Write(LogLevel.Critical, template, defaultArg ex null);
    static member Fatal(log : ILog, ex : Exception) =
        log.Write(LogLevel.Critical, ex.Message, ex);
    
     
    
