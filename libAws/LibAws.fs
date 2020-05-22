namespace LibAws

open Amazon.SQS
open Amazon
open Newtonsoft.Json
open System

module libAws = 
    let getSqsClient (credential:AwsCredential) = 
        new AmazonSQSClient(
            credential.AccessKey,
            credential.SecretKey,
            RegionEndpoint.APSoutheast1
        )
    
    let getAwsConfig (fn:string) = 
        fn
        |> IO.File.ReadAllText
        |> JsonConvert.DeserializeObject<AwsCredential>



