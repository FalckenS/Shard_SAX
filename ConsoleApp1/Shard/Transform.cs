/*
*
*   The transform class handles position, independent of physics and forces (although the physics
*       system will make use of the rotation and translation functions here).  Essentially this class
*       is a game object's location (X, Y), rotation and scale.  Usefully it also calculates the 
*       centre of an object as well as relative directions such as forwards and right.  If you want 
*       backwards and left, multiply forward or right by -1.
*       
*   @author Michael Heron
*   @version 1.0
*   
*/

using System;
using System.Numerics;

namespace Shard;

internal class Transform
{
    private Vector2 forward2d, right2d, centre2d;
    
    internal GameObject Owner { get; set; }
    internal float X { get; set; }
    internal float Y { get; set; }
    internal float Rotz { get; set; }
    internal string SpritePath { get; set; }
    internal float Width { get; set; }
    internal float Height { get; set; }
    internal ref Vector2 Forward2d => ref forward2d;
    internal ref Vector2 Right2d => ref right2d;
    internal ref Vector2 Centre2d => ref centre2d;
    internal float ScaleX { get; set; }
    internal float ScaleY { get; set; }
    internal float Lx { get; set; }
    internal float Ly { get; set; }

    internal Transform(GameObject ow)
    {
        Owner = ow;
        forward2d = new Vector2();
        right2d = new Vector2();
        centre2d = new Vector2();

        ScaleX = 1.0f;
        ScaleY = 1.0f;

        X = 0;
        Y = 0;

        Lx = 0;
        Ly = 0;

        rotate(0);
    }

    internal Vector2 getLastDirection()
    {
        float dx = X - Lx;
        float dy = Y - Ly;
        return new Vector2(-dx, -dy);
    }
    
    internal void recalculateCentre()
    {
        centre2d.X = X + Width * ScaleX / 2;
        centre2d.Y = Y + Height * ScaleY / 2;
    }

    internal void translate(double nx, double ny)
    {
        translate ((float)nx, (float)ny);
    }

    internal void translate(float nx, float ny)
    {
        Lx = X;
        Ly = Y;

        X += nx;
        Y += ny;
        
        recalculateCentre();
    }

    internal void translate(Vector2 vect)
    {
        translate(vect.X, vect.Y);
    }

    internal void rotate(float dir)
    {
        Rotz += dir;
        Rotz %= 360;

        float angle = (float)(Math.PI * Rotz / 180.0f);
        float sin = (float)Math.Sin(angle);
        float cos = (float)Math.Cos(angle);

        forward2d.X = cos;
        forward2d.Y = sin;
        
        right2d.X = -1 * forward2d.Y;
        right2d.Y = forward2d.X;
    }
}