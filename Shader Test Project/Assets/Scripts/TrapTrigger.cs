using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    [SerializeField] private Trap TargetTrap;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void ActivateTrap(TrapActivateStyle active)
    {
        TargetTrap.SetActiveState(active);
    }
}
