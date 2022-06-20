FROM elixir:1.13 as deps
WORKDIR /app

ENV MIX_ENV=prod

RUN mix local.hex --force
RUN mix local.rebar --force
COPY mix.exs mix.exs
COPY mix.lock mix.lock
RUN mix deps.get
RUN mix deps.compile

####

FROM elixir:1.13 as build
WORKDIR /app

ENV MIX_ENV=prod

RUN mix local.hex --force

COPY --from=deps /app /app
COPY . .

RUN mix release

###

FROM hexpm/elixir:1.13.4-erlang-25.0.1-debian-bullseye-20210902-slim
WORKDIR /app

RUN apt-get -y install openssl

COPY --from=build /app /app

RUN chmod +x /app/_build/prod/rel/smart_home_worker/bin/smart_home_worker

CMD [ "/app/_build/prod/rel/smart_home_worker/bin/smart_home_worker", "start" ]
