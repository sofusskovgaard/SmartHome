defmodule Worker.Series.HumiditySeries do
  use Instream.Series

  series do
    measurement "humidity"

    tag :location, default: "home"

    field :value
  end
end
