using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard
{
    // Can be used for future optimiziations of sprite rendering by batching.
    class SpriteBatch
    {
        public List<Sprite> Sprites { get; private set; }
        
        public void Draw(Sprite sprite) { Sprites.Add(sprite); }
        public void Draw(Sprite[] sprites) { Sprites.Concat(sprites); }
    }
}
