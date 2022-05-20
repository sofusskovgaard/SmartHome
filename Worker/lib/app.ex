defmodule App do
  use Application

  @impl true
  def start(_type, _args) do
    App.Supervisor.start_link(name: App.Supervisor)
  end
end
