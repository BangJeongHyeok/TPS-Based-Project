using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Rotate : Trap
{
    [SerializeField] GameObject RotateObject;
    [SerializeField] Vector3 RotatePower;
    
    bool isRotating;

    // Start is called before the first frame update
    void Start()
    {
        TrapSetting(true,false,()=>RotateAction());
    }

    // Update is called once per frame
    void Update()
    {
        if (isRotating)
        {
            RotateObject.transform.Rotate(RotatePower * Time.deltaTime);
        }
    }

    void RotateAction()
    {
        isRotating = true;
    }
}
