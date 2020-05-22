namespace libLogger
open Serilog
open Serilog.Events


module Logger = 
    type LogLevel = LogEventLevel
    let private configLog (name:string) (logLevel:LogEventLevel) = 
        //let logName = sprintf "log_%s-.txt" name
        Log.Logger <- LoggerConfiguration()
                        .WriteTo.Console()
                        //.WriteTo.File(logName, 
                        //        rollingInterval = RollingInterval.Day, 
                        //        restrictedToMinimumLevel = LogEventLevel.Information)
                        .MinimumLevel.Debug()
                        .CreateLogger()
    let init name = 
        configLog name
    let Debug fmt = 
        Printf.kprintf (fun s -> Log.Debug(s)) fmt 
    let Information fmt = 
        Printf.kprintf (fun s -> Log.Information(s)) fmt 
    let Warning fmt = 
        Printf.kprintf (fun s -> Log.Warning(s)) fmt 