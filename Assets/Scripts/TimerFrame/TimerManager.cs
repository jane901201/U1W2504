using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace TimerFrame
{
    public class TimerManager : MonoBehaviour
    {
        private ConcurrentDictionary<string, TimerTask> taskDict = new ConcurrentDictionary<string, TimerTask>();

        public static TimerManager Instance;

        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(TimerWheel());
        }

        private IEnumerator TimerWheel()
        {
            while (true)
            {
                double currentTime = Time.realtimeSinceStartupAsDouble;

                List<string> toRemove = new List<string>();

                foreach (var kvp in taskDict)
                {
                    var task = kvp.Value;

                    if (currentTime >= task.nextTriggerTime)
                    {
                        task.action?.Invoke();

                        if (task.repeat)
                        {
                            task.nextTriggerTime = currentTime + task.interval;
                        }
                        else
                        {
                            toRemove.Add(task.name);
                        }
                    }
                }

                foreach (var name in toRemove)
                {
                    taskDict.TryRemove(name, out _);
                }

                yield return null;
            }
        }

        public void AddTask(string name, double delay, Action callback, bool repeat = false, TimerUnit unit = TimerUnit.Second)
        {
            if (taskDict.ContainsKey(name)) return;

            double delayInSeconds = ConvertToSeconds(delay, unit);

            var task = new TimerTask
            {
                name = name,
                interval = delayInSeconds,
                nextTriggerTime = Time.realtimeSinceStartupAsDouble + delayInSeconds,
                repeat = repeat,
                action = callback
            };

            taskDict.TryAdd(name, task);
        }

        private double ConvertToSeconds(double delay, TimerUnit unit)
        {
            return unit switch
            {
                TimerUnit.Millisecond => delay / 1000.0,
                TimerUnit.Second => delay,
                TimerUnit.Minute => delay * 60.0,
                TimerUnit.Hour => delay * 3600.0,
                _ => delay
            };
        }
        
        public void AddRepeatingRandomTask(string name, float minDelay, float maxDelay, Action callback, TimerUnit unit = TimerUnit.Second)
        {
            RemoveTask(name); 

            void ScheduleNext()
            {
                float next = UnityEngine.Random.Range(minDelay, maxDelay);

                AddTask(name, next, () =>
                {
                    callback?.Invoke();

                    // 延迟一帧后再注册下一轮
                    Instance.StartCoroutine(DelayNextRound(name, minDelay, maxDelay, callback, unit));
                }, false, unit);
            }

            ScheduleNext(); 
        }

        private IEnumerator DelayNextRound(string name, float min, float max, Action cb, TimerUnit unit = TimerUnit.Second)
        {
            yield return null;
            AddRepeatingRandomTask(name, min, max, cb, unit);
        }

        
        public bool ResetTask(string name)
        {
            if (taskDict.TryGetValue(name, out var task))
            {
                task.nextTriggerTime = Time.realtimeSinceStartupAsDouble + task.interval;
                return true;
            }
            return false;
        }

        public bool HasTask(string name) => taskDict.ContainsKey(name);

        public bool RemoveTask(string name) => taskDict.TryRemove(name, out _);

        public void ClearAllTasks() => taskDict.Clear();
        
        public void DebugLogAllTasks()
        {
            Debug.Log("[TimerMonitor] 当前所有定时任务：");

            foreach (var kvp in taskDict)
            {
                string name = kvp.Key;
                TimerTask task = kvp.Value;

                float progress = Mathf.Clamp01((float)(1.0 - task.TimeLeft / task.TotalTime));
                int blocks = Mathf.RoundToInt(progress * 10);
                string bar = new string('▇', blocks) + new string('▁', 10 - blocks);

                Debug.Log($"🔹 {name}：{bar} ({task.TimeLeft:F1}s / {task.TotalTime:F1}s)");
            }
        }
        
        public IReadOnlyDictionary<string, TimerTask> GetAllTasks()
        {
            return taskDict;
        }


    }
}
