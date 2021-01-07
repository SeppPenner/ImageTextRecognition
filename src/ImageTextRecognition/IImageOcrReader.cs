// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImageOcrReader.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The image OCR reader interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ImageTextRecognition
{
    /// <summary>
    /// The image OCR reader interface.
    /// </summary>
    public interface IImageOcrReader
    {
        /// <summary>
        /// Reads the text on the given image.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The read <see cref="string"/>.</returns>
        string ReadTextOnImage(string fileName);
    }
}