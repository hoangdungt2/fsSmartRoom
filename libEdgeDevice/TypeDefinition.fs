namespace LibEdgeDevice
type BME280ReadType = 
| Temperature
| Humidity
| Altitude
| Pressure

[<Measure>] type degC
[<Measure>] type degF
[<Measure>] type hpA   // pressure 

//let convertDegCToF c = 
//    c * 1.8<degF/degC> + 32.0<degF>

type BME280HumidTemp = 
    {
        Humidity : float    // %
        Temperature : float<degC> // degC
    }

type EdgeIOResult<'a> = 
    | Ok of 'a
    | Error of string list