using System;
using UnityEngine;

namespace TimerFrame
{
    public class TimerTask
    {
        public string name;
        public double interval;           // 秒为单位
        public double nextTriggerTime;    // 绝对时间戳
        public bool repeat;
        public Action action;
        public double TotalTime => interval;
        public double TimeLeft => nextTriggerTime - Time.realtimeSinceStartupAsDouble;
        public float Progress => Mathf.Clamp01((float)(1.0 - TimeLeft / interval));

    }
}