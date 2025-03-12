using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace Shard.SAX.Graphics2D
{
    /// <summary>
    /// A loaded texture represented by an ImageResult object (StbImageSharp).
    /// Loads the texture in the constructor. If you want to use a Texture without loading
    /// it, use its path and handle loading yourself.
    /// </summary>
    class Texture
    {
        /// <summary>
        /// If the texture loaded without exceptions, it is alive, if exceptions occured,
        /// it is not.
        /// </summary>
        public readonly bool IsAlive;
        /// <summary>
        /// Width of the texture.
        /// </summary>
        public readonly int Width;
        /// <summary>
        /// Height of the texture.
        /// </summary>
        public readonly int Height;
        /// <summary>
        /// Absolute file path.
        /// </summary>
        public readonly string AbsoluteFilePath;
        /// <summary>
        /// ImageResult is initialized only if the file was read correctly.
        /// If IsAlive = false, then ImageResult is null.
        /// </summary>
        public readonly ImageResult ImageResult;
        private TextureLoader _textureLoader;
        /// <summary>
        /// Read texture from absolute file path. Use Bootstrap when specifying 
        /// paths relative to the assets folder.
        /// </summary>
        /// <param name="absoluteFilePath"></param>
        public Texture(String absoluteFilePath)
        {
            AbsoluteFilePath = absoluteFilePath;
            // Read image data the other way around. Prevents upside down images.
            StbImage.stbi_set_flip_vertically_on_load(1);
            try
            {
                Stream stream = File.OpenRead(absoluteFilePath);
                ImageResult = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                Width = ImageResult.Width;
                Height = ImageResult.Height;
                bool success = false;
                try
                {
                    _textureLoader = TextureLoaderFactory.get();
                    _textureLoader.Load(ImageResult.Data, Width, Height);
                    success = true;
                }
                catch (Exception ex) { success = false; Debug.Log("Error loading texture data. \"" + ex.Message + "\""); }
                if (success) { IsAlive = true; } else { IsAlive = false; }

            }
            catch (Exception ex)
            {
                IsAlive = false;
                Debug.Log("Error loading texture file at : " + absoluteFilePath + ". \" " + ex.Message + "\"");
            }
        }
        public void Use(TextureUnit texUnit) 
        { 
            _textureLoader.Use(texUnit);
        }
    }
}
