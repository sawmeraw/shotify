APP_NAME := parser-go
MAIN := ./cmd/app
BIN_DIR := bin

.PHONY: build run debug test clean

build:
	@mkdir -p $(BIN_DIR)
	go build -o $(BIN_DIR)/$(APP_NAME) $(MAIN)

run: build
	./$(BIN_DIR)/$(APP_NAME)

debug:
	@mkdir -p $(BIN_DIR)
	go build -gcflags="all=-N -l" -o $(BIN_DIR)/$(APP_NAME) $(MAIN)
	dlv exec ./$(BIN_DIR)/$(APP_NAME) --headless --listen=:8080 --api-version=2

test:
	go test ./...

clean:
	rm -rf $(BIN_DIR)