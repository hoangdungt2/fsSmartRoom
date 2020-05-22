# fsSmartRoom
A hobby project that provides monitor and control of temperature and humidity in one room.

## Diagram

The idea of this project is to control/monitor room temperature/humidity automatically/manually.
The diagram is as follows
![diagram](/diagram.png)
* Measurement of temperature/humidity is taken from a BME280 sensor via I2C interface.
* The control of AC is done by infrared signal. (not yet implemented)
* Data storage is in AWS database (Postgres).
* Monitoring is done by Grafana which is connected to Postgres database. (not yet implemented)
