/*
*
*   The physics body class does... a lot.  It handles the computation of internal values such as 
*       the min and max values for X and Y (used by the Sweep and Prune algorithm, as well as 
*       collision detection in general).  It registers and processes the colliders that belong to 
*       an object.  It handles the application of forces and torque as well as drag and angular drag.
*       It lets an object add colliders, and then exposes those colliders for narrow phase collision 
*       detection.  It handles some naive default collision responses such as a simple reflection
*       or 'stop on collision'.
*       
*   Important to note though that while this is called a PhysicsBody, no claims are made for the 
*       *accuracy* of the physics.  If you are planning to do anything that requires the physics
*       calculations to be remotely correct, you're going to have to extend the engine so it does 
*       that.  All I'm interested in here is showing you how it's *architected*. 
*       
*   This is also the subsystem which I am least confident about people relying on, because it is 
*       virtually untestable in any meaningful sense.  I spent three days trying to track down a 
*       bug that mean that an object would pass through another one at a rate of approximately
*       once every half hour...
*       
*   @author Michael Heron
*   @author Samuel Falck (added some stuff for 3d collision detection and did some refactoring)
*   @version 1.0 (SAX)
*   
*   Several substantial contributions to the code made by others:
*   @author Mårten Åsberg (see Changelog for 1.0.1)
*   
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace Shard;

internal class PhysicsBody
{
    private readonly List<Collider> myColliders;
    private float torque;
    private Vector2 force;
    
    public float[] MinAndMaxX { get; private set; }
    public float[] MinAndMaxY { get; private set; }
    public float[] MinAndMaxZ { get; private set; }
    public Color DebugColor { get; set; }
    public float AngularDrag { get; set; }
    public float Drag { get; set; }
    public GameObject Parent { get; }
    public Transform Trans { get; }
    public float Mass { get; set; }
    public float MaxForce { get; set; }
    public float MaxTorque { get; set; }
    public bool Kinematic { get; set; }
    public bool PassThrough { get; set; }
    public bool UsesGravity { get; set; }
    public bool StopOnCollision { get; set; }
    public bool ReflectOnCollision { get; set; }
    public bool ImpartForce { get; set; }
    public CollisionHandler Colh { get; }
    
    public PhysicsBody(GameObject p)
    {
        DebugColor = Color.Green;
        myColliders = new List<Collider>();
        Parent = p;
        Trans = p.Transform;
        Colh = (CollisionHandler)p;
        AngularDrag = 0.01f;
        Drag = 0.01f;
        Drag = 0.01f;
        Mass = 1;
        MaxForce = 10;
        MaxTorque = 2;
        UsesGravity = false;
        StopOnCollision = true;
        ReflectOnCollision = false;
        MinAndMaxX = new float[2];
        MinAndMaxY = new float[2];
        MinAndMaxZ = new float[2];
        // Debug.getInstance().log ("Setting physics enabled");
        PhysicsManager.getInstance().addPhysicsObject(this);
    }
    
    public List<Collider> getColliders()
    {
        return myColliders;
    }

    public void applyGravity(float modifier, Vector2 dir)
    {
        Vector2 gf = dir * modifier;
        addForce(gf);
    }
    
    private void addForce(Vector2 dir)
    {
        if (Kinematic) { return; }
        
        dir /= Mass;
        
        // Set a lower bound.
        if (dir.LengthSquared() < 0.0001) { return; }
        
        force += dir;
        
        // Set a higher bound.
        if (force.Length() > MaxForce)
        {
            force = Vector2.Normalize(force) * MaxForce;
        }
    }
    
    public void drawMe()
    {
        foreach (Collider col in myColliders)
        {
            col.drawMe(DebugColor);
        }
    }

    public float[] getMinAndMax(string axis)
    {
        float min = int.MaxValue;
        float max = -1 * min;
        
        foreach (Collider col in myColliders)
        {
            float[] tmp;
            switch (axis)
            {
                case "x":
                    tmp = col.MinAndMaxX;
                    break;
                case "y":
                    tmp = col.MinAndMaxY;
                    break;
                case "z":
                    tmp = col.MinAndMaxZ;
                    break;
                default:
                    throw new Exception("Axis " + axis + " is not supported");
            }
            if (tmp[0] < min)
            {
                min = tmp[0];
            }
            if (tmp[1] > max)
            {
                max = tmp[1];
            }
        }
        return [min, max];
    }

    public void addTorque(float dir)
    {
        if (Kinematic) return;
        
        torque += dir / Mass;
        if (torque > MaxTorque)
        {
            torque = MaxTorque;
        }
        if (torque < -1 * MaxTorque)
        {
            torque = -1 * MaxTorque;
        }
    }

    public void impartForces(PhysicsBody other, float massProp)
    {
        other.addForce(force * massProp);
        recalculateColliders();
    }

    public void stopForces()
    {
        force = Vector2.Zero;
    }

    public void reflectForces(Vector2 impulse)
    {
        Vector2 reflect = new Vector2(0, 0);
        Debug.Log ("Reflecting " + impulse);

        // We're being pushed to the right or left, so we must have collided with the right or left.
        if (impulse.X is > 0 or < 0)
        {
            reflect.X = -1;
        }

        // We're being pushed upwards or downwards, so we must have collided with the top or bottom.
        if (impulse.Y is < 0 or > 0)
        {
            reflect.Y = -1;
        }
        
        force *= reflect;
        Debug.Log("Reflect is " + reflect);
    }

    public void reduceForces(float prop) {
        force *= prop;
    }

    public void addForce(Vector2 dir, float force) {
        addForce(dir * force);
    }

    public void recalculateColliders()
    {
        foreach (Collider col in getColliders())
        {
            col.recalculate();
        }
        MinAndMaxX = getMinAndMax("x");
        MinAndMaxY = getMinAndMax("y");
        MinAndMaxZ = getMinAndMax("z");
    }

    public void physicsTick()
    {
        float rot = torque;

        if (Math.Abs(torque) < AngularDrag)
        {
            torque = 0;
        }
        else
        {
            torque -= Math.Sign(torque) * AngularDrag;
        }
        
        Trans.rotate(rot);
		Trans.translate(this.force);
        
        float force = this.force.Length();
        if (force < Drag)
        {
            stopForces();
        }
        else if (force > 0)
        {
            this.force = (this.force / force) * (force - Drag);
        }
    }
    
    private void addCollider(Collider col)
    {
        myColliders.Add(col);
    }

    public ColliderRect addRectCollider()
    {
        ColliderRect collider = new ColliderRect(Parent.Transform);
        addCollider(collider);
        return collider;
    }

    public ColliderRect addRectCollider(int x, int y, int wid, int ht)
    {
        ColliderRect collider = new ColliderRect(Parent.Transform, x, y, wid, ht);
        addCollider(collider);
        return collider;
    }
    
    public ColliderCircle addCircleCollider()
    {
        ColliderCircle collider = new ColliderCircle(Parent.Transform);
        addCollider(collider);
        return collider;
    }

    public ColliderCircle addCircleCollider(int x, int y, int rad)
    {
        ColliderCircle collider = new ColliderCircle(Parent.Transform, x, y, rad);
        addCollider(collider);
        return collider;
    }
    
    public void addCuboidCollider()
    {
        ColliderCuboid collider = new ColliderCuboid(Parent.Transform);
        addCollider(collider);
    }
    
    public void addSphereCollider()
    {
        ColliderSphere collider = new ColliderSphere(Parent.Transform);
        addCollider(collider);
    }

    public Vector2? checkCollisions(Vector2 other)
    {
        foreach (Collider c in myColliders)
        {
            Vector2? d = c.checkCollision(other);
            if (d != null) return d;
        }
        return null;
    }
}