using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Teleporter : MonoBehaviour
{
[SerializeField] bool isOneWay;
private Teleporter CurTeleport = null;
[SerializeField] Teleporter Teleporter1;
[SerializeField] Teleporter Teleporter2;

    void Start()
    {
        Teleporter1.TeleportEvent.AddListener((GameObject obj, Teleporter thisTel)=> Teleport2Other(obj, thisTel, true));
        Teleporter1.OutTeleportEvent.AddListener((Teleporter thisTel)=> OutTeleport(thisTel));

        if(!isOneWay)
        {
        Teleporter2.TeleportEvent.AddListener((GameObject obj, Teleporter thisTel) => Teleport2Other(obj, thisTel, false));
            Teleporter2.OutTeleportEvent.AddListener((Teleporter thisTel) => OutTeleport(thisTel));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Teleport2Other(GameObject target,Teleporter thisTel, bool isOne)
    {
        if (CurTeleport == thisTel)
        { 
            target.transform.position = isOne ? Teleporter2.transform.position : Teleporter1.transform.position;
        }
    }

    void OutTeleport(Teleporter thisTel)
    {
        CurTeleport = thisTel;
    }
}
