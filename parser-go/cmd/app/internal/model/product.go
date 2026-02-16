package model

import "fmt"

//ProductData represents one product code with an embedded map including sizes and skus
type ProductData struct {
	BrandName    string
	SupplierCode string
	ProductCode  string
	BaseColor    string
	Gender       string
	Season       string
	//can be either shoe model or apparel name
	ModelName string
	ColorName string
	SizeSKUs  map[string]string
	//nullable width for apparel
	Width       *string
	CostPrice   *string
	RetailPrice *string
}

func (p ProductData) String() string {
	return fmt.Sprintf("Brand: %s\nSupplierCode: %s\nProductCode: %s\nGender: %s\nModelName: %s\nColorName: %s\nWidth: %v\nCostPrice: %v\nRetailPrice: %v\nSizeSKUs: %v\n", p.BrandName, p.SupplierCode, p.ProductCode, p.Gender, p.ModelName, p.ColorName, p.Width, p.CostPrice, p.RetailPrice, p.SizeSKUs)
}
