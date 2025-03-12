using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard.SAX.Graphics2D
{
    // Can be used for future optimiziations of sprite rendering by batching.
    class SpriteBatch
    {
        private List<Sprite> _sprites;
        public SpriteBatch() { _sprites = new List<Sprite>(); }
        public void Draw(Sprite sprite) { _sprites.Add(sprite); }
        public void Draw(Sprite[] sprites) { _sprites.Concat(sprites); }
        public void UnDraw(Sprite sprite) { _sprites.Remove(sprite); }
        public void Clear() { _sprites.Clear(); }
        public Sprite[] GetCurrent() { return _sprites.ToArray(); }
    }
}
