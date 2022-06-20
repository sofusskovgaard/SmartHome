import Config

config :logger, :console,
  metadata: [:queue, :ws_topic]
