using SkiaSharp;

namespace AssetToolkit.Image;

public class ImageService
{
    /// <summary>
    /// Takes a byte array and saves it as a PNG image to the specified file path.
    /// </summary>
    /// <param name="filePath">The file path without extensions.</param>
    /// <param name="bytes">The bytes to save.</param>
    /// <param name="width">The image width.</param>
    /// <param name="height">The image height.</param>
    public void SavePngImage(string filePath, byte[] bytes, uint width, uint height)
    {
        using SKBitmap bitmap = new((int) width, (int) height);
        unsafe
        {
            fixed (byte* data = bytes)
            {
                bitmap.SetPixels((IntPtr)data);
            }
        }

        using SKImage pngImage = SKImage.FromBitmap(bitmap);
        using SKData pngData = pngImage.Encode(SKEncodedImageFormat.Png, 100);

        // Save to file.
        using var stream = File.OpenWrite($"{filePath}.png");
        pngData.SaveTo(stream);
    }
}
