package adidas

import (
	"fmt"

	"github.com/xuri/excelize/v2"
)

func Process(relativeFilePath string) error {

	f, err := excelize.OpenFile(relativeFilePath)
	if err != nil {
		return err
	}
	defer f.Close()
	fmt.Printf("Number of sheets: %d\n", f.SheetCount)
	return nil
}
