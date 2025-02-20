using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard.Shard
{
    // A collection of TextureRegions generated from a sheet texture.
    class TextureSheet
    {
        private Texture _sheet;
        private int _nrOfTexturesWide;
        private int _nrOfTexturesHigh;
        public readonly TextureRegion[][] TextureRegions;

        public TextureSheet(Texture sheet, int nrOfTexturesWide, int nrOfTexturesHigh) 
        {
            _nrOfTexturesWide = nrOfTexturesWide;
            _nrOfTexturesHigh = nrOfTexturesHigh;
            _sheet = sheet;
            double regionWidth = sheet.Width / _nrOfTexturesWide;
            double regionHeight = sheet.Height / _nrOfTexturesHigh;
            
            for ( int i = 0; i < nrOfTexturesWide * nrOfTexturesHigh; i++ ) 
            {
                double xpos = (regionWidth * i) % sheet.Width;
                double ypos = _sheet.Height - (regionHeight * ((i+1) % nrOfTexturesWide));
                TextureRegions[i % (nrOfTexturesWide)][(int)Math.Floor(i / (double)nrOfTexturesWide)] = 
                    (new TextureRegion(sheet,xpos,ypos,regionWidth,regionHeight));
            }
        }
    }
}
