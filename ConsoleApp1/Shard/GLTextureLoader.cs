using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using StbImageSharp;
using System.IO;
using System;

namespace Shard
{
    /// <summary>
    /// Texture loader for the OpenGL state machine. For use within GL context.
    /// </summary>
    public class GLTextureLoader : TextureLoader
    {
        /// <summary>
        /// GL texture object handle
        /// </summary>
        private int _handle;
        /// <summary>
        /// Default texture loading using OpenGL.
        /// </summary>
        /// <param name="imageData">image bye data.</param>
        /// <param name="width">image width, in pixels.</param>
        /// <param name="height">image height, in pixels.</param>
        public void Load(byte[] imageData, int width, int height)
        {
            _handle = GL.GenTexture();
            //GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _handle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, imageData);
            // Nearest Filter
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            // Wrap mode : Repeat
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            // GL mipmaps
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            // Unbind tex
            GL.BindTexture(TextureTarget.Texture2D, 0);

        }
        /// <summary>
        /// Binds gl texture.
        /// </summary>
        public void Use(TextureUnit textureUnit)
        {
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, _handle);
        }
    }
}
