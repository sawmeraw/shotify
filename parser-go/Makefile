APP_NAME := parser-go 
MAIN := ./cmd/app
BIN_DIR := bin

.PHONY: build run test clean

build:
	@mkdir -p $(BIN_DIR)
	go build -o $(BIN_DIR)/$(APP_NAME) $(MAIN)

run: build
	./$(BIN_DIR)/$(APP_NAME)

test:
	go test ./...

clean:
	rm -rf $(BIN_DIR)
