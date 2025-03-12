/*
*   @author Samuel Falck
*/
using Shard;
using Shard.SAX.Graphics2D;
using Shard.SAX.IO;

namespace GameFlappyBird;

internal class Wall : GameObject, CollisionHandler
{
    private Sprite _sprite;
    private const float WallSpeed = 100f;

    public bool BirdHasPassed { get; set; }

    public override void initialize()
    {
        // Transform X, Y is bottom right of GameObject
        Transform2D.X = Bootstrap.getDisplay().getWidth() / 2f;
        Transform2D.Width = 100;

        setPhysicsEnabled();
        MyBody.Kinematic = true;
        MyBody.addRectCollider();

        addTag("Red");
        BirdHasPassed = false;
        _sprite = new Sprite(AssetManager2.getTexture("pipe-red.png"), 0, 0, 100, 0);
    }

    public override void update()
    {
        Transform2D.X -= WallSpeed * (float)Bootstrap.getDeltaTime();
        DisplayOpenGL display = (DisplayOpenGL)Bootstrap.getDisplay();
        _sprite.X = Transform2D.X + 400;
        _sprite.Y = Transform2D.Y + 400;
        _sprite.Height = Transform2D.Height;
        display.SpriteBatch.Draw(_sprite);

    }

    public void onCollisionEnter(PhysicsBody x) { }

    public void onCollisionExit(PhysicsBody x) { }

    public void onCollisionStay(PhysicsBody x) { }
}