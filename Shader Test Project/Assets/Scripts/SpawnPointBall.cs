using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnPointBall : MonoBehaviour
{


    LineRenderer lineRenderer;
    Rigidbody BallRigid;

    public UnityEvent<Vector3> SaveEvent;

    float Power = 15;
    bool isDynamic = false;

    void Start()
    {
        ComponentSetting();
    }

    void ComponentSetting()
    {
        BallRigid = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        //if (isDynamic)
        //    CheckVelocity(transform.position);
    }


    public void DrawParbolaLine(Vector3 origin, Vector3 dir, Vector3 point)
    {
        Vector3 GravityAmount = Vector3.zero;


        int Count = 0;
        while (Count < 500)
        {
            lineRenderer.positionCount = 500;

            GravityAmount.y -= 9.81f * Time.deltaTime;
            lineRenderer.SetPosition(Count, origin + Count * dir * Power * Time.deltaTime + GravityAmount * Count * Time.deltaTime);
            RaycastHit rayHit;
            //Debug.DrawRay(origin + Count * dir * Power * Time.deltaTime + GravityAmount * Count * Time.deltaTime, ((origin + (Count+1) * dir * Power * Time.deltaTime + GravityAmount * (Count + 1) * Time.deltaTime) - (origin + Count * dir * Power * Time.deltaTime + GravityAmount * Count * Time.deltaTime)).normalized, Color.red, 1);

            if (Physics.Raycast(origin + Count * dir * Power * Time.deltaTime + GravityAmount * Count * Time.deltaTime, ((origin + (Count + 1) * dir * Power * Time.deltaTime + GravityAmount * (Count + 1) * Time.deltaTime) - (origin + Count * dir * Power * Time.deltaTime + GravityAmount * Count * Time.deltaTime)).normalized, out rayHit, 0.1f))
            {
                lineRenderer.positionCount = (Count + 1);
                break;
            }
            Count++;

        }

    }

    public void AddForceBall(Vector3 origin, Vector3 dir, Vector3 point)
    {
        lineRenderer.positionCount = 0;

        BallRigid.transform.position = origin;
        BallRigid.velocity = Vector3.zero;
        BallRigid.AddForce(dir * (Power * 0.7f), ForceMode.VelocityChange);//이상하게 포물선값의 0.7정도 넣어야 포물선에 근사값이 나옴
        
        isDynamic = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(isDynamic)
        {
            BallRigid.velocity = Vector3.zero;

            Vector3 CenterPos = Vector3.zero;
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                CenterPos += collision.contacts[i].point;
            }

            SaveEvent.Invoke(CenterPos/ collision.contacts.Length);
            Debug.Log("SaveEvent Invoke!");
            isDynamic = false;
        }
    }

    void CheckVelocity(Vector3 position)
    {
        if(BallRigid.velocity.x < 0.05f || BallRigid.velocity.y < 0.05f || BallRigid.velocity.z < 0.05f)
        {
            BallRigid.velocity = Vector3.zero;

            SaveEvent.Invoke(position);
            Debug.Log("SaveEvent Invoke!");
            isDynamic = false;
        }
    }
}
