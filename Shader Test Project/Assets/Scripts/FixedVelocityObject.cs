using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FixedVelocityObject : MonoBehaviour
{
    [SerializeField] bool DontCare_Y = false;
    [SerializeField] float MaxVelocity;
    Rigidbody rigid;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rigid.velocity = new Vector3(Mathf.Clamp(rigid.velocity.x,-MaxVelocity,MaxVelocity), DontCare_Y == true ? rigid.velocity.y : Mathf.Clamp(rigid.velocity.y, -MaxVelocity, MaxVelocity),Mathf.Clamp(rigid.velocity.z, -MaxVelocity, MaxVelocity));
    }
}
