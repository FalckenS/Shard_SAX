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
        private long _lastTimeMilliSeconds;
        private float _milliSecondsSinceStart = 0;
        
        public bool IsPaused { get; private set; }
        public float MilliSecondsBetweenKeyFrames {  get; set; }
        private float _keyFramesPerMilliSecond { get { return ( 1f / MilliSecondsBetweenKeyFrames); } }
        private List<T> _keyFrames;


        public Animation(List<T> keyFrames)
        {
            _keyFrames = keyFrames;
        }
        public Animation(T[] keyFrames)
        {
            _keyFrames = [.. keyFrames];
        }

        public T GetKeyFrame(long currentTimeMilli,PlayMode playMode)
        {
            if (_lastTimeMilliSeconds == 0) { _lastTimeMilliSeconds = currentTimeMilli; }
            if (IsPaused) { return _last; };
            float deltaTime = currentTimeMilli - _lastTimeMilliSeconds;
            _lastTimeMilliSeconds = currentTimeMilli;
            _milliSecondsSinceStart = _milliSecondsSinceStart + deltaTime;
            
            bool hasLooped = false;
            if (_milliSecondsSinceStart >= (_keyFrames.Count * (1 / _keyFramesPerMilliSecond))) hasLooped = true;
            
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
                    if (hasLooped && FramesSinceStart >= _keyFrames.Count * 2) { return setLastAndReturn(_keyFrames.First()); } 
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
        private float FramesSinceStart { get { return _milliSecondsSinceStart * _keyFramesPerMilliSecond; } }
        private int ForwardIndex { get { return (int)Math.Floor(FramesSinceStart % _keyFrames.Count);}}
        private int BackwardIndex { get { return _keyFrames.Count - ForwardIndex - 1; } }
        private int PingPongIndex { 
            get 
            {
                int nrOfLoops = (int)Math.Floor(FramesSinceStart) / _keyFrames.Count;
                if (IsEven(nrOfLoops)) { return ForwardIndex; }
                else { return BackwardIndex; }
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
