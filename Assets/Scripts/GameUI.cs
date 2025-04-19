using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class GameUI : MonoBehaviour
    {
        public static GameUI Instance;
        [Header("PlayerHPPanel")]
        [SerializeField] private GameObject playerHPPanel;
        [Header("EnemyHPPanel")]
        [SerializeField] private GameObject enemyHPPanel;
        [Header("PlayerItem")]
        [SerializeField] private GameObject playerItemPanel;
        [Header("EnemyItem")]
        [SerializeField] private GameObject enemyItemPanel;

        [Header("Icon")]
        [SerializeField] private Image playerIcon;
        [SerializeField] private Image enemyIcon;
        
        private void Awake()
        {
            Instance = this;
        }

        public void SetPlayerItemIcon(Sprite sprite)
        {
            playerIcon.sprite = sprite;
            playerIcon.enabled = sprite != null;
        }

        public void ClearPlayerItemIcon()
        {
            playerIcon.sprite = null;
            playerIcon.enabled = false;
        }

    }
}