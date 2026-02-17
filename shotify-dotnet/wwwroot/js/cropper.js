/**
 * @param {Uint8ClampedArray} data
 * @param {number} canvasWidth
 * @param {number} canvasHeight
 * @returns {{r: number, g: number, b: number}} 
 */

function getBackgroundColor(data, width, height) {
  const colorMap = new Map();

  for (let y = 0; y < height; y++) {
    for (let x = 0; x < width; x++) {
      const i = (y * width + x) * 4;
      const r = data[i];
      const g = data[i + 1];
      const b = data[i + 2];
      const a = data[i + 3];

      if (a === 0) continue;

      const key = `${r},${g},${b}`;
      const count = colorMap.get(key) || 0;
      colorMap.set(key, count + 1);
    }
  }

  let maxColor = null;
  let maxCount = -1;
  for (const [key, count] of colorMap.entries()) {
    if (count > maxCount) {
      maxCount = count;
      maxColor = key;
    }
  }

  if (!maxColor) return { r: 0, g: 0, b: 0 };

  const [r, g, b] = maxColor.split(',').map(Number);
  return { r, g, b };
}

/**
 * @param {File} file 
 * @returns {Promise<File>} processed file
 */


export const processImage = async (file, padding = 40) => {
    const PADDING = padding;

    return new Promise((resolve, reject) => {
        const img = new Image();
        img.src = URL.createObjectURL(file);

        img.onload = () => {
            const canvas = document.createElement('canvas');
            const ctx = canvas.getContext('2d');
            if (!ctx) {
                reject(new Error('Canvas context not available'));
                return;
            }

            const canvasWidth = img.width;
            const canvasHeight = img.height;
            canvas.width = canvasWidth;
            canvas.height = canvasHeight;
            ctx.drawImage(img, 0, 0, canvasWidth, canvasHeight);

            const imageData = ctx.getImageData(0, 0, canvasWidth, canvasHeight);
            const data = imageData.data;
            const backgroundColor = getBackgroundColor(data, canvasWidth, canvasHeight);

            // Find the content bounding box
            const topEdge = calculateTopPadding(data, canvasWidth, canvasHeight, backgroundColor);
            const bottomPadding = calculateBottomPadding(data, canvasWidth, canvasHeight, backgroundColor);
            const bottomEdge = canvasHeight - bottomPadding;
            const { leftStart, rightEnd } = findEdges(data, canvasWidth, canvasHeight, backgroundColor);

            // Content dimensions
            const contentWidth = rightEnd - leftStart + 1;
            const contentHeight = bottomEdge - topEdge;

            // Add desired padding, then make it square
            const paddedWidth = contentWidth + PADDING * 2;
            const paddedHeight = contentHeight + PADDING * 2;
            const squareSize = Math.max(paddedWidth, paddedHeight);

            const finalCanvas = document.createElement('canvas');
            finalCanvas.width = squareSize;
            finalCanvas.height = squareSize;
            const finalCtx = finalCanvas.getContext('2d');

            if (!finalCtx) {
                reject(new Error('Final canvas context not available'));
                return;
            }

            // Fill entire square with background color
            finalCtx.fillStyle = `rgb(${backgroundColor.r !== 0 ? backgroundColor.r : 255}, ${backgroundColor.g !== 0 ? backgroundColor.g : 255}, ${backgroundColor.b !== 0 ? backgroundColor.b : 255})`;
            finalCtx.fillRect(0, 0, squareSize, squareSize);

            // Center the content in the square
            const x = (squareSize - contentWidth) / 2;
            const y = (squareSize - contentHeight) / 2;

            finalCtx.drawImage(
                canvas,
                leftStart, topEdge, contentWidth, contentHeight,
                x, y, contentWidth, contentHeight
            );

            finalCanvas.toBlob((blob) => {
                if (!blob) {
                    reject(new Error('Failed to create blob'));
                    return;
                }
                const processedFile = new File([blob], file.name, { type: 'image/jpeg' });
                resolve(processedFile);
            }, 'image/jpeg');
        };

        img.onerror = () => {
            reject(new Error('Failed to load image'));
        };
    });
};
/**
 * @param {Uint8ClampedArray} data
 * @param {number} canvasWidth
 * @param {number} canvasHeight 
 * @param {{r: number, g: number, b: number}} backgroundColor 
 * @returns {number} canvas height 
 */
//return the top edge of the object in the image
export const calculateTopPadding = (
    data,
    canvasWidth,
    canvasHeight,
    backgroundColor
) => {
    const tolerance = 10;
    // Minimum non-background pixels in a row to count as content
    const minContentPixels = Math.max(3, Math.floor(canvasWidth * 0.005));

    for (let y = 0; y < canvasHeight; y++) {
        let contentCount = 0;
        for (let x = 0; x < canvasWidth; x++) {
            const pixelIndex = (y * canvasWidth + x) * 4;
            const r = data[pixelIndex];
            const g = data[pixelIndex + 1];
            const b = data[pixelIndex + 2];
            const a = data[pixelIndex + 3];

            if (a === 0) continue;
            if (
                Math.abs(r - backgroundColor.r) > tolerance ||
                Math.abs(g - backgroundColor.g) > tolerance ||
                Math.abs(b - backgroundColor.b) > tolerance
            ) {
                contentCount++;
                if (contentCount >= minContentPixels) return y;
            }
        }
    }
    return canvasHeight;
};

/**
 * @param {Uint8ClampedArray} data
 * @param {number} canvasWidth
 * @param {number} canvasHeight 
 * @param {{r: number, g: number, b: number}} backgroundColor 
 * @returns {number} 
 */


export const calculateBottomPadding = (
    data,
    canvasWidth,
    canvasHeight,
    backgroundColor
) => {
    const tolerance = 10;
    const minContentPixels = Math.max(3, Math.floor(canvasWidth * 0.005));

    for (let y = canvasHeight - 1; y >= 0; y--) {
        let contentCount = 0;
        for (let x = 0; x < canvasWidth; x++) {
            const pixelIndex = (y * canvasWidth + x) * 4;
            const r = data[pixelIndex];
            const g = data[pixelIndex + 1];
            const b = data[pixelIndex + 2];
            const a = data[pixelIndex + 3];

            if (a === 0) continue;
            if (
                Math.abs(r - backgroundColor.r) > tolerance ||
                Math.abs(g - backgroundColor.g) > tolerance ||
                Math.abs(b - backgroundColor.b) > tolerance
            ) {
                contentCount++;
                if (contentCount >= minContentPixels) return canvasHeight - y - 1;
            }
        }
    }
    return canvasHeight;
};

//find the edges on the sides of the object

/**
 * @param {Uint8ClampedArray} data
 * @param {number} canvasWidth
 * @param {number} canvasHeight 
 * @param {{r: number, g: number, b: number}} backgroundColor 
 * @returns {{start: number, end: number}}
 */

export const findEdges = (
    data,
    canvasWidth,
    canvasHeight,
    backgroundColor
) => {
    let leftStart = canvasWidth;
    let rightEnd = 0;
    //some tolerance for the transparency 
    const tolerance = 10;

    // console.log("Background color: ", backgroundColor.r, backgroundColor.g, backgroundColor.b);

    const isBackground = (r,g,b,a) =>{
        if (a === 0) return true;
        return (
            Math.abs(r - backgroundColor.r) <= tolerance &&
            Math.abs(g - backgroundColor.g) <= tolerance &&
            Math.abs(b - backgroundColor.b) <= tolerance
        )
    }

    for (let y = 0; y < canvasHeight; y++) {
        for (let x = 0; x < canvasWidth; x++) {
            const pixelIndex = (y * canvasWidth + x) * 4;
            const r = data[pixelIndex];
            const g = data[pixelIndex + 1];
            const b = data[pixelIndex + 2];
            const a = data[pixelIndex + 3];

            if (!isBackground(r, g, b, a)) {
                if (x < leftStart) leftStart = x;
                if (x > rightEnd) rightEnd = x;
            }
        }
    }
    // console.log({leftStart, rightEnd});
    return { leftStart, rightEnd };
};

/**
 * @param {File} file
 * @returns {Promise<string>}
 */
export const fileToDataUrl = (file) => {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => {
            resolve(reader.result);
        };
        reader.onerror = () => {
            reject(new Error('Failed to read file'));
        };
        //base64 encoded url for the src property of the image
        reader.readAsDataURL(file);
    });
};

/**
 * @param {string} productCode
 * @param {string[]} images 
 * @returns {Promise<void>}
 */
export async function previewImagesDownload(productCode, images){
    const fileArray = [];

    for (let image of images){
        try{
            const response = await fetch(image);
            if(!response.ok){
                return;
            }
            const blob = await response.blob();
            try{
                const file = new File([blob], productCode, {type: "image/jpeg"});
                const processedFile = await processImage(file);
                fileArray.push(processedFile);
            } catch(error){
                console.error(error);
                return;
            }
        } catch(error){
            console.error(error);
        }
        
    }

    for (let file of fileArray){
        const dataUrl  = await fileToDataUrl(file);
        const link = document.createElement("a");
        link.href = dataUrl;
        const hash = randomString(8);
        productCode = productCode?  productCode.replace(".", "") : productCode;
        link.download = `${productCode ? productCode : "shotify"}-${hash.substring(0, 4)}.jpeg`;
        link.click();
    }
}

/**
 * Generate a random alphanumeric string
 * @param {number} length - desired length of the string
 * @returns {string}
 */
export function randomString(length) {
  const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
  let result = '';
  for (let i = 0; i < length; i++) {
    const randomIndex = Math.floor(Math.random() * chars.length);
    result += chars[randomIndex];
  }
  return result;
}