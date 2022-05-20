defmodule Worker.Series.TemperatureSeries do
  use Instream.Series

  series do
    measurement "temperature"

    tag :location, default: "home"

    field :value
  end
end
