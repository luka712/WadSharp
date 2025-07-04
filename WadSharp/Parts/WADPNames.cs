namespace WadSharp.Parts;

/// <summary>
/// PNAMES is a WAD lump that includes all the names for wall patches.
/// For more <see cref="https://doomwiki.org/wiki/PNAMES">PNAMES</see>.
/// </summary>
public class WADPNames
{
    /// <summary>
    ///  An integer holding the number of following patches. 
    /// </summary>
    public uint NumMapPatches { get; set; }

    /// <summary>
    /// The name of the patches.
    /// </summary>
    public List<string> PatchNames { get; set; } = new();
}
