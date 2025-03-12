using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard.SAX.Graphics2D
{
    class TextureLoaderFactory
    {
        public static TextureLoader get() 
        {
            return new GLTextureLoader();
            /*
            DisplayEngine current = Bootstrap.DisplayEngine;
            switch (current)
            {
                case DisplayEngine.OpenGL: return new GLTextureLoader();
                default:throw new NotSupportedException();

            }
            */
        }
    }
}
