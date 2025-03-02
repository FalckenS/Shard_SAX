using Shard.Shard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard
{
    internal class Sprite
    {
        public float X { get; set; }
        public float Y {  get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        private TextureRegion _textureRegion;
        // Constructors
        public Sprite(TextureRegion textureRegion, float x, float y, float width, float height) 
        {
            X = x;Y = y;Width = width;Height = height;_textureRegion = textureRegion;
        }
        public Sprite(TextureRegion textureRegion) : this(textureRegion,0,0,textureRegion.TextureWidth,textureRegion.TextureHeight) { }

        public Sprite(TextureRegion textureRegion, float x, float y) : this(textureRegion,x,y,textureRegion.RegionWidth,textureRegion.TextureHeight){ }
    
        public Sprite(Texture texture, float rx, float ry, float rwidth, float rheight, float x, float y, float width, float height) : 
            this(new TextureRegion(texture,rx,ry,rwidth,rheight),x,y,width,height){ }
        public Sprite(Texture texture, float x, float y) : this(texture,0,0,texture.Width,texture.Height,x,y,texture.Width,texture.Height){ }
        public Sprite(Texture texture, float x, float y, float width, float height) : this(texture,0,0,texture.Width,texture.Height,x,y,width,height){ }



    }
}
