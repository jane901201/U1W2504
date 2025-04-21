using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    [SerializeField] private IItem item;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
   
            if (player.AddItem(item))
            {
                Destroy(gameObject);
            }
        }
        if (other.tag == "Enemy")
        {
            // TODO: Let enemy use item in the future.
            Enemy enemy = other.GetComponent<Enemy>();
            // if (enemy.AddItem(item))
            // {
            //     enemy.UseItem();
            //     Destroy(gameObject);
            // }
            Destroy(gameObject);
        }
    }
}