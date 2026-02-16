package processor

import "testing"

func TestParseBaseColor(t *testing.T) {
	tests := []struct {
		input    string
		expected string
	}{
		{"BLACK OR GREY", "BLACK"},
		{"Black or White", "Black"},
		{"Green Or Blue", "Green"},
		{"Red", "Red"},
		{"", ""},
		{"ORANGE OR RED OR BLUE", "ORANGE"},
	}

	for _, tt := range tests {
		got := parseBaseColor(tt.input)
		if got != tt.expected {
			t.Errorf("parseBaseColor(%q) = %q; want %q", tt.input, got, tt.expected)
		}
	}
}

func TestCalcRetailPrice(t *testing.T) {
	tests := []struct {
		input    string
		expected string
	}{
		{"100", "99.99"},
		{"390", "389.99"},
		{"49.95", "49.94"},
		{"0", "-0.01"},
		{"invalid", "invalid"},
	}

	for _, tt := range tests {
		got := calcRetailPrice(tt.input)
		if got != tt.expected {
			t.Errorf("calcRetailPrice(%q) = %q; want %q", tt.input, got, tt.expected)
		}
	}
}

func TestCalcCostPrice(t *testing.T) {
	tests := []struct {
		input    string
		expected string
	}{
		{"100", "77.27"},
		{"229.41", "177.27"},
		{"0", "0.00"},
		{"invalid", "invalid"},
	}

	for _, tt := range tests {
		got := calcCostPrice(tt.input)
		if got != tt.expected {
			t.Errorf("calcCostPrice(%q) = %q; want %q", tt.input, got, tt.expected)
		}
	}
}
