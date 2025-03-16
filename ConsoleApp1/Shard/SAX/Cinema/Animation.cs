using Shard.SAX.Graphics2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard.SAX.Cinema
{
    internal class Animation<T>
    {
        private T _last;
        private long _lastTimeMilliSeconds;
        private float _milliSecondsSinceStart;
        
        public PlayMode PlayMode { private get; set; } = PlayMode.FORWARD_LOOP;
        public string Name { get; private set; }
        
        public bool IsPaused { get; private set; }
        public float MilliSecondsBetweenKeyFrames {  get; set; }
        private float KeyFramesPerMilliSecond { get { return ( 1f / MilliSecondsBetweenKeyFrames); } }
        private List<T> _keyFrames;

        public delegate void AnimationHasLoopedDelegate();
        public event AnimationHasLoopedDelegate AnimationHasLooped;


        public Animation(string name, List<T> keyFrames, float milliSecondsBetweenKeyFrames)
        {
            Name = name;
            _keyFrames = keyFrames;
            _milliSecondsSinceStart = 0;
            MilliSecondsBetweenKeyFrames = milliSecondsBetweenKeyFrames;
        }
        public Animation(string name, T[] keyFrames, float milliSecondsBetweenKeyFrames)
        {
            _keyFrames = [.. keyFrames];
        }

        public Animation(string name, List<T> keyFrames, float milliSecondsBetweenKeyFrames, PlayMode playMode) : 
            this(name,keyFrames,milliSecondsBetweenKeyFrames){ PlayMode = playMode; }
        public Animation(string name, T[] keyFrames, float milliSecondsBetweenKeyFrames, PlayMode playMode) :
            this(name,keyFrames,milliSecondsBetweenKeyFrames){ PlayMode = playMode; }

        public T GetKeyFrame(long currentTimeMilli)
        {
            if (_lastTimeMilliSeconds == 0) { _lastTimeMilliSeconds = currentTimeMilli; }
            if (IsPaused) { return _last; };
            float deltaTime = currentTimeMilli - _lastTimeMilliSeconds;
            _lastTimeMilliSeconds = currentTimeMilli;
            _milliSecondsSinceStart = _milliSecondsSinceStart + deltaTime;
            
            bool hasLooped = false;
            if (_milliSecondsSinceStart >= (_keyFrames.Count * (1 / KeyFramesPerMilliSecond))) hasLooped = true;
            
            switch (PlayMode)
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
        private float FramesSinceStart { get { return _milliSecondsSinceStart * KeyFramesPerMilliSecond; } }
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

        public void Play(long currentTimeMilli) { 
            IsPaused = false; 
            _lastTimeMilliSeconds = currentTimeMilli;
        }
        public void Pause() { IsPaused = true; }
        public void Reset() 
        {
            Pause();
            _milliSecondsSinceStart = 0;
            _lastTimeMilliSeconds = 0;
            _last = default;

        }
        public void InsertKeyFrame(int index, T keyFrame) {_keyFrames.Insert(index, keyFrame);}
        public void AddKeyFrame(T keyFrame) {_keyFrames.Add(keyFrame);}
        public void RemoveKeyFrameAt(int index) {_keyFrames.RemoveAt(index);}
        public void RemoveKeyFrame(T keyFrame) {_keyFrames.Remove(keyFrame);}
        public bool HasLoopedSinceLastGet(long currentTimeMilli) 
        {
            if (IsPaused) { return false; }
            float delta = _lastTimeMilliSeconds - currentTimeMilli;
            if (((_milliSecondsSinceStart % (MilliSecondsBetweenKeyFrames * _keyFrames.Count)) + delta) > (MilliSecondsBetweenKeyFrames * _keyFrames.Count)) 
            { return true; } else return false;
        }
    }
}
