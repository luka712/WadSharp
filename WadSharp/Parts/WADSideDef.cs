namespace WadSharp.Parts;


/// <summary>
/// A sidedef contains the wall texture data for each linedef (though sidedefs do not reference 
/// linedefs directly, indeed it is the other way around).
/// <para/>
/// Each sidedef contains texture data, offsets for the textures and the number of the sector it 
/// references (this is how sectors get their 'shape').
/// <para/>
/// For more information, see <see href="https://doomwiki.org/wiki/SIDEDEFS">Doom Wiki</see>.
/// </summary>
public class WADSideDef
{
    /// <summary>
    /// How many pixels to shift all the sidedef textures on the X axis (right or left). 
    /// </summary>
    public short XOffset { get; set; }

    /// <summary>
    /// How many pixels to shift all the sidedef textures on the Y axis (up or down). 
    /// </summary>
    public short YOffset { get; set; }

    /// <summary>
    /// Name of the upper texture.
    /// </summary>
    public string? UpperTextureName { get; set; }

    /// <summary>
    /// Name of the lower texture.
    /// </summary>
    public string? LowerTextureName { get; set; }

    /// <summary>
    /// Name of the middle texture.
    /// </summary>
    public string? MiddleTextureName { get; set; }

    /// <summary>
    /// Sector index.
    /// </summary>
    public ushort SectorId { get; set; }
}
