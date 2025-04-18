using System;

namespace TimerFrame
{
    public class TimerTask
    {
        public string name;
        public double interval;           // 秒为单位
        public double nextTriggerTime;    // 绝对时间戳
        public bool repeat;
        public Action action;
    }
}