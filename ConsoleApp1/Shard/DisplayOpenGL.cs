using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Shard
{
    class DisplayOpenGL : Display
    {
        private OpenGLWindow _openGLWindow;
        private List<GameObject> _gameObjects;

        public override void initialize()
        {
            NativeWindowSettings settings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
                Title = "OpenGL Single-Threaded Game Loop"
            };
            _openGLWindow = new OpenGLWindow(settings);
            _openGLWindow.MakeCurrent();
            
            _gameObjects = new List<GameObject>();
        }
        
        public override void clearDisplay()
        {
            _openGLWindow.Clear();
        }
        
        public override void display()
        {
            if (_gameObjects.Count > 0)
            {
                _openGLWindow.DrawRedTriangle();
            }
            _openGLWindow.Render();
            _openGLWindow.ProcessEvents(0.0);
        }

        public override void showText(string text, double x, double y, int size, int r, int g, int b)
        {
        }

        public override void showText(char[,] text, double x, double y, int size, int r, int g, int b)
        {
        }
        
        public override void addToDraw(GameObject gameObject)
        {
            _gameObjects.Add(gameObject);
        }
    }

    class OpenGLWindow : NativeWindow
    {
        private int _vbo;
        private int _vao;
        private int _shaderProgram;

        public OpenGLWindow(NativeWindowSettings settings) : base(settings)
        {
            MakeCurrent();
            GL.LoadBindings(new GLFWBindingsContext());
            DrawBlueTriangle();
        }
        
        private void DrawBlueTriangle()
        {
            float[] vertices =
            [
                0.0f,  0.5f,  0.0f,
                0.5f, -0.5f,  0.0f,
                -0.5f, -0.5f,  0.0f
            ];

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            string vertexShaderSource = @"#version 330 core
            layout(location = 0) in vec3 aPos;
            void main() { gl_Position = vec4(aPos, 1.0); }";

            string fragmentShaderSource = @"#version 330 core
            out vec4 FragColor;
            void main() { FragColor = vec4(0.0, 0.0, 1.0, 1.0); }";

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);
            CheckShaderCompile(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);
            CheckShaderCompile(fragmentShader);

            _shaderProgram = GL.CreateProgram();
            GL.AttachShader(_shaderProgram, vertexShader);
            GL.AttachShader(_shaderProgram, fragmentShader);
            GL.LinkProgram(_shaderProgram);
            CheckProgramLink(_shaderProgram);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public void DrawRedTriangle()
        {
            float[] vertices =
            [
                0.0f,  0.5f,  0.0f,
                0.5f, -0.5f,  0.0f,
                -0.5f, -0.5f,  0.0f
            ];

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            string vertexShaderSource = @"#version 330 core
            layout(location = 0) in vec3 aPos;
            void main() { gl_Position = vec4(aPos, 1.0); }";

            string fragmentShaderSource = @"#version 330 core
            out vec4 FragColor;
            void main() { FragColor = vec4(1.0, 0.0, 0.0, 1.0); }";

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);
            CheckShaderCompile(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);
            CheckShaderCompile(fragmentShader);

            _shaderProgram = GL.CreateProgram();
            GL.AttachShader(_shaderProgram, vertexShader);
            GL.AttachShader(_shaderProgram, fragmentShader);
            GL.LinkProgram(_shaderProgram);
            CheckProgramLink(_shaderProgram);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        private void CheckShaderCompile(int shader)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Shader compilation error: {infoLog}");
            }
        }

        private void CheckProgramLink(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                throw new Exception($"Program linking error: {infoLog}");
            }
        }

        public void Clear()
        {
            GL.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        public void Render()
        {
            GL.UseProgram(_shaderProgram);
            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            Context.SwapBuffers();
        }

        public void Cleanup()
        {
            GL.DeleteBuffer(_vbo);
            GL.DeleteVertexArray(_vao);
            GL.DeleteProgram(_shaderProgram);
        }
    }
}