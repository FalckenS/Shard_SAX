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
            _frame++;
            if (_frame == 200)
            {
                Console.WriteLine(_frame);
                GameObject gameObject = new GameObject();
                Bootstrap.getDisplay().addToDraw(gameObject);
            }
        }
    }
}