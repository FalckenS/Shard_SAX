using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Shard.SAX.Graphics2D
{
    internal class TextureRegion
    {
        private Texture _texture;

        public readonly int RegionX;
        public readonly int RegionY;
        public readonly int RegionWidth;
        public readonly int RegionHeight;
        public byte[] ImageData { get { return _texture.ImageResult.Data; } }

        /// <summary>
        /// Cover the whole of the given texture by default.
        /// </summary>
        /// <param name="texture">Texture</param>
        public TextureRegion(Texture texture) : this(texture, 0, 0, texture.Width, texture.Height) { }
        
        public TextureRegion(Texture texture, int x, int y, int width, int height)
        {
            _texture = texture;
            RegionX = x;
            RegionY = y;
            RegionWidth = width;
            RegionHeight = height;
        }
        // I can do Animation<TextureRegion> = new AnimatedTexture(animationspritesheet, width)
        // Something like ...
        // runningAnimation =
        //new Animation<TextureRegion>(0.033f, atlas.findRegions("running"), PlayMode.LOOP);

        // Getters for texture width / height
        public int TextureWidth { get { return _texture.Width; } }
        public int TextureHeight { get { return _texture.Height; } }
        public void Use() { _texture.Use(TextureUnit.Texture0); }
    }
}
