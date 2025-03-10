using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK.Mathematics;

namespace Shard;

class GameDoom : Game
{
    private int _frame;
    private TextToRender hello;
    private CubeObject wall_1, wall_2, wall_3, wall_4, floor;
    private Player player;
    private LightObject light;

    public override void initialize()
    {
        _frame = 0;

        hello = new TextToRender("Hello", 60.0f, 50.0f, 1, 255, 255, 255);

        wall_1 = new CubeObject(0, 0, -25, 0, 0, 0, 10, 10, 4, 5.0f, 2.0f, 1);
        wall_2 = new CubeObject(0, 0, 25, 0, 0, 0, 10, 10, 4, 5.0f, 2.0f, 1);
        wall_3 = new CubeObject(25, 0, 0, 0, 90, 0, 10, 10, 4, 5.0f, 2.0f, 1);
        wall_4 = new CubeObject(-25, 0, 0, 0, 90, 0, 10, 10, 4, 5.0f, 2.0f, 1);
        //wall_1 = new CubeObject(0, 0, -25, 0, 0, 0, 50, 20, 4, 1, 1, 1);
        //wall_2 = new CubeObject(0, 0, 25, 0, 0, 0, 50, 20, 4, 1, 1, 1);
        //wall_3 = new CubeObject(25, 0, 0, 0, 90, 0, 50, 20, 4, 1, 1, 1);
        //wall_4 = new CubeObject(-25, 0, 0, 0, 90, 0, 50, 20, 4, 1, 1, 1);
        RenderParams renderParamsBrick = new RenderParams();
        renderParamsBrick.diff = AssetManager2.getTexture("brick_diff.jpg");
        renderParamsBrick.normal = AssetManager2.getTexture("brick_normal.jpg");
        renderParamsBrick.shininess = 8.0f;
        renderParamsBrick.specular = new Vector3(0.3f);
        wall_1.RParams = renderParamsBrick;
        wall_2.RParams = renderParamsBrick;
        wall_3.RParams = renderParamsBrick;
        wall_4.RParams = renderParamsBrick;

        floor = new CubeObject(0, 0, 0, 0, 0, 0, 10, 4, 10, 5.0f, 0.5f, 5.0f);
        RenderParams renderParamsFloor = new RenderParams();
        renderParamsFloor.diff = AssetManager2.getTexture("forest_diff.jpg");
        renderParamsFloor.normal = AssetManager2.getTexture("forest_normal.jpg");
        renderParamsFloor.shininess = 8.0f;
        renderParamsFloor.specular = new Vector3(0.1f);
        floor.RParams = renderParamsFloor;

        light = new LightObject(15, 15, -15, 0, 0, 0, 1, 1, 1, new Vector3(1, 1, 1), LightSourceType.Point);
        //light = new LightObject(0, 15, 0, 0, 0, 0, 1, 1, 1, new Vector3(1, 1, 1), LightSourceType.Point);
        light.Ambient = new Vector3(0.1f);
        light.Diffuse = new Vector3(1.0f);
        light.Specular = new Vector3(1.0f);
        player = new Player(0, 3, 0);
        Camera playerCam = player.GetCamera();
        Bootstrap.getDisplay().LinkCamera(playerCam);
        playerCam.AspectRatio = Bootstrap.getDisplay().getWidth() / (float)Bootstrap.getDisplay().getHeight();
    }

    public override void update()
    {
        _frame++;
        Bootstrap.getDisplay().showText("Hello", -380, 350, 1, 255, 255, 255);
        Bootstrap.getDisplay().addToDrawCube(wall_1);
        Bootstrap.getDisplay().addToDrawCube(wall_2);
        Bootstrap.getDisplay().addToDrawCube(wall_3);
        Bootstrap.getDisplay().addToDrawCube(wall_4);
        Bootstrap.getDisplay().addToDrawCube(floor);
        Bootstrap.getDisplay().addToDrawLight(light);

    }

    public override int getTargetFrameRate()
    {
        return 60;
    }
}
