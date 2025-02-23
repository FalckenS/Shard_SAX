using System;
using System.Collections.Generic;
using System.Drawing;

namespace Shard;

class GameDoom : Game
{
    private int _frame;
    private TextToRender hello;
    private List<RectangleGameObject> _rectangleGameObjects;
    private CubeObject cube1, cube2;
    private Player player;

    public override void initialize()
    {
        _frame = 0;

        hello = new TextToRender("Hello", 60.0f, 50.0f, 1, 255, 255, 255);

        cube1 = new CubeObject(0, 0, -50, 0, 0, 0, 100.0f, 20.0f, 1, Bootstrap.getAssetManager().getAssetPath("brick.jpg"));
        cube2 = new CubeObject(0, 0, 0, 0, 0, 0, 100, 0.5f, 100, Bootstrap.getAssetManager().getAssetPath("floor.jpg"));
        player = new Player(0, 3, 0);
        Camera playerCam = player.GetCamera();
        Bootstrap.getDisplay().LinkCamera(playerCam);
        playerCam.AspectRatio = Bootstrap.getDisplay().getWidth() / (float)Bootstrap.getDisplay().getHeight();
    }

    public override void update()
    {
        _frame++;
        Bootstrap.getDisplay().showText("Hello", 60.0f, 50.0f, 1, 255, 255, 255);
        Bootstrap.getDisplay().addToDrawCube(cube1);
        Bootstrap.getDisplay().addToDrawCube(cube2);

    }

    public override int getTargetFrameRate()
    {
        return 60;
    }
}
