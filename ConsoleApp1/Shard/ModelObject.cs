using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SDL2;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Shard
{
    internal class ModelObject : GameObject
    {
        public List<Vector3> Vertices { get; private set; } = new List<Vector3>();
        public List<Vector3> Normals { get; private set; } = new List<Vector3>();
        public List<Vector2> TextureCoords { get; private set; } = new List<Vector2>();
        public List<ModelFace> Faces { get; private set; } = new List<ModelFace>();

        private int _vbo, _vao;

        RenderParams renderParams;

        private Matrix4 _initRotMatrix;
        private Matrix4 _initTransMatrix;
        private Matrix4 _initScaleMatrix;

        private Matrix4 _transMatrix;
        private Matrix4 _rotMatrix;
        private Matrix4 _scaleMatrix;

        public Matrix4 ModelMatrix;

        public ModelObject(float tx, float ty, float tz, // translation
                          float rx, float ry, float rz, // rotation
                          float sx, float sy, float sz)
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

            _initTransMatrix = Matrix4.CreateTranslation(Transform.X, Transform.Y, Transform.Z);
            Matrix4 rotX = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Transform.Rotx));
            Matrix4 rotY = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Transform.Roty));
            Matrix4 rotZ = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Transform.Rotz));
            _initRotMatrix = rotX * rotY * rotZ;
            _initScaleMatrix = Matrix4.CreateScale(Transform.ScaleX, Transform.ScaleY, Transform.ScaleZ);

            _transMatrix = Matrix4.Identity;
            _rotMatrix = Matrix4.Identity;
            _scaleMatrix = Matrix4.Identity;

            calcModel();
        }

        private void calcModel()
        {
            ModelMatrix = _initScaleMatrix * _scaleMatrix * _initRotMatrix * _rotMatrix * _initTransMatrix * _transMatrix;
        }

        public Matrix4 ScaleMatrix
        {
            get { return _scaleMatrix; }
            set { 
                _scaleMatrix = value; 
                calcModel();
            }
        }

        public Matrix4 RotMatrix
        {
            get { return _rotMatrix; }
            set
            {
                _rotMatrix = value;
                calcModel();
            }
        }

        public Matrix4 TransMatrix
        {
            get { return _transMatrix; }
            set
            {
                _transMatrix = value;
                calcModel();
            }
        }

        public RenderParams RParams { get => renderParams; set => renderParams = value; }

        public int GetVAO()
        {
            return _vao;
        }


        public void Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("OBJ file not found!", filePath);
            }

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] tokens = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length == 0) continue;

                    switch (tokens[0])
                    {
                        case "v": 
                            Vertices.Add(ParseVector3(tokens));
                            break;
                        case "vn": 
                            Normals.Add(Vector3.Normalize(ParseVector3(tokens)));
                            break;
                        case "vt": 
                            TextureCoords.Add(ParseVector2(tokens));
                            break;
                        case "f":
                            Faces.Add(ParseFace(tokens));
                            break;
                    }
                }
            }

            SetupModel();

        }

        private Vector3 ParseVector3(string[] tokens)
        {
            return new Vector3(
                float.Parse(tokens[1], CultureInfo.InvariantCulture),
                float.Parse(tokens[2], CultureInfo.InvariantCulture),
                float.Parse(tokens[3], CultureInfo.InvariantCulture)
            );
        }

        private Vector2 ParseVector2(string[] tokens)
        {
            return new Vector2(
                float.Parse(tokens[1], CultureInfo.InvariantCulture),
                float.Parse(tokens[2], CultureInfo.InvariantCulture)
            );
        }

        private ModelFace ParseFace(string[] tokens)
        {
            ModelFace face = new ModelFace();
            for (int i = 1; i < tokens.Length; i++)
            {
                string[] indices = tokens[i].Split('/');
                int vertexIndex = int.Parse(indices[0]) - 1;
                int textureIndex = indices.Length > 1 && indices[1] != "" ? int.Parse(indices[1]) - 1 : -1;
                int normalIndex = indices.Length > 2 ? int.Parse(indices[2]) - 1 : -1;

                face.Vertices.Add(new FaceVertex(vertexIndex, textureIndex, normalIndex));
            }
            return face;
        }

        private void SetupModel()
        {
            List<float> vertices = new List<float>();
            for (int i = 0; i < Faces.Count; i++)
            {
                ModelFace face = Faces[i];
                for (int j = 0; j < face.Vertices.Count; j++)
                {
                    FaceVertex faceVertex = face.Vertices[j];
                    vertices.Add(Vertices[faceVertex.VertexIndex].X);
                    vertices.Add(Vertices[faceVertex.VertexIndex].Y);
                    vertices.Add(Vertices[faceVertex.VertexIndex].Z);
                    vertices.Add(TextureCoords[faceVertex.TextureIndex].X);
                    vertices.Add(TextureCoords[faceVertex.TextureIndex].Y);
                    vertices.Add(Normals[faceVertex.NormalIndex].X);
                    vertices.Add(Normals[faceVertex.NormalIndex].Y);
                    vertices.Add(Normals[faceVertex.NormalIndex].Z);
                }
            }

            float[] vertices_array = vertices.ToArray();

            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices_array.Length * sizeof(float), vertices_array, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));


            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 5 * sizeof(float));

            GL.BindVertexArray(0);
        }
    }


    public class ModelFace
    {
        public List<FaceVertex> Vertices { get; } = new List<FaceVertex>();
    }
    public class FaceVertex
    {
        public int VertexIndex;
        public int TextureIndex;
        public int NormalIndex;

        public FaceVertex(int v, int t, int n)
        {
            VertexIndex = v;
            TextureIndex = t;
            NormalIndex = n;
        }
    }



}



