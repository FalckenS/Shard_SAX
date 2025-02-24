using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SharpFont;

namespace Shard;

public class FreeTypeFont
{
    private readonly Dictionary<uint, Character> _characters = new Dictionary<uint, Character>();
    int _vao;
    int _vbo;

    public FreeTypeFont(uint pixelheight)
    {
        // Initialize library
        Library lib = new Library();

        Face face = new Face(lib, "Resource/FreeSans.ttf");

        //Assembly assembly = Assembly.GetExecutingAssembly();
        //string[] names = assembly.GetManifestResourceNames();
        //Stream resource_stream = assembly.GetManifestResourceStream("Test.Resource.arial.ttf");
        //MemoryStream ms = new MemoryStream();
        //resource_stream.CopyTo(ms);
        //Face face = new Face(lib, ms.ToArray(), 0);

        face.SetPixelSizes(0, pixelheight);

        // Set 1 byte pixel alignment 
        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

        // Set texture unit
        GL.ActiveTexture(TextureUnit.Texture0);

        // Load first 128 characters of ASCII set
        for (uint c = 0; c < 128; c++)
        {
            try
            {
                // Load glyph
                // face.LoadGlyph(c, LoadFlags.Render, LoadTarget.Normal);
                face.LoadChar(c, LoadFlags.Render, LoadTarget.Normal);
                GlyphSlot glyph = face.Glyph;
                FTBitmap bitmap = glyph.Bitmap;

                // Create glyph texture
                int texObj = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texObj);
                GL.TexImage2D(TextureTarget.Texture2D, 0,
                              PixelInternalFormat.R8, bitmap.Width, bitmap.Rows, 0,
                              PixelFormat.Red, PixelType.UnsignedByte, bitmap.Buffer);

                // Set texture parameters
                GL.TextureParameter(texObj, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TextureParameter(texObj, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TextureParameter(texObj, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TextureParameter(texObj, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                // Add character
                Character ch = new Character();
                ch.TextureID = texObj;
                ch.Size = new Vector2(bitmap.Width, bitmap.Rows);
                ch.Bearing = new Vector2(glyph.BitmapLeft, glyph.BitmapTop);
                ch.Advance = glyph.Advance.X.Value;
                _characters.Add(c, ch);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        // Bind default texture
        GL.BindTexture(TextureTarget.Texture2D, 0);

        // Set default (4 byte) pixel alignment 
        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);

        float[] vquad =
        [ 
         // x      y       u     v    
            0.0f,  0.0f,   0.0f, 1.0f,  // Bottom-left
            0.0f,  1.0f,   0.0f, 0.0f,  // Top-left
            1.0f,  1.0f,   1.0f, 0.0f,  // Top-right
            0.0f,  0.0f,   0.0f, 1.0f,  // Bottom-left
            1.0f,  1.0f,   1.0f, 0.0f,  // Top-right
            1.0f,  0.0f,   1.0f, 1.0f   // Bottom-right
        ];

        // Create [Vertex Buffer Object](https://www.khronos.org/opengl/wiki/Vertex_Specification#Vertex_Buffer_Object)
        _vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, 4 * 6 * 4, vquad, BufferUsageHint.StaticDraw);

        // [Vertex Array Object](https://www.khronos.org/opengl/wiki/Vertex_Specification#Vertex_Array_Object)
        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * 4, 0);
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * 4, 2 * 4);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }

    public void RenderText(string text, float x, float y, float scale, Vector3 color)
    {
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindVertexArray(_vao);

        (float startX, float startY) = GetTextCenterPosition(text, x, y, scale);

        // Iterate through all characters
        float charX = 0.0f;
        foreach (char c in text)
        {
            if (!_characters.ContainsKey(c)) continue;

            Character ch = _characters[c];

            float xPos = startX + charX + ch.Bearing.X * scale;
            float yPos = startY + (ch.Size.Y - ch.Bearing.Y) * scale;

            float w = ch.Size.X * scale;
            float h = ch.Size.Y * scale;

            // Advance cursor for next character
            charX += (ch.Advance >> 6) * scale;

            // Create transformation matrices
            Matrix4 scaleM = Matrix4.CreateScale(new Vector3(w, h, 1.0f));
            Matrix4 transM = Matrix4.CreateTranslation(new Vector3(xPos, yPos, 0.0f));
            Matrix4 modelM = scaleM * transM; 

            GL.UniformMatrix4(0, false, ref modelM);
            GL.Uniform3(2, color);

            // Render glyph texture
            GL.BindTexture(TextureTarget.Texture2D, ch.TextureID);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }
        GL.BindVertexArray(0);
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    private (float, float) GetTextCenterPosition(string text, float x, float y, float scale)
    {
        float totalWidth = 0.0f;
        float maxHeight = 0.0f;

        foreach (char c in text)
        {
            if (!_characters.ContainsKey(c)) continue;

            Character ch = _characters[c];
            totalWidth += (ch.Advance >> 6) * scale;
            float charHeight = ch.Size.Y * scale;
            if (charHeight > maxHeight)
            {
                maxHeight = charHeight;
            }
        }
        // Adjust x, y to center
        float startX = x - totalWidth / 2.0f;
        float startY = y - maxHeight / 2.0f;
        return (startX, startY);
    }
    
    private struct Character
    {
        public int TextureID { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Bearing { get; set; }
        public int Advance { get; set; }
    }
}