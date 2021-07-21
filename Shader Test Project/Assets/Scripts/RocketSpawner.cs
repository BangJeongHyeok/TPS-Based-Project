using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketSpawner : MonoBehaviour
{
    [SerializeField] GameObject Ball;
    [SerializeField] GameObject StartPosition;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LaunchRocket(2));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LaunchRocket(float Delay)
    {
        yield return new WaitForSeconds(Delay);
        Instantiate(Ball, StartPosition.transform.position, Quaternion.identity).GetComponent<Ball>().AddForce(transform.forward, 50f);

        StartCoroutine(LaunchRocket(Delay));
    }
}
