namespace WadSharp.Parts;

/// <summary>
/// The GL_NODES lump contains the information for the GL BSP tree.
/// An important property which the normal NODES lump lacks is that the bounding boxes 
/// are guaranteed to cover the whole subsector, not just the segs that lie along linedefs (as happens with NODES). 
/// </summary>
public class WADGLNode
{
    /// <summary>
    /// X coordinate of partition line start.
    /// </summary>
    public short XPartition { get; set; }

    /// <summary>
    /// Y coordinate of partition line start.
    /// </summary>
    public short YPartition { get; set; }

    /// <summary>
    /// Change in x from start to end of partition line.
    /// </summary>
    public short DxPartition { get; set; }

    /// <summary>
    /// Change in y from start to end of partition line.
    /// </summary>
    public short DyPartition { get; set; }

    /// <summary>
    /// The bounding box of the right child.
    /// </summary>
    public WADBoundingBox RightBoundingBox { get; set; } = new();

    /// <summary>
    /// The bounding box of the left child.
    /// </summary>
    public WADBoundingBox LeftBoundingBox { get; set; } = new();

    /// <summary>
    /// The right child.
    /// </summary>
    public uint RightChildId { get; set; }

    /// <summary>
    /// The left child.
    /// </summary>
    public uint LeftChildId { get; set; }
}
