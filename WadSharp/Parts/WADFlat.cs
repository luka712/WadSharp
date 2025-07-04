namespace WadSharp.Parts;

/// <summary>
/// A flat is an image that is drawn on the floors and ceilings of sectors.
/// It is usually a 64x64 pixel image, but can be any size.
/// It contains a list of indices into the Doom palette colors.
/// For more information, see the <see href="https://doomwiki.org/wiki/Flat">Doom Wiki</see>
/// or <see href="https://doom.fandom.com/wiki/Flat">Doom Fandom</see>.
/// </summary>
public class WADFlat
{
    /// <summary>
    /// The name of the flat. The name is 8 characters long and is padded with null characters.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The indices into palette colors.
    /// </summary>
    public List<int> IndicesIntoPaletteColors { get; set; } = new();

    /// <summary>
    /// The width of the flat.
    /// </summary>
    public uint Width { get; set; }

    /// <summary>
    /// The height of the flat.
    /// </summary>
    public uint Height { get; set; }
}
