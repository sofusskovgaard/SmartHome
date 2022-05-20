defmodule Worker.Util.CommandHandlerUtilities do
  use AMQP

  require Logger

  def initialize_consumer(exchange, queue, queue_error, routing_key) do
    {:ok, conn} = Connection.open(Application.get_env(:smart_home_worker, :rabbitmq, "amqp://guest:guest@localhost"))
    {:ok, chan} = Channel.open(conn)
    setup_consumer_queue(chan, exchange, queue, queue_error, routing_key)

    # Register the GenServer process as a consumer
    {:ok, _consumer_tag} = Basic.consume(chan, queue)
    {:ok, chan}
  end

  def initialize_producer(exchange, queue, queue_error, routing_key) do
    {:ok, conn} = Connection.open(Application.get_env(:smart_home_worker, :rabbitmq, "amqp://guest:guest@localhost"))
    {:ok, chan} = Channel.open(conn)
    setup_producer_queue(chan, exchange, queue, queue_error, routing_key)

    {:ok, chan}
  end

  defp setup_producer_queue(chan, exchange, queue, queue_error, routing_key) do
    # make sure the exchange exists
    :ok = Exchange.declare(chan, exchange, :direct, durable: true)
    :ok = Exchange.bind(chan, "amq.topic", exchange, routing_key: routing_key)

    # make sure error queue exists
    {:ok, _} = Queue.declare(chan, queue_error, durable: true)

    # make sure queue exists
    {:ok, _} = Queue.declare(chan, queue,
                  durable: true,
                  arguments: [
                    {"x-dead-letter-exchange", :longstr, ""},
                    {"x-dead-letter-routing-key", :longstr, queue_error}
                  ]
                )

    # bind mqtt messages to this queue
    :ok = Queue.bind(chan, queue, exchange, routing_key: routing_key)
  end

  defp setup_consumer_queue(chan, exchange, queue, queue_error, routing_key) do
    # make sure the exchange exists
    :ok = Exchange.declare(chan, exchange, :direct, durable: true)
    :ok = Exchange.bind(chan, exchange, "amq.topic", routing_key: routing_key)

    # make sure error queue exists
    {:ok, _} = Queue.declare(chan, queue_error, durable: true)

    # make sure queue exists
    {:ok, _} = Queue.declare(chan, queue,
                  durable: true,
                  arguments: [
                    {"x-dead-letter-exchange", :longstr, ""},
                    {"x-dead-letter-routing-key", :longstr, queue_error}
                  ]
                )

    # bind mqtt messages to this queue
    :ok = Queue.bind(chan, queue, exchange, routing_key: routing_key)
  end

  def handle_rescue(reason, channel, tag, redelivered, payload) do
    case reason do
      :exit ->
        if not redelivered do
          :ok = Basic.nack(channel, tag)
        else
          :ok = Basic.reject(channel, tag, requeue: false)
        end
      exception ->
        :ok = Basic.reject(channel, tag, requeue: not redelivered)
        Logger.error("An error occurred while processing payload: " <> payload)
        Logger.error(exception)
    end
  end
end
