import Config

config :smart_home_worker, Common.InfluxDBConnection,
  auth: [method: :token, token: "YPL3MNrFFRxSSWVGfreDaIxy4fAIIr1VkeK1-SrsACUeY5Qm2rUYlZnHnCrchMStj9ugrhcjUQXUtWlUJdsXaw=="],
  bucket: "main",
  org: "smart-home",
  host: "localhost",
  version: :v2

config :logger, :console,
  metadata: [:queue, :ws_topic]
