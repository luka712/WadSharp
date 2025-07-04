namespace WadSharp.Parts;


/// <summary>
/// SEGS (an abbreviation for segments) is the name of a WAD lump which is a component of a level. 
/// It is normally generated automatically from other data for the level using a node builder tool.
/// <para/>
/// Segs are segments of linedefs, and describe the portion of a linedef that borders the subsector that the seg belongs to.
/// <para/>
/// Initially, one seg is created for each one-sided linedef, and two segs are created for each two-sided linedef (one for each side). 
/// If the area that a seg borders is later divided into two different subsectors, 
/// then a new vertex is created at the division point and the seg is split into two at that vertex.
/// The segs are stored in sequential order corresponding to subsector number. 
/// The seg entries are referenced from the subsector entries, which are referenced from the nodes lump.
/// <para/>
/// For more information, see <see href="https://doomwiki.org/wiki/Seg">Doom Wiki</see>.
/// </summary>
public class WADSeg
{
    /// <summary>
    /// Starting vertex number.
    /// </summary>
    public short StartVertexId { get; set; }

    /// <summary>
    /// Ending vertex number.
    /// </summary>
    public short EndVertexId { get; set; }

    /// <summary>
    /// Angle, full circle is -32768 to 32767. 
    /// </summary>
    public short Angle { get; set; }

    /// <summary>
    /// <see cref="WADLineDef"/> number.
    /// </summary>
    public short LineDefId { get; set; }

    /// <summary>
    /// Direction: 0 (same as <see cref="WADLineDef"/>) or 1 (opposite of <see cref="WADLineDef"/>).
    /// </summary>
    public short Direction { get; set; }

    /// <summary>
    /// Offset: distance along <see cref="WADLineDef"/> to start of seg.
    /// </summary>
    public short Offset { get; set; }
}
