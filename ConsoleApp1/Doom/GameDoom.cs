using System;
using System.Collections.Generic;
using System.Drawing;

namespace Shard;

class GameDoom : Game
{
    private int _frame;
    private TextToRender hello;
    private List<RectangleGameObject> _rectangleGameObjects;
    private CubeObject wall_1, wall_2, wall_3, wall_4, floor;
    private Player player;

    public override void initialize()
    {
        _frame = 0;

        hello = new TextToRender("Hello", 60.0f, 50.0f, 1, 255, 255, 255);

        wall_1 = new CubeObject(0, 0, -25, 0, 0, 0, 50.0f, 20.0f, 1, Bootstrap.getAssetManager().getAssetPath("brick.jpg"));
        wall_2 = new CubeObject(0, 0, 25, 0, 0, 0, 50.0f, 20.0f, 1, Bootstrap.getAssetManager().getAssetPath("brick.jpg"));
        wall_3 = new CubeObject(25, 0, 0, 0, 90, 0, 50.0f, 20.0f, 1, Bootstrap.getAssetManager().getAssetPath("brick.jpg"));
        wall_4 = new CubeObject(-25, 0, 0, 0, 90, 0, 50.0f, 20.0f, 1, Bootstrap.getAssetManager().getAssetPath("brick.jpg"));
        floor = new CubeObject(0, 0, 0, 0, 0, 0, 50, 0.5f, 50, Bootstrap.getAssetManager().getAssetPath("floor.jpg"));
        player = new Player(0, 3, 0);
        Camera playerCam = player.GetCamera();
        Bootstrap.getDisplay().LinkCamera(playerCam);
        playerCam.AspectRatio = Bootstrap.getDisplay().getWidth() / (float)Bootstrap.getDisplay().getHeight();
    }

    public override void update()
    {
        _frame++;
        Bootstrap.getDisplay().showText("Hello", 60.0f, 50.0f, 1, 255, 255, 255);
        Bootstrap.getDisplay().addToDrawCube(wall_1);
        Bootstrap.getDisplay().addToDrawCube(wall_2);
        Bootstrap.getDisplay().addToDrawCube(wall_3);
        Bootstrap.getDisplay().addToDrawCube(wall_4);
        Bootstrap.getDisplay().addToDrawCube(floor);

    }

    public override int getTargetFrameRate()
    {
        return 60;
    }
}
