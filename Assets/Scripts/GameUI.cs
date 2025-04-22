using System.Collections;
using TimerFrame;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;


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
        [SerializeField] private TextMeshProUGUI  playerItemDescription;
        
        [Header("EnemyItem")]
        [SerializeField] private GameObject enemyItemPanel;
        [Header("EffectImage")]
        [SerializeField] private Image playereffectImage; 
        [SerializeField] private Image enemyEffectImage; 
        [Header("Icon")]
        [SerializeField] private Image playerIcon;
        [SerializeField] private Image enemyIcon;
        [SerializeField] private Sprite girlLoveIcon;
        [SerializeField] private Sprite girlSadIcon; 
        [SerializeField] private Sprite boyLoveIcon;
        [SerializeField] private Sprite boySadIcon;

        [Header("StateChange")]
        [SerializeField] private Image stateChangeImage;
        [SerializeField] private Sprite playerChaseEnemy;
        [SerializeField] private Sprite enemyChasePlayer;
        [SerializeField] private float waitTime = 1f;
        
        private Image playerItemImage;
        private Image enemyItemImage;
        
        private GameObject[] playerHpImages = new GameObject[5];
        private GameObject[] enemyHpImages = new GameObject[5];

        private void Awake()
        {
            Instance = this;
            if (playerItemPanel != null)
                playerItemImage = playerItemPanel.GetComponent<Image>();

            if (enemyItemPanel != null)
                enemyItemImage = enemyItemPanel.GetComponent<Image>();
        }

        private void Start()
        {
            foreach (Transform child in playerHPPanel.transform)
            {
                playerHpImages[child.GetSiblingIndex()] = child.gameObject;
            }
            
            Debug.Log(playerHpImages.Length);

            foreach (Transform child in enemyHPPanel.transform)
            {
                enemyHpImages[child.GetSiblingIndex()] = child.gameObject;
            }
        } 
        
        public void FlushIcon()
        {
            GameState nowGameState = GameSystem.Instance.GameState;

            stateChangeImage.gameObject.SetActive(true);
            if (nowGameState == GameState.EnemyChasePlayer)
            {
                stateChangeImage.sprite = enemyChasePlayer;
                playerIcon.sprite = girlSadIcon;
                enemyIcon.sprite = boyLoveIcon;
            }
            else
            {
                stateChangeImage.sprite = playerChaseEnemy;
                playerIcon.sprite = girlLoveIcon;
                enemyIcon.sprite = boySadIcon;
            }
            TimerManager.Instance.AddTask($"{name}_FlushIcon_Wait{waitTime}AndSetNull", waitTime, () =>
            {
                stateChangeImage.gameObject.SetActive(false);
            });
        }

        public void SetPlayerItemIcon(Sprite sprite, string description)
        {
            if (playerItemImage == null)
            {
                Debug.LogError("playerItemImage is null!");
                return;
            }
            playerItemImage.sprite = sprite;
            playerItemDescription.text = description;
        }

        public void ClearPlayerItemIcon()
        {
            if (playerItemImage == null) return;
            playerItemImage.sprite = null;
            playerItemDescription.text = "";
        }

        public void SetPlayerHPIcon(int hp)
        {
            //Debug.Log("Player HP : " + hp);
            for (int i = 0; i < playerHpImages.Length; i++)
            {
                playerHpImages[i].SetActive(false);
            }
            for (int i = hp - 1; i >= 0; i--)
            {
                // TODO: Out of bounds bug here.
                playerHpImages[i].SetActive(true);
            }
        }

        public void SetPlayerEffectImage(Sprite sprite, float duration)
        {
            playereffectImage.gameObject.SetActive(true);
            playereffectImage.sprite = sprite;
            StartCoroutine(WaitAndSetFalse(duration, playereffectImage.gameObject));
        }

        public void SetPlayerEffectImage(Sprite sprite, bool active)
        {
            playereffectImage.sprite = sprite;
            playereffectImage.gameObject.SetActive(active);
        }
        
        public void SetEnemyEffectImage(Sprite sprite, float duration)
        {
            enemyEffectImage.gameObject.SetActive(true);
            enemyEffectImage.sprite = sprite;
            StartCoroutine(WaitAndSetFalse(duration, enemyEffectImage.gameObject));
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
        
        public IEnumerator WaitAndSetFalse(float waitTime, GameObject gameObject)
        {
            yield return new WaitForSeconds(waitTime);
            gameObject.SetActive(false);
        }

    }
}