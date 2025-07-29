using WadSharp.Parts;

namespace WadSharp;

/// <summary>
/// The representation of Doom WAD file.
/// </summary>
public class WAD
{
    private const int THINGS_INDEX = 1;
    private const int LINE_DEFS_INDEX = 2;
    private const int SIDE_DEFS_INDEX = 3;
    private const int VERTEXES_INDEX = 4;
    private const int SEGS_INDEX = 5;
    private const int SUB_SECTORS_INDEX = 6;
    private const int NODES_INDEX = 7;
    private const int SECTORS_INDEX = 8;
    private const int GL_VERTICES_INDEX = 12;
    private const int GL_SEGS_INDEX = 13;
    private const int GL_SUB_SECTORS_INDEX = 14;
    private const int GL_NODES_INDEX = 15;

    /// <summary>
    /// The ASCII characters "IWAD" or "PWAD". 
    /// </summary>
    public char[] Identification { get; set; } = null!;

    /// <summary>
    /// An integer specifying the number of lumps in the WAD. 
    /// </summary>
    public uint NumLumps { get; set; }

    /// <summary>
    /// An integer holding a pointer to the location of the directory. 
    /// </summary>
    public uint InfoTableOffset { get; set; }

    /// <summary>
    /// The directory associates names of lumps with the data that belong to them. 
    /// It consists of a number of entries, each with a length of 16 bytes.
    /// The length of the directory is determined by the number given in the WAD header.
    /// </summary>
    public List<WADLump> Directories { get; set; } = new();

    public WADLevel LoadLevel(string directoryName)
    {
        directoryName = directoryName.ToUpperInvariant();
        int index = Directories.FindIndex(x => new string(x.Name).Replace("\0", "") == directoryName);

        if (index < 0)
        {
            throw new ArgumentException($"The directory {directoryName} does not exist.");
        }

        // Select all the flats. Does not select for single level.
        List<WADFlat> flats = Directories
            .Where(x => x.Flat != null)
            .Select(x => x.Flat!)
            .ToList();

        List<WADPlayPal> playPals = Directories
            .Where(x => x.PlayPal != null)
            .Select(x => x.PlayPal!)
            .ToList();

        List<WADTexture1> textures1 = Directories
            .Where(x => x.Texture1 != null)
            .Select(x => x.Texture1)
            .ToList();

        List<WADTexture2> textures2 = Directories
            .Where(x => x.Texture2 != null)
            .Select(x => x.Texture2)
            .ToList();

        List<WADPictureFormat> patches = Directories
            .Where(x => x.PictureFormat != null)
            .Select(x => x.PictureFormat)
            .ToList();

        WADPNames? pnames = Directories.FirstOrDefault(x => x.PNames != null)?.PNames;

        List<WADGLVertex> glVertices = Directories.Count() > index + GL_VERTICES_INDEX ? Directories[index + GL_VERTICES_INDEX].GLVertices : new();
        List<WADGLSeg> glSegs = Directories.Count > index + GL_SEGS_INDEX ? Directories[index + GL_SEGS_INDEX].GLSegs : new();
        List<WADGLSubSector> glSubSectors = Directories.Count > index + GL_SUB_SECTORS_INDEX ? Directories[index + GL_SUB_SECTORS_INDEX].GLSubSectors : new();
        List<WADGLNode> glNodes = Directories.Count > index + GL_NODES_INDEX ? Directories[index + GL_NODES_INDEX].GLNodes : new();

        return new WADLevel
        {
            Name = directoryName,
            Things = Directories[index + THINGS_INDEX].Things,
            LineDefs = Directories[index + LINE_DEFS_INDEX].LineDefs,
            SideDefs = Directories[index + SIDE_DEFS_INDEX].SideDefs,
            Vertices = Directories[index + VERTEXES_INDEX].Vertices,
            Segs = Directories[index + SEGS_INDEX].Segs,
            SubSectors = Directories.Count > index + SUB_SECTORS_INDEX ? Directories[index + SUB_SECTORS_INDEX].SubSectors : new(),
            Nodes = Directories.Count > index + NODES_INDEX ? Directories[index + NODES_INDEX].Nodes : new(),
            Sectors = Directories.Count > index + SECTORS_INDEX ? Directories[index + SECTORS_INDEX].Sectors : new(),
            Patches = patches,
            Texture1s = textures1,
            Texture2s = textures2,
            PNames = pnames ?? new(),
            GLVertices = glVertices,
            GLSegs = glSegs,
            GLSubSectors = glSubSectors,
            GLNodes = glNodes,
            GLVerticesVersion = Directories.First().GLVerticesVersion,
            Flats = flats,
            PlayPals = playPals
        };
    }
}
