namespace WadSharp.Parts;

/// <summary>
/// The color in Doom palette.
/// </summary>
public struct WADColor
{
    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    /// <param name="a">The alpha component. By default, it is <c>255</c>.</param>
    public WADColor(byte r, byte g, byte b, byte a = 0)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    /// <summary>
    /// The red component of the color.
    /// </summary>
    public byte R { get; set; }

    /// <summary>
    /// The green component of the color.
    /// </summary>
    public byte G { get; set; }

    /// <summary>
    /// The blue component of the color.
    /// </summary>
    public byte B { get; set; }

    /// <summary>
    /// The alpha component of the color.
    /// </summary>
    public byte A { get; set; }
}
