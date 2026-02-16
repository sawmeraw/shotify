package processor

import (
	"fmt"
	"strings"

	"github.com/sawmeraw/shotify/parser-go/cmd/app/internal/model"
)

type ProductProcessor interface {
	Parse() ([]model.ProductData, error)
	Close() error
}

func New(filePath, brand string) (ProductProcessor, error) {
	switch strings.ToLower(strings.TrimSpace(brand)) {
	case "adidas-b2b":
		return NewAdidasProcessor(filePath)
	case "nike-b2b":
		return NewNikeProcessor(filePath)
	case "asics-b2b":
		return NewAsicsProcessor(filePath)
	// case "on":
	// 	return NewOnProcessor(filePath)
	// case "newbalance-elastic":
	// 	return NewNewBalanceProcessor(filePath)
	default:
		return nil, fmt.Errorf("unsupported brand: %s", brand)
	}
}

func FindColumnIndex(header []string, name string) int {
	for i, col := range header {
		if strings.EqualFold(col, name) {
			return i
		}
	}
	return -1
}
