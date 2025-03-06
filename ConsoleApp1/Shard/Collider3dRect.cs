using System;
using System.Drawing;
using System.Numerics;

namespace Shard;

internal class Collider3dRect : Collider
{
    private readonly Transform3D transform;
    private float xCenterPos, yCenterPos, zCenterPos;
    private float width, height, depth;
    
    public Collider3dRect(Transform3D transform)
    {
        this.transform = transform;
        recalculate();
    }

    public override void recalculate()
    {
        if (transform == null)
        {
            return;
        }
        
        width = transform.Wid * transform.Scalex;
        height = transform.Ht * transform.Scaley;
        depth = transform.Depth * transform.ScaleZ;

        xCenterPos = transform.X + (width / 2);
        yCenterPos = transform.Y + (height / 2);
        zCenterPos = transform.Z + (depth / 2);

        MinAndMaxX[0] = xCenterPos - width  / 2;
        MinAndMaxX[1] = xCenterPos + width  / 2;
        MinAndMaxY[0] = yCenterPos - height / 2;
        MinAndMaxY[1] = yCenterPos + height / 2;
        MinAndMaxZ[0] = zCenterPos - depth  / 2;
        MinAndMaxZ[1] = zCenterPos + depth  / 2;
    }

    public override Vector2? checkCollision(Vector2 c)
    {
        throw new Exception("Check collision with Vector2 not supported with 3d collider");
    }
    
    public override Vector2? checkCollision(ColliderRect c)
    {
        throw new Exception("Check collision with ColliderRect not supported with 3d collider");
    }

    public override Vector2? checkCollision(ColliderCircle circle)
    {
        throw new Exception("Check collision with ColliderCircle not supported with 3d collider");
    }
    
    public override bool checkCollision(Collider3dRect other)
    {
        return MinAndMaxX[0] < other.MinAndMaxX[1] && other.MinAndMaxX[0] < MinAndMaxX[1] &&
               MinAndMaxY[0] < other.MinAndMaxY[1] && other.MinAndMaxY[0] < MinAndMaxY[1] &&
               MinAndMaxZ[0] < other.MinAndMaxZ[1] && other.MinAndMaxZ[0] < MinAndMaxZ[1];
    }

    public override void drawMe(Color col)
    {
        throw new NotImplementedException();
    }
}