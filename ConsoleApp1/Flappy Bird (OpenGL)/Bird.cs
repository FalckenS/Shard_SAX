/*
*   @author Samuel Falck
*/
using System;
using System.Collections.Generic;
using SDL2;
using Shard;
using Shard.Shard;

namespace GameFlappyBird;

internal class Bird : GameObject, CollisionHandler, InputListener
{
    private const float Mass = 0.1f;
    private const float FlyForce = 1.5f;

    
    private TextureSheet _birdSheet;
    private Animation<TextureRegion> _birdAnim;
    private Sprite _birb;
    DisplayOpenGL _display = (DisplayOpenGL)Bootstrap.getDisplay();


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

        
        _birdSheet = new TextureSheet(AssetManager2.getTexture("flappy_spritesheet.png"), 4, 1);
        _birdAnim = new Animation<TextureRegion>(_birdSheet.TextureRegionsList);
        _birdAnim.MilliSecondsBetweenKeyFrames = 1000;
        _birdAnim.Play();
        _birb = new Sprite(_birdAnim.GetKeyFrame(Bootstrap.getCurrentMillis(),PlayMode.FORWARD_LOOP), 100,0,100,100);

        _display.SpriteBatch.Draw(_birb);
    }
    
    public void handleInput(InputEvent inp, string eventType)
    {
        switch (eventType)
        {
            case "KeyDown" when inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_E && !_spacePressed:
                System.Console.WriteLine("TRIGGERED FLY UP");
                _spacePressed = true;
                // Fly up
                MyBody.addForce(Transform.Forward2d, FlyForce);
                
                break;
            case "KeyUp" when inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_E && _spacePressed:
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
        
        _birb.SetTextureRegion(_birdAnim.GetKeyFrame(Bootstrap.getCurrentMillis(),PlayMode.FORWARD_LOOP));
        _display.SpriteBatch.Draw(_birb);
        
        _birb.Y = Transform.Y + 400;
        System.Console.WriteLine("BIRB Y : " +Transform.Y);
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