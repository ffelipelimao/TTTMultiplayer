SERVER_PROJ := Server/TTT.Server.csproj
CONFIG ?= Debug

.PHONY: help server build run watch clean restore add-package remove-package list-packages

help:
	@echo "  make server          Build and run the server (alias of run)"
	@echo "  make run             Build and run the server"
	@echo "  make build           Build the server only"
	@echo "  make watch           Run the server, rebuilding on file changes"
	@echo "  make clean           Remove the server build artifacts"
	@echo "  make restore         Restore NuGet packages"
	@echo "  make add-package PKG=<name> [VERSION=<version>]"
	@echo "                       Add a NuGet package to the server project"
	@echo "  make remove-package PKG=<name>"
	@echo "                       Remove a NuGet package from the server project"
	@echo "  make list-packages   List the server project NuGet packages"

server: run 

run:
	dotnet run --project $(SERVER_PROJ) -c $(CONFIG)

build:
	dotnet build $(SERVER_PROJ) -c $(CONFIG)

watch: 
	dotnet watch --project $(SERVER_PROJ) run -c $(CONFIG)

clean:
	dotnet clean $(SERVER_PROJ) -c $(CONFIG)
	rm -rf Server/bin Server/obj

restore:
	dotnet restore $(SERVER_PROJ)

# Ex.: make add-package PKG=Serilog
#      make add-package PKG=Serilog VERSION=4.2.0
add-package:
ifndef PKG
	$(error PKG nao informado. Use: make add-package PKG=<nome> [VERSION=<versao>])
endif
	dotnet add $(SERVER_PROJ) package $(PKG) $(if $(VERSION),--version $(VERSION),)

# Ex.: make remove-package PKG=Serilog
remove-package:
ifndef PKG
	$(error PKG nao informado. Use: make remove-package PKG=<nome>)
endif
	dotnet remove $(SERVER_PROJ) package $(PKG)

list-packages:
	dotnet list $(SERVER_PROJ) package
