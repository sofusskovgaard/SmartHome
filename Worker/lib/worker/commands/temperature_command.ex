defmodule Worker.Commands.TemperatureCommand do
  defstruct [:temperature, :ts]
  use ExConstructor
end
