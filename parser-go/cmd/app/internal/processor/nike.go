package processor

import (
	"encoding/csv"
	"fmt"
	"math"
	"os"
	"strconv"
	"strings"

	"github.com/sawmeraw/goexcelparser/cmd/app/internal/model"
)

type NikeProcessor struct {
	file *os.File
}

func NewNikeProcessor(relativeFilePath string) (*NikeProcessor, error) {
	f, err := os.Open(relativeFilePath)
	if err != nil {
		return nil, fmt.Errorf("Error opening nike file: %w", err)
	}
	return &NikeProcessor{file: f}, nil
}

func (p *NikeProcessor) Close() error {
	return p.file.Close()
}

func (p *NikeProcessor) Parse() ([]model.ProductData, error) {
	reader := csv.NewReader(p.file)
	rows, err := reader.ReadAll()
	if err != nil {
		return nil, fmt.Errorf("Error reading CSV: %w", err)
	}

	if len(rows) == 0 {
		return nil, fmt.Errorf("Empty CSV file")
	}

	header := rows[0]
	// BOM handling for UTF-8 CSV files
	if len(header) > 0 {
		header[0] = strings.TrimPrefix(header[0], "\ufeff")
	}

	productCodeIdx := FindColumnIndex(header, "Product Code")
	if productCodeIdx == -1 {
		return nil, fmt.Errorf("Column 'Product Code' not found")
	}
	sizeIdx := FindColumnIndex(header, "Size")
	if sizeIdx == -1 {
		return nil, fmt.Errorf("Column 'Size' not found")
	}
	upcIdx := FindColumnIndex(header, "UPC")
	if upcIdx == -1 {
		return nil, fmt.Errorf("Column 'UPC' not found")
	}
	productNameIdx := FindColumnIndex(header, "Product Name")
	if productNameIdx == -1 {
		return nil, fmt.Errorf("Column 'Product Name' not found")
	}
	colorNameIdx := FindColumnIndex(header, "Primary Colour Name")
	if colorNameIdx == -1 {
		return nil, fmt.Errorf("Column 'Primary Colour Name' not found")
	}
	colorDescIdx := FindColumnIndex(header, "General Colour Description")
	if colorDescIdx == -1 {
		return nil, fmt.Errorf("Column 'General Colour Description' not found")
	}
	genderIdx := FindColumnIndex(header, "Gender Age Summary")
	if genderIdx == -1 {
		return nil, fmt.Errorf("Column 'Gender Age Summary' not found")
	}
	retailPriceIdx := FindColumnIndex(header, "Suggested Retail Price")
	if retailPriceIdx == -1 {
		return nil, fmt.Errorf("Column 'Suggested Retail Price' not found")
	}
	costPriceIdx := FindColumnIndex(header, "Standard Wholesale Price (Wholesale Includes VAT)")
	if costPriceIdx == -1 {
		return nil, fmt.Errorf("Column 'Standard Wholesale Price (Wholesale Includes VAT)' not found")
	}

	productMap := make(map[string]*model.ProductData)

	for _, row := range rows[1:] {
		productCode := strings.TrimSpace(row[productCodeIdx])
		size := strings.TrimSpace(row[sizeIdx])
		upc := strings.TrimSpace(row[upcIdx])

		if productCode == "" || size == "" || upc == "" {
			continue
		}

		if _, ok := productMap[productCode]; !ok {
			genderValue := strings.TrimSpace(row[genderIdx])
			var gender string
			switch strings.ToUpper(genderValue) {
			case "WOMENS":
				gender = "W"
			case "MENS":
				gender = "M"
			case "UNISEX":
				gender = "U"
			default:
				gender = "K"
			}

			retailPrice := calcRetailPrice(strings.TrimSpace(row[retailPriceIdx]))
			costPrice := calcCostPrice(strings.TrimSpace(row[costPriceIdx]))

			productMap[productCode] = &model.ProductData{
				BrandName:    "Nike",
				SupplierCode: "NK",
				ProductCode:  productCode,
				ModelName:    strings.TrimSpace(row[productNameIdx]),
				BaseColor:    parseBaseColor(strings.TrimSpace(row[colorNameIdx])),
				ColorName:    strings.TrimSpace(row[colorDescIdx]),
				Gender:       gender,
				SizeSKUs:     make(map[string]string),
				RetailPrice:  &retailPrice,
				CostPrice:    &costPrice,
			}
		}

		productMap[productCode].SizeSKUs[size] = upc
	}

	products := make([]model.ProductData, 0, len(productMap))
	for _, p := range productMap {
		products = append(products, *p)
	}

	return products, nil
}

func calcRetailPrice(s string) string {
	price, err := strconv.ParseFloat(s, 64)
	if err != nil {
		return s
	}
	return fmt.Sprintf("%.2f", price-0.01)
}

func calcCostPrice(s string) string {
	price, err := strconv.ParseFloat(s, 64)
	if err != nil {
		return s
	}
	discounted := price * (1 - 0.2273)
	// Round to 2 decimal places
	discounted = math.Round(discounted*100) / 100
	return fmt.Sprintf("%.2f", discounted)
}

func parseBaseColor(s string) string {
	if idx := strings.Index(strings.ToUpper(s), " OR "); idx != -1 {
		return strings.TrimSpace(s[:idx])
	}
	return s
}
