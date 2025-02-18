using System;
using System.Collections.Generic;
using System.Drawing;

namespace Shard;

class GameTestOpenGL : Game
{
    private int _frame;
    private TextToDisplay hello;
    private List<RectangleGameObject> _rectangleGameObjects;

    public override void initialize()
    {
        _frame = 0;
        hello = new TextToDisplay("Hello", 60.0f, 50.0f, 1, 255, 255, 255);
        
        RectangleGameObject rectangle = new RectangleGameObject();
        rectangle.Transform.X = -1;
        rectangle.Transform.Y = 0;
        rectangle.Transform.Ht = 1;
        rectangle.Transform.Wid = 2;
    }

    public override void update()
    {
        _frame++;
        Bootstrap.getDisplay().showText("Hello", 60.0f, 50.0f, 1, 255, 255, 255);
        //Bootstrap.getDisplay().drawLine(0, 0, 1, 0, Color.Red);
    }
}

class RectangleGameObject : GameObject, CollisionHandler
{
    public override void initialize()
    {
        setPhysicsEnabled();
        MyBody.Mass = 1;
        MyBody.Kinematic = true;
        MyBody.addRectCollider();
    }
    
    public override void update()
    {
        Display display = Bootstrap.getDisplay();

        int minX = (int)MyBody.MinAndMaxX[0];
        int minY = (int)MyBody.MinAndMaxY[0];
        int maxX = (int)MyBody.MinAndMaxX[1];
        int maxY = (int)MyBody.MinAndMaxY[1];
        
        display.drawLine(minX, minY, maxX, minY, Color.Red);
        display.drawLine(minX, minY, minX, maxY, Color.Red);
        display.drawLine(maxX, minY, maxX, maxY, Color.Red);
        display.drawLine(minX, maxY, maxX, maxY, Color.Red);
    }

    public void onCollisionEnter(PhysicsBody x)
    {
        throw new NotImplementedException();
    }

    public void onCollisionExit(PhysicsBody x)
    {
        throw new NotImplementedException();
    }

    public void onCollisionStay(PhysicsBody x)
    {
        throw new NotImplementedException();
    }
}