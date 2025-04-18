using UnityEngine;

namespace DefaultNamespace
{
    public class GameUI : MonoBehaviour
    {
        [Header("PlayerHPPanel")]
        [SerializeField] private GameObject playerHPPanel;
        [Header("EnemyHPPanel")]
        [SerializeField] private GameObject enemyHPPanel;
        [Header("PlayerItem")]
        [SerializeField] private GameObject playerItemPanel;
        [Header("EnemyItem")]
        [SerializeField] private GameObject enemyItemPanel;

        [SerializeField] private GameObject playerIcon;
        [SerializeField] private GameObject enemyIcon;

    }
}