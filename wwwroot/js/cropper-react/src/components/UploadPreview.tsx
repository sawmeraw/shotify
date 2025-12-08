interface ImagePreviewProps {
    files: File[],
    onRemove: (index: number) => void,
}

const ImagePreview: React.FC<ImagePreviewProps> = ({ files, onRemove }) => {
    return (
        <>
            <div className="w-full px-2 py-4 bg-white rounded overflow-hidden flex flex-col shadow-lg h-full">
                <h2 className="text-xl font-semibold mb-4">Preview</h2>
                <div className="flex flex-row gap-4">
                    {files.map((file, index) => (
                        <div key={index} className="relative border-2 border-sky-300 rounded-md">
                            <img
                                src={URL.createObjectURL(file)}
                                alt={`Preview ${file.name}`}
                                className="w-32 h-32 object-cover rounded-lg"
                            />
                            <button
                                type="button"
                                onClick={() => onRemove(index)}
                                className="absolute top-1 right-1 bg-red-500 text-white rounded-full w-6 h-6 flex items-center justify-center hover:bg-red-600"
                            >
                                &times;
                            </button>
                        </div>
                    ))}
                </div>
            </div>
        </>
    )
}

export default ImagePreview;