namespace Shard;

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using SDL2;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

class DisplayOpenGL : Display
{
    private const float RenderDownscale = 0.5f;
    private IntPtr _window, _glContext;
    private Shader _shaderText, _shaderShape;
    private FreeTypeFont _font;
    private int _vao, _vbo;
    
    private List<TextToRender> _textsToRender;
    private List<LineToRender> _linesToRender;
    private List<RectangleToRender> _rectanglesToRender;
    
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
        _glContext = SDL.SDL_GL_CreateContext(_window);

        if (_glContext == IntPtr.Zero)
        {
            OpenGLContextFail();
            return;
        }

        GL.LoadBindings(new MySdlBindingsContext());
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
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
        foreach (LineToRender line in _linesToRender)
        {
            RenderLine(line);
        }
        foreach (RectangleToRender rectangle in _rectanglesToRender)
        {
            RenderRectangle(rectangle);
        }
        SwapBuffer();
    }
    
    public override void clearDisplay()
    {
        _textsToRender.Clear();
        _linesToRender.Clear();
        _rectanglesToRender.Clear();
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
    
    private void RenderLine(LineToRender lineInfo)
    {
        float rCol = lineInfo.Color.R;
        float gCol = lineInfo.Color.G;
        float bCol = lineInfo.Color.B;
        float[] vertices =
        [
            // Start point
            lineInfo.X1*RenderDownscale, lineInfo.Y1*RenderDownscale, 0, rCol, gCol, bCol,
            // End point
            lineInfo.X2*RenderDownscale, lineInfo.Y2*RenderDownscale, 0, rCol, gCol, bCol
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

    private void RenderRectangle(RectangleToRender rectangleInfo)
    {
        float rCol = rectangleInfo.Color.R;
        float gCol = rectangleInfo.Color.G;
        float bCol = rectangleInfo.Color.B;
        float[] vertices =
        [
            // Bottom-left
            rectangleInfo.X1*RenderDownscale, rectangleInfo.Y1*RenderDownscale, 0, rCol, gCol, bCol,
            // Bottom-right
            rectangleInfo.X2*RenderDownscale, rectangleInfo.Y2*RenderDownscale, 0, rCol, gCol, bCol,
            // Top-right
            rectangleInfo.X3*RenderDownscale, rectangleInfo.Y3*RenderDownscale, 0, rCol, gCol, bCol,
            // Top-left
            rectangleInfo.X4*RenderDownscale, rectangleInfo.Y4*RenderDownscale, 0, rCol, gCol, bCol
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

internal class TextToRender(string text, float xPos, float yPos, int size, int r, int g, int b)
{
    public string Text { get; } = text;
    public float XPos { get; } = xPos;
    public float YPos { get; } = yPos;
    public int Size { get; } = size;
    public int R { get; } = r;
    public int G { get; } = g;
    public int B { get; } = b;
}

public class RectangleToRender(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, Color color)
{
    public float X1 { get; } = x1;
    public float Y1 { get; } = y1;
    public float X2 { get; } = x2;
    public float Y2 { get; } = y2;
    public float X3 { get; } = x3;
    public float Y3 { get; } = y3;
    public float X4 { get; } = x4;
    public float Y4 { get; } = y4;
    public Color Color { get; } = color;
}