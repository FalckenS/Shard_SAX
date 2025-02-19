using System;
using System.Collections.Generic;
using System.Drawing;

namespace Shard;

class GameDoom : Game
{
    private int _frame;
    private TextToRender hello;
    private List<RectangleGameObject> _rectangleGameObjects;
    private CubeObject cube;

    public override void initialize()
    {
        _frame = 0;
        //hello = new TextToRender("Hello", 60.0f, 50.0f, 1, 255, 255, 255);

        cube = new CubeObject(0, 0, 0, 45, 45, 0, 2, 0.5f, 1);
        Bootstrap.getDisplay().addToDrawCube(cube);
    }

    public override void update()
    {
        _frame++;
        //Bootstrap.getDisplay().showText("Hello", 60.0f, 50.0f, 1, 255, 255, 255);
        Bootstrap.getDisplay().addToDrawCube(cube);
    }
}
