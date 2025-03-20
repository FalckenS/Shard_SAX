using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Shard
{
    enum LightSourceType
    {
        Point,
        Dir
    }

    class LightObject : GameObject
    {
        Vector3 color;
        Vector3 position;
        LightSourceType type;
        Vector3 ambient;
        Vector3 diffuse;
        Vector3 specular;

        public Matrix4 ModelMatrix;

        public LightObject(float tx, float ty, float tz, // translation
                          float rx, float ry, float rz, // rotation
                          float sx, float sy, float sz,
                          Vector3 _color, LightSourceType _type)
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
            color = _color;
            position = new Vector3(tx, ty, tz);
            type = _type;

            ModelMatrix = calcModel();
        }

        public Matrix4 calcModel()
        {
            Matrix4 trans = Matrix4.CreateTranslation(Transform.X, Transform.Y, Transform.Z);
            Matrix4 rotX = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Transform.Rotx));
            Matrix4 rotY = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Transform.Roty));
            Matrix4 rotZ = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Transform.Rotz));
            Matrix4 scale = Matrix4.CreateScale(Transform.ScaleX, Transform.ScaleY, Transform.ScaleZ);
            return scale * rotZ * rotY * rotX * trans;
        }

        public Vector3 getColor()
        {
            return color;
        }

        public Vector3 getPosition()
        {
            return position;
        }

        public LightSourceType getType()
        {
            return type;
        }

        public Vector3 Ambient { get => ambient; set => ambient = value; }
        public Vector3 Diffuse { get => diffuse; set => diffuse = value; }
        public Vector3 Specular { get => specular; set => specular = value; }

    }
}
