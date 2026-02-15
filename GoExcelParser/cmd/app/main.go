package main

import (
	"flag"
	"fmt"
	"log"
	"net/http"
	"os"
	"path/filepath"

	"github.com/sawmeraw/goexcelparser/cmd/app/internal/processor"
	"github.com/sawmeraw/goexcelparser/cmd/app/internal/server"
	"github.com/sawmeraw/goexcelparser/cmd/app/internal/writer"
)

const DataDir = "files"

func main() {
	cliMode := flag.Bool("cli", false, "run in CLI mode")
	port := flag.String("port", ":8080", "server listen address")
	flag.Parse()

	if *cliMode {
		runCLI(flag.Args())
	} else {
		runServer(*port)
	}
}

func runServer(addr string) {
	mux := server.NewMux()
	log.Printf("Starting server on %s", addr)
	log.Fatal(http.ListenAndServe(addr, mux))
}

func runCLI(args []string) {
	if len(args) < 2 {
		fmt.Println("Usage: app --cli <filename> <brand>")
		fmt.Println("  filename: file name without extension in the files/ directory")
		fmt.Println("  brand:    brand name (adidas, nike)")
		os.Exit(1)
	}

	name := args[0]
	brand := args[1]

	if err := ensureDir(); err != nil {
		log.Fatal(err)
	}

	filePath, err := findFile(name)
	if err != nil {
		log.Fatal(err)
	}

	proc, err := processor.New(filePath, brand)
	if err != nil {
		log.Fatal(err)
	}
	defer proc.Close()

	products, err := proc.Parse()
	if err != nil {
		log.Fatal(err)
	}

	outputPath := filepath.Join(DataDir, "rex_output.xlsx")
	if err := writer.WriteProductsToExcel(products, outputPath); err != nil {
		log.Fatal(err)
	}

	fmt.Printf("Wrote %d products to %s\n", len(products), outputPath)
}

func findFile(name string) (string, error) {
	for _, ext := range []string{"xlsx", "csv"} {
		path := filepath.Join(DataDir, name) + "." + ext
		if _, err := os.Stat(path); err == nil {
			return path, nil
		}
	}
	return "", fmt.Errorf("no file found for %q in %s/", name, DataDir)
}

func ensureDir() error {
	if _, err := os.Stat(DataDir); os.IsNotExist(err) {
		return os.MkdirAll(DataDir, 0755)
	}
	return nil
}
