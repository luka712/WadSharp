using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WadSharp.Parts;

/// <summary>
/// SSECTORS (an abbreviation for subsectors) is the name of a WAD lump which is a component of a level.
/// It is normally generated automatically from other data for the level using a node builder tool.
/// <para/>
/// A subsector is a range of seg (linedef segment) numbers.
/// These segs form part (or all) of a single sector.
/// Each subsector is constructed so that when a player is anywhere within a particular subsector,
/// no part of any one seg will block the view of any other in that subsector.
/// <para/>
/// A subsector can be considered to be a convex polygon, but some of its edges may be implicit.
/// This is because segs are generated only where linedefs already exist. (It is not uncommon for a subsector to consist of a single seg.)
/// The Doom engine uses the partition lines in the nodes structure to determine which subsector any particular point lies in.
/// <para/>
/// Subsectors are referenced from the nodes lump.
/// <para/>
/// For more information, see <see href="https://doomwiki.org/wiki/SSECTORS">Doom Wiki</see>.
/// </summary>
public class WADSubSector
{
    /// <summary>
    /// Seg count.
    /// </summary>
    public short SegCount { get; set; }

    /// <summary>
    /// First seg number.
    /// </summary>
    public short FirstSegId { get; set; }
}
