namespace Shard;

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using SDL2;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;





class DisplayOpenGL3D : Display
{
    private const float RenderDownscale = 0.5f;
    private IntPtr _window, _glContext;
    private Shader _shaderText, _shaderShape, _shaderCube;
    private FreeTypeFont _font;
    private int _vao, _vbo;
    private int _vaoCube, _vboCube, _eboCube;
    private Matrix4 _model, _view, _projection;
    private Camera _camera;

    private Dictionary<string, Texture> _textureBuffer;
    private List<TextToRender> _textsToRender;
    private List<LineToRender> _linesToRender;
    private List<RectangleToRender> _rectanglesToRender;
    private List<CubeObject> _cubesToRender;

    private void prepareBaseCube()
    {
        //float[] _vertices =
        //{
        //    // Position         Texture coordinates
        //     0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
        //     0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
        //    -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
        //    -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
        //};

       float[] _vertices = {
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
        };

    uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        _vaoCube = GL.GenVertexArray();
        GL.BindVertexArray(_vaoCube);

        _vboCube = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCube);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

        _eboCube = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboCube);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

        _shaderCube = new Shader("Shaders/cube.vert", "Shaders/cube.frag");

        var vertexLocation = _shaderCube.GetAttribLocation("aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

        var texCoordLocation = _shaderCube.GetAttribLocation("aTexCoord");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

    }

    public override void initialize()
    {
        setSize(1000, 800);
        SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING);
        SDL_ttf.TTF_Init();
        _window = SDL.SDL_CreateWindow("Shard Game Engine",
            SDL.SDL_WINDOWPOS_CENTERED,
            SDL.SDL_WINDOWPOS_CENTERED,
            getWidth(),
            getHeight(),
            SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);

        //SDL.SDL_ShowCursor(SDL.SDL_DISABLE);
        SDL.SDL_SetRelativeMouseMode(SDL.SDL_bool.SDL_TRUE);


        _glContext = SDL.SDL_GL_CreateContext(_window);

        if (_glContext == IntPtr.Zero)
        {
            OpenGLContextFail();
            return;
        }

        GL.LoadBindings(new MySdlBindingsContext());
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(0, BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

        _shaderText = new Shader("Shaders/text.vert", "Shaders/text.frag");
        _shaderShape = new Shader("Shaders/simple.vert", "Shaders/simple.frag");
        _shaderCube = new Shader("Shaders/cube.vert", "Shaders/cube.frag");

        // prepareBaseCube();

        _font = new FreeTypeFont(32);

        _textsToRender = new List<TextToRender>();
        _linesToRender = new List<LineToRender>();
        _rectanglesToRender = new List<RectangleToRender>();
        _cubesToRender = new List<CubeObject>();
        _textureBuffer = new Dictionary<string, Texture>();
    }

    public override void LinkCamera(Camera camera)
    {
        _camera = camera;
    }

    public override void display()
    {
        Resize();
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.Viewport(0, 0, getWidth(), getHeight());

        _shaderShape.Use();
        
        foreach (LineToRender line in _linesToRender)
        {
            RenderLine(line);
        }
        foreach (RectangleToRender rectangle in _rectanglesToRender)
        {
            RenderRectangle(rectangle);
        }


        // 3D rendering
        _view = _camera.GetViewMatrix(); //Matrix4.CreateTranslation(0.0f, 0.0f, -10.0f);
        _projection = _camera.GetProjectionMatrix(); //Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), getWidth() / (float)getHeight(), 0.1f, 100.0f);

        foreach (CubeObject cube in _cubesToRender)
        {
            RenderCube(cube);
        }

        // Text rendering

        _shaderText.Use();

        Matrix4 projectionM = Matrix4.CreateOrthographicOffCenter(0.0f, getWidth(), getHeight(), 0.0f, -1.0f, 1.0f);
        GL.UniformMatrix4(1, false, ref projectionM);

        foreach (TextToRender text in _textsToRender)
        {
            _font.RenderText(text.Text, text.XPos, text.YPos, text.Size,
                new Vector3(text.R / 255.0f, text.G / 255.0f, text.B / 255.0f),
                new Vector2(1f, 0f)
            );
        }

        SwapBuffer();
    }

    public override void clearDisplay()
    {
        _textsToRender.Clear();
        _linesToRender.Clear();
        _rectanglesToRender.Clear();
        _cubesToRender.Clear();
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    public override void showText(char[,] text, double x, double y, int size, int r, int g, int b)
    {

    }

    public override void showText(string text, double x, double y, int size, int r, int g, int b)
    {
        _textsToRender.Add(new TextToRender(text, (float)x, (float)y, size, r, g, b));
    }

    public override void drawLine(int x, int y, int x2, int y2, Color col)
    {
        _linesToRender.Add(new LineToRender(x, y, x2, y2, col));
    }

    public override void drawRectangle(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, Color color)
    {
        _rectanglesToRender.Add(new RectangleToRender(x1, y1, x2, y2, x3, y3, x4, y4, color));
    }

    public override void addToDrawCube(CubeObject cube)
    {
        _cubesToRender.Add(cube);
        if (!_textureBuffer.ContainsKey(cube.Transform.SpritePath))
        {
            _textureBuffer[cube.Transform.SpritePath] = Texture.LoadFromFile(cube.Transform.SpritePath);
        }
    }

    private void SwapBuffer()
    {
        SDL.SDL_GL_SwapWindow(_window);
    }

    private void OpenGLContextFail()
    {
        Console.WriteLine("Fail to create OpenGL context：" + SDL.SDL_GetError());
        SDL.SDL_DestroyWindow(_window);
        SDL.SDL_Quit();
    }

    private void Resize()
    {
        SDL.SDL_GetWindowSize(_window, out var w, out var h);
        setSize(w, h);
    }

    private void Destroy()
    {
        SDL.SDL_GL_DeleteContext(_glContext);
        SDL.SDL_DestroyWindow(_window);
        SDL.SDL_Quit();
    }

    private void RenderLine(LineToRender lineToRender)
    {
        float[] vertices =
        [
            // Positions
            lineToRender.X1*RenderDownscale, lineToRender.Y1*RenderDownscale, 0.0f, 0, 0, 0, // Start point
            lineToRender.X2*RenderDownscale, lineToRender.Y2*RenderDownscale, 0.0f, 0, 0, 0  // End point
        ];

        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();

        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        // Define position attribute (Location = 0)
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        // Define color attribute (Location = 1)
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        // Use shader and draw the line
        _shaderShape.Use();
        GL.BindVertexArray(_vao);
        GL.DrawArrays(PrimitiveType.Lines, 0, 2);
    }

    private void RenderRectangle(RectangleToRender rectangleToRender)
    {
        float[] vertices =
        [
            // Positions                                      // Colors (Red)
            // Bottom-left
            rectangleToRender.X1*RenderDownscale, rectangleToRender.Y1*RenderDownscale, 0.0f, 0, 0, 0,
            // Bottom-right
            rectangleToRender.X2*RenderDownscale, rectangleToRender.Y2*RenderDownscale, 0.0f, 0, 0, 0,
            // Top-right
            rectangleToRender.X3*RenderDownscale, rectangleToRender.Y3*RenderDownscale, 0.0f, 0, 0, 0,
            // Top-left
            rectangleToRender.X4*RenderDownscale, rectangleToRender.Y4*RenderDownscale, 0.0f, 0, 0, 0
        ];

        uint[] indices =
        {
            0, 1, 2,  // First Triangle
            2, 3, 0   // Second Triangle
        };

        // Generate and bind VAO
        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);

        // Generate and bind VBO
        _vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        // Generate and bind EBO
        int ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        // Define vertex attributes
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        // Unbind buffers
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        // Use shader and draw the rectangle
        _shaderShape.Use();
        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    private void RenderCube(CubeObject cube)
    {
        float w, h, d;
        w = cube.Transform.Scalex;
        h = cube.Transform.Scaley;
        d = cube.Transform.Scalez;

        float[] _vertices = {
            -w/2, -h/2, -d/2,  0.0f, 0.0f,
             w/2, -h/2, -d/2,  w/2,    0.0f,
             w/2,  h/2, -d/2,  w/2,    h/2,
             w/2,  h/2, -d/2,  w/2,    h/2,
            -w/2,  h/2, -d/2,  0.0f, h/2,
            -w/2, -h/2, -d/2,  0.0f, 0.0f,

            -w/2, -h/2,  d/2,  0.0f, 0.0f,
             w/2, -h/2,  d/2,  w/2,    0.0f,
             w/2,  h/2,  d/2,  w/2,    h/2,
             w/2,  h/2,  d/2,  w/2,    h/2,
            -w/2,  h/2,  d/2,  0.0f, h/2,
            -w/2, -h/2,  d/2,  0.0f, 0.0f,

            -w/2,  h/2,  d/2,  0.0f, h/2, //d,    0.0f,
            -w/2,  h/2, -d/2,  d/2,    h/2,
            -w/2, -h/2, -d/2,  d/2,    0.0f, //0.0f, h,
            -w/2, -h/2, -d/2,  d/2,    0.0f, //0.0f, h,
            -w/2, -h/2,  d/2,  0.0f, 0.0f,
            -w/2,  h/2,  d/2,  0.0f, h/2, //d,    0.0f,

             w/2,  h/2,  d/2,  0.0f, h/2, //d/2,    0.0f,
             w/2,  h/2, -d/2,  d/2,    h/2,
             w/2, -h/2, -d/2,  d/2,    0.0f, //0.0f, h,
             w/2, -h/2, -d/2,  d/2,    0.0f, //0.0f, h,
             w/2, -h/2,  d/2,  0.0f, 0.0f,
             w/2,  h/2,  d/2,  0.0f, h/2, //d,    0.0f,

            -w/2, -h/2, -d/2,  0.0f, d/2,
             w/2, -h/2, -d/2,  w/2,    d/2,
             w/2, -h/2,  d/2,  w/2,    0.0f,
             w/2, -h/2,  d/2,  w/2,    0.0f,
            -w/2, -h/2,  d/2,  0.0f, 0.0f,
            -w/2, -h/2, -d/2,  0.0f, d/2,

            -w/2,  h/2, -d/2,  0.0f, d/2,
             w/2,  h/2, -d/2,  w/2,    d/2,
             w/2,  h/2,  d/2,  w/2,    0.0f,
             w/2,  h/2,  d/2,  w/2,    0.0f,
            -w/2,  h/2,  d/2,  0.0f, 0.0f,
            -w/2,  h/2, -d/2,  0.0f, d/2
        };

        uint[] _indices =
            {
            0, 1, 3,
            1, 2, 3
        };

        _vaoCube = GL.GenVertexArray();
        GL.BindVertexArray(_vaoCube);

        _vboCube = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCube);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

        _eboCube = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboCube);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

        var vertexLocation = _shaderCube.GetAttribLocation("aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

        var texCoordLocation = _shaderCube.GetAttribLocation("aTexCoord");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        


        _shaderCube.Use();
        //GL.BindVertexArray(_vaoCube);
        _model = cube.calcModel();
        _shaderCube.SetMatrix4("model", _model);
        _shaderCube.SetMatrix4("view", _view);
        _shaderCube.SetMatrix4("projection", _projection);

        _shaderCube.SetInt("texture0", 0);
        Texture tex = _textureBuffer[cube.Transform.SpritePath];
        tex.Use(TextureUnit.Texture0);

        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    private class MySdlBindingsContext : IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
        {
            [DllImport("SDL2")]
            static extern IntPtr SDL_GL_GetProcAddress([MarshalAs(UnmanagedType.LPStr)] string procName);
            return SDL_GL_GetProcAddress(procName);
        }
    }

    private class LineToRender(float x1, float y1, float x2, float y2, Color color)
    {
        public float X1 { get; } = x1;
        public float Y1 { get; } = y1;
        public float X2 { get; } = x2;
        public float Y2 { get; } = y2;
        public Color Color { get; } = color;
    }
}

//internal class TextToRender(string text, float xPos, float yPos, int size, int r, int g, int b)
//{
//    public string Text { get; } = text;
//    public float XPos { get; } = xPos;
//    public float YPos { get; } = yPos;
//    public int Size { get; } = size;
//    public int R { get; } = r;
//    public int G { get; } = g;
//    public int B { get; } = b;
//}

//public class RectangleToRender(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, Color color)
//{
//    public float X1 { get; } = x1;
//    public float Y1 { get; } = y1;
//    public float X2 { get; } = x2;
//    public float Y2 { get; } = y2;
//    public float X3 { get; } = x3;
//    public float Y3 { get; } = y3;
//    public float X4 { get; } = x4;
//    public float Y4 { get; } = y4;
//    public Color Color { get; } = color;
//}