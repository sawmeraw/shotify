package processor

import (
	"fmt"
	"strings"

	"github.com/sawmeraw/goexcelparser/cmd/app/internal/model"
)

type ProductProcessor interface {
	Parse() ([]model.ProductData, error)
	Close() error
}

func New(filePath, brand string) (ProductProcessor, error) {
	switch strings.ToLower(strings.TrimSpace(brand)) {
	case "adidas":
		return NewAdidasProcessor(filePath)
	case "nike":
		return NewNikeProcessor(filePath)
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
