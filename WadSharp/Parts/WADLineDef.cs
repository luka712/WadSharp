namespace WadSharp.Parts;

[Flags]
public enum WADLineDefFlag : ushort
{
    /// <summary>
    /// Blocks players and monsters from crossing the line (Doom).
    /// </summary>
    Blocking = 0x0001,

    /// <summary>
    /// Blocks monsters only from crossing the line (Doom).
    /// </summary>
    BlockMonsters = 0x0002,

    /// <summary>
    /// Line has two sides (e.g., a window or transparent wall) (Doom).
    /// </summary>
    TwoSided = 0x0004,

    /// <summary>
    /// Upper texture is un-pegged (anchors from the ceiling) (Doom).
    /// </summary>
    DontPegTop = 0x0008,

    /// <summary>
    /// Lower texture is un-pegged (anchors from the floor) (Doom).
    /// </summary>
    DontPegBottom = 0x0010,

    /// <summary>
    /// Appears as one-sided on the auto map despite being two-sided (Doom).
    /// </summary>
    Secret = 0x0020,

    /// <summary>
    /// Blocks sound propagation across the line (Doom).
    /// </summary>
    SoundBlock = 0x0040,

    /// <summary>
    /// Never appears on the auto map, even if discovered (Doom).
    /// </summary>
    DontDraw = 0x0080,

    /// <summary>
    /// Always appears on the auto map from the start (Doom).
    /// </summary>
    Mapped = 0x0100,

    /// <summary>
    /// Line is a railing (Strife) OR passes use action (Boom) OR repeatable special (ZDoom).
    /// </summary>
    RailingPassUseRepeatSpecial = 0x0200,

    /// <summary>
    /// Blocks floating monsters (Strife) OR mid-texture is 3D walkable (Eternity) OR part of SPAC activation flags (ZDoom).
    /// </summary>
    BlockFloaters3DMidTexSpac = 0x0400, // Bit 10 conflict

    /// <summary>
    /// Part of SPAC activation flags: monster crosses (ZDoom).
    /// </summary>
    SpacmCross = 0x0800,

    /// <summary>
    /// Part of SPAC activation flags: hit by projectile (ZDoom) OR 25% translucent (Strife).
    /// </summary>
    SpacImpactTranslucent = 0x1000, // Bit 12 conflict

    /// <summary>
    /// 75% translucent (Strife) OR monsters can activate (ZDoom).
    /// </summary>
    TransparentMonstersCanActivate = 0x2000, // Bit 13 conflict

    /// <summary>
    /// Blocks players only (ZDoom).
    /// </summary>
    BlockPlayers = 0x4000, // Bit 14

    /// <summary>
    /// Blocks everything, including gunshots and missiles (ZDoom).
    /// </summary>
    BlockEverything = 0x8000 // Bit 15
}

/// <summary>
/// The WAD line definition.
/// </summary>
public class WADLineDef
{
    /// <summary>
    /// Starting Vertex: Starting (X,Y) coordinate.
    /// </summary>
    public ushort StartVertexId { get; set; }

    /// <summary>
    /// Ending Vertex: Ending (X,Y) coordinate.
    /// </summary>
    public ushort EndVertexId { get; set; }

    /// <summary>
    /// Flags: Attribute bits.
    /// </summary>
    public ushort Flags { get; set; }

    /// <summary>
    /// Linedef type: Special action or behaviour.
    /// </summary>
    public ushort SpecialType { get; set; }

    /// <summary>
    /// Tag: Associates sector(s)/line(s) with the special.
    /// </summary>
    public ushort SectorTag { get; set; }

    /// <summary>
    /// Front Side Def.
    /// </summary>
    public ushort FrontSideDef { get; set; }

    /// <summary>
    /// Back Side Def.
    /// </summary>
    public ushort BackSideDef { get; set; }

    /// <summary>
    /// Checks if the line definition has the specified flag.
    /// </summary>
    /// <param name="flag">The <see cref="WADLineDefFlag"/>.</param>
    /// <returns><c>true</c> if flag is present, <c>false</c> otherwise.</returns>
    public bool HasFlag(WADLineDefFlag flag)
    {
        return (Flags & (ushort)flag) == (ushort)flag;
    }
}
