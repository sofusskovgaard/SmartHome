defmodule API.Routes.MainRoute do
  use Plug.Router

  use API.Utils.Websockets

  plug :match
  plug :fetch_query_params
  plug Plug.RequestId
  plug Plug.Parsers,
    parsers: [:json],
    pass: ["application/json"],
    json_decoder: Jason
  plug :dispatch

  socket "/ws", API.Websocket

  forward "/api/data", to: API.Routes.DataRoute
  forward "/api/blinds", to: API.Routes.BlindsRoute

  get "/" do
    send_resp(conn, 200, "Hello Bountiful World!")
  end

  match _ do
    send_resp(conn, 404, "oops")
  end
end
