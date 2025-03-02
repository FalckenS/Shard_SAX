using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Shard.Shard
{
    internal class TextureRegion
    {
        private Texture _texture;

        public readonly float RegionX;
        public readonly float RegionY;
        public readonly float RegionWidth;
        public readonly float RegionHeight;

        /// <summary>
        /// Cover the whole of the given texture by default.
        /// </summary>
        /// <param name="texture">Texture</param>
        public TextureRegion(Texture texture) : this(texture, 0, 0, texture.Width, texture.Height) { }
        
        public TextureRegion(Texture texture, float x, float y, float width, float height)
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
        public float TextureWidth { get { return _texture.Width; } }
        public float TextureHeight { get { return _texture.Height; } }


    }
}
