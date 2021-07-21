using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_UnKinematic : Trap
{
    [SerializeField] Rigidbody TargetRigid;

    // Start is called before the first frame update
    void Start()
    {
        TrapSetting(false,true,()=> UnKimenatic());
    }

    void UnKimenatic()
    {
        TargetRigid.isKinematic = false;
        TargetRigid.useGravity = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
