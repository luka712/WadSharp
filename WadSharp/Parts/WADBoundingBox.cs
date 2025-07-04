namespace WadSharp.Parts;

/// <summary>
/// A bounding box consists of four short values (top, bottom, left and right) giving the
/// upper and lower bounds of the y coordinate and the lower and upper bounds of the x coordinate (in that order). 
/// </summary>
public struct WADBoundingBox
{
    /// <summary>
    /// The top of bounding box.
    /// </summary>
    public short Top { get; set; }

    /// <summary>
    /// The bottom of bounding box.
    /// </summary>
    public short Bottom { get; set; }

    /// <summary>
    /// The left of bounding box.
    /// </summary>
    public short Left { get; set; }

    /// <summary>
    /// The right of bounding box.
    /// </summary>
    public short Right { get; set; }
}
