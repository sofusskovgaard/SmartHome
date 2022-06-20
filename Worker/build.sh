!#/bin/bash

docker buildx build -t sofusskovgaard/elixir-app:latest -f ./Dockerfile --push --platform=linux/arm64 .