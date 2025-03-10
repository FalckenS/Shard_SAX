using System;
using System.Drawing;

namespace Shard;

internal class GameTestOpenGL : Game
{
    public override void initialize()
    {
        RectangleGameObject rectangle = new RectangleGameObject();
        rectangle.Transform.X = 0;
        rectangle.Transform.Y = 0;
        rectangle.Transform.Width = 100;
        rectangle.Transform.Height = 100;
    }

    public override void update()
    {
        Bootstrap.getDisplay().showText("Hello", 60.0f, 50.0f, 1, 255, 255, 255);
    }
}

internal class RectangleGameObject : GameObject, CollisionHandler
{
    private int _minX;
    private int _minY;
    private int _maxX;
    private int _maxY;
    private Display _display;
    
    public override void initialize()
    {
        _display = Bootstrap.getDisplay();
        
        setPhysicsEnabled();
        MyBody.Mass = 1;
        MyBody.Kinematic = true;
        MyBody.addRectCollider();

        UpdatePosition();
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
        _display.drawLine(_maxX, _maxY, _maxX, _minY, Color.Green);
        _display.drawLine(_maxX, _maxY, _minX, _maxY, Color.Green);
    }
    
    private void Render()
    {
        _display.drawRectangle(
            _minX, _maxY,
            _maxX, _maxY,
            _maxX, _minY,
            _minX, _minY,
            Color.Blue);
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