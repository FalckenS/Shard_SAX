/*
*
*   The collider for circles. Handles circle/circle, circle/rect, and circle/point collisions.
*   @author Michael Heron
*   @author Samuel Falck (did some refactoring)
*   @version 1.0 (SAX)
*   
*/

using System;
using System.Drawing;
using System.Numerics;

namespace Shard;

internal class ColliderCircle : Collider
{
    private readonly bool fromTrans;
    private readonly Transform myRect;
    private readonly float xOff;
    private readonly float yOff;
    
    public float X { get; set; }
    public float Y { get; set; }
    public float Rad { get; set; }
    public float Left { get => MinAndMaxX[0]; set => MinAndMaxX[0] = value; }
    public float Right { get => MinAndMaxX[1]; set => MinAndMaxX[1] = value; }
    public float Top { get => MinAndMaxY[0]; set => MinAndMaxY[0] = value; }
    public float Bottom { get => MinAndMaxY[1]; set => MinAndMaxY[1] = value; }

    public ColliderCircle(Transform t)
    {
        myRect = t;
        fromTrans = true;
        RotateAtOffset = false;
        calculateBoundingBox();
    }

    public ColliderCircle(Transform t, float x, float y, float rad)
    {
        xOff = x;
        yOff = y;
        X = xOff;
        Y = yOff;
        Rad = rad;
        RotateAtOffset = true;
        myRect = t;
        fromTrans = false;
        calculateBoundingBox();
    }

    private void calculateBoundingBox()
    {
        float angle = (float)(Math.PI * myRect.Rotz / 180.0f);

        if (fromTrans)
        {
            float intWid = myRect.Wid * myRect.Scalex;
            Rad = intWid / 2;
            X = myRect.X + xOff + Rad;
            Y = myRect.Y + yOff + Rad;
        }
        else
        {
            X = myRect.X + xOff;
            Y = myRect.Y + yOff;
        }

        if (RotateAtOffset)
        {
            // Now we work out the X and Y based on the rotation of the body to which this belongs.
            float x1 = X - myRect.Centre.X;
            float y1 = Y - myRect.Centre.Y;

            float x2 = (float)(x1 * Math.Cos(angle) - y1 * Math.Sin(angle));
            float y2 = (float)(x1 * Math.Sin(angle) + y1 * Math.Cos(angle));

            X = x2 + myRect.Centre.X;
            Y = y2 + myRect.Centre.Y;
        }
        MinAndMaxX[0] = X - Rad;
        MinAndMaxX[1] = X + Rad;
        MinAndMaxY[0] = Y - Rad;
        MinAndMaxY[1] = Y + Rad;
    }

    public override void recalculate()
    {
        calculateBoundingBox();
    }
    
    public override Vector2? checkCollision(Vector2 c)
    {
        if (c.X >= Left &&
            c.X <= Right &&
            c.Y >= Top &&
            c.Y <= Bottom)
        {
            return new Vector2(0, 0);
        }
        return null;
    }

    public override Vector2? checkCollision(ColliderRect other)
    {
        double tx = X;
        double ty = Y;

        if (X < other.Left)
        {
            tx = other.Left;
        }
        else if (X > other.Right)
        {
            tx = other.Right;
        }
        
        if (Y < other.Top)
        {
            ty = other.Top;
        }
        else if (Y > other.Bottom)
        {
            ty = other.Bottom;
        }
        
        // PYTHAGORAS YO
        double dx = X - tx;
        double dy = Y - ty;

        double dist = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

        // if the distance is less than the radius, collision!
        if (dist < Rad)
        {
            double depth = Rad - dist;
            Vector2 dir;
            if (dist == 0)
            {
                // Here we hit the exact edge, oh no. This will cause the vector calculations to break. You can't
                // normalize a 0,0 vector - it's mathematically incoherent.   
                //
                // So what we need to do is get the direction the circle was moving, reverse it, and then push it out
                // that way.  We have to do it that way otherwise we *might* push it through a collider. We have to
                // assume if the last position it was in was fine after the physics took effect, then it is hopefully
                // fine for us to push it there.
                
                dir = myRect.getLastDirection();
                dir = Vector2.Normalize(dir);
            }
            else
            {
                dir = new Vector2((float)dx, (float)dy);
                dir = Vector2.Normalize(dir);
                dir *= (float)depth;
            }
            return dir;
        }
        return null;
    }

    public override Vector2? checkCollision(ColliderCircle c)
    {
        double xpen = Math.Pow(c.X - this.X, 2);
        double ypen = Math.Pow(c.Y - this.Y, 2);
        double radsq = Math.Pow(c.Rad + this.Rad, 2);
        double dist = xpen + ypen;
        double depth = (c.Rad + Rad) - Math.Sqrt(dist);

        if (dist <= radsq)
        {
            var dir = new Vector2(X - c.X, Y - c.Y);
            dir = Vector2.Normalize(dir);
            dir *= (float)depth;
            return dir;
        }
        return null;
    }
    
    public override void drawMe(Color col)
    {
        Bootstrap.getDisplay().drawCircle((int)X, (int)Y, (int)Rad, col);
    }
}