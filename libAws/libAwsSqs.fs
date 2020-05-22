namespace LibAws
open Amazon.SQS
open Amazon.SQS.Model

module libAwsSqs =
    let sendSqsMsg (sqsClient:AmazonSQSClient) queueUrl msg = 
        let msgRes = 
            SendMessageRequest(queueUrl,msg)
            |> sqsClient.SendMessageAsync 
            |> Async.AwaitTask
            |> Async.RunSynchronously
        msgRes.HttpStatusCode
