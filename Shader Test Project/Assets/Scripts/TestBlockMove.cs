using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBlockMove : MonoBehaviour
{
    Vector3 OriginPos;
    float Amount;
    [SerializeField] float YAmount;
    [SerializeField] float Speed;

    // Start is called before the first frame update
    void Start()
    {
        OriginPos = transform.position;
    }

    void Update()
    {
        Amount += Speed * Time.deltaTime;
        transform.position = OriginPos + new Vector3(0,Mathf.Sin(Amount) * YAmount,0);
    }
}
