using DefaultNamespace;

namespace TimerFrame
{
    using System.Text;
    using UnityEngine;
    using UnityEngine.UI;
    using TimerFrame;

    public class TimerMonitorUI : MonoBehaviour
    {
        [SerializeField] private Text monitorText; // Inspector中绑定
        [SerializeField] private float refreshInterval = 0.01f;

        private bool isVisible = true;

        private void Start()
        {
            InvokeRepeating(nameof(UpdateMonitor), 0f, refreshInterval);
        }

        private void Update()
        {
            isVisible = GameSystem.Instance.Debug;
            monitorText.gameObject.SetActive(isVisible);
        }

        private void UpdateMonitor()
        {
            if (TimerManager.Instance == null || monitorText == null || !isVisible) return;

            var sb = new StringBuilder();
            sb.AppendLine("<b>⏱ Timer Monitor</b>");

            foreach (var kvp in TimerManager.Instance.GetAllTasks())
            {
                var task = kvp.Value;
                float timeLeft = Mathf.Max(0f, (float)task.TimeLeft);
                float total = (float)task.TotalTime;
                float progress = task.Progress;

                int barCount = 10;
                int filled = Mathf.RoundToInt(progress * barCount);
                string bar = new string('■', filled) + new string('□', barCount - filled);

                sb.AppendLine($"{kvp.Key}：{bar}  {timeLeft:F1}s / {total:F1}s");
            }

            monitorText.text = sb.ToString();
        }
    }


}