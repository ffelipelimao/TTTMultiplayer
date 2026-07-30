SERVER_PROJ := Server/TTT.Server/TTT.Server.csproj
CONFIG ?= Debug

.PHONY: help server build run watch clean

help:
	@echo "  make server   Build and run the server (alias of run)"
	@echo "  make run      Build and run the server"
	@echo "  make build    Build the server only"
	@echo "  make watch    Run the server, rebuilding on file changes"
	@echo "  make clean    Remove the server build artifacts"

server: run 

run:
	dotnet run --project $(SERVER_PROJ) -c $(CONFIG)

build:
	dotnet build $(SERVER_PROJ) -c $(CONFIG)

watch: 
	dotnet watch --project $(SERVER_PROJ) run -c $(CONFIG)

clean: 
	dotnet clean $(SERVER_PROJ) -c $(CONFIG)
	rm -rf Server/TTT.Server/bin Server/TTT.Server/obj
