defmodule Worker.CommandHandlers.TemperatureCommandHandler do
  alias Worker.Commands.TemperatureCommand
  alias Worker.Series.TemperatureSeries

  alias Worker.Util.CommandHandlerUtilities

  use GenServer
  use AMQP

  require Logger

  def start_link(opts) do
    GenServer.start_link(__MODULE__, :ok, opts)
  end

  @routing_key "home.temperature"
  @exchange    "smart_home.exchange"
  @queue       "temperature-command-handler"
  @queue_error "#{@queue}_error"

  def init(_opts) do
    CommandHandlerUtilities.initialize_consumer(@exchange, @queue, @queue_error, @routing_key)
  end

  # Confirmation sent by the broker after registering this process as a consumer
  def handle_info({:basic_consume_ok, %{consumer_tag: _consumer_tag}}, chan) do
    {:noreply, chan}
  end

  # Sent by the broker when the consumer is unexpectedly cancelled (such as after a queue deletion)
  def handle_info({:basic_cancel, %{consumer_tag: _consumer_tag}}, chan) do
    {:stop, :normal, chan}
  end

  # Confirmation sent by the broker to the consumer process after a Basic.cancel
  def handle_info({:basic_cancel_ok, %{consumer_tag: _consumer_tag}}, chan) do
    {:noreply, chan}
  end

  def handle_info({:basic_deliver, payload, %{delivery_tag: tag, redelivered: redelivered}}, chan) do

    Task.Supervisor.async(Worker.TaskSupervisor, fn ->
      Logger.metadata(queue: @queue)
      consume(chan, tag, redelivered, payload)
    end)
    |> Task.await()

    {:noreply, chan}
  end

  defp consume(channel, tag, redelivered, payload) do
    :ok = Basic.ack(channel, tag)
    Logger.info(payload)

    data = TemperatureCommand.new(Jason.decode!(payload))

    series = %TemperatureSeries{
      fields: %TemperatureSeries.Fields{value: data.temperature},
      tags: %TemperatureSeries.Tags{location: "home"},
    }

    Common.InfluxDBConnection.write(series)

    API.Utils.Websockets.broadcast("temperature", to_string(data.temperature))

  rescue
    reason ->
      CommandHandlerUtilities.handle_rescue(reason, channel, tag, redelivered, payload)
  end
end
