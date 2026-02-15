package processor

import "testing"

func TestNormalizeSizes(t *testing.T) {
	tests := []struct {
		input    string
		expected string
	}{
		{"10", "10"},
		{"9-", "9.5"},
		{"10-K", "10K"},
		{" 11- ", "11.5"},
		{"8", "8"},
		{"7.5", "7.5"},
		{" 6-K ", "6K"},
	}

	for _, tt := range tests {
		got := normalizeSizes(tt.input)
		if got != tt.expected {
			t.Errorf("normalizeSizes(%q) = %q; want %q", tt.input, got, tt.expected)
		}
	}
}
