namespace WadSharp.Parts;

/// <summary>
/// The lump or directory associates names of lumps with the data that belong to them. 
/// It consists of a number of entries, each with a length of 16 bytes. 
/// </summary>
public class WADLump
{
    /// <summary>
    /// An integer holding a pointer to the start of the lump's data in the file. 
    /// </summary>
    public uint FilePos { get; set; }

    /// <summary>
    /// An integer representing the size of the lump in bytes. 
    /// </summary>
    public uint Size { get; set; }

    /// <summary>
    /// An ASCII string defining the lump's name. 
    /// The name has a limit of 8 characters, the same as the main portion of an MS-DOS filename. 
    /// The name must be nul-terminated if less than 8 characters;
    /// for maximum tool compatibility, it is best to pad any remainder with nul characters as well. 
    /// </summary>
    public char[] Name { get; set; } = null!;

    /// <summary>
    /// <see cref="WADThing"/>.
    /// </summary>
    public List<WADThing> Things { get; set; } = new();

    /// <summary>
    /// <see cref="WADVertex"/>.
    /// </summary>
    public List<WADVertex> Vertices { get; set; } = new();

    /// <summary>
    /// <see cref="WADLineDef"/>.
    /// </summary>
    public List<WADLineDef> LineDefs { get; set; } = new();

    /// <summary>
    /// <see cref="WADSideDef"/>.
    /// </summary>
    public List<WADSideDef> SideDefs { get; set; } = new();

    /// <summary>
    /// <see cref="WADNode"/>.
    /// </summary>
    public List<WADNode> Nodes { get; set; } = new();

    /// <summary>
    /// <see cref="WADSubSector"/>.
    /// </summary>
    public List<WADSubSector> SubSectors { get; set; } = new();

    /// <summary>
    /// <see cref="WADSeg"/>.
    /// </summary>
    public List<WADSeg> Segs { get; set; } = new();

    /// <summary>
    /// <see cref="WADSector"/>.
    /// </summary>
    public List<WADSector> Sectors { get; set; } = new();

    /// <summary>
    /// <see cref="WADPlayPal"/>.
    /// </summary>
    public WADPlayPal? PlayPal { get; set; }

    /// <summary>
    /// <see cref="WADFlat"/>.
    /// </summary>
    public WADFlat? Flat { get; set; }

    /// <summary>
    /// <see cref="WADPNames"/>.
    /// </summary>
    public WADPNames? PNames { get; set; }

    /// <summary>
    /// <see cref="WADGLVertex"/>.
    /// </summary>
    public List<WADGLVertex> GLVertices { get; set; } = new();

    /// <summary>
    /// The version of the GL_VERT lump.
    /// </summary>
    public WADGLVersion GLVerticesVersion { get; set; } = WADGLVersion.gNd1;

    /// <summary>
    /// The version of the GL_SEGS lump.
    /// </summary>
    public string GLSegsVersion { get; set; } = String.Empty;

    /// <summary>
    /// <see cref="WADGLSeg"/>.
    /// </summary>
    public List<WADGLSeg> GLSegs { get; set; } = new();

    /// <summary>
    /// <see cref="WadGLNode"/>.
    /// </summary>
    public List<WADGLNode> GLNodes { get; set; } = new();

    /// <summary>
    /// <see cref="WADGLSubSector"/>.
    /// </summary>
    public List<WADGLSubSector> GLSubSectors { get; set; } = new();

    /// <summary>
    /// <see cref="Texture1"/>.
    /// </summary>
    public WADTexture1 Texture1 { get; set; }

    /// <summary>
    /// <see cref="Texture2"/>.
    /// </summary>
    public WADTexture2 Texture2 { get; set; }

    /// <summary>
    /// <see cref="WadPictureFormat"/>.
    /// </summary>
    public WADPictureFormat PictureFormat { get; set; }
}
