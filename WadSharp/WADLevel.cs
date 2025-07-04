using WadSharp.Parts;

namespace WadSharp;

/// <summary>
/// The level of the WAD file.
/// </summary>
public class WADLevel
{
    /// <summary>
    /// The name of the level.
    /// </summary>
    public string Name { get; set; } = String.Empty;

    /// <summary>
    /// <see cref="WADThing"/>.
    /// </summary>
    public List<WADThing> Things { get; set; } = new();

    /// <summary>
    /// <see cref="WADLineDef"/>.
    /// </summary>
    public List<WADLineDef> LineDefs { get; set; } = new();

    /// <summary>
    /// <see cref="WADSideDef"/>.
    /// </summary>
    public List<WADSideDef> SideDefs { get; set; } = new();

    /// <summary>
    /// <see cref="WADVertex"/>.
    /// </summary>
    public List<WADVertex> Vertices { get; set; } = new();

    /// <summary>
    /// <see cref="WADSeg"/>.
    /// </summary>
    public List<WADSeg> Segs { get; set; } = new();

    /// <summary>
    /// <see cref="WADNode"/>.
    /// </summary>
    public List<WADNode> Nodes { get; set; } = new();

    /// <summary>
    /// <see cref="WADSubSector"/>.
    /// </summary>
    public List<WADSubSector> SubSectors { get; set; } = new();

    /// <summary>
    /// <see cref="WADSector"/>.
    /// </summary>
    public List<WADSector> Sectors { get; set; } = new();

    /// <summary>
    /// <see cref="WADPlaypal"/>.
    /// </summary>
    public List<WADPlayPal> PlayPals { get; set; } = new();

    /// <summary>
    /// <see cref="WADFlat"/>.
    /// </summary>
    public List<WADFlat> Flats { get; set; } = new();

    /// <summary>
    /// <see cref="WADPatch"/>.
    /// </summary>
    public List<WADPictureFormat> Patches { get; set; } = new();

    /// <summary>
    /// <see cref="WADTexture1"/>.
    /// </summary>
    public List<WADTexture1> Texture1s { get; set; } = new();

    /// <summary>
    /// <see cref="WADTexture2"/>.
    /// </summary>
    public List<WADTexture2> Texture2s { get; set; } = new();

    /// <summary>
    /// The <see cref="WADPNames"/> for the textures.
    /// </summary>
    public WADPNames PNames { get; set; } = new();

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
    /// <see cref="WADGLSubSector"/>.
    /// </summary>
    public List<WADGLSubSector> GLSubSectors { get; set; } = new();

    /// <summary>
    /// <see cref="WADGLNode"/>.
    /// </summary>
    public List<WADGLNode> GLNodes { get; set; } = new();
}
