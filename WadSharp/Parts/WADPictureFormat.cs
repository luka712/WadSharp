namespace WadSharp.Parts;

/// <summary>
/// Also known as PatchFormat, is format for most of the graphics in the WAD file, except for flats (floor and ceiling textures).
/// For more <see cref="https://doomwiki.org/wiki/Picture_format">picture format</see>.
/// </summary>
public class WADPictureFormat
{
    /// <summary>
    /// Name is not in doom format, but it's added here to make it more convenient to use.
    /// Name of Patch or name as listed in <see cref="WadPNames"/>.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The width of graphic.
    /// </summary>
    public ushort Width { get; set; }

    /// <summary>
    /// The height of graphic.
    /// </summary>
    public ushort Height { get; set; }

    /// <summary>
    /// Offset in pixels to the left of the origin.
    /// </summary>
    public short LeftOffset { get; set; }

    /// <summary>
    /// Offset in pixels below the origin.
    /// </summary>
    public short TopOffset { get; set; }

    /// <summary>
    /// List of columns. Each column is a list of posts (vertical pixel spans).
    /// </summary>
    public List<List<WADPictureFormatPost>> Columns { get; set; } = new();
}

/// <summary>
/// The POST lump is a WAD lump that contains the information for the Doom engine to render the graphics.
/// For more <see cref="https://doomwiki.org/wiki/Picture_format">picture format</see>.
/// </summary>
public class WADPictureFormatPost
{
    /// <summary>
    ///  The y offset of this post in this patch. If 0xFF, then end-of-column (not a valid post).
    /// </summary>
    public byte TopDelta { get; set; }

    /// <summary>
    /// Length of data in this post.
    /// </summary>
    public byte Length { get; set; }

    /// <summary>
    ///  Array of pixels in this post; each data pixel is an index into the Doom palette.
    /// </summary>
    public List<byte> Data { get; set; } = new();
}