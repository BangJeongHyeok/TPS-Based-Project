using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trap_Spikeball : Trap
{
    [SerializeField] private Rigidbody Pivot;
    [SerializeField] private Rigidbody SpikeBall;
    void Awake()
    {
        TrapSetting(false,true, ()=>SpikeTrap());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpikeTrap()
    {
        Pivot.isKinematic = false;
        SpikeBall.AddForce(transform.forward * 20, ForceMode.VelocityChange);
    }
}
