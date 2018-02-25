namespace FarLink

open System
open System.Collections.Generic
open Microsoft.Extensions.Logging


type ILoggerContainer =
    abstract Logger : ILogger

type LogScope = 
    internal {
        Maker : ILogger -> IDisposable        
    } 
    member __.Apply logger = __.Maker logger
    member __.Apply (ctnr : ILoggerContainer) = __.Maker (ctnr.Logger)

type Log private (level : LogLevel, ex : Exception option, template : string option)=
    member __.Write (logger : ILogger) = 
        match level with
        | LogLevel.Trace ->
            match ex with
            | Some ex ->
                match template with
                | Some template -> logger.LogTrace(EventId(0), ex, template)
                | None -> logger.LogTrace(EventId(0), ex, ex.Message)
            | None -> logger.LogTrace(Option.defaultValue "TRACE" template)
        | LogLevel.Debug ->
            match ex with
            | Some ex ->
                match template with
                | Some template -> logger.LogDebug(EventId(0), ex, template)
                | None -> logger.LogDebug(EventId(0), ex, ex.Message)
            | None -> logger.LogDebug(Option.defaultValue "DEBUG" template)
        | LogLevel.Information ->
            match ex with
            | Some ex ->
                match template with
                | Some template -> logger.LogInformation(EventId(0), ex, template)
                | None -> logger.LogInformation(EventId(0), ex, ex.Message)
            | None -> logger.LogInformation(Option.defaultValue "INFO" template)
        | LogLevel.Warning ->
            match ex with
            | Some ex ->
                match template with
                | Some template -> logger.LogWarning(EventId(0), ex, template)
                | None -> logger.LogWarning(EventId(0), ex, ex.Message)
            | None -> logger.LogWarning(Option.defaultValue "WARNING" template)
        | LogLevel.Error ->
            match ex with
            | Some ex ->
                match template with
                | Some template -> logger.LogError(EventId(0), ex, template)
                | None -> logger.LogError(EventId(0), ex, ex.Message)
            | None -> logger.LogError(Option.defaultValue "ERROR" template)
        | LogLevel.Critical ->
            match ex with
            | Some ex ->
                match template with
                | Some template -> logger.LogCritical(EventId(0), ex, template)
                | None -> logger.LogCritical(EventId(0), ex, ex.Message)
            | None -> logger.LogCritical(Option.defaultValue "CRITICAL" template)
        | _ -> invalidOp "Unknown log level"
    member __.Write (cntr : ILoggerContainer) = __.Write(cntr.Logger)
         
    static member Trace ex = Log(LogLevel.Trace, Some ex, None)
    static member Trace template = Log(LogLevel.Trace, None, Some template)
    static member Trace (ex, template) = Log(LogLevel.Trace, Some ex, Some template)
    static member Debug ex = Log(LogLevel.Debug, Some ex, None)
    static member Debug template = Log(LogLevel.Debug, None, Some template)
    static member Debug (ex, template) = Log(LogLevel.Debug, Some ex, Some template)
    static member Info ex = Log(LogLevel.Information, Some ex, None)
    static member Info template = Log(LogLevel.Information, None, Some template)
    static member Info (ex, template) = Log(LogLevel.Information, Some ex, Some template)
    static member Warn ex = Log(LogLevel.Warning, Some ex, None)
    static member Warn template = Log(LogLevel.Warning, None, Some template)
    static member Warn (ex, template) = Log(LogLevel.Warning, Some ex, Some template)
    static member Error ex = Log(LogLevel.Error, Some ex, None)
    static member Error template = Log(LogLevel.Error, None, Some template)
    static member Error (ex, template) = Log(LogLevel.Error, Some ex, Some template)
    static member Fatal ex = Log(LogLevel.Critical, Some ex, None)
    static member Fatal template = Log(LogLevel.Critical, None, Some template)
    static member Fatal (ex, template) = Log(LogLevel.Critical, Some ex, Some template)     
    static member Scope (str, [<ParamArray>] args: obj[]) =
        { Maker = fun lg -> lg.BeginScope(str, args) }
    static member Scope ([<ParamArray>] args: (string * obj)[]) =
        { Maker = fun lg -> lg.BeginScope(args |> Map.ofArray) }
    static member Scope (param: string * obj) =
        { Maker = fun lg -> lg.BeginScope([param] |> Map.ofSeq) }            
            
    
    
        
    