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
        }
        if (other.tag == "Enemy")
        {
            //TODO: add item to enemy
        }
        Destroy(gameObject);
    }
}