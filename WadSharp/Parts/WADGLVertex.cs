namespace WadSharp.Parts;

/// <summary>
/// The GL_VERT defines the geometry for OpenGL rendering, including additional vertices created during the BSP process(e.g., segment splits).
/// </summary>
public struct WADGLVertex
{
    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="x">X position.</param>
    /// <param name="y">Y position.</param>
    public WADGLVertex(float x, float y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// X position.
    /// </summary>
    public float X { get; set; }

    /// <summary>
    /// Y position.
    /// </summary>
    public float Y { get; set; }
}
