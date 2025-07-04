namespace WadSharp.Parts;

/// <summary>
/// Vertices are nothing more than coordinates on the map. 
/// Linedefs and segments(segs) reference vertices for their start-point and end-point.
/// The vertices for a map are stored in the VERTEXES lump,
/// which consists of a raw sequence of x, y coordinates as pairs of 16-bit signed integer
/// </summary>
public struct WADVertex
{
    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="x">X position.</param>
    /// <param name="y">Y position.</param>
    public WADVertex(short x, short y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// X position.
    /// </summary>
    public short X { get; set; }

    /// <summary>
    /// Y position.
    /// </summary>
    public short Y { get; set; }
}
