using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIteTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Item")
        {
            this.GetComponent<HateAIController>().Item = other.gameObject.GetComponent<IItem>();
        }

    }
}
