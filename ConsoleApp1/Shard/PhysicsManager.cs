/*
*
*   The Shard Physics Manager.   
*   
*   As with the PhysicsBody class, upon which this class depends, I make no claims as to the 
*       accuracy of the physics.  My interest in this course is showing you how an engine is 
*       architected.  It's not a course on game physics.  The task of making this work in 
*       a way that simulates real world physics is well beyond the scope of the course. 
*       
*   This class is responsible for a lot.  It handles the broad phase collision 
*       detection (via Sweep and Prune).  It handles the narrow phase collisions, making use of the 
*       collider objects and the Minkowski differences they generate.  It does some collision resolutions 
*       that are linked to the mass of colliding bodies.  And it has the management routines that 
*       let all that happen.
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
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace Shard;

// An internal class used to hold a combination of two potentially colliding objects. 
internal class CollidingObject
{
    internal PhysicsBody A { get; set; }
    internal PhysicsBody B { get; set; }

    public override bool Equals(object other) {
        return other is CollidingObject otherObject && (
            A == otherObject.A && B == otherObject.B ||
            A == otherObject.B && B == otherObject.A);
    }

    public override int GetHashCode() {
        return A.GetHashCode() ^ B.GetHashCode();
    }

    public override String ToString()
    {
        return "[" + A.Parent.ToString() + " v " + B.Parent.ToString() + "]";
    }
}

/*
*   SAP is Search and Prune, a broad pass collision detection algorithm and the one used in the course. This is an
*   internal class used to contain a node in what will be a linked list used elsewhere in this code.
*/
public class SAPEntry
{
    private SAPEntry previous;
    
    internal float StartPos { get; set; }
    internal float EndPos { get; set; }
    internal PhysicsBody Owner { get; set; }
    internal SAPEntry Next { get; set; }
    
    public static void clearList(SAPEntry node)
    {
        // Let's clear everything so the garbage collector can do its work
        
        if (node == null) return;
        
        while (node != null && node.Next != null)
        {
            node = node.Next;
            node.previous.Next = null;
            node.previous = null;
        }
        node.previous = null;
    }

    public static SAPEntry addToList(SAPEntry node, SAPEntry entry)
    {
        SAPEntry current = node;

        // Start our list.
        if (current == null)
        {
            return entry;
        }
        
        // Is this our new head?
        if (entry.StartPos < current.StartPos)
        {
            current.previous = entry;
            entry.Next = current;
            return entry;
        }
        
        // Look for where we get inserted.
        while (current.Next != null && entry.StartPos > current.Next.StartPos)
        {
            current = current.Next;
        }
        
        if (current.Next != null)
        {
            // Insert ourselves into a chain.
            entry.previous = current;
            entry.Next = current.Next;
            current.Next = entry;
        }
        else
        {
            // We're at the end.
            current.Next = entry;
            entry.previous = current;
        }
        return node;
    }

    public static void outputList(SAPEntry node)
    {
        SAPEntry pointer = node;
        int counter = 0;
        string text = "";

        if (pointer == null)
        {
            Debug.getInstance().log("No List");
            return;
        }
        while (pointer != null)
        {
            text += "[" + counter + "]: " + pointer.Owner.Parent + ", ";
            pointer = pointer.Next;
            counter += 1;
        }
        Debug.getInstance().log("List:" + text);
    }
}

internal class PhysicsManager
{
    // 50 FPS = 20
    private const long TimeInterval = 20;
    
    private static PhysicsManager me;
    private readonly HashSet<CollidingObject> colliding;
    private readonly Vector2 gravityDir;
    private readonly List<PhysicsBody> allPhysicsObjects;
    private long lastUpdate;
    
    public float GravityModifier { get; set; }

    public static PhysicsManager getInstance()
    {
        return me ??= new PhysicsManager();
    }
    
    private PhysicsManager()
    {
        allPhysicsObjects = new List<PhysicsBody>();
        colliding = new HashSet<CollidingObject>();
        lastUpdate = Bootstrap.getCurrentMillis();

        gravityDir = new Vector2(0, 1);
        
        if (Bootstrap.checkEnvironmentalVariable("gravity_modifier"))
        {
            GravityModifier =
                float.Parse(Bootstrap.getEnvironmentalVariable("gravity_modifier"), CultureInfo.InvariantCulture);
        }
        else
        {
            GravityModifier = 0.1f;
        }

        if (Bootstrap.checkEnvironmentalVariable("gravity_dir")) {
            string tmp = Bootstrap.getEnvironmentalVariable("gravity_dir");
            string[] tmpBits = tmp.Split(",");
            gravityDir = new Vector2(int.Parse(tmpBits[0]), int.Parse(tmpBits[1]));
        }
        else {
            gravityDir = new Vector2 (0, 1);
        }
    }
    
    public bool update()
    {
        if (willTick() == false) return false;
        // Debug.Log("Tick: " + Bootstrap.TimeElapsed);

        lastUpdate = Bootstrap.getCurrentMillis();

        foreach (PhysicsBody body in allPhysicsObjects)
        {
            if (body.UsesGravity) {
                body.applyGravity(GravityModifier, gravityDir);
            }
            body.physicsTick();
            body.recalculateColliders();
        }
        
        if (!Bootstrap.getRunningGame().Is3d)
        {
            // Only for 2d
            
            List<CollidingObject> toRemove = new List<CollidingObject>();
            // Check for old collisions that should be persisted
            foreach (CollidingObject col in colliding)
            {
                CollisionHandler ch = col.A.Colh;
                CollisionHandler ch2 = col.B.Colh;

                // If the object has been destroyed in the interim, it should still trigger a collision exit.
                if (col.A.Parent.ToBeDestroyed) {
                    ch2.onCollisionExit (null);
                    toRemove.Add(col);
                }

                if (col.B.Parent.ToBeDestroyed)
                {
                    ch.onCollisionExit(null);
                    toRemove.Add(col);
                }

                Vector2? impulse = checkCollisionBetweenObjects2d(col.A, col.B);

                if (impulse != null)
                {
                    ch.onCollisionStay(col.B);
                    ch2.onCollisionStay(col.A);
                }
                else
                {
                    ch.onCollisionExit(col.B);
                    ch2.onCollisionExit(col.A);
                    toRemove.Add(col);
                }
            }
            
            foreach (CollidingObject col in toRemove)
            {
                colliding.Remove(col);
            }
            toRemove.Clear();
        }
        
        // Check for new collisions
        checkForCollisions();
        
        // Debug.Log("Time Interval is " + (Bootstrap.getCurrentMillis() - lastUpdate) + ", " + colliding.Count);
        return true;
    }
    
    private void checkForCollisions()
    {
        if (!Bootstrap.getRunningGame().Is3d)
        {
            // 2d
            
            HashSet<CollidingObject> potentialCollisions = getObjectsOverlappingForAxis(allPhysicsObjects, "x");
            narrowPass2d(potentialCollisions);
        }
        else
        {
            // 3d
            
            // Get collisions in the x-axis
            HashSet<CollidingObject> potentialCollisionsInXAxis = getObjectsOverlappingForAxis(allPhysicsObjects, "x");

            // For the y-axis, only look at the objects that overlap in the x-axis
            HashSet<PhysicsBody> xAxisOverlappingBodies = new HashSet<PhysicsBody>();
            foreach (CollidingObject collidingObject in potentialCollisionsInXAxis)
            {
                xAxisOverlappingBodies.Add(collidingObject.A);
                xAxisOverlappingBodies.Add(collidingObject.B);
            }
            HashSet<CollidingObject> potentialCollisionsInYAxis = getObjectsOverlappingForAxis(xAxisOverlappingBodies.ToList(), "y");

            // Get the potential collisions, the bodies overlapping in both x and y-axis
            HashSet<CollidingObject> potentialCollisions = new HashSet<CollidingObject>();
            foreach (CollidingObject collidingObjectX in potentialCollisionsInXAxis)
            {
                foreach (CollidingObject collidingObjectY in potentialCollisionsInYAxis)
                {
                    if (collidingObjectX.Equals(collidingObjectY))
                    {
                        potentialCollisions.Add(collidingObjectX);
                    }
                }
            }
            narrowPass3d(potentialCollisions);
        }
    }
    
    private HashSet<CollidingObject> getObjectsOverlappingForAxis(List<PhysicsBody> physicsBodies, string axis)
    {
        SAPEntry sap = null;
        foreach (PhysicsBody body in physicsBodies)
        {
            SAPEntry newEntery = new SAPEntry();
            float[] pos;
            switch (axis)
            {
                case "x":
                    pos = body.MinAndMaxX;
                    break;
                case "y":
                    pos = body.MinAndMaxY;
                    break;
                case "z":
                    pos = body.MinAndMaxZ;
                    break;
                default:
                    throw new Exception("Axis not supported for search and sweep!");
            }
            newEntery.Owner = body;
            newEntery.StartPos = pos[0];
            newEntery.EndPos = pos[1];
            sap = SAPEntry.addToList(sap, newEntery);
        }
        
        HashSet<CollidingObject> collisionsInAxis = new HashSet<CollidingObject>();
        List<SAPEntry> activeObjects = new List<SAPEntry>();
        List<int> toRemove = new List<int>();
        SAPEntry start = sap;
        
        // Search and prune
        while (start != null)
        {
            activeObjects.Add(start);
            for (int i = 0; i < activeObjects.Count; i++)
            {
                if (start == activeObjects[i])
                {
                    continue;
                }
                if (start.StartPos >= activeObjects[i].EndPos)
                {
                    toRemove.Add(i);
                }
                else
                {
                    CollidingObject col = new CollidingObject();

                    if (start.Owner.Mass > activeObjects[i].Owner.Mass)
                    {
                        col.A = start.Owner;
                        col.B = activeObjects[i].Owner;
                    }
                    else
                    {
                        col.B = start.Owner;
                        col.A = activeObjects[i].Owner;
                    }

                    if (!colliding.Contains(col))
                    {
                        collisionsInAxis.Add(col);
                    }
                    // Debug.getInstance().log("Adding potential collision: " + col.ToString());
                }
            }
            for (int j = toRemove.Count - 1; j >= 0; j--)
            {
                activeObjects.RemoveAt(toRemove[j]);
            }
            toRemove.Clear();
            start = start.Next;
        }
        // Debug.Log("Checking " + collisionsToCheck.Count + " collisions");
        SAPEntry.clearList(sap);
        return collisionsInAxis;
    }
    
    private void narrowPass2d(HashSet<CollidingObject> collisionsToCheck)
    {
        // Debug.getInstance().log("Active objects " + collisionsToCheck.Count);

        foreach (CollidingObject ob in collisionsToCheck)
        {
            Vector2? possibleImpulse = checkCollisionBetweenObjects2d(ob.A, ob.B);

            // Only proceed if colliding objects has impulse!
            if (!possibleImpulse.HasValue) continue;
            
            Vector2 impulse = possibleImpulse.Value;
            Debug.Log("Col is " + ob + ", impulse " + possibleImpulse);

            if (ob.A.PassThrough != true && ob.B.PassThrough != true)
            {
                float massTotal = ob.A.Mass + ob.B.Mass;
                float massProp;
                if (ob.A.Kinematic)
                {
                    massProp = 1;
                }
                else
                {
                    massProp = ob.A.Mass / massTotal;
                }
                if (ob.A.ImpartForce)
                {
                    ob.A.impartForces(ob.B, massProp);
                    ob.A.reduceForces(1.0f - massProp);
                }
                if (ob.B.Kinematic == false)
                {
                    ob.B.Parent.Transform.translate(-1 * (impulse.X * massProp), -1 * (impulse.Y * massProp));
                }
                if (ob.B.Kinematic)
                {
                    massProp = 1;
                }
                else
                {
                    massProp = 1.0f - massProp;
                }
                if (ob.A.Kinematic == false)
                {
                    ob.A.Parent.Transform.translate((impulse.X * massProp), (impulse.Y * massProp));
                }
                if (ob.A.StopOnCollision)
                {
                    ob.A.stopForces();
                }

                if (ob.B.StopOnCollision)
                {
                    ob.B.stopForces();
                }
            }
            
            ((CollisionHandler)ob.A.Parent).onCollisionEnter(ob.B);
            ((CollisionHandler)ob.B.Parent).onCollisionEnter(ob.A);
            colliding.Add(ob);
            
            if (ob.A.ReflectOnCollision)
            {                        
                ob.A.reflectForces(impulse);
            }
            if (ob.B.ReflectOnCollision)
            {
                ob.B.reflectForces(impulse);
            }
        }
    }
    
    private void narrowPass3d(HashSet<CollidingObject> collisionsToCheck)
    {
        foreach (CollidingObject ob in collisionsToCheck)
        {
            Vector2? possibleImpulse = checkCollisionBetweenObjects2d(ob.A, ob.B);

            // Only proceed if colliding objects has impulse!
            if (!possibleImpulse.HasValue) continue;
            
            ((CollisionHandler)ob.A.Parent).onCollisionStay(ob.B);
            ((CollisionHandler)ob.B.Parent).onCollisionStay(ob.A);
        }
    }

    public void addPhysicsObject(PhysicsBody body)
    {
        if (allPhysicsObjects.Contains(body)) return;
        allPhysicsObjects.Add(body);
    }

    public void removePhysicsObject(PhysicsBody body)
    {
        allPhysicsObjects.Remove(body);
    }

    public bool willTick()
    {
        return Bootstrap.getCurrentMillis() - lastUpdate > TimeInterval;
    }

    public void drawDebugColliders()
    {
        foreach (PhysicsBody body in allPhysicsObjects)
        {
            // Debug drawing - always happens.
            body.drawMe();
        }
    }

    private static Vector2? checkCollisionBetweenObjects2d(PhysicsBody a, PhysicsBody b)
    {
        foreach (Collider col in a.getColliders())
        {
            foreach (Collider col2 in b.getColliders())
            {
                Vector2? impulse = col.checkCollision(col2);
                if (impulse != null) return impulse;
            }
        }
        return null;
    }
}