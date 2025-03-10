using System;
using System.Drawing;
using System.Numerics;

namespace Shard;

internal class ColliderBox : Collider
{
    private readonly Transform3D transform;
    private float width, height, depth;
    
    internal ColliderBox(Transform3D transform)
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
    
    internal override bool checkCollision(Vector3 lineOrigin, Vector3 lineDirection)
    {
        // line(t) = lineOrigin + t×lineDirection
        // t is distance along the arline
        // t > 0 means in front lineOrigin
        
        // Uses slab method
        // A slab is the range of values the box occupies along a particular axis (for x the slab is from MinAndMaxX[0]
        // to MinAndMaxX[1]).
        // tmin and tmax are points along the line where intersections with the slab occur. This is the interval for
        // each axis where the line is "inside" the box along that axis.
        
        float txmin = (MinAndMaxX[0] - lineOrigin.X) / lineDirection.X;
        float txmax = (MinAndMaxX[1] - lineOrigin.X) / lineDirection.X;
        if (txmax < txmin)
        {
            (txmin, txmax) = (txmax, txmin);
        }

        float tymin = (MinAndMaxY[0] - lineOrigin.Y) / lineDirection.Y;
        float tymax = (MinAndMaxY[1] - lineOrigin.Y) / lineDirection.Y;
        if (tymax < tymin)
        {
            (tymin, tymax) = (tymax, tymin);
        }

        // If the intervals on both axes don't overlap, the line can't be intersecting the box (no collision)
        if (txmin > tymax || tymin > txmax) return false;

        // At this point, we know there is an overlap by t on the X and Y interval
        
        float tmin = Math.Max(txmin, tymin);
        float tmax = Math.Min(txmax, tymax);

        float tzmin = (MinAndMaxZ[0] - lineOrigin.Z) / lineDirection.Z;
        float tzmax = (MinAndMaxZ[1] - lineOrigin.Z) / lineDirection.Z;
        if (tzmax < tzmin)
        {
            (tzmin, tzmax) = (tzmax, tzmin);
        }

        // If the intervals don't overlap, the line can't be intersecting the box (no collision)
        if (tmin > tzmax || tzmin > tmax) return false;

        // Ensures the intersection is not completely behind the line
        return tmax >= 0;
    }
    
    internal override bool checkCollision(ColliderBox other)
    {
        // Check for separation on X axis
        if (MinAndMaxX[1] < other.MinAndMaxX[0] || other.MinAndMaxX[1] < MinAndMaxX[0]) return false;
    
        // Check for separation on Y axis
        if (MinAndMaxY[1] < other.MinAndMaxY[0] || other.MinAndMaxY[1] < MinAndMaxY[0]) return false;
    
        // Check for separation on Z axis
        if (MinAndMaxZ[1] < other.MinAndMaxZ[0] || other.MinAndMaxZ[1] < MinAndMaxZ[0]) return false;
    
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