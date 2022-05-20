defmodule API.Plugs.Auth do
  import Plug.Conn

  require Logger

  def init(opts) do
    opts
  end

  defp authenticate({conn, "Bearer " <> jwt, opts}) do
    case API.Token.verify(jwt) do
      {:ok,
       %{
         "aud" => ["http://localhost:4001" | _],
         "iss" => "https://sofusskovgaard.eu.auth0.com/"
       } = claims} ->
        claim_map = Map.new(claims["permissions"], fn c -> {c, true} end)

        if Enum.all?(opts, fn claim -> Map.get(claim_map, claim) end) do
          assign(conn, :user, claims)
        else
          send_401(conn, %{"message" => "You don't have the correct permissions"})
        end

      {:error, err} ->
        send_401(conn, %{error: err})

      _ ->
        send_401(conn, %{"message" => "Something went wrong with your token"})
    end
  end

  defp authenticate(conn) do
    send_401(conn)
  end

  defp send_401(
         conn,
         data \\ %{message: "Please make sure you have a bearer authentication header"}
       ) do
    conn
    |> put_resp_content_type("application/json")
    |> send_resp(401, Jason.encode!(data))
    |> halt
  end

  defp get_auth_header(conn) do
    case get_req_header(conn, "authorization") do
      [token] -> {conn, token}
      _ -> {conn}
    end
  end

  def call(%Plug.Conn{request_path: _path} = conn, opts) do
    Logger.info(opts)

    case get_auth_header(conn) do
      {_, token} -> authenticate({conn, token, opts})
      _ -> authenticate(conn)
    end
  end
end
