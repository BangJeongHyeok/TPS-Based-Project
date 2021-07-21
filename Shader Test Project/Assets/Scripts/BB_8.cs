using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BB_8 : MonoBehaviour
{
    NavMeshAgent NMAgent;

    public GameObject target;
    Vector3 TargetPos;

    public float RotSpeed;

    Transform Body;
    Transform Head;

    // Start is called before the first frame update
    void Start()
    {
        NMAgent = GetComponent<NavMeshAgent>();

        Body = transform.Find("Body");
        Head = transform.Find("Head");
    }

    // Update is called once per frame
    void Update()
    {
        SetTarget(target);
        Move();
    }

    private void Move()
    {
        Vector3 Dir = (transform.position - TargetPos).normalized;
        Body.transform.Rotate(Dir.z * RotSpeed,0,0);
        Head.transform.LookAt(TargetPos + new Vector3(0,1,0));
    }

    private void SetTarget(GameObject Target)
    {
        TargetPos = Target.transform.position;
        NMAgent.destination = TargetPos;
    }
}
