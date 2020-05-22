namespace LibMqtt
open System
open System.Text
open uPLibrary.Networking.M2Mqtt
open uPLibrary.Networking.M2Mqtt.Messages
open Newtonsoft.Json
open LibDatabase
module LibMqtt =
    
    let loadMqttConfigFromJson (jsonFile:string) = 
        jsonFile
        |> IO.File.ReadAllText
        |> JsonConvert.DeserializeObject<MqttConfig>

    let getMqttClient (config:MqttConfig) =
        let mqttClient = MqttClient(config.HostName,config.Port,false,null,null,MqttSslProtocols.None)
        let clientId = Guid.NewGuid().ToString()
        mqttClient.Connect(clientId,config.User,config.Password) |> ignore
        mqttClient
    
    let publishRawData (client:MqttClient) (topic:string) (rawData:RawDataType) = 
        rawData
        |> JsonConvert.SerializeObject
        |> Encoding.UTF8.GetBytes
        |> fun json -> client.Publish(topic,json)

