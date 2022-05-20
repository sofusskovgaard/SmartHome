defmodule API.Supervisor do
  use Supervisor

  def start_link(opts) do
    Supervisor.start_link(__MODULE__, :ok, opts)
  end

  @impl true
  def init(:ok) do
    children = [
      {API.Token.Auth0Strategy, time_interval: 30_000},
      {Task.Supervisor, name: API.TaskSupervisor},
      {Plug.Cowboy, scheme: :http, plug: API.Routes.MainRoute, options: [
        port: 4001,
        dispatch: dispatch(API.Routes.MainRoute)
      ]},
      Registry.child_spec(
        keys: :duplicate,
        name: API.Websockets.Registry
      )
    ]

    Supervisor.init(children, strategy: :one_for_one)
  end

  defp dispatch(plug, opts \\ []) do
    plug_cowboy_handler = [{:_, Plug.Cowboy.Handler, {plug, opts}}]
    socket_handlers = Enum.reverse(plug.__sockets__())

    [
      {:_, socket_handlers ++ plug_cowboy_handler}
    ]
  end
end
