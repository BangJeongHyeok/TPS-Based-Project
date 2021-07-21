using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SpawnPointBall cs_spawnPointBall;

    [SerializeField] private GameObject go_Player;
    private PlayerController2 cs_PlayerController;

    private CameraManager cs_CameraManager;
    private Vector3 SpawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        GetGameObject();
        GetComponenet();

        SpawnPointBallEvent();
    }

    void SpawnPointBallEvent()
    {
        cs_PlayerController.InteractionEventPress.AddListener((Vector3 origin, Vector3 dir, Vector3 pos)=> cs_spawnPointBall.DrawParbolaLine(origin, dir, pos));
        cs_PlayerController.InteractionEventUp.AddListener((Vector3 origin, Vector3 dir, Vector3 pos)=> cs_spawnPointBall.AddForceBall(origin, dir, pos));

        cs_spawnPointBall.SaveEvent.AddListener((Vector3 pos)=> SetNewSpawnPoint(pos));
    }

    void GetGameObject()
    {
    }

    void GetComponenet()
    {
        cs_PlayerController = go_Player.GetComponent<PlayerController2>();
        cs_CameraManager = Camera.main.transform.GetComponent<CameraManager>();

        //Player_PlayerController2.InteractionEvent += SetNewSpawnPoint;
    }

    // Update is called once per frame
    void Update()
    {
        SystemInput();
    }

    void SystemInput()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            PlayerRespawn();
        }
    }

    void SetNewSpawnPoint(Vector3 Pos)
    {
        SpawnPoint = Pos;
        Debug.DrawRay(Pos, Vector3.up, Color.red, 3f);
    }


    void PlayerRespawn()
    {
        go_Player.transform.position = SpawnPoint;
        go_Player.transform.rotation = Quaternion.identity;

        cs_PlayerController.PlayerDead(false);
        cs_CameraManager.RespawnProduction();
    }
}
