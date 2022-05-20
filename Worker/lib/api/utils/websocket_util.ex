defmodule API.Utils.Websockets do

  defmacro __using__(_) do
    quote do
      @before_compile unquote(__MODULE__)
      Module.register_attribute(__MODULE__, :sockets, accumulate: true)
      import API.Utils.Websockets
    end
  end

  defmacro __before_compile__(_) do
    quote do
      def __sockets__(), do: @sockets
    end
  end

  defmacro socket(path, module, opts \\ []) do
    quote do
      @sockets {unquote(path), unquote(module), unquote(opts)}
    end
  end

  def broadcast(key, message) do
    API.Websockets.Registry
    |> Registry.dispatch(key, fn(entries) ->
      for {pid, _} <- entries do
        if pid != self() do
          Process.send(pid, message, [])
        end
      end
    end)
  end
end
