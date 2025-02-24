using System;
using SDL2;
using Shard;

namespace GameFlappyBird;

internal class Bird : GameObject, CollisionHandler, InputListener
{
    private bool _spacePressed = false;
    public Shard.GameFlappyBird Game { get; set; } = null;
    private const float FlyForce = 1.5f;

    public override void initialize()
    {
        // Transform X, Y is bottom right of GameObject
        Transform2D.X = -300;
        Transform2D.Y = 300;
        Transform2D.Wid = 50;
        Transform2D.Ht = 50;
        Transform2D.rotate(90);
        
        setPhysicsEnabled();
        MyBody.addRectCollider();
        MyBody.Mass = 0.1f;
        MyBody.Drag = 0;
        MyBody.MaxForce = 20;
        MyBody.UsesGravity = true;
        
        Bootstrap.getInput().addListener(this);
        
        addTag("Green");
    }

    public override void update()
    {
        Bootstrap.getDisplay().addToDraw(this);
    }

    public override void physicsUpdate()
    {
        if (MyBody.MinAndMaxY[0] <= -Bootstrap.getDisplay().getHeight() / 2f)
        {
            Console.WriteLine("Bird hit flor!");
            Die();
        }

        if (MyBody.MinAndMaxY[1] >= Bootstrap.getDisplay().getHeight() / 2f)
        {
            Console.WriteLine("Bird hit roof!");
            Die();
        }
    }

    public void onCollisionEnter(PhysicsBody x)
    {
        Console.WriteLine("Bird hit wall!");
        Die();
    }

    private void Die()
    {
        ToBeDestroyed = true;
        Bootstrap.getInput().removeListener(this);
        Game.GameOver = true;
    }

    public void onCollisionExit(PhysicsBody x) {}

    public void onCollisionStay(PhysicsBody x) {}

    public void handleInput(InputEvent inp, string eventType)
    {
        switch (eventType)
        {
            case "KeyDown" when
                inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_SPACE && !_spacePressed:
                _spacePressed = true;
                FlyUp();
                break;
            case "KeyUp" when inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_SPACE && _spacePressed:
                _spacePressed = false;
                break;
        }
    }

    private void FlyUp()
    {
        MyBody.addForce(Transform.Forward, FlyForce);
    }
}