package server

import (
	"io"
	"log"
	"net/http"
	"os"
	"path/filepath"

	"github.com/sawmeraw/goexcelparser/cmd/app/internal/processor"
	"github.com/sawmeraw/goexcelparser/cmd/app/internal/writer"
)

func NewMux() *http.ServeMux {
	mux := http.NewServeMux()
	mux.HandleFunc("POST /process", handleProcess)
	return mux
}

func handleProcess(w http.ResponseWriter, r *http.Request) {
	if err := r.ParseMultipartForm(32 << 20); err != nil {
		http.Error(w, "failed to parse form: "+err.Error(), http.StatusBadRequest)
		return
	}

	brand := r.FormValue("brand")
	if brand == "" {
		http.Error(w, "missing 'brand' field", http.StatusBadRequest)
		return
	}

	file, header, err := r.FormFile("file")
	if err != nil {
		http.Error(w, "missing 'file' field: "+err.Error(), http.StatusBadRequest)
		return
	}
	defer file.Close()

	ext := filepath.Ext(header.Filename)
	tmp, err := os.CreateTemp("", "upload-*"+ext)
	if err != nil {
		http.Error(w, "failed to create temp file: "+err.Error(), http.StatusInternalServerError)
		return
	}
	defer os.Remove(tmp.Name())

	if _, err := io.Copy(tmp, file); err != nil {
		tmp.Close()
		http.Error(w, "failed to save upload: "+err.Error(), http.StatusInternalServerError)
		return
	}
	tmp.Close()

	proc, err := processor.New(tmp.Name(), brand)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}
	defer proc.Close()

	products, err := proc.Parse()
	if err != nil {
		http.Error(w, "processing failed: "+err.Error(), http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
	w.Header().Set("Content-Disposition", `attachment; filename="rex_output.xlsx"`)

	if err := writer.WriteProductsToWriter(products, w); err != nil {
		log.Printf("error writing response: %v", err)
	}
}
