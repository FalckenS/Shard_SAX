/*
*
*   @author Michael Heron
*   @author Samuel Falck
*   @version 2.0
*   
*/

namespace Shard;

internal class Transform3D(GameObject o) : Transform(o)
{
    public float Z { get; set; } = 0f;
    public float Depth { get; set; } = 1f;
    public float ScaleZ { get; set; } = 1f;
}