using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyItemTrigger : MonoBehaviour
{
    public AIController aiController;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Item")
        {
            aiController.Item = other.gameObject.GetComponent<ItemTrigger>();
        }

    }
}
