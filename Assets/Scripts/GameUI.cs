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
        
        private Image playerItemImage;
        private Image enemyItemImage;
        
        private void Awake()
        {
            Instance = this;
            if (playerItemPanel != null)
                playerItemImage = playerItemPanel.GetComponent<Image>();

            if (enemyItemPanel != null)
                enemyItemImage = enemyItemPanel.GetComponent<Image>();
        }

        public void SetPlayerItemIcon(Sprite sprite)
        {
            if (playerItemImage == null)
            {
                Debug.LogError("playerItemImage is null!");
                return;
            }
            playerItemImage.sprite = sprite;
        }

        public void ClearPlayerItemIcon()
        {
            if (playerItemImage == null) return;
            playerItemImage.sprite = null;
        }

        public void SetPlayerHPIcon(int hp)
        {
            
        }

        public void SetEnemyHPIcon(int hp)
        {
            
        }

    }
}