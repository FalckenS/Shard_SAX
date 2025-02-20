namespace Shard;

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using SDL2;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

internal class DisplayOpenGL : Display
{
    private IntPtr _window, _glContext;
    private Shader _shaderText, _shaderShape;
    private FreeTypeFont _font;
    private int _vao, _vbo;
    
    private List<TextToRender> _textsToRender;
    private List<LineToRender> _linesToRender;
    private List<RectangleToRender> _rectanglesToRender;
    
    public override void initialize()
    {
        setSize(800, 800);
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

        GL.LoadBindings(new MySdlBindingsContext());
        GL.ClearColor(0f, 0f, 0f, 1.0f);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(0, BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

        _shaderText = new Shader("Shaders/text.vert", "Shaders/text.frag");
        _shaderShape = new Shader("Shaders/simple.vert", "Shaders/simple.frag");

        _font = new FreeTypeFont(32);

        _textsToRender = new List<TextToRender>();
        _linesToRender = new List<LineToRender>();
        _rectanglesToRender = new List<RectangleToRender>();
    }

    public override void display()
    {
        Resize();
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.Viewport(0, 0, getWidth(), getHeight());
        
        // Create the projection matrix
        Matrix4 projectionM = Matrix4.CreateOrthographicOffCenter(
            -getWidth() / 2f,
            getWidth() / 2f,
            -getHeight() / 2f,
            getHeight() / 2f,
            -1.0f, 1.0f
        );

        // Set projection for text shader
        _shaderText.Use();
        int textProjectionLocation = GL.GetUniformLocation(_shaderText.Program, "projection");
        if (textProjectionLocation != -1)
        {
            GL.UniformMatrix4(textProjectionLocation, false, ref projectionM);
        }
        else
        {
            Console.WriteLine("Warning: 'projection' uniform not found in text shader.");
        }

        // Set projection for shape shader
        _shaderShape.Use();
        int shapeProjectionLocation = GL.GetUniformLocation(_shaderShape.Program, "projection");
        if (shapeProjectionLocation != -1)
        {
            GL.UniformMatrix4(shapeProjectionLocation, false, ref projectionM);
        }
        else
        {
            Console.WriteLine("Warning: 'projection' uniform not found in shape shader.");
        }
        
        _shaderText.Use();
        foreach (LineToRender line in _linesToRender)
        {
            RenderLine(line);
        }
        
        _shaderShape.Use();
        foreach (RectangleToRender rectangle in _rectanglesToRender)
        {
            RenderRectangle(rectangle);
        }
        foreach (TextToRender text in _textsToRender)
        {
            _font.RenderText(text.Text, text.XPos, text.YPos, text.Scale,
                new Vector3(text.RCol / 255.0f, text.GCol / 255.0f, text.BCol / 255.0f),
                new Vector2(1f, 0f)
            );
        }
        
        SDL.SDL_GL_SwapWindow(_window);
    }
    
    public override void clearDisplay()
    {
        _textsToRender.Clear();
        _linesToRender.Clear();
        _rectanglesToRender.Clear();
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }
    
    public override void showText(char[,] text, double xPos, double yPos, int size, int rCol, int gCol, int bCol)
    {
        throw new NotImplementedException();
    }
    
    public override void showText(string text, double xPos, double yPos, int scale, int rCol, int gCol, int bCol)
    {
        _textsToRender.Add(new TextToRender(text, (float)xPos, (float)yPos, scale, rCol, gCol, bCol));
    }

    public override void drawLine(int xPos, int yPos, int x2Pos, int y2Pos, Color color)
    {
        _linesToRender.Add(new LineToRender(xPos, yPos, x2Pos, y2Pos, color));
    }

    public override void drawRectangle(float x1Pos, float y1Pos, float x2Pos, float y2Pos, float x3Pos, float y3Pos, float x4Pos, float y4Pos, Color color)
    {
        _rectanglesToRender.Add(new RectangleToRender(x1Pos, y1Pos, x2Pos, y2Pos, x3Pos, y3Pos, x4Pos, y4Pos, color));
    }

    private void Resize()
    {
        SDL.SDL_GetWindowSize(_window, out int w, out int h);
        setSize(w, h);
    }
    
    private void RenderLine(LineToRender lineInfo)
    {
        float rCol = lineInfo.Color.R;
        float gCol = lineInfo.Color.G;
        float bCol = lineInfo.Color.B;
        float[] vertices =
        [
            // Start point
            lineInfo.XPos1, lineInfo.YPos1, 0, rCol, gCol, bCol,
            // End point
            lineInfo.XPos2, lineInfo.YPos2, 0, rCol, gCol, bCol
        ];

        // Generate and bind VAO, VBO
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

        // Unbind buffers
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        // Use shader and render the line
        _shaderShape.Use();
        GL.BindVertexArray(_vao);
        GL.DrawArrays(PrimitiveType.Lines, 0, 2);
    }

    private void RenderRectangle(RectangleToRender rectangleInfo)
    {
        float rCol = rectangleInfo.Color.R;
        float gCol = rectangleInfo.Color.G;
        float bCol = rectangleInfo.Color.B;
        float[] vertices =
        [
            // Bottom-left
            rectangleInfo.XPos1, rectangleInfo.YPos1, 0, rCol, gCol, bCol,
            // Bottom-right
            rectangleInfo.XPos2, rectangleInfo.YPos2, 0, rCol, gCol, bCol,
            // Top-right
            rectangleInfo.XPos3, rectangleInfo.YPos3, 0, rCol, gCol, bCol,
            // Top-left
            rectangleInfo.XPos4, rectangleInfo.YPos4, 0, rCol, gCol, bCol
        ];

        uint[] indices =
        [
            0, 1, 2,  // First Triangle
            2, 3, 0   // Second Triangle
        ];

        // Generate and bind VAO, VBO
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        // Generate and bind EBO
        int ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        // Define position attribute (Location = 0)
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        
        // Define color attribute (Location = 1)
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        // Unbind buffers
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        // Use shader and render the rectangle
        _shaderShape.Use();
        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    private class MySdlBindingsContext : IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
        {
            return SDL_GL_GetProcAddress(procName);

            [DllImport("SDL2")]
            static extern IntPtr SDL_GL_GetProcAddress([MarshalAs(UnmanagedType.LPStr)] string procName);
        }
    }

    private class LineToRender(float xPos1, float yPos1, float xPos2, float yPos2, Color color)
    {
        public float XPos1 { get; } = xPos1;
        public float YPos1 { get; } = yPos1;
        public float XPos2 { get; } = xPos2;
        public float YPos2 { get; } = yPos2;
        public Color Color { get; } = color;
    }
}

internal class TextToRender(string text, float xPos, float yPos, int scale, int rCol, int gCol, int bCol)
{
    public string Text { get; } = text;
    public float XPos { get; } = xPos;
    public float YPos { get; } = yPos;
    public int Scale { get; } = scale;
    public int RCol { get; } = rCol;
    public int GCol { get; } = gCol;
    public int BCol { get; } = bCol;
}

internal class RectangleToRender(float xPos1, float yPos1, float xPos2, float yPos2, float xPos3, float yPos3, float xPos4, float yPos4, Color color)
{
    public float XPos1 { get; } = xPos1;
    public float YPos1 { get; } = yPos1;
    public float XPos2 { get; } = xPos2;
    public float YPos2 { get; } = yPos2;
    public float XPos3 { get; } = xPos3;
    public float YPos3 { get; } = yPos3;
    public float XPos4 { get; } = xPos4;
    public float YPos4 { get; } = yPos4;
    public Color Color { get; } = color;
}