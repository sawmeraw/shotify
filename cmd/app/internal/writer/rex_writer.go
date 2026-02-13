package writer

import (
	"fmt"
	"os"
	"path/filepath"

	"github.com/sawmeraw/goexcelparser/cmd/app/internal/model"
	"github.com/xuri/excelize/v2"
)

func WriteProductsToExcel(products []model.ProductData, path string) error {

	if err := os.MkdirAll(filepath.Dir(path), 0755); err != nil {
		return err
	}

	f := excelize.NewFile()
	sheet := "Sheet1"

	headers := []string{
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

	for i, h := range headers {
		col := ExcelColumnName(i)
		cell := fmt.Sprintf("%s1", col)
		f.SetCellValue(sheet, cell, h)
	}

	f.SaveAs(path)

	return nil
}

func ExcelColumnName(n int) string {
	name := ""
	for n >= 0 {
		name = string(rune('A'+(n%26))) + name
		n = n/26 - 1
	}
	return name
}
