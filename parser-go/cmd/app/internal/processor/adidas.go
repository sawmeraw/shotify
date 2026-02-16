package processor

import (
	"fmt"
	"strings"

	"github.com/sawmeraw/shotify/parser-go/cmd/app/internal/model"
	"github.com/xuri/excelize/v2"
)

type AdidasProcessor struct {
	file               *excelize.File
	writeSheetIndex    int
	metadataSheetIndex int
	barcodeSheetIndex  int
}

type metaResult struct {
	data map[string]model.ProductData
	err  error
}

type barcodeResult struct {
	data map[string]map[string]string
	err  error
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

	metadataChan := make(chan metaResult)
	barcodeChan := make(chan barcodeResult)

	go func() {
		m, err := p.parseMetadata()
		metadataChan <- metaResult{data: m, err: err}
	}()

	go func() {
		b, err := p.parseBarcodes()
		barcodeChan <- barcodeResult{data: b, err: err}
	}()

	meta := <-metadataChan
	if meta.err != nil {
		return nil, meta.err
	}

	barcodes := <-barcodeChan
	if barcodes.err != nil {
		return nil, barcodes.err
	}

	final := make([]model.ProductData, 0, len(meta.data))

	for code, base := range meta.data {
		if skuMap, ok := barcodes.data[code]; ok {
			base.SizeSKUs = skuMap
		}
		final = append(final, base)
	}

	return final, nil
}

// this works dont touch it
func (p *AdidasProcessor) parseMetadata() (map[string]model.ProductData, error) {
	data := make(map[string]model.ProductData)

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
			switch genderValue {
			case "WOMEN":
				gender = "W"
			case "MEN":
				gender = "M"
			case "MEN & WOMEN":
				gender = "U"
			default:
				gender = "K"
			}

			currentProduct := model.ProductData{
				BrandName:    "Adidas",
				SupplierCode: "ad",
				ProductCode:  productCode,
				ColorName:    colorName,
				BaseColor:    baseColor,
				ModelName:    articleName,
				Gender:       gender,
				SizeSKUs:     make(map[string]string),
			}

			data[productCode] = currentProduct
		}
	}
	return data, nil
}

func (p *AdidasProcessor) parseBarcodes() (map[string]map[string]string, error) {
	productMap := make(map[string]map[string]string)

	sheetName := p.file.GetSheetName(p.barcodeSheetIndex)

	rows, err := p.file.GetRows(sheetName)

	if err != nil {
		return productMap, fmt.Errorf("Error reading rows from the second sheet: %w", err)
	}

	if len(rows) == 0 {
		return productMap, nil
	}

	productCodeColIndex := FindColumnIndex(rows[0], "Article No.")
	if productCodeColIndex == -1 {
		return nil, fmt.Errorf("Column 'Article No.' not found")
	}

	sizeColIndex := FindColumnIndex(rows[0], "Size-Australia")
	if sizeColIndex == -1 {
		return nil, fmt.Errorf("Column 'Size-Australia' not found'")
	}

	barcodeColIndex := FindColumnIndex(rows[0], "EAN Number")
	if barcodeColIndex == -1 {
		return nil, fmt.Errorf("Column 'EAN Number' not found")
	}

	for _, row := range rows[1:] {
		if productCodeColIndex < len(row) {
			currentProductCode := strings.TrimSpace(row[productCodeColIndex])
			currentBarcode := strings.TrimSpace(row[barcodeColIndex])
			currentSize := strings.TrimSpace(row[sizeColIndex])

			if currentProductCode == "" || currentSize == "" || currentBarcode == "" {
				continue
			}

			if _, ok := productMap[currentProductCode]; !ok {
				productMap[currentProductCode] = make(map[string]string)
			}

			productMap[currentProductCode][normalizeSizes(currentSize)] = currentBarcode
		}
	}

	return productMap, nil
}

func normalizeSizes(s string) string {
	size := strings.TrimSpace(s)

	if strings.HasSuffix(size, "-K") {
		size = strings.TrimSuffix(size, "-K") + "K"
	} else if strings.HasSuffix(size, "-") {
		size = strings.TrimSuffix(size, "-")
		size = strings.TrimSpace(size)
		size = size + ".5"
	}

	return size
}
