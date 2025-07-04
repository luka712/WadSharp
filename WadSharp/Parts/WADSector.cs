namespace WadSharp.Parts;

/// <summary>
/// A sector is an area referenced by sidedefs on the linedefs, with variable height determined by its floor and ceiling values.
/// Sectors should be closed areas, meaning that all the sidedefs that reference a particular sector should make up a closed shape.
/// Unclosed sectors can be used for a few special effects, but under some circumstances they can lead to undesirable results (including crashes).
/// <para/>
/// For more information, see <see href="https://doomwiki.org/wiki/SECTORS">Doom Wiki</see>.
/// </summary>
public class WADSector
{
    /// <summary>
    /// Floor height.
    /// </summary>
    public short FloorHeight { get; set; }

    /// <summary>
    /// Ceiling height.
    /// </summary>
    public short CeilingHeight { get; set; }

    /// <summary>
    /// Name of the floor texture.
    /// </summary>
    public string? FloorTexture { get; set; }

    /// <summary>
    /// Name of the ceiling texture.
    /// </summary>
    public string? CeilingTexture { get; set; }

    /// <summary>
    /// The light level of the sector.
    /// </summary>
    public short LightLevel { get; set; }

    /// <summary>
    /// Sector type.
    /// </summary>
    public short SpecialType { get; set; }

    /// <summary>
    /// Tag number.
    /// </summary>
    public short Tag { get; set; }
}
