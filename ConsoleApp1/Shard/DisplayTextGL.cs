using SDL2;
using System;
using System.Collections.Generic;
using OpenTK;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


namespace Shard
{



    public class MySDLBindingsContext : IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
        {
            [DllImport("SDL2")]
            extern static IntPtr SDL_GL_GetProcAddress([MarshalAs(UnmanagedType.LPStr)] string procName);

            return SDL_GL_GetProcAddress(procName);
        }
    }

    class TextInfo
    {
        string text;
        float x, y, scale;
        Vector3 color;
        Vector2 dir;

        public TextInfo(string text, float x, float y, float scale, Vector3 color, Vector2 dir)
        {
            this.text = text;
            this.x = x;
            this.y = y;
            this.scale = scale;
            this.color = color;
            this.dir = dir;
        }

        public string Text { get { return text; } }
        public float X { get { return x; } }
        public float Y { get { return y; } }
        public float Scale { get { return scale; } }
        public Vector3 Color { get { return color; } }
        public Vector2 Dir { get { return dir; } }
    }

    class DisplayTextGL : Display
    {

        private IntPtr _window, _glContext;

        private Shader _shader_text, _shader_shape;

        private FreeTypeFont _font;

        private List<TextInfo> _textInfos;

        private int _vao, _vbo;


        public void swapBuffer()
        {
            SDL.SDL_GL_SwapWindow(_window);
        }


        public override void initialize()
        {
            //create window
            //bind
            //setSize(1280, 864);
            setSize(400, 300);

            SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING);
            SDL_ttf.TTF_Init();
            _window = SDL.SDL_CreateWindow("Shard Game Engine",
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                getWidth(),
                getHeight(),
                SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);


            _glContext = SDL.SDL_GL_CreateContext(_window);

            if (_glContext == IntPtr.Zero)
            {
                Console.WriteLine("Fail to create OpenGL context：" + SDL.SDL_GetError());
                SDL.SDL_DestroyWindow(_window);
                SDL.SDL_Quit();
                return;
            }

            GL.LoadBindings(new MySDLBindingsContext());

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(0, BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            _shader_text = new Shader("Shaders/text.vert", "Shaders/text.frag");
            _shader_shape = new Shader("Shaders/simple.vert", "Shaders/simple.frag");
            //_shader_text.Use();

            _font = new FreeTypeFont(32);

            _textInfos = new List<TextInfo>();

        }

        public override void clearDisplay()
        {
            _textInfos.Clear();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void resize()
        {
            int w, h;
            SDL.SDL_GetWindowSize(_window, out w, out h);
            setSize(w, h);
        }

        public override void showText(string text, double x, double y, int size, int r, int g, int b)
        {
            showText(text, (float)x, (float)y, size, new Vector3(r / 255.0f, g / 255.0f, b / 255.0f), new Vector2(1f, 0f));
        }

        public override void showText(char[,] text, double x, double y, int size, int r, int g, int b)
        {

        }

        public void showText(string text, float x, float y, float scale, Vector3 color, Vector2 dir)
        {
            _textInfos.Add(new TextInfo(text, x, y, scale, color, dir));
        }

        public override void display()
        {
            resize();
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Viewport(0, 0, getWidth(), getHeight());

            _shader_text.Use();

            Matrix4 projectionM = Matrix4.CreateOrthographicOffCenter(0.0f, getWidth(), getHeight(), 0.0f, -1.0f, 1.0f);
            GL.UniformMatrix4(1, false, ref projectionM);

            foreach (TextInfo info in _textInfos)
            {
                _font.RenderText(info.Text, info.X, info.Y, info.Scale, info.Color, info.Dir);
            }

            _shader_shape.Use();

            DrawRedTriangle();
            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            swapBuffer();

        }


        public void destroy()
        {
            SDL.SDL_GL_DeleteContext(_glContext);
            SDL.SDL_DestroyWindow(_window);
            SDL.SDL_Quit();
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

            
        }
    }
}
