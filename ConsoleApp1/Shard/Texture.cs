using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StbImageSharp;

namespace Shard
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
        public readonly float Width;
        /// <summary>
        /// Height of the texture.
        /// </summary>
        public readonly float Height;
        /// <summary>
        /// Absolute file path.
        /// </summary>
        public readonly string AbsoluteFilePath;
        /// <summary>
        /// ImageResult is initialized only if the file was read correctly.
        /// If IsAlive = false, then ImageResult is null.
        /// </summary>
        public readonly ImageResult ImageResult;
        /// <summary>
        /// Read texture from absolute file path. Use Bootstrap when specifying 
        /// paths relative to the assets folder.
        /// </summary>
        /// <param name="absoluteFilePath"></param>
        public Texture(String absoluteFilePath) 
        { 
            AbsoluteFilePath = absoluteFilePath;
            try
            {
                Stream stream = File.OpenRead(absoluteFilePath);
                ImageResult = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                Width = ImageResult.Width;
                Height = ImageResult.Height;
                IsAlive = true;
            }
            catch (Exception ex)
            {
                IsAlive = false;
                Debug.Log("Error loading texture at : " + absoluteFilePath + ". \" " + ex.Message + "\"");
            }
        }
    }
}
