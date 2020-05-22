namespace LibDatabase
open Newtonsoft.Json
open System
open System.IO
open Npgsql
open Donald
open System.Data

module LibDbHelper = 
    let rawDataToParam (rawData:RawDataType) = 
        [ 
            newParam "timestamp"  (SqlType.DateTime rawData.Timestamp) 
            newParam "data_name"  (SqlType.String rawData.DataName) 
            newParam "data_value" (SqlType.Double rawData.DataValue)
            newParam "source_id"  (rawData.SourceId |> SqlType.String) 
        ]        
    ()
module LibDb =
    /// get config from files
    let getDbConfigFromFile (file:string) =
        file
        |> File.ReadAllText
        |> JsonConvert.DeserializeObject<PostgresDbConfig>
    let getConnectionString (config:PostgresDbConfig)= 
        sprintf "Server=%s;Port=%d;Username=%s;Password=%s;Database=%s;Timeout=0;Command Timeout=0;"
            config.Host config.Port config.User config.Password config.DbName
    /// get connection factory
    let getDbFactory (dbConfig:PostgresDbConfig) : DbConnectionFactory = 
        fun _ -> new NpgsqlConnection(getConnectionString dbConfig) :> IDbConnection

    /// writeRawData
    let insertRawDataAsync (connectionFactory:DbConnectionFactory) (rawData: RawDataType list) = 
        async{
            use conn = createConn connectionFactory
            let dbParams = 
                List.map LibDbHelper.rawDataToParam rawData
            return! 
                execManyAsync 
                    "INSERT INTO raw_data (timestamp,data_name,data_value,source_id) VALUES (@timestamp,@data_name,@data_value,@source_id);"
                    dbParams
                    conn
        }

    let insertRawData (connectionFactory:DbConnectionFactory) (rawData: RawDataType list) = 
        use conn = createConn connectionFactory
        use tran = beginTran conn 
        let dbParams = 
            List.map LibDbHelper.rawDataToParam rawData
        tranExecMany 
            "INSERT INTO raw_data (timestamp,data_name,data_value,source_id) VALUES (@timestamp,@data_name,@data_value,@source_id);"
            dbParams
            tran
        commitTran tran

    let insertSingleRawDataAsync (connectionFactory:DbConnectionFactory) (rawData: RawDataType) = 
        async{
            use conn = createConn connectionFactory
            use tran = beginTran conn 
            let dbParam = 
                LibDbHelper.rawDataToParam rawData
            return! 
                execAsync 
                    "INSERT INTO raw_data (timestamp,data_name,data_value,source_id) VALUES (@timestamp,@data_name,@data_value,@source_id);"
                    dbParam
                    conn
        }

    let insertSingleRawData (connectionFactory:DbConnectionFactory) (rawData: RawDataType) = 
            use conn = createConn connectionFactory
            use tran = beginTran conn 
            let dbParam = 
                LibDbHelper.rawDataToParam rawData
            tranExec 
                "INSERT INTO raw_data (timestamp,data_name,data_value,source_id) VALUES (@timestamp,@data_name,@data_value,@source_id);"
                dbParam
                tran
            commitTran tran