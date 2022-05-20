defmodule API.Websocket do
  @behaviour :cowboy_websocket

  require Logger

  def init(request, _state) do
    {:cowboy_websocket, request, nil}
  end

  def websocket_init(state) do
    API.Websockets.Registry
    |> Registry.register("heartbeat", {})

    {:ok, state}
  end

  def websocket_handle({:text, "ping"}, state) do
    {:reply, {:text, "pong"}, state}
  end

  def websocket_handle({:text, json}, state) do
    payload = Jason.decode!(json)
    type = String.to_existing_atom(payload["type"])

    Logger.metadata(ws_topic: type)

    message = case type do
      :subscribe ->
        topic = payload["topic"]

        result = if topic not in Registry.keys(API.Websockets.Registry, self()) do
          Logger.info("Registering topic: #{topic} for pid: #{ inspect self() }")
          API.Websockets.Registry
          |> Registry.register(topic, {})
          "true"
        else
          Logger.info("Topic already registered: #{topic} for pid: #{ inspect self() }")
          "false"
        end

        result
    end

    {:reply, {:text, if is_nil(message) do "" else message end}, state}

    rescue
      ex ->
        json = Jason.encode!(%{error: true ,message: ex.message})
        {:reply, {:text, json}, state}
  end

  def websocket_info(info, state) do
    {:reply, {:text, info}, state}
  end
end
