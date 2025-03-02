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
        private List<Sprite> _sprites;
        
        public void Draw(Sprite sprite) { _sprites.Add(sprite); }
        public void Draw(Sprite[] sprites) { _sprites.Concat(sprites); }
    }
}
