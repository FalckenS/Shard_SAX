using System;
using System.Drawing;
using System.Numerics;

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

    internal override bool checkCollision(Vector3 rayOrigin, Vector3 rayDirection)
    {
        // ray(t) = rayOrigin + t×rayDirection
        // t is distance along the array
        // t > 0 means in front rayOrigin
        // A point is on the sphere if: ∥point − sphereCenter∥^2 = radius^2
        
        // ∥(rayOrigin + t×rayDirection) − sphereCenter∥^2 = radius^2
        // This gives us: at^2 + bt + c = 0
        // oc = rayOrigin − sphereCenter
        // a  = rayDirection ⋅ rayDirection
        // b  = 2× (oc ⋅ rayDirection)
        // c  = oc ⋅ oc − radius2
        // Discriminant: b^2 − 4ac
        // If < 0: No real roots --> the ray misses the sphere
        // If < 0: Real roots --> the ray intersects the sphere
        
        Vector3 oc = rayOrigin - new Vector3(transform.X, transform.Y, transform.Z);
        float a = Vector3.Dot(rayDirection, rayDirection);
        float b = 2.0f * Vector3.Dot(oc, rayDirection);
        float c = Vector3.Dot(oc, oc) - transform.Radius * transform.Radius;
    
        float discriminant = b * b - 4 * a * c;
        if (discriminant < 0) return false; // No real roots
    
        float sqrtDiscriminant = (float)Math.Sqrt(discriminant);
        // Roots (the points where the ray enters/leaves the sphere):
        float t0 = (-b - sqrtDiscriminant) / (2 * a);
        float t1 = (-b + sqrtDiscriminant) / (2 * a);
    
        // If both t0 and t1 < 0, the entire intersections occurs behind the rayOrigin
        return t0 >= 0 || t1 >= 0;
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
        return distanceSquared < transform.Radius * transform.Radius &&
               distanceSquared < other.transform.Radius * other.transform.Radius;
    }

    internal override void drawMe(Color col)
    {
        throw new NotImplementedException();
    }
}