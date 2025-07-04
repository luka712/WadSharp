namespace WadSharp.Parts;

/// <summary>
/// NODES is the name of a WAD lump which is a component of a level.
/// It is normally generated automatically from other data for the level using a node builder tool.
/// <para/>
/// The nodes lump constitutes a binary space partition of the level.
/// It is a binary tree that sorts all the subsectors into the correct order for drawing.
/// Each node entry has a partition line associated with it that divides the area that the node represents into a left child area and a right child area.
/// Each child may be either another node entry (a subnode), or a subsector on the map.
/// <para/>
/// For more information, see <see href="https://doomwiki.org/wiki/NODES">Doom Wiki</see>.
/// </summary>
public class WADNode
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
    public ushort RightChildId { get; set; }

    /// <summary>
    /// The left child.
    /// </summary>
    public ushort LeftChildId { get; set; }
}
