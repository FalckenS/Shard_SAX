using System;
using System.Collections.Generic;
using System.Drawing;

namespace Shard;

class GameTestOpenGL : Game
{
    private int _frame;
    private TextToRender hello;
    private List<RectangleGameObject> _rectangleGameObjects;

    public override void initialize()
    {
        _frame = 0;
        hello = new TextToRender("Hello", 60.0f, 50.0f, 1, 255, 255, 255);
        
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
    }
}

class RectangleGameObject : GameObject, CollisionHandler
{
    private int _minX;
    private int _minY;
    private int _maxX;
    private int _maxY;
    private Display _display;
    
    public override void initialize()
    {
        setPhysicsEnabled();
        MyBody.Mass = 1;
        MyBody.Kinematic = true;
        MyBody.addRectCollider();
        
        _minX = (int)MyBody.MinAndMaxX[0];
        _minY = (int)MyBody.MinAndMaxY[0];
        _maxX = (int)MyBody.MinAndMaxX[1];
        _maxY = (int)MyBody.MinAndMaxY[1];
        
        _display = Bootstrap.getDisplay();
    }
    
    public override void update()
    {
        UpdatePosition();
        //RenderWithLines();
        Render();
    }

    private void UpdatePosition()
    {
        _minX = (int)MyBody.MinAndMaxX[0];
        _minY = (int)MyBody.MinAndMaxY[0];
        _maxX = (int)MyBody.MinAndMaxX[1];
        _maxY = (int)MyBody.MinAndMaxY[1];
    }

    private void RenderWithLines()
    {
        _display.drawLine(_minX, _minY, _maxX, _minY, Color.Green);
        _display.drawLine(_minX, _minY, _minX, _maxY, Color.Green);
        _display.drawLine(_maxX, _minY, _maxX, _maxY, Color.Green);
        _display.drawLine(_minX, _maxY, _maxX, _maxY, Color.Green);
    }
    
    private void Render()
    {
        _display.drawRectangle(
            _minX, _maxY,
            _maxX, _maxY,
            _maxX, _minY,
            _minX, _minY,
            Color.Green);
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