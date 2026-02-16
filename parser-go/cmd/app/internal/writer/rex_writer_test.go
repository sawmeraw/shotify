package writer

import (
	"testing"

	"github.com/sawmeraw/shotify/parser-go/cmd/app/internal/model"
)

func TestExcelColumnNames(t *testing.T) {
	tests := []struct {
		input    int
		expected string
	}{
		{0, "A"}, {1, "B"}, {25, "Z"}, {26, "AA"}, {27, "AB"}, {28, "AC"}, {51, "AZ"}, {52, "BA"}, {701, "ZZ"}, {702, "AAA"},
	}

	for _, tt := range tests {
		got := ExcelColumnName(tt.input)
		if got != tt.expected {
			t.Errorf("ExcelColumnName(%d) = %q; want %q", tt.input, got, tt.expected)
		}
	}
}

func TestBuildShortDescription(t *testing.T) {
	widthD := "D"

	tests := []struct {
		name     string
		product  model.ProductData
		size     string
		expected string
	}{
		{
			name: "mens default width",
			product: model.ProductData{
				BrandName: "Nike", Gender: "M",
				ModelName: "Alphafly 3", BaseColor: "Black",
				ColorName: "Grey/Blue",
			},
			size:     "10",
			expected: "NIKE M ALPHAFLY 3 (BLACK) GREY/BLUE SZ 10 (D)",
		},
		{
			name: "womens default width",
			product: model.ProductData{
				BrandName: "adidas", Gender: "W",
				ModelName: "Adizero Pro W", BaseColor: "White",
				ColorName: "core white",
			},
			size:     "8",
			expected: "ADIDAS W ADIZERO PRO (WHITE) CORE WHITE SZ 8 (B)",
		},
		{
			name: "explicit width overrides default",
			product: model.ProductData{
				BrandName: "Nike", Gender: "M",
				ModelName: "Pegasus", BaseColor: "Red",
				ColorName: "crimson", Width: &widthD,
			},
			size:     "9.5",
			expected: "NIKE M PEGASUS (RED) CRIMSON SZ 9.5 (D)",
		},
		{
			name: "kids no width",
			product: model.ProductData{
				BrandName: "adidas", Gender: "K",
				ModelName: "Runfalcon K", BaseColor: "Blue",
				ColorName: "navy",
			},
			size:     "5K",
			expected: "ADIDAS K RUNFALCON (BLUE) NAVY SZ 5K",
		},
		{
			name: "dots removed from color name",
			product: model.ProductData{
				BrandName: "Nike", Gender: "U",
				ModelName: "Air Max", BaseColor: "Green",
				ColorName: "volt/blk./wht.",
			},
			size:     "11",
			expected: "NIKE U AIR MAX (GREEN) VOLT/BLK/WHT SZ 11 (D)",
		},
		{
			name: "model suffix M stripped",
			product: model.ProductData{
				BrandName: "adidas", Gender: "M",
				ModelName: "Ultraboost M", BaseColor: "Black",
				ColorName: "core black",
			},
			size:     "12",
			expected: "ADIDAS M ULTRABOOST (BLACK) CORE BLACK SZ 12 (D)",
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			got := buildShortDescription(tt.product, tt.size)
			if got != tt.expected {
				t.Errorf("got  %q\nwant %q", got, tt.expected)
			}
		})
	}
}
