
using SharpGLTF.Schema2;
using System.Data.SqlTypes;
using System.Drawing;

namespace WadSharp.Parsing;

/// <summary>
/// Sprite contained in a sheet, with its coordinates and size.
/// </summary>
public class SheetSprite
{
    /// <summary>
    /// The constructor for the <see cref="SheetSprite"/>.
    /// </summary>
    /// <param name="sheet">The sheet to which this image belongs to.</param>
    /// <param name="image">The image from WAD file.</param>
    /// <param name="x">The x coordinate in sheet.</param>
    /// <param name="y">The y coordinate in sheet.</param>
    /// <param name="width">The width of this image.</param>
    /// <param name="height">The height of this image.</param>
    internal SheetSprite(Sheet sheet, ParserImage image, uint x, uint y, uint width, uint height)
    {
        Sheet = sheet;
        Image = image;
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// The image contained in the sprite.
    /// </summary>
    public ParserImage Image { get; init; }

    /// <summary>
    /// Sheet to which this sprite belongs.
    /// </summary>
    public Sheet Sheet { get; init; }

    /// <summary>
    /// The x coordinate of the sprite in the sheet.
    /// </summary>
    public uint X { get; init; }

    /// <summary>
    /// The y coordinate of the sprite in the sheet.
    /// </summary>
    public uint Y { get; init; }

    /// <summary>
    /// The width of the sprite in the sheet.
    /// </summary>
    public uint Width { get; init; }

    /// <summary>
    /// The height of the sprite in the sheet.
    /// </summary>
    public uint Height { get; init; }

    /// <summary>
    /// A rectangle representing the sprite in the sheet.
    /// </summary>
    public Rectangle Rectangle => new((int)X, (int)Y, (int)Width, (int)Height);

    /// <summary>
    /// The U0 coordinate of the sprite in the sheet.
    /// </summary>
    public float U0 => (float)X / Sheet.Width;

    /// <summary>
    /// The V0 coordinate of the sprite in the sheet.
    /// </summary>
    public float V0 => (float)Y / Sheet.Height;

    /// <summary>
    /// The V1 coordinate of the sprite in the sheet.
    /// </summary>
    public float U1 => (float)(X + Width) / Sheet.Width;

    /// <summary>
    /// The V1 coordinate of the sprite in the sheet.
    /// </summary>
    public float V1 => (float)(Y + Height) / Sheet.Height;

    /// <summary>
    /// Checks if the sprite intersects with the given sprite sheet part.
    /// </summary>
    /// <param name="other">The other <see cref="SheetSprite"/>.</param>
    /// <returns>
    /// <c>true</c> if the sprite intersects with the given sprite sheet part; otherwise, <c>false</c>.
    /// </returns>
    public bool Intersects(SheetSprite other)
        => Rectangle.IntersectsWith(other.Rectangle);

    /// <summary>
    /// Checks if the sprite intersects with the given <see cref="Rectangle"/>.
    /// </summary>
    /// <param name="rectangle">The <see cref="Rectangle"/>.</param>
    /// <returns>
    /// <c>true</c> if the sprite intersects with the rectangle; otherwise, <c>false</c>.
    /// </returns>
    public bool Intersects(Rectangle rectangle)
        => Rectangle.IntersectsWith(rectangle);

    /// <summary>
    /// Checks if the sprite intersects with the pixel at the given coordinates.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <returns>
    /// <c>true</c> if the sprite intersects with the pixel at the given coordinates; otherwise, <c>false</c>.
    /// </returns>
    public bool IntersectsPixel(uint x, uint y)
        => Rectangle.Contains((int)x, (int)y);
}

/// <summary>
/// The class which represents a sheet of images, such as a texture atlas.
/// </summary>
/// <param name="sheetCollection">The collection of sheets to which this sheet belongs to.</param>
/// <param name="name">The name of the sheet.</param>
public class Sheet
{
    /// <summary>
    /// The shelf. At start it is empty. 
    /// Will be size of first added image in height and full width of sheet.
    /// </summary>
    private Rectangle? shelf;

    /// <summary>
    /// The constructor for the <see cref="Sheet"/>.
    /// </summary>
    /// <param name="sheetCollection">The sheet collection to which this sheet belongs to.</param>
    /// <param name="name">The name of the sheet.</param>
    /// <param name="width">The width of the sheet in pixels.</param>
    /// <param name="height">The height of the sheet in pixels.</param>
    public Sheet(SheetCollection sheetCollection, string name, uint width, uint height)
    {
        SheetCollection = sheetCollection;
        Name = name;
        Width = width;
        Height = height;
        Data = new byte[width * height * 4];
    }

    /// <summary>
    /// The sheet collection to which this sheet belongs.
    /// </summary>
    public SheetCollection SheetCollection { get; init; }

    /// <summary>
    /// The name of the sheet.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// The width of the sheet in pixels.
    /// </summary>
    public uint Width { get; init; }

    /// <summary>
    /// The height of the sheet in pixels.
    /// </summary>
    public uint Height { get; init; }

    /// <summary>
    /// The raw RGBA data of the sheet.
    /// </summary>
    public byte[] Data { get; init; }

    /// <summary>
    /// The sprites contained in this sheet.
    /// </summary>
    public List<SheetSprite> Sprites { get; init; } = new();

    /// <summary>
    /// Tries to created and add the <see cref="SheetSprite"/> to the sheet from the given image.
    /// </summary>
    /// <param name="sprite">The <see cref="ParserImage"/> to from which to create sprite.</param>
    /// <returns>
    /// <c>true</c> if the sprite was created and added; otherwise, <c>false</c>.
    /// </returns>
    public bool TryAddSprite(ParserImage image)
    {
        int marginX = 1;
        int marginY = 1;

        if (shelf == null)
        {
            // Create the shelf.
            shelf = new Rectangle(0, 0, (int)Width, (int)(image.Height * marginY));
        }

        if (TryFindSpaceShelfs(image.Width, image.Height, out uint xCoord, out uint yCoord))
        {
            SheetSprite sprite = new(this, image, xCoord, yCoord, image.Width, image.Height);
            Sprites.Add(sprite);

            int shelfX = (int)xCoord + (int)image.Width;
            int shelfY = (int)yCoord;

            if (Width - shelfY - image.Height < 0)
            {
                // No space at all left in shelf we must return 
                return false;
            }

            shelf = new Rectangle(shelfX, shelfY, (int)(Width - shelfX), (int)image.Height);

            // Copy image data to sheet data.
            for (uint y = 0; y < image.Height; y++)
            {
                for (uint x = 0; x < image.Width; x++)
                {
                    uint sheetIndex = ((yCoord + y) * Width + (xCoord + x)) * 4;
                    uint imageIndex = (y * image.Width + x) * 4;
                    Data[sheetIndex] = image.Data[imageIndex];         // R
                    Data[sheetIndex + 1] = image.Data[imageIndex + 1]; // G
                    Data[sheetIndex + 2] = image.Data[imageIndex + 2]; // B
                    Data[sheetIndex + 3] = image.Data[imageIndex + 3]; // A
                }
            }

            return true;
        }
        return false;
    }

    /// <summary>
    /// Try to find space in the sheet for the given width and height.
    /// </summary>
    /// <param name="width">The required width.</param>
    /// <param name="height">The required height.</param>
    /// <param name="xCoord">The found  x coordinate in sheet.</param>
    /// <param name="yCoord">The found y coordinate in sheet.</param>
    /// <returns>
    /// If <c>true</c> , space was found and coordinates are set; otherwise, <c>false</c>.
    /// </returns>
    public bool TryFindSpaceShelfs(uint width, uint height, out uint xCoord, out uint yCoord)
    {
        uint limitX = Width - width;
        uint limitY = Height - height;

        xCoord = 0;
        yCoord = 0;

        int marginX = 1;
        int marginY = 1;

        // Try to fit in the shelf.
        if (shelf!.Value.Width >= width && shelf.Value.Height >= height)
        {
            xCoord = (uint)(shelf.Value.X);
            yCoord = (uint)(shelf.Value.Y);

            return true;
        }

        // Try to find space below the shelf, thus creating a new shelf.
        shelf = new Rectangle(0, (int)(shelf.Value.Y + height), (int)Width - (int)width, (int)height);

        // Check if the new shelf fits.
        if (shelf!.Value.Width >= width && shelf.Value.Height >= height)
        {
            xCoord = (uint)(shelf.Value.X);
            yCoord = (uint)(shelf.Value.Y);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Try to find space in the sheet for the given width and height.
    /// </summary>
    /// <param name="width">The required width.</param>
    /// <param name="height">The required height.</param>
    /// <param name="xCoord">The found  x coordinate in sheet.</param>
    /// <param name="yCoord">The found y coordinate in sheet.</param>
    /// <returns>
    /// If <c>true</c> , space was found and coordinates are set; otherwise, <c>false</c>.
    /// </returns>
    public bool TryFindSpace(uint width, uint height, out uint xCoord, out uint yCoord)
    {
        uint limitX = Width - width;
        uint limitY = Height - height;

        xCoord = 0;
        yCoord = 0;

        uint marginX = 1;
        uint marginY = 1;

        for (; yCoord < limitY; yCoord++)
        {
            for (xCoord = 0; xCoord < limitX; xCoord++)
            {
                bool hasSpace = true;

                Rectangle rectangle = new(
                    (int)(xCoord + marginX),
                    (int)(yCoord + marginY),
                    (int)(width - 2 * marginX),
                    (int)(height - 2 * marginY));

                // Try to intersect with existing sprites.
                SheetSprite? intersect = Sprites.FirstOrDefault(s => s.Intersects(rectangle));
                if (intersect != null)
                {
                    // We intersect with an existing sprite, so we cannot place the image here.
                    // Advance the x coordinate to the end of the sprite.
                    xCoord = intersect.X + intersect.Width;
                    hasSpace = false;
                }

                if (hasSpace)
                {
                    if (xCoord + 1 < limitX && yCoord + 1 < limitY)
                    {
                        return true;
                    }
                }
            }
        }

        return xCoord < limitX && yCoord < limitY;
    }
}

/// <summary>
/// Class which represents a sheet of images, such as a texture atlas.
/// </summary>
/// <param name="width">The width of sheets in collection.</param>
/// <param name="height">The height of sheets in collection.</param>
public class SheetCollection(uint width, uint height)
{
    private static int _sheetCounter = 0;

    /// <summary>
    /// List of sheets in the collection.
    /// </summary>
    public List<Sheet> Sheets { get; init; } = new();

    /// <summary>
    /// Creates a new empty sheet and adds it to the collection.
    /// </summary>
    /// <returns>The <see cref="Sheet"/>.</returns>
    public Sheet Create()
    {
        Sheet sheet = new(this, $"Sheet_{_sheetCounter}", width, height);
        Sheets.Add(sheet);
        _sheetCounter++;
        return sheet;
    }

    /// <summary>
    /// Gets the sprite with the given image name from any sheet in the collection.
    /// </summary>
    /// <param name="imageName">The name of image.</param>
    /// <returns>
    /// The <see cref="SheetSprite"/> with the given image name.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    /// If no sprite with the given image name is found.
    /// </exception>
    public SheetSprite GetSprite(string imageName)
    {
        foreach (Sheet sheet in Sheets)
        {
            foreach (SheetSprite sprite in sheet.Sprites)
            {
                if (sprite.Image.Name == imageName)
                {
                    return sprite;
                }
            }
        }
        throw new KeyNotFoundException($"Sprite with name '{imageName}' not found in any sheet.");
    }

    /// <summary>
    /// Creates a new image sheets from the given images.
    /// Each image is added to the sheet, if there is space.
    /// If there is no space, a new sheet is created.
    /// </summary>
    /// <param name="images">
    /// Input images to add to the sheet.
    /// </param>
    /// <param name="width">The width of the sheet in pixels. Default is <c>2048</c>.</param>
    /// <param name="height">The height of the sheet in pixels. Default is <c>2048</c>.</param>
    /// <returns>
    /// The created image sheets.
    /// </returns>
    public static SheetCollection Create(List<ParserImage> images, uint width = 2048, uint height = 2048)
    {
        SheetCollection sheetCollection = new(width, height);
        Sheet sheet = sheetCollection.Create();
        foreach (ParserImage image in images)
        {
            if (!sheet.TryAddSprite(image))
            {
                // No space in the current sheet, create a new one.
                sheet = sheetCollection.Create();

                if (!sheet.TryAddSprite(image))
                {
                    // This should never happen, as the image is smaller than the sheet.
                    throw new InvalidOperationException("Image is too large to fit in the sheet.");
                }
            }
        }

        return sheetCollection;
    }
}
