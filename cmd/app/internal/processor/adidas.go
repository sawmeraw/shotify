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

	data, err := p.getProductData()
	if err != nil {
		return data, fmt.Errorf("Error caught: %w", err)
	}

	for _, d := range data {
		fmt.Println(d)
		fmt.Println("-------")
	}
	return data, nil
}

// this works dont touch it
func (p *AdidasProcessor) getProductData() ([]model.ProductData, error) {
	var data []model.ProductData

	sheetName := p.file.GetSheetName(p.metadataSheetIndex)

	rows, err := p.file.GetRows(sheetName)

	if err != nil {
		return data, fmt.Errorf("Error reading rows from the first sheet: %w", err)
	}

	if len(rows) == 0 {
		return data, nil
	}

	productCodeColIndex := FindColumnIndex(rows[0], "Article No.")
	if productCodeColIndex == -1 {
		return nil, fmt.Errorf("Column 'Article No.' not found")
	}

	colorNameColIndex := FindColumnIndex(rows[0], "Colorway Name")
	if colorNameColIndex == -1 {
		return nil, fmt.Errorf("Column 'Colorway Name' not found")
	}

	baseColorColIndex := FindColumnIndex(rows[0], "B2B Base Color")
	if baseColorColIndex == -1 {
		return nil, fmt.Errorf("Column 'B2B Base Color' not found")
	}

	articleNameColIndex := FindColumnIndex(rows[0], "Article Name")
	if articleNameColIndex == -1 {
		return nil, fmt.Errorf("Column 'Article Name' not found")
	}

	genderColIndex := FindColumnIndex(rows[0], "B2B Gender Age")
	if genderColIndex == -1 {
		return nil, fmt.Errorf("Column 'B2B Gender Age' not found")
	}

	for _, row := range rows[1:] {
		if productCodeColIndex < len(row) {
			productCode := strings.TrimSpace(row[productCodeColIndex])
			colorName := strings.TrimSpace(row[colorNameColIndex])
			baseColor := strings.TrimSpace(row[baseColorColIndex])
			articleName := strings.TrimSpace(row[articleNameColIndex])
			genderValue := strings.TrimSpace(row[genderColIndex])
			var gender string
			if genderValue == "WOMEN" {
				gender = "W"
			} else if genderValue == "MEN" {
				gender = "M"
			} else if genderValue == "UNISEX" {
				gender = "U"
			} else {
				gender = "K"
			}

			currentProduct := model.ProductData{
				BrandName:    "adidas",
				SupplierCode: "ad",
				ProductCode:  productCode,
				ColorName:    colorName,
				BaseColor:    baseColor,
				ModelName:    articleName,
				Gender:       gender,
			}

			data = append(data, currentProduct)
		}
	}
	return data, nil
}
