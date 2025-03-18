using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard.SAX.Cinema
{
    /// <summary>
    /// Manages animation transitions, playspeeds and synchronizations between
    /// multiple animations.
    /// </summary>
    class Animator<T>
    {
        private Dictionary<string, Animation<T>> _animations;
        private Animation<T> _currentAnimation;
        private List<string> _queue;
        public T CurrentValue { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Animator(Animation<T>[] animations) 
        {
            _animations = new Dictionary<string, Animation<T>>();
            _queue = new List<string>();
            AddAll(animations);
            if (animations.Length > 0) { _currentAnimation = animations[0]; }
        }
        public Animator(List<Animation<T>> animations) : this(animations.ToArray()) 
        { 
            
        }
        public void AddAnim(Animation<T> anim) 
        {
            if (!Exists(anim)) { _animations.Add(anim.Name, anim); }
            else { Debug.Log("Animation " + anim.Name + "already exist in Animator, new value discarded."); }
        }
        public void AddAll(Animation<T>[] animations) 
        {
            foreach (Animation<T> anim in animations)
            {
                AddAnim(anim);
            }
        }
        public void RemoveAnim(string name) 
        {
            _animations.Remove(name); 
        }
        public void RemoveAnim(Animation<T> anim) 
        {
            RemoveAnim(anim.Name); 
        }
        public Animation<T> GetAnimation(string name) { return _animations[name]; }
        public Animation<T> GetCurrentAnim(){ return _currentAnimation; }
        public bool Exists(Animation<T> anim) { return Exists(anim.Name); }
        public bool Exists(string name) { return _animations.ContainsKey(name); }

        public void Play(long currentTimeMilli) 
        {
            _currentAnimation.Play(currentTimeMilli);
        }
        public void Pause()
        {
            _currentAnimation.Pause();
        }
        public void Switch(string name,long currentTimeMilli) 
        {
            _currentAnimation.Reset();
            _currentAnimation = _animations[name];
            _currentAnimation.Play(currentTimeMilli);
        }
        public void Switch(Animation<T> anim, long currentTimeMilli) 
        {
            if (!Exists(anim)) { AddAnim(anim); }
            Switch(anim.Name,currentTimeMilli);
        }
        public void Queue(string name) 
        { 
            _queue.Add(name);
        }
        public void Queue(Animation<T> anim) { Queue(anim.Name); }
        public T GetKeyFrame(long currentTimeMilliSeconds) 
        {
            // If the current is done playing and there is one queued, switch to the next one.
            if (_currentAnimation.HasLoopedSinceLastGet(currentTimeMilliSeconds) && _queue.Count > 0) 
            {
                Switch(_queue[0],currentTimeMilliSeconds);
                _queue.RemoveAt(0);
            }
            
            return _currentAnimation.GetKeyFrame(currentTimeMilliSeconds);   
        }

        public void SetGlobalPlayMode(PlayMode playMode) 
        { 
            _currentAnimation.PlayMode = playMode;
        }
        public void SetGlobalSpeed(float milliSecondsBetweenKeyFrames) 
        {
            foreach(KeyValuePair<string,Animation<T>> kvp in _animations) 
            { 
                kvp.Value.MilliSecondsBetweenKeyFrames = milliSecondsBetweenKeyFrames;
            }
        }
    }
}
