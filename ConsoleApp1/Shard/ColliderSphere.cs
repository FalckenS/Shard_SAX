using System;
using System.Drawing;

namespace Shard;

internal class ColliderSphere : Collider
{
    private readonly Transform3D transform;

    internal ColliderSphere(Transform3D transform)
    {
        this.transform = transform;
        recalculate();
    }

    internal override void recalculate()
    {
        MinAndMaxX[0] = transform.X - transform.Radius;
        MinAndMaxX[1] = transform.X + transform.Radius;
        MinAndMaxY[0] = transform.Y - transform.Radius;
        MinAndMaxY[1] = transform.Y + transform.Radius;
        MinAndMaxZ[0] = transform.Z - transform.Radius;
        MinAndMaxZ[1] = transform.Z + transform.Radius;
    }
    
    internal override bool checkCollision(ColliderBox other)
    {
        // Find the point in/on the box that is the closest to the sphere center
        float x = Math.Max(other.MinAndMaxX[0], Math.Min(transform.X, other.MinAndMaxX[1]));
        float y = Math.Max(other.MinAndMaxY[0], Math.Min(transform.Y, other.MinAndMaxY[1]));
        float z = Math.Max(other.MinAndMaxZ[0], Math.Min(transform.Z, other.MinAndMaxZ[1]));
        
        // Calculate the distance between the sphere center and the closest point (Euclidean distance)
        float distanceSquared = (x - transform.X) * (x - transform.X) +
                                (y - transform.Y) * (y - transform.Y) +
                                (z - transform.Z) * (z - transform.Z);
        return distanceSquared < transform.Radius * transform.Radius;
    }

    internal override bool checkCollision(ColliderSphere other)
    {
        // Calculate Euclidean distance between the sphere centers
        float distanceSquared = (other.transform.X - transform.X) * (other.transform.X - transform.X) +
                                (other.transform.Y - transform.Y) * (other.transform.Y - transform.Y) +
                                (other.transform.Z - transform.Z) * (other.transform.Z - transform.Z);
        return distanceSquared < transform.Radius * transform.Radius;
    }

    internal override void drawMe(Color col)
    {
        throw new NotImplementedException();
    }
}