namespace LibAws
open Amazon

type AwsCredential = 
    {
        AccessKey : string
        SecretKey : string
        SqsQueueUrl : string
    }

