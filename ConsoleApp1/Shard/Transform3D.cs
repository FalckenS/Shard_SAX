/*
*
*   @author Michael Heron
*   @author Samuel Falck
*   @version 2.0
*   
*/

namespace Shard
{
    class Transform3D : Transform
    {
        private float z=0;
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

        public float Depth { get; set; } = 1f;
        public float ScaleZ { get; set; } = 1f;
        public float Radius { get; set; } = 1f;

    }
}

