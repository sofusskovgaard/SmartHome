defmodule Common.InfluxDBConnection do
  use GenServer
  use Instream.Connection, otp_app: :smart_home_worker

  def start_link(opts) do
    GenServer.start_link(__MODULE__, :ok, opts)
  end

  @impl true
  def init(_opts) do
    query("CREATE DATABASE IF NOT EXISTS db", method: :post)
  end
end
