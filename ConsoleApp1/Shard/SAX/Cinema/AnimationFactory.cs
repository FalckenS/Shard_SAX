using Shard.SAX.Graphics2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard.SAX.Cinema
{
    class AnimationFactory
    {
        private TextureSheet _textureSheet;
        private float _millisecondsBetweenKeyFrames;
        private PlayMode _playMode;
        public AnimationFactory(TextureSheet textureSheet, float millisecondsBetweenKeyFrames, PlayMode playmode) 
        {
            _textureSheet = textureSheet;
            _millisecondsBetweenKeyFrames = millisecondsBetweenKeyFrames;
            _playMode = playmode;
        }

        public AnimationFactory(TextureSheet textureSheet, float millisecondsBetweenKeyFrames) :
            this(textureSheet, millisecondsBetweenKeyFrames, PlayMode.FORWARD_LOOP){ }

        public Animation<TextureRegion> CreateAnimation(string name, int from, int to) 
        {
            List<TextureRegion> rl = new List<TextureRegion>();
            for (int i = from ; i < to ; i++)
            {
                try
                {
                    rl.Add(_textureSheet.TextureRegionsList[i]);
                } catch (Exception) { throw; }
            }
            return new Animation<TextureRegion>(name,rl,_millisecondsBetweenKeyFrames,_playMode);
        }

        public Animation<TextureRegion> CreateAnimation(string name, int fromCol, int fromRow, int toCol, int toRow) 
        {
            List<TextureRegion> rl = new List<TextureRegion>();
            for (int i = fromCol;  i < toCol ; i++) 
            { 
                for(int j = fromRow; i <toCol; i++) 
                {
                    try
                    {
                        rl.Add(_textureSheet.TextureRegions[i][j]);
                    } catch(Exception) { throw; }
                    
                }
            }
            return new Animation<TextureRegion>(name,rl,_millisecondsBetweenKeyFrames,_playMode);
        }

        public Animation<TextureRegion>[] GenerateAnimations(string name, int nrOfAnimations) 
        {
            int nrOfFramesPerAnimation = _textureSheet.TextureRegionsList.Count / nrOfAnimations;
            // Check if the value is correct.
            Animation<TextureRegion>[] animationArray = new Cinema.Animation<TextureRegion>[nrOfAnimations];
            
            if ((nrOfFramesPerAnimation * nrOfAnimations) == _textureSheet.TextureRegionsList.Count)
            {
                for (int i = 0; i < nrOfAnimations; i++) 
                {
                    List<TextureRegion> rl = new List<TextureRegion>();
                    for (int j = 0; j < nrOfFramesPerAnimation; j++) 
                    {
                        try 
                        {
                            rl.Add(_textureSheet.TextureRegionsList[(i * nrOfFramesPerAnimation) + j]);
                        } catch (Exception) { throw; }
                    }
                    animationArray[i] = new Animation<TextureRegion>(name + i,rl,_millisecondsBetweenKeyFrames,_playMode);
                }
                return animationArray;
            }
            else throw new ArgumentOutOfRangeException("Error when generating animation. " + nrOfAnimations 
                + " not divisible by " + _textureSheet.TextureRegionsList.Count + ".");
        }
    }
}
