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

        private double _regionX;
        private double _regionY;
        private double _regionWidth;
        private double _regionHeight;

        /// <summary>
        /// Cover the whole of the given texture by default.
        /// </summary>
        /// <param name="texture">Texture</param>
        public TextureRegion(Texture texture) : this(texture, 0, 0, texture.Width, texture.Height) { }
        
        public TextureRegion(Texture texture, double x, double y, double width, double height)
        {
            _texture = texture;
            _regionX = x;
            _regionY = y;
            _regionWidth = width;
            _regionHeight = height;
        }
        // I can do Animation<TextureRegion> = new AnimatedTexture(animationspritesheet, width)
        // Something like ...
        // runningAnimation =
        //new Animation<TextureRegion>(0.033f, atlas.findRegions("running"), PlayMode.LOOP);
        
        


    }
}
