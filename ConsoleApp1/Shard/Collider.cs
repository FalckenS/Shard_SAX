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

using System.Drawing;
using System.Numerics;

namespace Shard;

internal abstract class Collider
{
    public float[] MinAndMaxX { get; protected set; } = new float[2];
    public float[] MinAndMaxY { get; protected set; } = new float[2];
    public float[] MinAndMaxZ { get; protected set; } = new float[2];
    public bool RotateAtOffset { get; init; }
    
    public Vector2? checkCollision(Collider c)
    {
        switch (c)
        {
            case ColliderRect rect:
                return checkCollision(rect);
            case ColliderCircle circle:
                return checkCollision(circle);
            case Collider3dRect rect3d:
                if (checkCollision(rect3d))
                {
                    return new Vector2(0, 0);
                }
                return null;
            default:
                Debug.getInstance().log("Bug");
                // Not sure how we got here but c'est la vie
                return null;
        }
    }
    
    public abstract void recalculate();

    public abstract Vector2? checkCollision(Vector2 c);
    
    public abstract Vector2? checkCollision(ColliderRect c);

    public abstract Vector2? checkCollision(ColliderCircle c);

    public virtual bool checkCollision(Collider3dRect collider3dRect)
    {
        throw new System.NotImplementedException();
    }
    
    // TODO sphere

    public abstract void drawMe(Color col);
}