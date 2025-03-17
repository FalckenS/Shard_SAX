/*
*   @author Samuel Falck
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SDL2;
using Shard;
using Shard.SAX.Cinema;
using Shard.SAX.Graphics2D;
using Shard.SAX.IO;

namespace GameFlappyBird;

internal class Bird : GameObject, CollisionHandler, InputListener
{
    private const float Mass = 0.1f;
    private const float FlyForce = 1.5f;

    private Animator<TextureRegion> _birdAnimator;
    private TextureSheet _birdSheet;
    private Animation<TextureRegion> _birdAnim;
    private Sprite _birb;
    DisplayOpenGL _display = (DisplayOpenGL)Bootstrap.getDisplay();
    private char sep = Path.DirectorySeparatorChar;


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


        _birdSheet = new TextureSheet(Shard.SAX.IO.TextureLoader.GetTexture("flappy"+ sep + "flappy_spritesheet.png"), 4, 1);
        TextureSheet redBirdSheet = new TextureSheet(Shard.SAX.IO.TextureLoader.GetTexture("flappy"+ sep + "flappy_sheet_red.png"),4,1);
        TextureSheetAnimationFactory redBirdAnimFactory = new TextureSheetAnimationFactory(redBirdSheet,500);
        TextureSheetAnimationFactory animFactory = new TextureSheetAnimationFactory(_birdSheet,1000);

        List<Animation<TextureRegion>> al= new List<Animation<TextureRegion>>();
        al.AddRange(animFactory.GenerateAnimations("flap", 1));
        al.AddRange(redBirdAnimFactory.GenerateAnimations("flapRed", 1));

        _birdAnimator = new Animator<TextureRegion>(al);
        _birdAnimator.Play(Bootstrap.getCurrentMillis());

        _birb = new Sprite(_birdAnimator.GetKeyFrame(Bootstrap.getCurrentMillis()), 100, 0, 100, 100);
        _display.SpriteBatch.Draw(_birb);

        
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
            case "KeyUp" when inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_E && _spacePressed:
                _spacePressed = false;
                break;
            case "KeyDown" when inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_R:
                if (_birdAnimator.GetCurrentAnim().Name == "flap0") { _birdAnimator.Queue("flapRed0"); }
                else { _birdAnimator.Queue("flap0"); }
                
                break;
        }
    }

    public override void update()
    {
        if (MyBody.MinAndMaxY[0] <= -Bootstrap.getDisplay().getHeight() / 2f)
        {
            Die();
        }
        if (MyBody.MinAndMaxY[1] >= Bootstrap.getDisplay().getHeight() / 2f)
        {
            Die();
        }

        //_birb.SetTextureRegion(_birdAnim.GetKeyFrame(Bootstrap.getCurrentMillis()));
        _birb.SetTextureRegion(_birdAnimator.GetKeyFrame(Bootstrap.getCurrentMillis()));
        _display.SpriteBatch.Draw(_birb);

        _birb.Y = Transform.Y + 400;
    }

    public void onCollisionEnter(PhysicsBody x)
    {
        Die();
    }

    private void Die()
    {
        ToBeDestroyed = true;
        Game.GameOver = true;
        Bootstrap.getInput().removeListener(this);
    }

    public void onCollisionExit(PhysicsBody x) { }

    public void onCollisionStay(PhysicsBody x) { }
}