package writer_test

import (
	"testing"

	"github.com/sawmeraw/goexcelparser/cmd/app/internal/writer"
)

func TestExcelColumnNames(t *testing.T) {
	tests := []struct {
		input    int
		expected string
	}{
		{0, "A"}, {1, "B"}, {25, "Z"}, {26, "AA"}, {27, "AB"}, {28, "AC"}, {51, "AZ"}, {52, "BA"}, {701, "ZZ"}, {702, "AAA"},
	}

	for _, tt := range tests {
		got := writer.ExcelColumnName(tt.input)
		if got != tt.expected {
			t.Errorf("ExcelColumnName(%d) = %q; want %q", tt.input, got, tt.expected)
		}
	}
}
