namespace WadSharp.Parts;

/// <summary>
/// The texture of the WAD file.
/// For more <see cref="https://doomwiki.org/wiki/TEXTURE1_and_TEXTURE2">TEXTURE1 and TEXTURE2</see>.
/// </summary>
public class WADTexture
{
    /// <summary>
    /// Name of the map texture.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///  A short integer defining the total width of the map texture. 
    /// </summary>
    public short Width { get; set; }

    /// <summary>
    /// A short integer defining the total height of the map texture.
    /// </summary>
    public short Height { get; set; }

    /// <summary>
    /// The number of map patches that make up this map texture. 
    /// </summary>
    public short PatchCount { get; set; }

    /// <summary>
    /// Array with the map patch structures for this texture. 
    /// </summary>
    public List<WADPatch> Patches { get; set; } = new();
}

/// <summary>
/// The <b>TEXTURE1</b> and <b>TEXTURE2</b> lumps define how wall patches from the WAD file should combine to form wall textures.
/// They do not contain the graphics themselves, merely their definition.
/// <para/>
/// For more <see cref="https://doomwiki.org/wiki/TEXTURE1_and_TEXTURE2">TEXTURE1 and TEXTURE2</see>.
/// </summary>
public class WADTexture1
{
    /// <summary>
    ///  An integer holding the number of map textures. 
    /// </summary>
    public uint NumTextures { get; set; }

    /// <summary>
    /// A list with the map texture structures.
    /// </summary>
    public List<WADTexture> Textures { get; set; } = new();
}

/// <summary>
/// The <b>TEXTURE1</b> and <b>TEXTURE2</b> lumps define how wall patches from the WAD file should combine to form wall textures.
/// They do not contain the graphics themselves, merely their definition.
/// <para/>
/// For more <see cref="https://doomwiki.org/wiki/TEXTURE1_and_TEXTURE2">TEXTURE1 and TEXTURE2</see>.
/// </summary>
public class WADTexture2 : WADTexture1
{

}