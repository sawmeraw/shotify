package main

import (
	"fmt"
	"log"
	"os"
	"path/filepath"
	"strings"

	"github.com/sawmeraw/goexcelparser/cmd/app/internal/processor"
)

const DataDir = "files"

func ensureFile(name string, brand string) error {
	exts := []string{"xlsx", "csv"}
	clean := strings.ToLower(strings.TrimSpace(brand))
	for _, ext := range exts {
		relativeFilePath := filepath.Join(DataDir, name) + "." + ext
		fmt.Printf("Checking: %s\n", relativeFilePath)
		_, err := os.Stat(relativeFilePath)
		if err != nil {
			if os.IsNotExist(err) {
				continue
			}
			return err
		}
		switch clean {
		case "adidas":
			processor, err := processor.NewAdidasProcessor(relativeFilePath)
			if err != nil {
				return fmt.Errorf("Error caught: %w", err)
			}
			_, err = processor.Parse()
			if err != nil {
				return fmt.Errorf("Error caught: %w", err)
			}
		default:
			return fmt.Errorf("unsupported brand: %s", clean)
		}
	}
	return fmt.Errorf("no matching file found for %s", name)
}

func ensureDir() error {
	if _, err := os.Stat(DataDir); os.IsNotExist(err) {
		return os.MkdirAll(DataDir, 0755)
	}
	return nil
}

func main() {

	args := os.Args

	if len(args) < 3 {
		fmt.Println("Please provide all args in the form of [filename (no extensions)] [brand name] with no square brackets.")
		return
	}

	err := ensureDir()
	if err != nil {
		log.Println(err)
	}

	filePath := os.Args[1]
	brandName := os.Args[2]
	ensureFile(filePath, brandName)
}
