defmodule API.Token do
  use Joken.Config

  add_hook(JokenJwks, strategy: API.Token.Auth0Strategy)

  # @impl Joken.Config
  # def token_config do
  #   default_claims()
  #   |> add_claim("aut", fn -> "https://sofusskovgaard.eu.auth0.com" end, &(&1 == "https://sofusskovgaard.eu.auth0.com"))
  # end
end

defmodule API.Token.Auth0Strategy do
  use JokenJwks.DefaultStrategyTemplate

  def init_opts(opts) do
    url = "https://sofusskovgaard.eu.auth0.com/.well-known/jwks.json"
    Keyword.merge(opts, jwks_url: url)
  end
end
