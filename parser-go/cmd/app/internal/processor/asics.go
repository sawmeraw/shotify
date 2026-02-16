package processor

import (
	"fmt"
	"strconv"
	"strings"
	"time"

	"github.com/sawmeraw/shotify/parser-go/cmd/app/internal/model"
	"github.com/xuri/excelize/v2"
)

type AsicsProcessor struct {
	file *excelize.File
}

func NewAsicsProcessor(filePath string) (*AsicsProcessor, error) {
	f, err := excelize.OpenFile(filePath)
	if err != nil {
		return nil, fmt.Errorf("error opening asics file: %w", err)
	}
	return &AsicsProcessor{file: f}, nil
}

func (p *AsicsProcessor) Close() error {
	return p.file.Close()
}

func (p *AsicsProcessor) Parse() ([]model.ProductData, error) {
	rows, err := p.file.GetRows("Collection")
	if err != nil {
		return nil, fmt.Errorf("error reading Collection sheet: %w", err)
	}

	if len(rows) == 0 {
		return nil, fmt.Errorf("empty Collection sheet")
	}

	header := rows[0]

	eanIdx := FindColumnIndex(header, "EAN code")
	if eanIdx == -1 {
		return nil, fmt.Errorf("column 'EAN code' not found")
	}
	fashionThemeIdx := FindColumnIndex(header, "Fashion Theme")
	if fashionThemeIdx == -1 {
		return nil, fmt.Errorf("column 'Fashion Theme' not found")
	}
	tradingCodeIdx := FindColumnIndex(header, "Trading code")
	if tradingCodeIdx == -1 {
		return nil, fmt.Errorf("column 'Trading code' not found")
	}
	colorCodeIdx := FindColumnIndex(header, "Color code")
	if colorCodeIdx == -1 {
		return nil, fmt.Errorf("column 'Color code' not found")
	}
	itemNameIdx := FindColumnIndex(header, "Item name")
	if itemNameIdx == -1 {
		return nil, fmt.Errorf("column 'Item name' not found")
	}
	colorNameIdx := FindColumnIndex(header, "Color name")
	if colorNameIdx == -1 {
		return nil, fmt.Errorf("column 'Color name' not found")
	}
	sizeIdx := FindColumnIndex(header, "Size US")
	if sizeIdx == -1 {
		return nil, fmt.Errorf("column 'Size US' not found")
	}
	unitPriceIdx := FindColumnIndex(header, "Unit price")
	if unitPriceIdx == -1 {
		return nil, fmt.Errorf("column 'Unit price' not found")
	}
	retailPriceIdx := FindColumnIndex(header, "Suggested Retail Price")
	if retailPriceIdx == -1 {
		return nil, fmt.Errorf("column 'Suggested Retail Price' not found")
	}

	productMap := make(map[string]*model.ProductData)

	for _, row := range rows[1:] {
		if len(row) <= retailPriceIdx {
			continue
		}

		tradingCode := strings.TrimSpace(row[tradingCodeIdx])
		colorCode := strings.TrimSpace(row[colorCodeIdx])
		ean := strings.TrimSpace(row[eanIdx])
		size := strings.TrimSpace(row[sizeIdx])

		if tradingCode == "" || ean == "" || size == "" {
			continue
		}

		productCode := tradingCode + "." + colorCode

		if _, ok := productMap[productCode]; !ok {
			gender := string(getGenderFromTradingCode(tradingCode))
			if gender == "\x00" {
				gender = "U"
			}

			retailPrice := asicsParsePrice(strings.TrimSpace(row[retailPriceIdx]))
			costPrice := asicsParsePrice(strings.TrimSpace(row[unitPriceIdx]))
			season := asicsMapSeason(strings.TrimSpace(row[fashionThemeIdx]))

			// base color from color name: take first word before "/"
			baseColor := ""
			cn := strings.TrimSpace(row[colorNameIdx])
			if idx := strings.Index(cn, "/"); idx != -1 {
				baseColor = strings.TrimSpace(cn[:idx])
			} else {
				baseColor = cn
			}

			productMap[productCode] = &model.ProductData{
				BrandName:    "Asics",
				SupplierCode: "AC",
				ProductCode:  productCode,
				ModelName:    strings.TrimSpace(row[itemNameIdx]),
				BaseColor:    baseColor,
				ColorName:    cn,
				Gender:       gender,
				Season:       season,
				SizeSKUs:     make(map[string]string),
				RetailPrice:  &retailPrice,
				CostPrice:    &costPrice,
			}
		}

		productMap[productCode].SizeSKUs[size] = ean
	}

	products := make([]model.ProductData, 0, len(productMap))
	for _, p := range productMap {
		products = append(products, *p)
	}

	return products, nil
}

func getGenderFromTradingCode(tradingCode string) rune {
	if len(tradingCode) < 7 {
		return rune(0)
	}

	switch tradingCode[3] {
	case '1':
		return 'M'
	case '2':
		return 'W'
	case '3':
		return 'U'
	case '4':
		return 'K'
	default:
		return rune(0)
	}
}

// asicsParsePrice strips the "$" prefix, parses as float, and subtracts 0.01 for retail.
func asicsParsePrice(s string) string {
	s = strings.TrimPrefix(s, "$")
	s = strings.ReplaceAll(s, ",", "")
	price, err := strconv.ParseFloat(s, 64)
	if err != nil {
		return s
	}
	return fmt.Sprintf("%.2f", price-0.01)
}

// asicsMapSeason maps a 3-letter month abbreviation to YY-QN format based on current year.
func asicsMapSeason(monthAbbr string) string {
	months := map[string]int{
		"JAN": 1, "FEB": 2, "MAR": 3,
		"APR": 4, "MAY": 5, "JUN": 6,
		"JUL": 7, "AUG": 8, "SEP": 9,
		"OCT": 10, "NOV": 11, "DEC": 12,
	}

	m, ok := months[strings.ToUpper(monthAbbr)]
	if !ok {
		return monthAbbr
	}

	quarter := (m-1)/3 + 1
	year := time.Now().Year() % 100

	return fmt.Sprintf("%02d-Q%d", year, quarter)
}
