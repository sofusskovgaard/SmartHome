import Config

config :smart_home_worker, Common.InfluxDBConnection,
  auth: [method: :token, token: System.get_env("INFLUXDB_TOKEN")], # "4LkDT8Zq00xVu1MjPWRbRx_kFTgwk0bsM_GRhwMYb2Dh3W7Fz58-3dHQKksmAjTS5AWefo1fxBFmqm8nr3KBdA=="],
  bucket: System.get_env("INFLUXDB_BUCKET"), # "main",
  org: System.get_env("INFLUXDB_ORG"), # "smart-home",
  host: System.get_env("INFLUXDB_HOST"), # "10.135.16.167",
  version: :v2

config :smart_home_worker, rabbitmq: "amqp://#{System.get_env("RABBITMQ_USER")}:#{System.get_env("RABBITMQ_PASSWORD")}@#{System.get_env("RABBITMQ_HOST")}:#{System.get_env("RABBITMQ_PORT")}"
