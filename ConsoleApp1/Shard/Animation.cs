using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard.Shard
{
    internal class Animation<T>
    {
        private T _last;
        private float _lastTimeSeconds;
        private float _secondsSinceStart = 0;
        
        public bool IsPaused { get; private set; }
        public int KeyFramesPerSecond {  get; set; }
        private List<T> _keyFrames;


        public Animation(List<T> keyFrames)
        {
            _keyFrames = keyFrames;
        }
        public Animation(T[] keyFrames)
        {
            _keyFrames = [.. keyFrames];
        }

        public T GetKeyFrame(float currentTimeSeconds,PlayMode playMode)
        {
            if (IsPaused) { return _last; };
            float deltaTime = currentTimeSeconds - _lastTimeSeconds;
            _lastTimeSeconds = currentTimeSeconds;
            _secondsSinceStart += deltaTime;

            bool hasLooped = false;
            if (_secondsSinceStart + deltaTime >= (_keyFrames.Count * (1 / (float)KeyFramesPerSecond))) hasLooped = true;
            
            switch (playMode)
            {
                case PlayMode.FORWARD_ONCE:
                    if (hasLooped) { return setLastAndReturn(_keyFrames.Last()); }
                    else return setLastAndReturn(_keyFrames[ForwardIndex]);
                case PlayMode.FORWARD_LOOP:
                    return setLastAndReturn(_keyFrames[ForwardIndex]);
                case PlayMode.REVERSED_ONCE:
                    if (hasLooped) { return setLastAndReturn(_keyFrames.First()); }
                    else return setLastAndReturn(_keyFrames[BackwardIndex]);
                case PlayMode.REVERSED_LOOP:
                    return setLastAndReturn(_keyFrames[BackwardIndex]);
                case PlayMode.PINGPONG_ONCE:
                    if (hasLooped) { return setLastAndReturn(_keyFrames.First()); } 
                    else { return setLastAndReturn(_keyFrames[PingPongIndex]); }
                case PlayMode.PINGPONG_LOOP:
                        return setLastAndReturn(_keyFrames[PingPongIndex]);
                default:
                    return setLastAndReturn(_keyFrames.First());   
            }
        }

        private T setLastAndReturn(T t) { _last = t; return t; }
        private bool IsEven(int i) { if(i % 2 == 0) {  return true; } else { return false; }}
        private bool IsOdd(int i) { return !IsEven(i); }
        private float FramesSinceStart { get { return _secondsSinceStart * KeyFramesPerSecond; } }
        private int ForwardIndex { get { return (int)Math.Floor(FramesSinceStart % _keyFrames.Count);}}
        private int BackwardIndex { get { return _keyFrames.Count - ForwardIndex; } }
        private int PingPongIndex { 
            get 
            {
                int nrOfLoops = (int)Math.Floor(FramesSinceStart) / _keyFrames.Count;
                if (IsEven(nrOfLoops)) { return ForwardIndex - (nrOfLoops / 2); }
                else { return BackwardIndex - ((nrOfLoops - 1) / 2); }
            } 
        }

        public void Play() { IsPaused = false; }
        public void Pause() { IsPaused = true; }
        public void InsertKeyFrame(int index, T keyFrame) {_keyFrames.Insert(index, keyFrame);}
        public void AddKeyFrame(T keyFrame) {_keyFrames.Add(keyFrame);}
        public void RemoveKeyFrameAt(int index) {_keyFrames.RemoveAt(index);}
        public void RemoveKeyFrame(T keyFrame) {_keyFrames.Remove(keyFrame);}
    }
}
