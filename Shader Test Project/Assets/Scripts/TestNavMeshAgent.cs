using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestNavMeshAgent : MonoBehaviour
{
    public Transform TargetObj;
    NavMeshAgent nma;
    // Start is called before the first frame update
    void Start()
    {
        nma = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        nma.SetDestination(TargetObj.position);
    }
}
