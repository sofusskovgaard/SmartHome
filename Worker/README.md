# Worker / API

This projcet is both a service worker as well as a web API. The worker part of this application consumes telemetry data via RabbitMQ ingests it into an InfluxDB instance. The web API is used to display telemetry data and send commands to the RabbitMQ broker.

## API Endpoints

|Endpoint|Method|Description|URL parameters|Query parameters|
|---|---|---|---|---|
|/api/data/`:measurement`/`:location`|`GET`|Returns an array of data points based on query parameters|`:measurement`<br/>The type of telemetry data, supported values are `temperature` or `humidity`<br/>`:location`.<br/>The location of capture, only supportedd value is `home`.|`offset_ts`<br/>Returns data points collected after the specified unix timestamp<br/>`offset_minutes`<br/>Returns data points collected since the specified amount of seconds ago<br/>`offset_hours`<br/>Same as `offset_minutes` but in hours</br>`offset_days`<br/>Same as `offset_minutes` but in days|
|/api/data/`:measurement`/`:location`/latest|`GET`|Returns a data point based on query parameters|`:measurement`<br/>The type of telemetry data, supported values are `temperature` or `humidity`<br/>`:location`.<br/>The location of capture, only supportedd value is `home`.|`offset_days`<br/>Returns data points collected since the specified amount of seconds ago<br/>|
|/api/blinds/toggle|`POST`|Sends a command to RabbitMQ that makes the controller toggle the servo|||

## Commands

|Routing key|Description|Example|
|---|---|---|
|`home.humidity`|This command contains telemetry data regarding the current `humidity` at the location `home`|`{"humidity":37,"ts":1653045263}`|
|`home.temperature`|This command contains telemetry data regarding the current `temperature` at the location `home`|`{"temperature":25,"ts":1653045298}`|
|`home.toggle_blinds`|This command is only sent by the API and not consumed by the worker service|The payload doesn't matter, it's preferably empty|

# Goals
- [x] Collect telemetry data from RabbitMQ messages and store them in a database.
- [x] Only authenticated users are allowed to send and receive telemetry data.
- [ ] Authenticated users should be able to delete old data.

