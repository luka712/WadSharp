
namespace WadSharp.Parts;

/// <summary>
/// The patch of the WAD file.
/// For more <see cref="https://doomwiki.org/wiki/TEXTURE1_and_TEXTURE2">TEXTURE1 and TEXTURE2</see>.
/// </summary>
public class WADPatch
{
    /// <summary>
    /// A short int defining the horizontal offset of the patch relative to the upper-left of the texture. 
    /// </summary>
    public short OriginX { get; set; }

    /// <summary>
    /// A short int defining the vertical offset of the patch relative to the upper-left of the texture. 
    /// </summary>
    public short OriginY { get; set; }

    /// <summary>
    /// A short int defining the patch number (as listed in PNAMES) to draw.
    /// Lookup in the <see cref="WadPNames"/> for the name of the patch.
    /// </summary>
    public short PatchIndex { get; set; }

    /// <summary>
    /// A short int possibly intended to define if the patch was to be drawn normally or mirrored. Unused. 
    /// </summary>
    public short StepDir { get; set; }

    /// <summary>
    /// A short int possibly intended to define a special colormap to draw the patch with, like a brightmap. Unused. 
    /// </summary>
    public short ColorMap { get; set; }
}
