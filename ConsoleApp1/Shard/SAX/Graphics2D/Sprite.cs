using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard.SAX.Graphics2D
{
    internal class Sprite
    {
        public float X { get; set; }
        public float Y {  get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public byte[] ImageData { get { return _textureRegion.ImageData; } }
        
        public int TextureWidth { get { return _textureRegion.TextureWidth; } }
        public int TextureHeight { get {  return _textureRegion.TextureHeight; } }
        public int RegionWidth { get { return _textureRegion.RegionWidth; } }
        public int RegionHeight { get { return _textureRegion.RegionHeight; } }
        public int RegionX { get { return _textureRegion.RegionX; } }
        public int RegionY { get {  return _textureRegion.RegionY; } }

        private TextureRegion _textureRegion;
        // Constructors
        public Sprite(TextureRegion textureRegion, float x, float y, float width, float height) 
        {
            X = x;Y = y;Width = width;Height = height; _textureRegion = textureRegion;
        }
        public Sprite(TextureRegion textureRegion) : this(textureRegion,0,0,textureRegion.TextureWidth,textureRegion.TextureHeight) { }

        public Sprite(TextureRegion textureRegion, float x, float y) : this(textureRegion,x,y,textureRegion.RegionWidth,textureRegion.TextureHeight){ }
    
        public Sprite(Texture texture, int rx, int ry, int rwidth, int rheight, float x, float y, float width, float height) : 
            this(new TextureRegion(texture,rx,ry,rwidth,rheight),x,y,width,height){ }
        public Sprite(Texture texture, float x, float y) : this(texture,0,0,texture.Width,texture.Height,x,y,texture.Width,texture.Height){ }
        public Sprite(Texture texture, float x, float y, float width, float height) : this(texture,0,0,texture.Width,texture.Height,x,y,width,height){ }
        public void SetTextureRegion(TextureRegion tr) { _textureRegion = tr; }
        public void Use() { _textureRegion.Use(); }
    }
}
