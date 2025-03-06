/*
*
*   The abstract collider class that is the base of all collisions. Specific variations should extend from this or one
*   of its children.
*
*   @author Michael Heron
*   @author Samuel Falck (added some stuff for 3d collision detection and did some refactoring)
*   @version 1.0 (SAX)
*   
*/

using System;
using System.Drawing;
using System.Numerics;

namespace Shard;

internal abstract class Collider
{
    internal float[] MinAndMaxX { get; set; } = new float[2];
    internal float[] MinAndMaxY { get; set; } = new float[2];
    internal float[] MinAndMaxZ { get; set; } = new float[2];
    internal bool RotateAtOffset { get; init; }
    
    internal Vector2? checkCollision(Collider c)
    {
        switch (c)
        {
            case ColliderRect rect:
                return checkCollision(rect);
            case ColliderCircle circle:
                return checkCollision(circle);
            case ColliderCuboid cuboid:
                // Dont care about impulse for 3d collisions
                if (checkCollision(cuboid)) return new Vector2(0, 0);
                return null;
            case ColliderSphere sphere:
                // Dont care about impulse for 3d collisions
                if (checkCollision(sphere)) return new Vector2(0, 0);
                return null;
            default:
                Debug.getInstance().log("Bug");
                // Not sure how we got here but c'est la vie
                return null;
        }
    }
    
    internal abstract void recalculate();

    internal virtual Vector2? checkCollision(Vector2 c)
    {
        throw new Exception("Check collision with Vector2 not supported with 2d collider");
    }

    internal virtual Vector2? checkCollision(ColliderRect c)
    {
        throw new Exception("Check collision with ColliderRect not supported with 2d collider");
    }

    internal virtual Vector2? checkCollision(ColliderCircle c)
    {
        throw new Exception("Check collision with ColliderCircle not supported with 2d collider");
    }

    internal virtual bool checkCollision(ColliderCuboid c)
    {
        throw new Exception("Check collision with ColliderCuboid not supported with 3d collider");
    }

    internal virtual bool checkCollision(ColliderSphere c)
    {
        throw new Exception("Check collision with ColliderSphere not supported with 3d collider");
    }

    internal abstract void drawMe(Color col);
}