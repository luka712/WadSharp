using WadSharp.Parts;

namespace WadSharp.Parsing;


/// <summary>
/// Useful for parsing WAD files into common data structures.
/// Mainly used to make it easier to work with WAD files.
/// </summary>
public class Parser
{
    /// <summary>
    /// Parse the <see cref="WADPictureFormat"/> list into a list of <see cref="ParserImage"/>
    /// </summary>
    /// <param name="graphics">The patches.</param>
    /// <param name="playpal">The palette.</param>
    /// <returns>The parsed images.</returns>
    public List<ParserImage> ParsePatches(IEnumerable<WADPictureFormat> graphics, WADPlayPal playpal)
    {
        List<ParserImage> images = new();
        foreach (WADPictureFormat patch in graphics)
        {
            images.Add(ParserImage.Parse(patch, playpal));
        }
        return images;
    }

    /// <summary>
    /// Parse the <see cref="WADFlat"/> list into a list of <see cref="ParserImage"/>"
    /// </summary>
    /// <param name="flats">The flats.</param>
    /// <param name="playpal">The palette.</param>
    /// <returns>The parsed images.</returns>
    public List<ParserImage> ParseFlats(IEnumerable<WADFlat> flats, WADPlayPal playpal)
    {
        List<ParserImage> images = new List<ParserImage>();
        foreach (WADFlat flat in flats)
        {
            images.Add(ParserImage.Parse(flat, playpal));
        }
        return images;
    }

    public List<ParserImage> ParseTextures(
        List<WADTexture1> textures,
        WADPNames patchesNames,
        List<WADPictureFormat> pictureFormatData,
        WADPlayPal playpal)
    {
        List<ParserImage> images = new();
        foreach (WADTexture1 texture in textures)
        {
            foreach (WADTexture tex in texture.Textures)
            {
                images.Add(ParserImage.Parse(tex, patchesNames, pictureFormatData, playpal));
                images.Add(ParserImage.Parse(tex, patchesNames, pictureFormatData, playpal));
            }
        }

        return images;
    }

    public List<ParserImage> ParseTextures(
        List<WADTexture2> textures,
        WADPNames patchesNames,
        List<WADPictureFormat> pictureFormatData,
        WADPlayPal playpal)
    {
        List<ParserImage> images = new();
        foreach (WADTexture2 texture in textures)
        {
            foreach (WADTexture tex in texture.Textures)
            {
                images.Add(ParserImage.Parse(tex, patchesNames, pictureFormatData, playpal));
            }
        }

        return images;
    }

    public List<ParserSector> ParseSectors(WADLevel level)
    {
        List<ParserImage> images = ParserImage.Parse(level);

        List<ParserSector> sectors = ParserSector.Parse(
            level.Sectors,
            level.GLSubSectors,
            level.SideDefs,
            level.LineDefs,
            level.GLSegs,
            level.GLVertices,
            level.Vertices,
            images);

        return sectors;
    }
}
