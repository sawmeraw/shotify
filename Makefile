DOTNET_DIR := shotify-dotnet
GO_DIR := parser-go

.PHONY: up down dotnet go migrate all

up:
	docker compose up -d

down:
	docker compose down

dotnet:
	cd $(DOTNET_DIR) && dotnet run

go:
	$(MAKE) -C $(GO_DIR) run

migrate:
	cd $(DOTNET_DIR) && dotnet ef database update

all: up dotnet
