using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody rigid;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10)
            Destroy(this.gameObject);
    }

    public void AddForce(Vector3 dir, float Power)
    {
        rigid.AddForce(dir * Power, ForceMode.Impulse);
    }
}
