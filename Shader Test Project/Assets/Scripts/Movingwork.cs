using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movingwork : MonoBehaviour
{
    Rigidbody rigid;
    [SerializeField] GameObject TargetObj;
    Vector3 OriginPos;//WorldPos����
    [SerializeField] Vector3 GoalPos;//WorldPos����
    [SerializeField] float GoalTime = 0;
    bool isArrived = false;
    float Timer = 0;

    void Start()
    {
        rigid = TargetObj.GetComponent<Rigidbody>();
        OriginPos = TargetObj.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {


        if (isArrived)
        {
            rigid.position = Vector3.Lerp(GoalPos, OriginPos, Timer/GoalTime);
        }
        else
        {
            rigid.position = Vector3.Lerp(OriginPos, GoalPos, Timer/GoalTime);
        }
        Timer += Time.deltaTime;

        if (Timer >= GoalTime)
        {
            Timer = 0;
            isArrived = !isArrived;
        }
    }
}
