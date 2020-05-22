namespace LibDatabase

open System

type PostgresDbConfig = 
    {
        Host : string;
        User : string;
        DbName : string;
        Password : string;
        Port : int
    }

type RawDataType = 
    {
        Timestamp : DateTime
        DataName  : string
        DataValue : float 
        SourceId  : string
    }
    