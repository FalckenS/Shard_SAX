using System;
using System.Drawing;

namespace Shard;

internal class ColliderCuboid : Collider
{
    private readonly Transform3D transform;
    private float width, height, depth;
    
    internal ColliderCuboid(Transform3D transform)
    {
        this.transform = transform;
        recalculate();
    }

    internal override void recalculate()
    {
        if (transform == null) return;
        
        width = transform.Width * transform.ScaleX;
        height = transform.Height * transform.ScaleY;
        depth = transform.Depth * transform.ScaleZ;

        MinAndMaxX[0] = transform.X - width  / 2;
        MinAndMaxX[1] = transform.X + width  / 2;
        MinAndMaxY[0] = transform.Y - height / 2;
        MinAndMaxY[1] = transform.Y + height / 2;
        MinAndMaxZ[0] = transform.Z - depth  / 2;
        MinAndMaxZ[1] = transform.Z + depth  / 2;
    }
    
    internal override bool checkCollision(ColliderCuboid other)
    {
        // Check for separation on X axis
        if (MinAndMaxX[1] < other.MinAndMaxX[0] || other.MinAndMaxX[1] < MinAndMaxX[0])
            return false;
    
        // Check for separation on Y axis
        if (MinAndMaxY[1] < other.MinAndMaxY[0] || other.MinAndMaxY[1] < MinAndMaxY[0])
            return false;
    
        // Check for separation on Z axis
        if (MinAndMaxZ[1] < other.MinAndMaxZ[0] || other.MinAndMaxZ[1] < MinAndMaxZ[0])
            return false;
    
        // If no separation, there is a collision
        return true;
    }

    internal override bool checkCollision(ColliderSphere other)
    {
        // To avoid code duplication, just call the checkCollision on the sphere
        return other.checkCollision(this);
    }

    internal override void drawMe(Color col)
    {
        throw new NotImplementedException();
    }
}