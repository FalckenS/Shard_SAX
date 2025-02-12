using System;

namespace Shard
{
    class GameTestOpenGL : Game
    {
        private int _frame;

        public override void initialize()
        {
            _frame = 0;
        }

        public override void update()
        {
            Bootstrap.getDisplay().showText("Hello", 60.0f, 50.0f, 1, 255, 255, 255);

            _frame++;
            if (_frame == 2000)
            {
                Console.WriteLine(_frame);
                GameObject gameObject = new GameObject();
                Bootstrap.getDisplay().addToDraw(gameObject);
            }
        }
    }
}