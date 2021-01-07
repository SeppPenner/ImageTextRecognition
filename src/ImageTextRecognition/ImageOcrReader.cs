// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageOcrReader.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The image OCR reader class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ImageTextRecognition
{
    using IronOcr;

    /// <inheritdoc cref="IImageOcrReader"/>
    /// <summary>
    /// The image OCR reader class.
    /// </summary>
    /// <seealso cref="IImageOcrReader"/>
    public class ImageOcrReader : IImageOcrReader
    {
        /// <inheritdoc cref="IImageOcrReader"/>
        /// <summary>
        /// Reads the text on the given image.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The read <see cref="string"/>.</returns>
        /// <seealso cref="IImageOcrReader"/>
        public string ReadTextOnImage(string fileName)
        {
            var ocrHelper = new IronTesseract();
            using var input = new OcrInput(fileName);
            input.Deskew();
            input.DeNoise();
            var result = ocrHelper.Read(input);
            return result.Text;
        }
    }
}