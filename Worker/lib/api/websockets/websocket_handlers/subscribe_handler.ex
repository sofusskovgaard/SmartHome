defmodule API.Websocket.Handlers.SubscribeHandler do
  use GenServer

  require Logger

  def start_link(opts) do
    GenServer.start_link(__MODULE__, :ok, opts)
  end

  def init(opts) do
    Logger.info("created #{inspect __MODULE__}.")
    { :ok, opts }
  end
end
