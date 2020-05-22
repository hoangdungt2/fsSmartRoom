// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open FluentScheduler
open Newtonsoft.Json
open LibDatabase
open LibEdgeDevice
open LibMqtt
open libLogger
open LibAws
open Amazon

let awsCredentialFile = @"./smartRoomAwsCredential.json"
let awsConfig = libAws.getAwsConfig awsCredentialFile
let bme280 = 
    DevBme280.getBmeI2cDevice 1 0x76

let formRawDataTypeNow sourceId (data:BME280HumidTemp) = 
    let timestamp = 
        DateTime.UtcNow
    [
        {
            Timestamp = timestamp
            SourceId  = sourceId
            DataName  = "temperature"
            DataValue = data.Temperature |> float
        };
        {
            Timestamp = timestamp
            SourceId  = sourceId
            DataName  = "humidity"
            DataValue = data.Humidity
        }
    ]

Logger.init "bme280test" Logger.LogLevel.Debug

let josePayaLebarRoomId = "8cfd13bf-4ffb-4b47-bf97-781d314def37"

let sqsClient = 
    awsConfig
    |> libAws.getSqsClient


let readBmeUpdateDb _ = 
    let readVal = 
        DevBme280.readBme280HumidTemperatureAsync bme280
        |> Async.RunSynchronously
    match readVal with
    | Ok data ->
        printfn "[%s]: Read Ok: Temp = %.1fdegC, Humid=%.0f%%" 
            (DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"))
            (float data.Temperature) data.Humidity
        let rawData = 
            formRawDataTypeNow josePayaLebarRoomId data

        rawData
        |> List.map (JsonConvert.SerializeObject)
        |> List.iter (libAwsSqs.sendSqsMsg sqsClient awsConfig.SqsQueueUrl >> ignore)

    | Error msg ->
        Logger.Warning "%A" msg
    ()

// form sheduler registry
let schedRegistry = Registry()
schedRegistry.Schedule(readBmeUpdateDb).ToRunEvery(5).Minutes() |> ignore
JobManager.Initialize schedRegistry

[<EntryPoint>]
let main argv =
    readBmeUpdateDb ()
    JobManager.Start()
    while true do
        Async.Sleep 100000 |> Async.RunSynchronously
    0 // return an integer exit code