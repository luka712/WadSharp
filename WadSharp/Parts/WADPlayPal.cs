namespace WadSharp.Parts
{
    /// <summary>
    /// The PLAYPAL lump is a set of color palettes the Doom engine uses to set the main colors for all graphics,
    /// including full color fades and tinting effects.
    /// For more <see cref="https://doomwiki.org/wiki/PLAYPAL">PLAYPAL</see>.
    /// </summary>
    public class WADPlayPal
    {
        /// <summary>
        /// List of colors for palettes.
        /// This contains multiple palettes and their colors.
        /// Usually Doom has 14 palettes, but some WADs may have more or less.
        /// </summary>
        public List<WADColor> PalettesColors { get; set; } = new();
    }
}
