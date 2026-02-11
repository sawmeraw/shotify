package processor

import (
	"fmt"
	"strings"

	"github.com/sawmeraw/goexcelparser/cmd/app/internal/model"
	"github.com/xuri/excelize/v2"
)

type AdidasProcessor struct {
	file               *excelize.File
	writeSheetIndex    int
	metadataSheetIndex int
	barcodeSheetIndex  int
}

func NewAdidasProcessor(relativeFilePath string) (*AdidasProcessor, error) {
	f, err := excelize.OpenFile(relativeFilePath)
	if err != nil {
		return nil, fmt.Errorf("Error opening adidas file: %w", err)
	}
	//dont do this here, do it after the file has been read
	//defer f.Close()

	sheetIndex, err := f.NewSheet("MassUploadProducts")
	if err != nil {
		return nil, fmt.Errorf("Couldnt create new sheet in the file: %w", err)
	}

	metaDataSheetIndex, err := f.GetSheetIndex("English")
	if err != nil {
		return nil, fmt.Errorf("Couldnt find the first sheet English in the file: %w", err)
	}

	barcodeSheetIndex, err := f.GetSheetIndex("EAN UPC")
	if err != nil {
		return nil, fmt.Errorf("Couldnt find the second EAN UPC sheet in the file: %w", err)
	}

	return &AdidasProcessor{
		file:               f,
		writeSheetIndex:    sheetIndex,
		metadataSheetIndex: metaDataSheetIndex,
		barcodeSheetIndex:  barcodeSheetIndex,
	}, nil
}

func (p *AdidasProcessor) Close() error {
	return p.file.Close()
}

func (p *AdidasProcessor) Parse() ([]model.ProductData, error) {
	var data []model.ProductData

	productCodes, err := p.getUnqiueProductCodes()
	if err != nil {
		return data, fmt.Errorf("Error caught: %w", err)
	}

	for _, code := range productCodes {
		fmt.Printf("Product Code found: %s\n", code)
	}
	return data, nil
}

// this works dont touch it
func (p *AdidasProcessor) getUnqiueProductCodes() ([]string, error) {
	var codes []string

	sheetName := p.file.GetSheetName(p.metadataSheetIndex)

	rows, err := p.file.GetRows(sheetName)

	if err != nil {
		return codes, fmt.Errorf("Error reading rows from the first sheet: %w", err)
	}

	if len(rows) == 0 {
		return codes, nil
	}

	colIndex := FindColumnIndex(rows[0], "Article No.")
	if colIndex == -1 {
		return nil, fmt.Errorf("Column 'Article No.' not found")
	}

	seen := make(map[string]bool)
	for _, row := range rows[1:] {
		if colIndex < len(row) {
			val := strings.TrimSpace(row[colIndex])
			if val != "" && !seen[val] {
				seen[val] = true
				codes = append(codes, val)
			}
		}
	}
	return codes, nil
}
