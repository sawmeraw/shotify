package processor

import (
	"strings"

	"github.com/sawmeraw/goexcelparser/cmd/app/internal/model"
)

type ProductProcessor interface {
	Parse() ([]model.ProductData, error)
}

func FindColumnIndex(header []string, name string) int {
	for i, col := range header {
		if strings.EqualFold(col, name) {
			return i
		}
	}
	return -1
}
