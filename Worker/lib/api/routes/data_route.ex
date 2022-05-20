defmodule API.Routes.DataRoute do
  import Plug.Conn
  use Plug.Router

  plug :match
  plug API.Plugs.Auth, ["read:latest-data", "read:data-period"]
  plug :fetch_query_params
  plug Plug.RequestId
  plug :dispatch

  get "/:measurement/:location" do
    params = fetch_query_params(conn).query_params

    cond do
      params["offset_ts"] ->
        response = Common.InfluxDBConnection.query(
          """
            from(bucket: "main")
              |> range(start: time(v: #{params["offset_ts"]}))
              |> filter(fn: (r) => r["_measurement"] == "#{measurement}")
              |> filter(fn: (r) => r["location"] == "#{location}")
              |> filter(fn: (r) => r["_field"] == "value")
              |> aggregateWindow(every: 1m, fn: mean, createEmpty: false)
              |> truncateTimeColumn(unit: 1ms)
              |> yield(name: "mean")
          """,
          query_language: :flux
        )
        conn
        |> put_resp_header("content-type", "application/json")
        |> send_resp(200, Jason.encode!(response))
      not is_nil(params["offset_minutes"]) ->
        response = Common.InfluxDBConnection.query(
          """
            from(bucket: "main")
              |> range(start: -#{params["offset_minutes"]}m)
              |> filter(fn: (r) => r["_measurement"] == "#{measurement}")
              |> filter(fn: (r) => r["location"] == "#{location}")
              |> filter(fn: (r) => r["_field"] == "value")
              |> aggregateWindow(every: 180s, fn: mean, createEmpty: false)
              |> truncateTimeColumn(unit: 1ms)
              |> yield(name: "mean")
          """,
          query_language: :flux
        )
        conn
        |> put_resp_header("content-type", "application/json")
        |> send_resp(200, Jason.encode!(response))
      params["offset_hours"] ->
        response = Common.InfluxDBConnection.query(
          """
            from(bucket: "main")
              |> range(start: -#{params["offset_hours"]}h)
              |> filter(fn: (r) => r["_measurement"] == "#{measurement}")
              |> filter(fn: (r) => r["location"] == "#{location}")
              |> filter(fn: (r) => r["_field"] == "value")
              |> aggregateWindow(every: 15m, fn: mean, createEmpty: false)
              |> truncateTimeColumn(unit: 1ms)
              |> yield(name: "mean")
          """,
          query_language: :flux
        )
        conn
        |> put_resp_header("content-type", "application/json")
        |> send_resp(200, Jason.encode!(response))
      params["offset_days"] ->
        response = Common.InfluxDBConnection.query(
          """
            from(bucket: "main")
              |> range(start: -#{params["offset_days"]}d)
              |> filter(fn: (r) => r["_measurement"] == "#{measurement}")
              |> filter(fn: (r) => r["location"] == "#{location}")
              |> filter(fn: (r) => r["_field"] == "value")
              |> aggregateWindow(every: 1m, fn: mean, createEmpty: false)
              |> truncateTimeColumn(unit: 1ms)
              |> yield(name: "mean")
          """,
          query_language: :flux
        )
        conn
        |> put_resp_header("content-type", "application/json")
        |> send_resp(200, Jason.encode!(response))
      true ->
        conn
        |> put_resp_header("content-type", "application/json")
        |> send_resp(404, "you have to specify an offset")
    end
  end

  get "/:measurement/:location/latest" do
    params = fetch_query_params(conn).query_params

    cond do
      params["offset_days"] ->
        response = Common.InfluxDBConnection.query(
          """
            from(bucket: "main")
              |> range(start: -#{params["offset_days"]}d)
              |> filter(fn: (r) => r["_measurement"] == "#{measurement}")
              |> filter(fn: (r) => r["location"] == "#{location}")
              |> filter(fn: (r) => r["_field"] == "value")
              |> aggregateWindow(every: 1h, fn: last, createEmpty: false)
              |> truncateTimeColumn(unit: 1ms)
              |> sort(columns: ["_time"], desc: true)
              |> limit(n:1, offset: 0)
              |> yield(name: "last")
          """,
          query_language: :flux
        )
        conn
        |> put_resp_header("content-type", "application/json")
        |> send_resp(200, Jason.encode!(response))
      true ->
        conn
        |> send_resp(404, "you have to specify an offset")
    end
  end

  match _ do
    send_resp(conn, 404, "oops")
  end
end
