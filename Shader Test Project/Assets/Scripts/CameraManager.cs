using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float MouseSensivity;
    [SerializeField] private float ZoomSpeed;

    [SerializeField] private GameObject CameraTarget;
    [SerializeField] private Vector3 PositionOffSet;
    
    [SerializeField] private float MaxCameraDistance;

    [SerializeField] LayerMask CameraRayCheck;

    private Camera camera;

    private float GoalCameraDistance = 10;
    private float CurCameraDistance = 10;
    private  Vector3 CameraDir = new Vector3(0,0,-1);

    private float CameraXAmount;
    private float CameraYAmount;

    private Vector3 CurPosition;

    void Start()
    {
        ComponenetSetting();
    }

    void ComponenetSetting()
    {
        camera = GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        CameraControl();
    }

    void Update()
    {
        CameraInput();
    }

    public float CameraTargetRotation()
    {
        //return (transform.rotation.eulerAngles.y > 180) ? transform.rotation.eulerAngles.y - 360f : transform.rotation.eulerAngles.y;
        return transform.rotation.eulerAngles.y;

        Vector3 Anggimo = (CameraTarget.transform.position - transform.position).normalized;
        return Mathf.Atan2(Anggimo.z, Anggimo.x) * Mathf.Rad2Deg;
    }

    void CameraInput()
    {
        Cursor.lockState = CursorLockMode.Locked;

        CameraXAmount += Input.GetAxis("Mouse X") * MouseSensivity * Time.deltaTime;
        CameraYAmount += Input.GetAxis("Mouse Y") * MouseSensivity * Time.deltaTime;
        CameraYAmount = Mathf.Clamp(CameraYAmount, -60*Mathf.Deg2Rad, 90 * Mathf.Deg2Rad);

        CameraDir = new Vector3(Mathf.Cos(-CameraXAmount), Mathf.Sin(CameraYAmount), Mathf.Sin(-CameraXAmount)).normalized;//수평은 CameraXAmount로 처리, 수직은 CameraYAmount

        GoalCameraDistance = Mathf.Clamp(GoalCameraDistance + -Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * Time.deltaTime, 1, MaxCameraDistance);
        CurCameraDistance = Mathf.Lerp(CurCameraDistance, GoalCameraDistance, 7.2f * Time.deltaTime);
        
        //transform.position = CameraTarget.transform.position + PositionOffSet + CameraDir * CurCameraDistance;
        //transform.LookAt(CameraTarget.transform.position + PositionOffSet);
    }

    public Vector3 GetCameraDir()
    {
        return -CameraDir;
    }

    void CameraControl()
    {
        //옳게된 오프셋
        Vector3 FixedOffset = CameraTarget.transform.right * PositionOffSet.x + CameraTarget.transform.up * PositionOffSet.y + CameraTarget.transform.forward * PositionOffSet.z;

        //각도는 알잘딱으로
        transform.LookAt(CameraTarget.transform.position + FixedOffset);

        RaycastHit rayHit;
        //Debug.DrawRay(CameraTarget.transform.position + PositionOffSet, CameraDir,Color.red);
        if (Physics.Raycast(CameraTarget.transform.position + FixedOffset, CameraDir, out rayHit,CurCameraDistance, CameraRayCheck))
        {
            CurPosition = rayHit.point;
        }
        else
        {
            CurPosition = CameraTarget.transform.position + PositionOffSet + CameraDir * CurCameraDistance;
        }

        transform.position = Vector3.Lerp(transform.position, CurPosition, 10.2f * Time.deltaTime);//포지션 튕기는걸 방지하는 목적
    }

    public void RespawnProduction()
    {
        StopAllCoroutines();
        StartCoroutine(Coroutine_RespawnProduction());
    }

    IEnumerator Coroutine_RespawnProduction()
    {
        GL.Clear(true,true, Color.black);

        float ScreenValue = 0.001f;
        camera.rect = new Rect(0, (1 - ScreenValue) * 0.5f, 1, ScreenValue);

        while (camera.rect.height < 0.99f)
        {
            ScreenValue += 10 * Time.deltaTime;

            camera.rect = new Rect(0, Mathf.Lerp(camera.rect.y, (1 - ScreenValue) * 0.5f, 14.2f * Time.deltaTime), 1, Mathf.Lerp(camera.rect.height, ScreenValue, 14.2f * Time.deltaTime));
            yield return null;
        }
        ScreenValue = 1;
        camera.rect = new Rect(0, 1-ScreenValue, 1, ScreenValue);
    }
}
