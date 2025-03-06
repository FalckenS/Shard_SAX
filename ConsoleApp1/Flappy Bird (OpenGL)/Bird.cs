/*
*   @author Samuel Falck
*/
using System;
using SDL2;
using Shard;

namespace GameFlappyBird;

internal class Bird : GameObject, CollisionHandler, InputListener
{
    private const float Mass = 0.1f;
    private const float FlyForce = 1.5f;
    
    public Shard.GameFlappyBird Game { init; get; }
    private bool _spacePressed;

    public override void initialize()
    {
        // Transform X, Y is bottom right of GameObject
        Transform2D.X = -300;
        Transform2D.Y = 300;
        Transform2D.Width = 50;
        Transform2D.Height = 50;
        Transform2D.rotate(90);
        
        setPhysicsEnabled();
        MyBody.addRectCollider();
        MyBody.Mass = Mass;
        MyBody.Drag = 0;
        MyBody.MaxForce = 20;
        MyBody.UsesGravity = true;
        
        Bootstrap.getInput().addListener(this);
        addTag("Green");
        _spacePressed = false;
    }
    
    public void handleInput(InputEvent inp, string eventType)
    {
        switch (eventType)
        {
            case "KeyDown" when inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_SPACE && !_spacePressed:
                _spacePressed = true;
                // Fly up
                MyBody.addForce(Transform.Forward2d, FlyForce);
                break;
            case "KeyUp" when inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_SPACE && _spacePressed:
                _spacePressed = false;
                break;
        }
    }

    public override void update()
    {
        if (MyBody.MinAndMaxY[0] <= -Bootstrap.getDisplay().getHeight() / 2f)
        {
            Console.WriteLine("Bird hit floor!");
            Die();
        }
        if (MyBody.MinAndMaxY[1] >= Bootstrap.getDisplay().getHeight() / 2f)
        {
            Console.WriteLine("Bird hit roof!");
            Die();
        }
        Bootstrap.getDisplay().addToDraw(this);
    }

    public void onCollisionEnter(PhysicsBody x)
    {
        Console.WriteLine("Bird hit wall!");
        Die();
    }

    private void Die()
    {
        ToBeDestroyed = true;
        Game.GameOver = true;
        Bootstrap.getInput().removeListener(this);
    }
    
    public void onCollisionExit(PhysicsBody x) {}

    public void onCollisionStay(PhysicsBody x) {}
}