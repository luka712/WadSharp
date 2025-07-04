namespace WadSharp.Parts;

/// <summary>
/// The GL_SEGS lump defines all the GL segs, which form the boundaries of each GL subsector.
/// </summary>
public class WADGLSeg
{
    /// <summary>
    /// The version of the GL seg.
    /// </summary>
    public WADGLVersion Version { get; internal set; } = WADGLVersion.gNd1;

    /// <summary>
    /// The mini seg flag. If the line ID is equal to this value, it is a mini seg.
    /// </summary>
    public const ushort MINI_SEG_FLAG = 0xFFFF;

    /// <summary>
    /// Is the start vertex id a GL vertex?
    /// </summary>
    public bool IsStartVertexIdGLVertex => CheckIfVertexIsGLVertex(StartVertexId);

    /// <summary>
    /// Is the end vertex id a GL vertex?
    /// </summary>
    public bool IsEndVertexIdGLVertex => CheckIfVertexIsGLVertex(EndVertexId);

    /// <summary>
    /// The start and end vertex values define precisely where the seg lies.
    /// Bit 15 plays a special role: when 0, the vertex is a normal one (from VERTEXES), when 1 the vertex is a GL vertex (from GL_VERT lump). 
    /// </summary>
    public int StartVertexId { get; set; }

    /// <summary>
    /// Gets the start vertex ID without the GL vertex flag.
    /// </summary>
    public int GLStartVertexId => GetGLVertexId(StartVertexId);

    /// <summary>
    /// Ending vertex number.
    /// </summary>
    public int EndVertexId { get; set; }

    /// <summary>
    /// Gets the end vertex ID without the GL vertex flag.
    /// </summary>
    public int GLEndVertexId => GetGLVertexId(EndVertexId);

    /// <summary>
    /// The linedef number is the linedef that the seg lies along, or 0xFFFF if the seg does not lie along any linedef.
    /// Those segs are called "minisegs", since they are never drawn by the engine,
    /// they only serve to mark the boundary of the subsector (for creating plane polygons). 
    /// </summary>
    public ushort LineDefId { get; set; }

    /// <summary>
    /// The side number is 0 if the seg lies along the RIGHT (FRONT) side of the linedef, or 1 if the seg lies along the LEFT (BACK) side.
    /// This is the same as the 'direction' field in the normal SEGS lump. Ignored for "minisegs". 
    /// </summary>
    public ushort Side { get; set; }

    /// <summary>
    /// Checks if the seg lies along the right(front) side of the linedef.
    /// Same as <see cref="IsFrontSide"/> since right and front are often used interchangeably.
    /// Same as <c>Side == 0</c>.
    /// </summary>
    public bool IsRightSide => Side == 0;

    /// <summary>
    /// Checks if the seg lies along the front(right) side of the linedef.
    /// Same as <see cref="IsRightSide"/> since right and front are often used interchangeably.
    /// Same as <c>Side == 0</c>.
    /// </summary>
    public bool IsFrontSide => IsRightSide;

    /// <summary>
    /// For segs that lie along a one-sided linedef, this field is simply 0xFFFF.
    /// Otherwise, this field contains the index for the GL seg in the adjacent subsector which borders on the current seg.
    /// There is guaranteed to be a one-to-one correspondence between the segs that lie on either side of the border between any two subsectors.
    /// A corollary is that a "seg" start vertex is the same as its partner's end vertex, and vice versa. 
    /// </summary>
    public uint PartnerSegId { get; set; }

    /// <summary>
    /// Is mini seg id.
    /// Mini segs are never drawn by the engine, they only serve to mark the boundary of
    /// the subsector(for creating plane polygons).
    /// </summary>
    public bool IsMiniSeg => (LineDefId & MINI_SEG_FLAG) == MINI_SEG_FLAG;

    /// <summary>
    /// Checks if the vertex ID is a GL vertex.
    /// </summary>
    /// <param name="vertexId">The vertex to check.</param>
    /// <returns>True if GLVertex, false otherwise.</returns>
    private bool CheckIfVertexIsGLVertex(int vertexId)
    {
        // The 15th bit is set to 1 for GL vertices. This is version "gNd1" and "gNd2".
        int flag = 1 << 15;

        if (Version is WADGLVersion.gNd3 or WADGLVersion.gNd4)
        {
            // The 30th bit is set to 1 for GL vertices.
            flag = 1 << 30;
        }
        else if (Version == WADGLVersion.gNd5)
        {
            // The 31st bit is set to 1 for GL vertices.
            flag = 1 << 31;
        }

        return (vertexId & flag) == flag;
    }

    /// <summary>
    /// Gets the GL vertex ID without the GL vertex flag.
    /// </summary>
    /// <param name="vertexId">The id for which to flip flag off.</param>
    /// <returns>The id without the flag.</returns>
    private int GetGLVertexId(int vertexId)
    {
        if (Version is WADGLVersion.gNd3 or WADGLVersion.gNd4)
        {
            // The 30th bit is set to 1 for GL vertices.
            return vertexId & ~(1 << 30);
        }
        else if (Version == WADGLVersion.gNd5)
        {
            // The 31st bit is set to 1 for GL vertices.
            return vertexId & ~(1 << 31);
        }

        return vertexId & ~(1 << 15);
    }
}
