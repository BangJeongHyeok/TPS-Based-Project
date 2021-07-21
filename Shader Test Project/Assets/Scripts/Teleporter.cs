using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Teleporter : MonoBehaviour
{
    public UnityEvent<GameObject, Teleporter> TeleportEvent;
    public UnityEvent<Teleporter> OutTeleportEvent;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            if (TeleportEvent != null)
                TeleportEvent.Invoke(collision.gameObject, this);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            OutTeleportEvent.Invoke(this);
        }
    }
}
