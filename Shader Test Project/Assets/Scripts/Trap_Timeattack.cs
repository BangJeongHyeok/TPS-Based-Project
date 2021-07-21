using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Timeattack : Trap
{
    [SerializeField] private float OriginY;
    [SerializeField] private float GoalY;
    [SerializeField] private float GoalTime;
    [SerializeField] private List<GameObject> Blocks = new List<GameObject>();
    private List<float> BlocksY = new List<float>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Blocks.Count; i++)
            BlocksY.Add(OriginY);

        TrapSetting(false,false,()=> StartTimeattack());
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Blocks.Count; i++)
        {
            Blocks[i].transform.localPosition = new Vector3(Blocks[i].transform.localPosition.x, BlocksY[i], Blocks[i].transform.localPosition.z);
        }
    }

    void StartTimeattack()
    {
        StopAllCoroutines();
        StartCoroutine(AllUp(0.6f));
    }

    IEnumerator AllUp(float time)
    {
        float Y = OriginY;
        float curTime = 0;
        while (curTime < time)
        {
            curTime += Time.deltaTime;
            Y = Mathf.Lerp(OriginY, GoalY, curTime/time); 

            for (int i = 0; i < BlocksY.Count; i++)
            {
                BlocksY[i] = Y;
            }
            yield return null;

        }

        yield return new WaitForSeconds(1f); 

        StartCoroutine(AllDown(GoalTime));
    }

    IEnumerator AllDown(float time)
    {
        float CurTime = 0;

        while (CurTime < time)
        {
            CurTime += Time.deltaTime;
            if((int)((CurTime / time) * BlocksY.Count) < BlocksY.Count)
                BlocksY[(int)((CurTime / time)* BlocksY.Count)] = Mathf.Lerp(GoalY,OriginY, (CurTime % (time / BlocksY.Count)) /(time/BlocksY.Count));

            yield return null;
        }
    }
}
