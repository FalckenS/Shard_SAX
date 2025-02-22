using Shard;

namespace GameFlappyBird;

internal class Wall : GameObject, CollisionHandler
{
    public bool BirdHasPassed { get; set; } = false;
    private const float WallSpeed = 100f;
    
    public override void initialize()
    {
        // Transform X, Y is bottom right of GameObject
        Transform2D.X = Bootstrap.getDisplay().getWidth() / 2f;
        Transform2D.Wid = 100;
        
        setPhysicsEnabled();
        MyBody.Kinematic = true;
        MyBody.addRectCollider();
        
        addTag("Red");
    }
    
    public override void update()
    {
        Transform2D.X -= WallSpeed * (float)Bootstrap.getDeltaTime();
        Bootstrap.getDisplay().addToDraw(this);
    }

    public void onCollisionEnter(PhysicsBody x) {}

    public void onCollisionExit(PhysicsBody x) {}

    public void onCollisionStay(PhysicsBody x) {}
}