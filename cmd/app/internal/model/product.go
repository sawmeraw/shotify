package model

//ProductData represents one product code with an embedded map including sizes and skus
type ProductData struct {
	BrandName    string
	SupplierCode string
	ProductCode  string
	Gender       string
	//can be either shoe model or apparel name
	ModelName string
	ColorName string
	SizeSKUs  map[string]string
	//nullable width for apparel
	Width       *string
	CostPrice   *string
	RetailPrice *string
}
