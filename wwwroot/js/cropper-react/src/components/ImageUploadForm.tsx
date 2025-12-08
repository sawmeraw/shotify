'use client';

import { useState, useRef } from 'react';
import ImagePreview from './UploadPreview';
import { v4 as uuid } from "uuid";
import { processImage, fileToDataUrl } from '../utils/ImageProcessor';

export default function ImageUploadForm() {
    const [files, setFiles] = useState<File[]>([]);
    const [processedFiles, setProcessedFiles] = useState<string[]>([]); // Store processed image URLs
    const [isDragging, setIsDragging] = useState<boolean>(false);
    const fileInputRef = useRef<HTMLInputElement>(null);

    const handleFileChange = (e) => {
        if (e.target.files) {
            const selectedFiles = Array.from(e.target.files);
            setFiles((prevFiles) => [...prevFiles, ...selectedFiles]);
        }
    };

    const handleDragOver = (e: DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        setIsDragging(true);
    };

    const handleDragLeave = () => {
        setIsDragging(false);
    };

    const handleDrop = (e: DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        setIsDragging(false);
        const droppedFiles = Array.from(e.dataTransfer.files);
        setFiles((prevFiles) => [...prevFiles, ...droppedFiles]);
    };

    const removeFile = (index: number) => {
        setFiles((prevFiles) => prevFiles.filter((_, i) => i !== index));
    };

    const handleButtonClick = () => {
        if (fileInputRef.current) {
            fileInputRef.current.click();
        }
    };

    const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        if (files.length === 0) {
            return;
        }

        try {
            const processedFiles = await Promise.all(
                files.map(async (file) => {
                    const processedImage = await processImage(file);
                    return processedImage;
                })
            );

            const processedImageUrls = await Promise.all(
                processedFiles.map(async (file) => {
                    return await fileToDataUrl(file);
                })
            );

            setProcessedFiles(processedImageUrls);
        } catch (error) {
            console.error('Error processing files:', error);
        }
    };


    const downloadImage = (dataUrl: string, fileName: string) => {
        const link = document.createElement('a');
        link.href = dataUrl;
        link.download = `imgnext-${uuid().substring(0, 4)}.jpeg`;
        link.click();
    };

    const downloadAllImages = () => {
        processedFiles.forEach((dataUrl, index) => {
            downloadImage(dataUrl, `imgnext_edit-${uuid().substring(0, 4)}`)
        })
    }

    const clearAllImages = () => {
        setFiles([])
        setProcessedFiles([])
    }

    return (
        <>
            <div className="flex w-full">
                <div className="w-1/3 relative bg-white rounded overflow-hidden flex flex-col flex-shrink-0 shadow-lg">
                    <form onSubmit={handleSubmit} className="p-6 bg-white rounded-lg shadow-md">
                        <div
                            onDragOver={handleDragOver}
                            onDragLeave={handleDragLeave}
                            onDrop={handleDrop}
                            className={`p-6 border-2 border-dashed ${isDragging ? 'border-blue-500 bg-blue-50' : 'border-gray-300 bg-gray-50'
                                } rounded-lg text-center cursor-pointer`}
                        >
                            <p className="text-gray-600">
                                Drag & drop images here or{' '}
                                <button
                                    type="button"
                                    onClick={handleButtonClick}
                                    className="text-blue-500 underline focus:outline-none"
                                >
                                    click to upload
                                </button>
                            </p>
                        </div>

                        <input
                            type="file"
                            ref={fileInputRef}
                            className="hidden"
                            multiple
                            onChange={handleFileChange}
                            accept="image/*"
                        />

                        <div className='flex gap-4'><button
                            type="submit"
                            disabled={files.length === 0}
                            className="w-1/3 mt-6 px-4 py-2 bg-black text-white font-semibold rounded hover:bg-stone-600 disabled:bg-gray-300 disabled:cursor-not-allowed"
                        >
                            Process Images
                        </button>
                            <button type='button' onClick={clearAllImages} className=" mt-6 px-4 py-2 bg-black text-white font-semibold rounded hover:bg-stone-600 disabled:bg-gray-300 disabled:cursor-not-allowed">Clear All</button>
                        </div>
                    </form>
                </div>
                <div className="w-2/3">
                    <ImagePreview files={files} onRemove={removeFile} />
                </div>

            </div>

            {processedFiles.length > 0 && (
                <div className="w-full mt-4">
                    <div className='flex gap-8 mb-4'>
                        <h2 className="text-2xl inline font-semibold">Processed Images</h2>
                        <button type='button' onClick={downloadAllImages} className='px-4 py-2 bg-black text-white font-semibold rounded hover:bg-stone-600 disabled:bg-gray-300 disabled:cursor-not-allowed'>Download</button>
                    </div>
                    <div className="flex gap-4">
                        {processedFiles.map((dataUrl, index) => (
                            <div key={index} className="flex flex-col items-center border-2 border-sky-300 rounded">
                                <img
                                    src={dataUrl}
                                    alt={`Processed ${index}`}
                                    className="w-32 h-32 object-contain rounded-lg"
                                />
                            </div>
                        ))}
                    </div>
                </div>
            )}

        </>
    );
}