using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard.Shard
{
    class TextureLoaderFactory
    {
        public static TextureLoader get() 
        {
            DisplayEngine current = Bootstrap.DisplayEngine;
            switch (current)
            {
                case DisplayEngine.OpenGL: return new GLTextureLoader();
                default:throw new NotSupportedException();

            }
        }
    }
}
