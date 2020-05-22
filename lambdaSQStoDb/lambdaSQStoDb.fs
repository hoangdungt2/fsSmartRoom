namespace lambdaSQStoDb


open Amazon.Lambda.Core
open Newtonsoft.Json
open System
open LibDatabase
open Amazon.Lambda.SQSEvents



// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[<assembly: LambdaSerializer(typeof<Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer>)>]
()


type Function() =
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    let dbFactory = 
        LibDb.getDbConfigFromFile @"./smartRoomDbCredential.json"
        |> LibDb.getDbFactory

    member __.FunctionHandler (input: SQSEvent) (_: ILambdaContext) =
        input.Records
        |> Seq.toArray
        |> Array.map (fun json -> JsonConvert.DeserializeObject<RawDataType>(json.Body))
        |> Array.iter (LibDb.insertSingleRawData dbFactory)