using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_LivingWall : Trap
{
    bool isMove = false;
    [SerializeField] GameObject MovingBlock;
    Vector3 OriginPos;
    [SerializeField] Vector3 TargetPos;
    [SerializeField] float MoveSpeed = 10f;

    void Start()
    {
        OriginPos = MovingBlock.transform.position;

        TrapSetting(false,false,()=> MoveObject(), () => MoveObject());
    }

    private void Update()
    {
        if (isMove)
        {
            MovingBlock.transform.position = Vector3.Lerp(MovingBlock.transform.position, OriginPos + TargetPos, MoveSpeed * Time.deltaTime);
        }
        else
        {
            MovingBlock.transform.position = Vector3.Lerp(MovingBlock.transform.position, OriginPos, MoveSpeed * Time.deltaTime);
        }
    }

    void MoveObject()
    {
        isMove = !isMove;
    }
}
