defmodule Worker.Supervisor do
  use Supervisor

  def start_link(opts) do
    Supervisor.start_link(__MODULE__, :ok, opts)
  end

  @impl true
  def init(:ok) do
    children = [
      {Task.Supervisor, name: Worker.TaskSupervisor},

      Worker.CommandHandlers.TemperatureCommandHandler,
      Worker.CommandHandlers.HumidityCommandHandler
    ]

    Supervisor.init(children, strategy: :one_for_one)
  end
end
