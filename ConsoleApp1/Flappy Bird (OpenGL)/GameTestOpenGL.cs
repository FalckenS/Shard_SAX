using System.Drawing;

namespace Shard;

internal class GameTestOpenGL : Game
{
    public override void initialize()
    {
        RectangleGameObject rectangle = new RectangleGameObject();
        rectangle.Transform2D.X = 0;
        rectangle.Transform2D.Y = 0;
        rectangle.Transform2D.Wid = 100;
        rectangle.Transform2D.Ht = 100;
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
    
    public override void initialize()
    {
        setPhysicsEnabled();
        MyBody.Kinematic = true;
        MyBody.addRectCollider();

        UpdatePosition();
    }
    
    public override void update()
    {
        UpdatePosition();
        RenderWithLines();
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
        Bootstrap.getDisplay().drawLine(_minX, _minY, _maxX, _minY, Color.Red);
        Bootstrap.getDisplay().drawLine(_minX, _minY, _minX, _maxY, Color.Red);
        Bootstrap.getDisplay().drawLine(_maxX, _maxY, _maxX, _minY, Color.Blue);
        Bootstrap.getDisplay().drawLine(_maxX, _maxY, _minX, _maxY, Color.Blue);
    }

    public void onCollisionEnter(PhysicsBody x) {}

    public void onCollisionExit(PhysicsBody x) {}

    public void onCollisionStay(PhysicsBody x) {}
}