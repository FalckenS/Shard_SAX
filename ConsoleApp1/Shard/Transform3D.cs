/*
*
*   Our game engine functions in 2D, but all its features except for graphics can mostly be extended
*       from existing data structures.
*       
*   @author Michael Heron
*   @version 1.0
*   
*/

namespace Shard
{
    class Transform3D : Transform
    {
        private float z;
        private float rotx, roty;
        private float scalez;

        public Transform3D(GameObject o) : base(o)
        {
        }

        public float Z
        {
            get => z;
            set => z = value;
        }



        public float Scalez
        {
            get => scalez;
            set => scalez = value;
        }
        public float Rotx { get => rotx; set => rotx = value; }
        public float Roty { get => roty; set => roty = value; }
    }
}
