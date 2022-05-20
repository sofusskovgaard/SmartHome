defmodule API.Routes.BlindsRoute do
  use Plug.Router
  use AMQP

  use API.Utils.Websockets

  plug :match
  plug API.Plugs.Auth
  plug :fetch_query_params
  plug Plug.RequestId
  plug :dispatch

  post "/toggle" do
    exchange = "smart_home.exchange"
    queue = "toggle-blinds-command-handler"
    error_queue = "toggle-blinds-command-handler-error"
    routing_key = "home.toggle_blinds"

    { :ok, chan } = Worker.Util.CommandHandlerUtilities.initialize_producer(exchange, queue, error_queue, routing_key)

    case Basic.publish(chan, exchange, routing_key, "") do
      :ok ->
        conn |> send_resp(201, "")
      _ -> conn |> send_resp(500, "")
    end
  end

  match _ do
    send_resp(conn, 404, "oops")
  end
end
