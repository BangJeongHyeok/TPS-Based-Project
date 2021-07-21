using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float MoveSpeed;
    [SerializeField] float SprintSpeed;
    [SerializeField] float JumpPower;
    [SerializeField] Vector3 PositionOffset;


    float HorizontalValue;
    float VerticalValue;
    bool Sprint;
    bool isGround;
    bool isDead = false;
    Vector3 FloorDir;

    Vector3 AimPoint;

    float CurrentGravityScale = 0;

    //�÷��̾� ���ͷ���
    private float InteractionDistance = 3f;
    public delegate void PlayerInteraction(Vector3 origin, Vector3 dir, Vector3 point);
    //public PlayerInteraction InteractionEvent;
    public UnityEvent<Vector3, Vector3, Vector3> InteractionEventDown;
    public UnityEvent<Vector3, Vector3, Vector3> InteractionEventPress;
    public UnityEvent<Vector3, Vector3, Vector3> InteractionEventUp;

    Animator Animator_PlayerAnimator;
    CameraManager PlayerCamera;
    CharacterController Controller_PlayerController;

    private GameObject Object_MainBody;
    private GameObject Object_Ragdoll;


    void Start()
    {
        ComponentSetting();
        ObjectSetting();

        //InteractionEventDown.AddListener((origin, dir, point) => DrawParbolaLine(origin, dir, point));
    }

    void ComponentSetting()
    {
        Animator_PlayerAnimator = GetComponent<Animator>();
        PlayerCamera = GameObject.Find("MainCamera").GetComponent<CameraManager>();
        Controller_PlayerController = GetComponent<CharacterController>();
    }

    void ObjectSetting()
    {
        Object_MainBody = transform.Find("MainBody").gameObject;
        Object_Ragdoll = transform.Find("Ragdoll").gameObject;
    }

    void VisibleRagdoll(bool isVisible)//���׵� �����ֱ�
    {

        if (isVisible)
        {
            SyncRagdollTransform(Object_MainBody.transform.Find("Character1_Reference"), Object_Ragdoll.transform.Find("Character1_Ragdoll"));
        }

        Object_MainBody.SetActive(!isVisible);
        Object_Ragdoll.SetActive(isVisible);
    }

    void SyncRagdollTransform(Transform OriginTr, Transform RagdollTr)//���׵� ������
    {
        RagdollTr.position = OriginTr.position;
        RagdollTr.rotation = OriginTr.rotation;

        if (RagdollTr.GetComponent<Rigidbody>() != null)
            RagdollTr.GetComponent<Rigidbody>().velocity = Vector3.zero;


        if (OriginTr.childCount > 0)
            for (int i = 0; i < OriginTr.childCount; i++)
            {
                SyncRagdollTransform(OriginTr.GetChild(i), RagdollTr.GetChild(i));
            }
    }

    void FixedUpdate()
    {
        if (!isDead)
        {
            GravityCheck();
            CharacterMove();
        }
    }

    private void Update()
    {
        if (!isDead)
            CharacterInput();
    }

    void CharacterInput()//����
    {
        if (isGround)
        {
            HorizontalValue = Mathf.Lerp(HorizontalValue, Input.GetAxis("Horizontal") * (1 - System.Convert.ToInt32(Animator_PlayerAnimator.GetBool("Randing"))), 6.4f * Time.deltaTime);
            VerticalValue = Mathf.Lerp(VerticalValue, Input.GetAxis("Vertical") * (1 - System.Convert.ToInt32(Animator_PlayerAnimator.GetBool("Randing"))), 6.4f * Time.deltaTime);


            Animator_PlayerAnimator.SetFloat("Horizontal", HorizontalValue);
            Animator_PlayerAnimator.SetFloat("Vertical", VerticalValue);

            Sprint = (Input.GetAxis("Sprint") != 0);
            Animator_PlayerAnimator.SetBool("Sprint", Sprint);
        }

        if (Input.GetKeyDown(KeyCode.Space) && !Animator_PlayerAnimator.GetBool("Randing"))//���� ���濡 �����Ϸ��� New InputSystem ����ҰŰ��⵵ �ϰ�
        {
            CharacterJump();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (InteractionEventDown != null)
            {
                Vector3 FixedPositionOffset = transform.right * PositionOffset.x + transform.up * PositionOffset.y + transform.forward * PositionOffset.z;

                RaycastHit rayHit;
                if (Physics.Raycast(transform.position + FixedPositionOffset, AimPoint, out rayHit, InteractionDistance))
                    InteractionEventDown.Invoke(transform.position + FixedPositionOffset, AimPoint, rayHit.point);
                else
                    InteractionEventDown.Invoke(transform.position + FixedPositionOffset, AimPoint, Vector3.zero);
            }

        }
        else if (Input.GetKey(KeyCode.E))
        {
            if (InteractionEventPress != null)
            {
                Vector3 FixedPositionOffset = transform.right * PositionOffset.x + transform.up * PositionOffset.y + transform.forward * PositionOffset.z;

                RaycastHit rayHit;
                if (Physics.Raycast(transform.position + FixedPositionOffset, AimPoint, out rayHit, InteractionDistance))
                    InteractionEventPress.Invoke(transform.position + FixedPositionOffset, AimPoint, rayHit.point);
                else
                    InteractionEventPress.Invoke(transform.position + FixedPositionOffset, AimPoint, Vector3.zero);
            }

        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            if (InteractionEventUp != null)
            {
                Vector3 FixedPositionOffset = transform.right * PositionOffset.x + transform.up * PositionOffset.y + transform.forward * PositionOffset.z;

                RaycastHit rayHit;
                if (Physics.Raycast(transform.position + FixedPositionOffset, AimPoint, out rayHit, InteractionDistance))
                    InteractionEventUp.Invoke(transform.position + FixedPositionOffset, AimPoint, rayHit.point);
                else
                    InteractionEventUp.Invoke(transform.position + FixedPositionOffset, AimPoint, Vector3.zero);
            }

        }
    }

    void CharacterJump()//����
    {
        if (isGround)
        {
            //RigidBody_PlayerRigid.AddForce(Vector3.up * JumpPower, ForceMode.VelocityChange);
            CurrentGravityScale = JumpPower;
            isGround = false;
            Animator_PlayerAnimator.SetBool("isGround", isGround);
        }
    }

    void CharacterMove()//������
    {
        //�����̼�
        if (!Input.GetMouseButton(1))
        {
            AimPoint = PlayerCamera.GetCameraDir();//�ٶ󺸰� �ִ� ������ ���� �����ֵ���
            transform.eulerAngles = new Vector3(0, PlayerCamera.CameraTargetRotation(), 0);
        }

        //������
        //Controller_PlayerController.Move(Quaternion.Euler(FloorDir) * (new Vector3(HorizontalValue, CurrentGravityScale, VerticalValue)) * (Sprint ? SprintSpeed : MoveSpeed) * Time.deltaTime);
        Controller_PlayerController.Move(new Vector3(0,CurrentGravityScale,0) * Time.deltaTime);//중력

        Controller_PlayerController.Move(transform.rotation *Quaternion.Euler(FloorDir) * (new Vector3(HorizontalValue, 0, VerticalValue)) * (Sprint ? SprintSpeed : MoveSpeed) * Time.deltaTime);
    }


    void GravityCheck()//�ٴ� üũ
    {
        
        var ray = new Ray((transform.position + Controller_PlayerController.center - new Vector3(0, Controller_PlayerController.height / 2, 0)) + Vector3.up * 0.1f, Vector3.down);
        Debug.DrawRay((transform.position + Controller_PlayerController.center - new Vector3(0, Controller_PlayerController.height / 2, 0)) + Vector3.up * 0.1f, Vector3.down, Color.blue);

        RaycastHit rayHit;
        var maxDistance = 0.2f;
        isGround = Physics.Raycast(ray, out rayHit, maxDistance);
        if (rayHit.transform != null)
        {
            FloorDir = Vector3.Cross(transform.right, rayHit.normal);
            Debug.DrawRay(rayHit.point, FloorDir, Color.blue);

            transform.position = rayHit.point;
            //CurrentGravityScale = 0;
        }
        else
        {
            
            FloorDir = transform.forward;


            CurrentGravityScale -= 9.81f * Time.deltaTime;//중력 적용
        }

        Animator_PlayerAnimator.SetBool("isGround", isGround);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isDead)
        {
            if (CheckBreakForce(collision, 12f))
                PlayerDead(true);//���� ó��
        }
    }

    bool CheckBreakForce(Collision collision, float BreakForce)//��ݷ� üũ
    {
        Debug.Log(collision.relativeVelocity);
        return (Mathf.Abs(collision.relativeVelocity.x) > BreakForce || Mathf.Abs(collision.relativeVelocity.y) > BreakForce || Mathf.Abs(collision.relativeVelocity.z) > BreakForce);//���� �̻��� ����� ������
    }

    public void PlayerDead(bool Dead)//������ Ȯ���ϴ� ���� �߿��� �Լ�
    {
        isDead = Dead;
        //RigidBody_PlayerRigid.velocity = Vector3.zero;
        //RigidBody_PlayerRigid.isKinematic = Dead;
        VisibleRagdoll(Dead);
    }
}
