package writer

import (
	"fmt"
	"io"
	"os"
	"path/filepath"
	"sort"
	"strconv"
	"strings"

	"github.com/sawmeraw/goexcelparser/cmd/app/internal/model"
	"github.com/xuri/excelize/v2"
)

var headers = []string{
	"ManufacturerSKU",
	"SupplierSKU",
	"ShortDescription",
	"Size",
	"Colour",
	"Season",
	"Custom1",
	"Custom2",
	"Custom3",
	"SupplierBuy",
	"BuyPriceEx",
	"DirectCosts",
	"RRP",
	"POSPriceMarkupTarget",
	"POSPrice",
	"WebPrice",
	"DiscountPrice",
	"DiscountEnd",
	"ProductType",
	"WebstoreMenuID",
	"LongDescription",
	"WarrantyDetails",
	"LeadTime",
	"CartonQty",
	"CoreProduct",
	"Manufacturer",
	"Brand",
	"SupplierCode",
}

func buildExcelFile(products []model.ProductData) *excelize.File {
	f := excelize.NewFile()
	sheet := "Sheet1"

	for i, h := range headers {
		col := ExcelColumnName(i)
		cell := fmt.Sprintf("%s1", col)
		f.SetCellValue(sheet, cell, h)
	}

	row := 2
	for _, p := range products {
		sizes := make([]string, 0, len(p.SizeSKUs))
		for size := range p.SizeSKUs {
			sizes = append(sizes, size)
		}
		sort.Slice(sizes, func(i, j int) bool {
			a, _ := strconv.ParseFloat(strings.TrimRight(sizes[i], "K"), 64)
			b, _ := strconv.ParseFloat(strings.TrimRight(sizes[j], "K"), 64)
			return a < b
		})

		for _, size := range sizes {
			barcode := p.SizeSKUs[size]
			desc := buildShortDescription(p, size)

			values := map[int]string{
				0:  p.ProductCode,  // ManufacturerSKU
				1:  barcode,        // SupplierSKU
				2:  desc,           // ShortDescription
				3:  size,           // Size
				4:  p.BaseColor,    // Colour
				26: p.BrandName,    // Brand
				27: p.SupplierCode, // SupplierCode
			}
			if p.CostPrice != nil {
				values[9] = *p.CostPrice  // SupplierBuy
				values[10] = *p.CostPrice // BuyPriceEx
			}
			if p.RetailPrice != nil {
				values[12] = *p.RetailPrice // RRP
				values[14] = *p.RetailPrice // POSPrice
			}

			for col, val := range values {
				cell := fmt.Sprintf("%s%d", ExcelColumnName(col), row)
				f.SetCellValue(sheet, cell, val)
			}
			row++
		}
	}

	return f
}

func WriteProductsToExcel(products []model.ProductData, path string) error {
	if err := os.MkdirAll(filepath.Dir(path), 0755); err != nil {
		return err
	}
	f := buildExcelFile(products)
	return f.SaveAs(path)
}

func WriteProductsToWriter(products []model.ProductData, w io.Writer) error {
	f := buildExcelFile(products)
	_, err := f.WriteTo(w)
	return err
}

func buildShortDescription(p model.ProductData, size string) string {
	width := ""
	if p.Width != nil {
		width = fmt.Sprintf(" (%s)", *p.Width)
	} else {
		switch p.Gender {
		case "M", "U":
			width = " (D)"
		case "W":
			width = " (B)"
		}
	}
	modelName := strings.ToUpper(p.ModelName)
	for _, suffix := range []string{" M", " W", " K", " U"} {
		if trimmed, found := strings.CutSuffix(modelName, suffix); found {
			modelName = trimmed
			break
		}
	}

	return fmt.Sprintf("%s %s %s (%s) %s SZ %s%s",
		strings.ToUpper(p.BrandName),
		strings.ToUpper(p.Gender),
		modelName,
		strings.ToUpper(p.BaseColor),
		strings.ReplaceAll(strings.ToUpper(p.ColorName), ".", ""),
		size,
		width,
	)
}

func ExcelColumnName(n int) string {
	name := ""
	for n >= 0 {
		name = string(rune('A'+(n%26))) + name
		n = n/26 - 1
	}
	return name
}
