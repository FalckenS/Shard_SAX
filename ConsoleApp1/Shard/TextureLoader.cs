using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard
{
    interface TextureLoader
    {
        void Load(byte[] imgdata, int width, int height);
        void Use(TextureUnit textureUnit);
    }
}
