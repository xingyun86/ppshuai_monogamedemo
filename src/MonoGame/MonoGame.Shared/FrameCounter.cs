#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace MonoGame.Shared
{
    public class FrameCounter
    {
        public long TotalFrames { get; private set; }
        public float TotalSeconds { get; private set; }
        public float AverageFramesPerSecond { get; private set; }
        public float CurrentFramesPerSecond { get; private set; }

        public const int MAXIMUM_COUNTER = 100;

        private Queue<float> _frameQueue = new Queue<float>();

        public void Update(float deltaTime)
        {
            CurrentFramesPerSecond = 1.0f / deltaTime;

            _frameQueue.Enqueue(CurrentFramesPerSecond);

            if (_frameQueue.Count > MAXIMUM_COUNTER)
            {
                _frameQueue.Dequeue();
                AverageFramesPerSecond = _frameQueue.Average(i => i);
            }
            else
            {
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }

            TotalFrames++;
            TotalSeconds += deltaTime;
        }
    }
}
