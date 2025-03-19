using System;
using System.Collections.Generic;
using System.Drawing;
//using System.Numerics;
using OpenTK.Mathematics;
using Shard;

namespace Shard;

class GameDoom : Game
{
    private int _frame, _second, _score;
    private bool _scoreUpdated;
    private TextToRender hello;
    private CubeObject wall_1, wall_2, wall_3, wall_4, floor;
    private Player player;
    private LightObject light, light2;
    private TargetObject target;
    private ModelObject gun;
    private Camera _camera;
    private long startTime;

    private CubeObject cube;

    public override void initialize()
    {
        _frame = 0;
        _second = 0;
        _score = 0;
        _scoreUpdated = false;
        startTime = Bootstrap.getCurrentMillis();

        cube = new CubeObject(0, 10, -10, 0, 30, 0, 5, 5, 5, 1, 1, 1);
        RenderParams renderParamsCube = new RenderParams();
        renderParamsCube.useDiffuseMap = 0;
        renderParamsCube.useNormalMap = 0;
        renderParamsCube.color = new Vector3(0.2f, 0.5f, 0.7f);
        renderParamsCube.shininess = 16.0f;
        renderParamsCube.specular = new Vector3(1);
        cube.RParams = renderParamsCube;


        hello = new TextToRender("Hello", 60.0f, 50.0f, 1, 255, 255, 255);

        wall_1 = new CubeObject(0, 0, -25, 0, 0, 0, 10, 10, 4, 5.0f, 6.0f, 1);
        wall_2 = new CubeObject(0, 0, 25, 0, 0, 0, 10, 10, 4, 5.0f, 6.0f, 1);
        wall_3 = new CubeObject(25, 0, 0, 0, 90, 0, 10, 10, 4, 5.0f, 6.0f, 1);
        wall_4 = new CubeObject(-25, 0, 0, 0, 90, 0, 10, 10, 4, 5.0f, 6.0f, 1);
        RenderParams renderParamsBrick = new RenderParams();
        renderParamsBrick.pathDiff = Bootstrap.getAssetManager().getAssetPath("brick_diff.jpg");
        renderParamsBrick.pathNormal = Bootstrap.getAssetManager().getAssetPath("brick_normal.jpg");
        renderParamsBrick.shininess = 8.0f;
        renderParamsBrick.specular = new Vector3(0.3f);
        renderParamsBrick.useDiffuseMap = 1;
        renderParamsBrick.useNormalMap = 1;
        wall_1.RParams = renderParamsBrick;
        wall_2.RParams = renderParamsBrick;
        wall_3.RParams = renderParamsBrick;
        wall_4.RParams = renderParamsBrick;

        floor = new CubeObject(0, 0, 0, 0, 0, 0, 10, 4, 10, 5.0f, 0.5f, 5.0f);
        RenderParams renderParamsFloor = new RenderParams();
        renderParamsFloor.pathDiff = Bootstrap.getAssetManager().getAssetPath("forest_diff.jpg");
        renderParamsFloor.pathNormal = Bootstrap.getAssetManager().getAssetPath("forest_normal.jpg");
        renderParamsFloor.shininess = 8.0f;
        renderParamsFloor.specular = new Vector3(0.1f);
        renderParamsFloor.useDiffuseMap = 1;
        renderParamsFloor.useNormalMap = 1;
        floor.RParams = renderParamsFloor;

        light = new LightObject(-10, 30, 15, 0, 0, 0, 1f, 1f, 1f, new Vector3(1, 1, 1), LightSourceType.Point);
        light.Ambient = new Vector3(0.1f);
        light.Diffuse = new Vector3(0.5f);
        light.Specular = new Vector3(1.0f);

        light2 = new LightObject(-10, 30, -15, 0, 0, 0, 1f, 1f, 1f, new Vector3(1, 1, 1), LightSourceType.Point);
        light2.Ambient = new Vector3(0.1f);
        light2.Diffuse = new Vector3(0.5f);
        light2.Specular = new Vector3(1.0f);

        player = new Player(0, 10, 0);
        _camera = player.GetCamera();
        Bootstrap.getDisplay().LinkCamera(_camera);
        _camera.AspectRatio = Bootstrap.getDisplay().getWidth() / (float)Bootstrap.getDisplay().getHeight();

        target = new TargetObject(-15, 10, -15, 0, 0, 0, 3, 3, 3, 0.2f, 0.2f, 0.2f);
        target.LinkCamera(_camera);
        RenderParams renderParamsTarget = new RenderParams();
        renderParamsTarget.shininess = 4.0f;
        renderParamsTarget.specular = new Vector3(0.1f);
        renderParamsTarget.useDiffuseMap = 0;
        renderParamsTarget.useNormalMap = 0;
        renderParamsTarget.color = new Vector3(0, 0.5f, 1);
        target.RParams = renderParamsTarget;

        gun = new ModelObject(0, 0, 0, 0, 180, 0, 0.001f, 0.001f, 0.001f);
        gun.Load(Bootstrap.getAssetManager().getAssetPath("gun.obj"));
        RenderParams renderParamsGun = new RenderParams();
        renderParamsGun.pathDiff = Bootstrap.getAssetManager().getAssetPath("gun_diff.jpg");
        renderParamsGun.pathNormal = Bootstrap.getAssetManager().getAssetPath("gun_normal.jpg");
        renderParamsGun.shininess = 8.0f;
        renderParamsGun.specular = new Vector3(0.1f);
        renderParamsGun.useDiffuseMap = 1;
        renderParamsGun.useNormalMap = 0;
        gun.RParams = renderParamsGun;
        player.LinkModel("gun", gun);
        player.SetModelOffset("gun", new Vector3(1, -1, -1.5f));
    }

    public override void update()
    {

        Bootstrap.getDisplay().showText("Score: "+ _score, -380, 350, 1, 255, 255, 255);
        Bootstrap.getDisplay().addToDrawCube(wall_1);
        Bootstrap.getDisplay().addToDrawCube(wall_2);
        Bootstrap.getDisplay().addToDrawCube(wall_3);
        Bootstrap.getDisplay().addToDrawCube(wall_4);
        Bootstrap.getDisplay().addToDrawCube(floor);
        Bootstrap.getDisplay().addToDrawLight(light);
        Bootstrap.getDisplay().addToDrawLight(light2);

        _frame++;
        int timeStep = 2;
        long currentTime = Bootstrap.getCurrentMillis();
        //if (_frame == timeStep * getTargetFrameRate())
        if (currentTime - startTime >= 2000)
        {
            //_frame -= timeStep * getTargetFrameRate();
            startTime = currentTime;
            _second += timeStep;
            Debug.getInstance().log("second " + _second);
            Random random = new Random();
            float randomMinX = -10; float randomMaxX = 10;
            float randomMinY = 5; float randomMaxY = 15;
            float randomFloatX = randomMinX + (randomMaxX - randomMinX) * random.NextSingle();
            float randomFloatY = randomMinY + (randomMaxY - randomMinY) * random.NextSingle();
            target.Transform.X = randomFloatX;
            target.Transform.Y = randomFloatY;
            target.Hit = false;
            _scoreUpdated = false;
        }

        if (!target.Hit)
        {
            Bootstrap.getDisplay().addToDrawCube(target);
        }
        else
        {
            if (!_scoreUpdated)
            {
                _score++;
                _scoreUpdated = true;
            }

        }


        Bootstrap.getDisplay().addToDrawModel(gun);

        int w = Bootstrap.getDisplay().getWidth();
        int h = Bootstrap.getDisplay().getHeight();
        int offset = h / 100;
        Bootstrap.getDisplay().drawLine(w / 2 - offset, h / 2, w / 2 + offset, h / 2, Color.Red);
        Bootstrap.getDisplay().drawLine(w / 2, h / 2 - offset, w / 2, h / 2 + offset, Color.Red);

    }

    public override int getTargetFrameRate()
    {
        return 30;
    }
}


class TargetObject : CubeObject, InputListener, CollisionHandler
{

    private Camera _sourceCamera;
    private bool _hit;

    public TargetObject(float tx, float ty, float tz,
                          float rx, float ry, float rz,
                          float sx, float sy, float sz,
                          float _w, float _h, float _d) 
        : base(tx, ty, tz, rx, ry, rz, sx, sy, sz, _w, _h, _d)
    {
        setPhysicsEnabled();
        MyBody.addBoxCollider();
        Bootstrap.getInput().addListener(this);
        _hit = false;
    }

    public void LinkCamera(Camera camera)
    {
        _sourceCamera = camera;
    }

    public bool Hit
    {
        get => _hit;
        set => _hit = value;
    }
   
    

    public void handleInput(InputEvent inp, string eventType)
    {
        if (Bootstrap.getRunningGame().isRunning() == false)
        {
            return;
        }

        if (eventType == "MouseDown")
        {
            System.Numerics.Vector3 pos = new System.Numerics.Vector3(_sourceCamera.Position.X, _sourceCamera.Position.Y, _sourceCamera.Position.Z);
            System.Numerics.Vector3 dir = new System.Numerics.Vector3(_sourceCamera.Front.X, _sourceCamera.Front.Y, _sourceCamera.Front.Z);
            if (MyBody.checkCollisions(pos, dir))
            {
                Debug.getInstance().log("Hit!");
                _hit = true;    
            }

        }
    }

    public void onCollisionEnter(PhysicsBody x) { }
    public void onCollisionExit(PhysicsBody x) { }
    public void onCollisionStay(PhysicsBody x) { }



}
