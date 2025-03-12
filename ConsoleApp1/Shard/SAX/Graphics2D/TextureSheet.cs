using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard.SAX.Graphics2D
{
    // A collection of TextureRegions generated from a sheet texture.
    class TextureSheet
    {
        private Texture _sheet;
        private int _nrOfTexturesWide;
        private int _nrOfTexturesHigh;
        public readonly TextureRegion[][] TextureRegions;

        public List<TextureRegion> TextureRegionsList
        {
            get
            {
                List<TextureRegion> res = new List<TextureRegion>();
                foreach (TextureRegion[] ts in TextureRegions)
                {
                    res.AddRange(ts);
                }
                return res;
            }
        }

        public TextureSheet(Texture sheet, int nrOfTexturesWide, int nrOfTexturesHigh) 
        {
            TextureRegions = new TextureRegion[nrOfTexturesHigh][];
            for (int i = 0; i < nrOfTexturesHigh; i++) 
            {
                TextureRegions[i] = new TextureRegion[nrOfTexturesWide];
            }
            _nrOfTexturesWide = nrOfTexturesWide;
            _nrOfTexturesHigh = nrOfTexturesHigh;
            _sheet = sheet;
            float regionWidth = sheet.Width / _nrOfTexturesWide;
            float regionHeight = sheet.Height / _nrOfTexturesHigh;
            
            for ( int i = 0; i < nrOfTexturesHigh; i++ ) 
            {
                float ypos = (_sheet.Height - (regionHeight * i));
                for ( int j = 0; j < nrOfTexturesWide; j++) 
                {
                    float xpos = (float)((regionWidth * j));
                    TextureRegions[i][j] =
                   (new TextureRegion(sheet, (int)xpos, (int)ypos, (int)regionWidth, (int)regionHeight));
                }
            }
        }
    }
}
