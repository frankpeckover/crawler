using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{

    public delegate void TriggerDelegate();

    public static event TriggerDelegate exitTriggerCollided;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player") 
        {
            exitTriggerCollided?.Invoke();
            gameObject.transform.parent.gameObject.SetActive(false);
        }
    }

}
