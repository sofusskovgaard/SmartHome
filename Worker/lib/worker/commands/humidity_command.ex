defmodule Worker.Commands.HumidityCommand do
  defstruct [:humidity, :ts]
  use ExConstructor
end
