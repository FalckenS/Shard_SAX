using OpenTK.Mathematics;
using SDL2;
using System;
using System.Collections.Generic;

namespace Shard
{
    class CubeObject : GameObject
    {
        RenderParams renderParams;

        public Matrix4 ModelMatrix;

        public CubeObject(float tx, float ty, float tz, // translation
                          float rx, float ry, float rz, // rotation
                          float sx, float sy, float sz,  // scale
                          float _w, float _h, float _d) // w h d
        {
            Transform.X = tx;
            Transform.Y = ty;
            Transform.Z = tz;
            Transform.Rotx = rx;
            Transform.Roty = ry;
            Transform.Rotz = rz;
            Transform.ScaleX = sx;
            Transform.ScaleY = sy;
            Transform.ScaleZ = sz;
            Transform.Width = _w;
            Transform.Height = _h;
            Transform.Depth = _d;

            ModelMatrix = calcModel();
        }

        public RenderParams RParams { get => renderParams; set => renderParams = value; }

        public Matrix4 calcModel()
        {
            Matrix4 trans = Matrix4.CreateTranslation(Transform.X, Transform.Y, Transform.Z);
            Matrix4 rotX = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Transform.Rotx));
            Matrix4 rotY = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Transform.Roty));
            Matrix4 rotZ = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Transform.Rotz));
            Matrix4 scale = Matrix4.CreateScale(Transform.ScaleX, Transform.ScaleY, Transform.ScaleZ);
            return scale * rotZ * rotY * rotX * trans;

        }
    }
}
