using UnityEngine;
using UnityEngine.Serialization;
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
        [Header("PlayerIcon")]
        [SerializeField] private Image playerIcon;
        [Header("EnemyIcon")]
        [SerializeField] private Image enemyIcon;

        [FormerlySerializedAs("loveIcon")] [SerializeField] private Sprite girlLoveIcon;
        [FormerlySerializedAs("grilSadIcon")] [FormerlySerializedAs("sadIcon")] [SerializeField] private Sprite girlSadIcon; 
        [SerializeField] private Sprite boyLoveIcon;
        [SerializeField] private Sprite boySadIcon;
        
        private Image playerItemImage;
        private Image enemyItemImage;
        
        private GameObject[] playerHpImages = new GameObject[3];
        private GameObject[] enemyHpImages = new GameObject[3];

        private void Awake()
        {
            Instance = this;
            if (playerItemPanel != null)
                playerItemImage = playerItemPanel.GetComponent<Image>();

            if (enemyItemPanel != null)
                enemyItemImage = enemyItemPanel.GetComponent<Image>();

            foreach (Transform child in playerHPPanel.transform)
            {
                playerHpImages[child.GetSiblingIndex()] = child.gameObject;
            }

            foreach (Transform child in enemyHPPanel.transform)
            {
                enemyHpImages[child.GetSiblingIndex()] = child.gameObject;
            }
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
            for (int i = 0; i < playerHpImages.Length; i++)
            {
                playerHpImages[i].SetActive(false);
            }
            for (int i = hp - 1; i >= 0; i--)
            {
                playerHpImages[i].SetActive(true);
            }
        }

        public void SetEnemyHPIcon(int hp)
        {
            for (int i = 0; i < enemyHpImages.Length; i++)
            {
                enemyHpImages[i].SetActive(false);
            }
            for (int i = hp - 1; i >= 0; i--)
            {
                enemyHpImages[i].SetActive(true);
            }
        }

        public void SetPlayerIcon(ICharacter character)
        {
            if (character.CharacterState.Emotion == CharacterState.EmotionType.Love)
            {
                playerIcon.sprite = girlLoveIcon;
            }
            else
            {
                playerIcon.sprite = girlSadIcon;
            }
        }

        public void SetEnemyIcon(ICharacter character)
        {
            if (character.CharacterState.Emotion == CharacterState.EmotionType.Love)
            {
                enemyIcon.sprite = boyLoveIcon;
            }
            else
            {
                enemyIcon.sprite = boySadIcon;
            }
        }

    }
}