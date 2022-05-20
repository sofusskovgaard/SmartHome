defmodule App.MixProject do
  use Mix.Project

  def project do
    [
      app: :smart_home_worker,
      version: "0.1.0",
      elixir: "~> 1.13",
      start_permanent: Mix.env() == :prod,
      deps: deps()
    ]
  end

  # Run "mix help compile.app" to learn about applications.
  def application do
    [
      extra_applications: [:logger],
      mod: {App, []}
    ]
  end

  # Run "mix help deps" to learn about dependencies.
  defp deps do
    [
      {:amqp, "~> 3.1"},
      {:instream, "~> 2.0.0-rc"},
      {:exconstructor, "~> 1.2.6"},
      {:finch, "~> 0.12.0"},
      {:joken, "~> 2.4"},
      {:joken_jwks, "~> 1.6.0"},
      {:jason, "~> 1.2"},
      {:plug, "~> 1.13"},
      {:cowboy, "~> 2.9"},
      {:plug_cowboy, "~> 2.0"},
    ]
  end
end
