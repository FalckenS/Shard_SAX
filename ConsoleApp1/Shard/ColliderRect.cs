/*
*
*   The specific collider for rectangles.   Handles rect/circle, rect/rect and rect/vector.
*   @author Michael Heron
*   @author Samuel Falck (did some refactoring)
*   @version 1.0 (SAX)
*   
*/

using System;
using System.Drawing;
using System.Numerics;

namespace Shard;

internal class ColliderRect : Collider
{
    private readonly bool fromTrans;
    private readonly Transform myRect;
    private float x;
    private float y;
    private float wid;
    private float ht;
    private readonly float baseWid;
    private readonly float baseHt;
    
    public float Left { get => MinAndMaxX[0]; set => MinAndMaxX[0] = value; }
    public float Right { get => MinAndMaxX[1]; set => MinAndMaxX[1] = value; }
    public float Top { get => MinAndMaxY[0]; set => MinAndMaxY[0] = value; }
    public float Bottom { get => MinAndMaxY[1]; set => MinAndMaxY[1] = value; }
    
    public ColliderRect(Transform t)
    {
        myRect = t;
        fromTrans = true;
        RotateAtOffset = false;
        calculateBoundingBox();
    }

    public ColliderRect(Transform t, float x, float y, float wid, float ht)
    {
        this.x = x;
        this.y = y;
        baseWid = wid;
        baseHt = ht;
        RotateAtOffset = true;
        myRect = t;
        fromTrans = false;
    }

    private void calculateBoundingBox()
    {
        if (myRect == null) return;
        if (fromTrans)
        {
            wid = myRect.Wid * myRect.Scalex;
            ht = myRect.Ht * myRect.Scaley;
        }
        else
        {
            wid = baseWid * myRect.Scalex;
            ht = baseHt * myRect.Scaley;
        }

        float angle = (float)(Math.PI * myRect.Rotz / 180.0f);
        double cos = Math.Cos(angle);
        double sin = Math.Sin(angle);

        // Bit of trig here to calculate the new height and width
        float nwid = (float)(Math.Abs(wid * cos) + Math.Abs(ht * sin));
        float nht = (float)(Math.Abs(wid * sin) + Math.Abs(ht * cos));

        x = myRect.X + wid / 2;
        y = myRect.Y + ht / 2;
        wid = nwid;
        ht = nht;

        if (RotateAtOffset)
        {
            // Now we work out the X and Y based on the rotation of the body to which this belongs.
            float x1 = x - myRect.Centre.X;
            float y1 = y - myRect.Centre.Y;

            float x2 = (float)(x1 * Math.Cos(angle) - y1 * Math.Sin(angle));
            float y2 = (float)(x1 * Math.Sin(angle) + y1 * Math.Cos(angle));

            x = x2 + myRect.Centre.X;
            y = y2 + myRect.Centre.Y;
        }
        MinAndMaxX[0] = x - wid / 2;
        MinAndMaxX[1] = x + wid / 2;
        MinAndMaxY[0] = y - ht / 2;
        MinAndMaxY[1] = y + ht / 2;
    }

    public override void recalculate()
    {
        calculateBoundingBox();
    }

    private ColliderRect calculateMinkowskiDifference(ColliderRect other)
    {
        ColliderRect mink = new ColliderRect(null);

        // A set of calculations that gives us the Minkowski difference for this intersection.
        float left = Left - other.Right;
        float top = other.Top - Bottom;
        float width = wid + other.wid;
        float height = ht + other.ht;
        float right = Right - other.Left;
        float bottom = other.Bottom - Top;

        mink.wid = width;
        mink.ht = height;
        mink.MinAndMaxX = [left, right];
        mink.MinAndMaxY = [top, bottom];
        
        return mink;
    }

    private Vector2? calculatePenetration(Vector2 checkPoint)
    {
        const float coff = 0.2f;

        // Check the right edge
        float min = Math.Abs(Right - checkPoint.X);
        Vector2? impulse = new Vector2(-1 * min - coff, checkPoint.Y);
        
        // Now compare against the Left edge
        if (Math.Abs(checkPoint.X - Left) <= min)
        {
            min = Math.Abs(checkPoint.X - Left);
            impulse = new Vector2(min + coff, checkPoint.Y);
        }

        // Now the bottom
        if (Math.Abs(Bottom - checkPoint.Y) <= min)
        {
            min = Math.Abs(Bottom - checkPoint.Y);
            impulse = new Vector2(checkPoint.X, min + coff);
        }

        // And now the top
        if (Math.Abs(Top - checkPoint.Y) <= min)
        {
            min = Math.Abs(Top - checkPoint.Y);
            impulse = new Vector2(checkPoint.X, -1 * min - coff);
        }
        return impulse;
    }

    public override Vector2? checkCollision(ColliderRect other)
    {
        ColliderRect cr = calculateMinkowskiDifference(other);
        if (cr.Left <= 0 && cr.Right >= 0 && cr.Top <= 0 && cr.Bottom >= 0)
        {
            return cr.calculatePenetration(new Vector2(0, 0));
        }
        return null;
    }

    public override void drawMe(Color col)
    {
        Display d = Bootstrap.getDisplay();

        d.drawLine((int)MinAndMaxX[0], (int)MinAndMaxY[0], (int)MinAndMaxX[1], (int)MinAndMaxY[0], col);
        d.drawLine((int)MinAndMaxX[0], (int)MinAndMaxY[0], (int)MinAndMaxX[0], (int)MinAndMaxY[1], col);
        d.drawLine((int)MinAndMaxX[1], (int)MinAndMaxY[0], (int)MinAndMaxX[1], (int)MinAndMaxY[1], col);
        d.drawLine((int)MinAndMaxX[0], (int)MinAndMaxY[1], (int)MinAndMaxX[1], (int)MinAndMaxY[1], col);

        d.drawCircle((int)x, (int)y, 2, col);
    }

    public override Vector2? checkCollision(ColliderCircle c)
    {
        Vector2? possibleV = c.checkCollision(this);

        if (possibleV is Vector2 v)
        {
            v.X *= -1;
            v.Y *= -1;
            return v;
        }
        return null;
    }

    public override Vector2? checkCollision(Vector2 other)
    {
        if (other.X >= Left &&
            other.X <= Right &&
            other.Y >= Top &&
            other.Y <= Bottom)
        {
            return new Vector2(0, 0);
        }
        return null;
    }
}