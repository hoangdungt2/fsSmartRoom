namespace LibEdgeDevice
open System
open Iot.Device.Bmxx80.PowerMode
open Iot.Units
open System.Device.I2c
open Iot.Device.Bmxx80


module DevBme280 =
    //let deviceId = 0x76
    //let busId    = 1
    let defaultSeaLevel = Pressure.MeanSeaLevel

    // getaBME device
    //   deviceId default is 0x76
    //   busId default is 1
    let getBmeI2cDevice (busId) deviceId = 
        let i2cSettings = I2cConnectionSettings(busId, deviceId)
        let i2cDevice   = I2cDevice.Create(i2cSettings)
        let i2CBmpe80   = new Bme280(i2cDevice)        
        i2CBmpe80.TemperatureSampling <- Sampling.LowPower
        i2CBmpe80.PressureSampling    <- Sampling.UltraHighResolution
        i2CBmpe80.HumiditySampling    <- Sampling.Standard
        i2CBmpe80

    /// get measurement
    let readBme280SingleValueAsync (bmeDevice: Bme280) (readType:BME280ReadType) = 
        async{ 
            bmeDevice.SetPowerMode(Bmx280PowerMode.Forced)
            do! bmeDevice.GetMeasurementDuration() |> Async.Sleep
            return
                match readType with
                | Temperature -> 
                    let flag,value = bmeDevice.TryReadTemperature()
                    if flag then
                        Ok value.Celsius
                    else
                        Error ["[Temperature]: Fail to read"]
                | Humidity ->
                    let flag,value = bmeDevice.TryReadHumidity()
                    if flag then
                        Ok value
                    else
                        Error ["[Humidity]: Fail to read"]
                | Altitude ->
                    let flag,value = bmeDevice.TryReadAltitude(defaultSeaLevel)
                    if flag then
                        Ok value
                    else
                        Error ["[Altitude]: Fail to read"]
                | Pressure ->
                    let flag,value = bmeDevice.TryReadPressure()
                    if flag then
                        Ok value.Hectopascal
                    else
                        Error ["[Pressure]: Fail to read"]
            
        }            

    //read humidity and temperature
    let readBme280HumidTemperatureAsync (bmeDevice: Bme280) = 
        async{ 
            bmeDevice.SetPowerMode(Bmx280PowerMode.Forced)
            do! bmeDevice.GetMeasurementDuration() |> Async.Sleep
            let flagTemp ,valueTemp  = bmeDevice.TryReadTemperature()
            let flagHumid,valueHumid = bmeDevice.TryReadHumidity()
            return
                match flagTemp,flagHumid with 
                | true,true -> Ok {Humidity = valueHumid; Temperature = valueTemp.Celsius*1.<degC>}
                | _ -> Error ["Read failed"]
        }