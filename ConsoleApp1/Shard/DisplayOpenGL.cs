using System.Drawing;

namespace Shard;

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using SDL2;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class MySdlBindingsContext : IBindingsContext
{
    public IntPtr GetProcAddress(string procName)
    {
        [DllImport("SDL2")]
        static extern IntPtr SDL_GL_GetProcAddress([MarshalAs(UnmanagedType.LPStr)] string procName);
        return SDL_GL_GetProcAddress(procName);
    }
}

public class OpenGLLine(float x1, float x2, float y1, float y2, Color color)
{
    private float _x1 = x1/2, _x2 = x2/2, _y1 = y1/2, _y2 = y2/2;
    private Color _color = color;

    public float GetX1()
    {
        return _x1;
    }
    public float GetX2()
    {
        return _x2;
    }
    public float GetY1()
    {
        return _y1;
    }
    public float GetY2()
    {
        return _y2;
    }
    public Color GetColor()
    {
        return _color;
    }
}

class DisplayOpenGL : Display
{
    private IntPtr _window, _glContext;
    private Shader _shaderText, _shaderShape;
    private FreeTypeFont _font;
    private int _vao, _vbo;
    
    private List<TextToDisplay> _textsToDisplay;
    private List<OpenGLLine> _linesToDisplay;
    
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

        _textsToDisplay = new List<TextToDisplay>();
        _linesToDisplay = new List<OpenGLLine>();
    }

    public override void display()
    {
        Resize();
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.Viewport(0, 0, getWidth(), getHeight());

        _shaderText.Use();

        Matrix4 projectionM = Matrix4.CreateOrthographicOffCenter(0.0f, getWidth(), getHeight(), 0.0f, -1.0f, 1.0f);
        GL.UniformMatrix4(1, false, ref projectionM);
        
        foreach (TextToDisplay text in _textsToDisplay)
        {
            _font.RenderText(text.Text, text.XPos, text.YPos, text.Size,
                new Vector3(text.R / 255.0f, text.G / 255.0f, text.B / 255.0f),
                new Vector2(1f, 0f)
            );
        }
        foreach (OpenGLLine line in _linesToDisplay)
        {
            RenderLine(line);
        }
        
        SwapBuffer();
    }
    
    public override void clearDisplay()
    {
        _textsToDisplay.Clear();
        _linesToDisplay.Clear();
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }
    
    public override void showText(string text, double x, double y, int size, int r, int g, int b)
    {
        _textsToDisplay.Add(new TextToDisplay(text, (float)x, (float)y, size, r, g, b));
    }

    public override void showText(char[,] text, double x, double y, int size, int r, int g, int b)
    {

    }

    public override void drawLine(int x, int y, int x2, int y2, Color col)
    {
        _linesToDisplay.Add(new OpenGLLine(x, x2, y, y2, col));
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
    
    private void RenderLine(OpenGLLine line)
    {
        float[] vertices =
        {
            // Positions                      // Color (Red)
            line.GetX1(), line.GetY1(), 0.0f, (float)line.GetColor().R, (float)line.GetColor().G, (float)line.GetColor().B, // Start point
            line.GetX2(), line.GetY2(), 0.0f, (float)line.GetColor().R, (float)line.GetColor().G, (float)line.GetColor().B  // End point
        };

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

    private void DrawRedRectangle()
    {
        float[] vertices =
        {
            // Positions      // Colors (Red)
            -0.5f, -0.5f, 0.0f,   1.0f, 0.0f, 0.0f, // Bottom-left
            0.5f, -0.5f, 0.0f,   1.0f, 0.0f, 0.0f, // Bottom-right
            0.5f,  0.5f, 0.0f,   1.0f, 0.0f, 0.0f, // Top-right
            -0.5f,  0.5f, 0.0f,   1.0f, 0.0f, 0.0f  // Top-left
        };

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
}

class TextToDisplay(string text, float xPos, float yPos, int size, int r, int g, int b)
{
    public string Text { get; } = text;
    public float XPos { get; } = xPos;
    public float YPos { get; } = yPos;
    public int Size { get; } = size;
    public int R { get; } = r;
    public int G { get; } = g;
    public int B { get; } = b;
}