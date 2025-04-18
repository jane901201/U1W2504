using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    [SerializeField] private IItem item;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            player.AddItem(item);
            Destroy(gameObject);
        }
        if (other.tag == "Enemy")
        {
            Destroy(gameObject);
        }
    }
}