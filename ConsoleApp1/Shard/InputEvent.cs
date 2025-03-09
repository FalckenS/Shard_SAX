/*
*
*   This is a general, simple container for all the information someone might want to know about 
*       keyboard or mouse input.   The same object is used for both, so use your common sense 
*       to work out whether you can use the contents of, say 'x' and 'y' when registering for 
*       a key event.
*   @author Michael Heron
*   @version 1.0
*   
*/

namespace Shard
{
    class InputEvent
    {
        private int x;
        private int y;
        private int lx;
        private int ly;
        private int dx;
        private int dy;
        private int button;
        private int key;
        private string classification;

        public int X
        {
            get => x;
            set => x = value;
        }
        public int Y
        {
            get => y;
            set => y = value;
        }

        public int Lx
        {
            get => lx;
            set => lx = value;
        }
        public int Ly
        {
            get => ly;
            set => ly = value;
        }

        public int Dx
        {
            get => dx;
            set => dx = value;
        }
        public int Dy
        {
            get => dy;
            set => dy = value;
        }

        public int Button
        {
            get => button;
            set => button = value;
        }
        public string Classification
        {
            get => classification;
            set => classification = value;
        }
        public int Key
        {
            get => key;
            set => key = value;
        }
    }
}
