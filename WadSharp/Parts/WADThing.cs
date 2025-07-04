namespace WadSharp.Parts;

/// <summary>
/// Things represent players, monsters, pick-ups, and projectiles.
/// Inside the game, these are known as actors, or mobjs.
/// They also represent obstacles, certain decorations, player start positions and teleport landing sites.
/// <para/>
/// While some mobjs, such as projectiles and special effects, can only be created during play,
/// most things can be placed in a map from a map editor through an associated editor number.
/// When the map is loaded, an actor that corresponds to that number will be spawned at the location of that map thing.
/// See thing types for a listing of all things that have an associated editor number.
/// <para/>
/// For more information, see <see href="https://doomwiki.org/wiki/Thing">Doom Wiki</see>.
/// </summary>
public class WADThing
{
    /// <summary>
    /// X position.
    /// </summary>
    public short X { get; set; }

    /// <summary>
    /// Y position.
    /// </summary>
    public short Y { get; set; }

    /// <summary>
    /// Angle: The angle that the thing is facing.
    /// It is stored in degrees. At 0 degrees, the thing is facing east (right).
    /// </summary>
    public short Angle { get; set; }

    /// <summary>
    /// Type: The type of thing.
    /// See <a href="https://doomwiki.org/wiki/Thing_types">Doom Wiki</a> for a list of thing types.
    /// For example to find the player start position, you can use type 1 (Player 1 Start), that is <c>Type == 1</c>.
    /// Note that thing types lookup is stored in <c>Doom Engine</c>.
    /// </summary>
    public ushort Type { get; set; }

    /// <summary>
    /// Flags: The flags of the thing.
    /// </summary>
    public ushort Flags { get; set; }
}
