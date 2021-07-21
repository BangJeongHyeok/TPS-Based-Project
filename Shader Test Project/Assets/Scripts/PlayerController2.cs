using UnityEngine;
using UnityEngine.Events;

public class PlayerController2 : MonoBehaviour
{
    [SerializeField] float MoveSpeed;
    [SerializeField] float SprintSpeed;
    [SerializeField] float AirMassSpeed;//공중 움직임 속도 관련
    [SerializeField] float JumpPower;
    [SerializeField] Vector3 PositionOffset;


    float HorizontalValue;
    float VerticalValue;
    bool Sprint;
    bool isGround;
    bool isDead = false;
    Vector3 FloorDir;
    Vector3 FloorDir_right;
    //float PointedsYPos = 0;

    Vector3 AimPoint;

    //�÷��̾� ���ͷ���
    private float InteractionDistance = 3f;
    public delegate void PlayerInteraction(Vector3 origin, Vector3 dir, Vector3 point);
    //public PlayerInteraction InteractionEvent;
    public UnityEvent<Vector3, Vector3, Vector3> InteractionEventDown;
    public UnityEvent<Vector3, Vector3, Vector3> InteractionEventPress;
    public UnityEvent<Vector3, Vector3, Vector3> InteractionEventUp;

    Animator Animator_PlayerAnimator;
    Rigidbody RigidBody_PlayerRigid;
    CameraManager PlayerCamera;

    private GameObject Object_MainBody;
    private GameObject Object_Ragdoll;

    //������ ���� �ִ� ������
    [SerializeField] CapsuleCollider FeetCollider;
    float MinimumHeight = 0.44f;
    float MaximumHeight = 0.784604f;
    float PivotYPos = 0.8405053f;
    [SerializeField] Transform LeftFeet;
    [SerializeField] Transform RightFeet;
    [Range(0, 0.1f)]    
    [SerializeField] float DistanceToGround;
    [SerializeField] LayerMask FeetCheckLayer;


    void ChangeFeetCollider()
    {
        float MinimumYPos = 0;
        float MaximumYPos = 0;
        RaycastHit rayHit_R;
        RaycastHit rayHit_L;
        Debug.DrawRay(new Vector3(RightFeet.transform.position.x, transform.position.y + PivotYPos, RightFeet.transform.position.z), Vector3.down, Color.red);
        Debug.DrawRay(new Vector3(LeftFeet.transform.position.x, transform.position.y + PivotYPos, LeftFeet.transform.position.z), Vector3.down, Color.red);
        if (Physics.Raycast(new Vector3(RightFeet.transform.position.x, transform.position.y + PivotYPos, RightFeet.transform.position.z), Vector3.down, out rayHit_R, MaximumHeight + 1, FeetCheckLayer))
        {
            MinimumYPos = rayHit_R.point.y;
        }

        if (Physics.Raycast(new Vector3(LeftFeet.transform.position.x, transform.position.y + PivotYPos, LeftFeet.transform.position.z), Vector3.down, out rayHit_L, MaximumHeight + 1, FeetCheckLayer))
        {
            if (rayHit_L.point.y > MinimumYPos)
            {
                MaximumYPos = MinimumYPos;
                MinimumYPos = rayHit_L.point.y;
            }
            else
            {
                MaximumYPos = rayHit_L.point.y;
            }
        }
        //if (transform.InverseTransformPoint(rayHit_R.point).y > FeetCollider.center.y - MaximumHeight / 2 && transform.InverseTransformPoint(rayHit_L.point).y > FeetCollider.center.y - MaximumHeight / 2)
        //            FeetCollider.height = Mathf.Clamp(FeetCollider.height + Time.deltaTime, MinimumHeight, MaximumHeight);
        //    else if()
        //            FeetCollider.height = Mathf.Clamp(FeetCollider.height - Time.deltaTime, MinimumHeight, MaximumHeight);

        //FeetCollider.height = Mathf.Clamp(PivotYPos - transform.InverseTransformPoint(0, MinimumYPos, 0).y, MinimumHeight, MaximumHeight);
        //FeetCollider.center = new Vector3(FeetCollider.center.x, PivotYPos - FeetCollider.height/2, FeetCollider.center.z);
        
    }

    void Start()
    {
        ComponentSetting();
        ObjectSetting();

        //InteractionEventDown.AddListener((origin, dir, point) => DrawParbolaLine(origin, dir, point));
    }

    void ComponentSetting()
    {
        Animator_PlayerAnimator = GetComponent<Animator>();
        RigidBody_PlayerRigid = GetComponent<Rigidbody>();
        PlayerCamera = GameObject.Find("MainCamera").GetComponent<CameraManager>();
    }

    void ObjectSetting()
    {
        Object_MainBody = transform.Find("MainBody").gameObject;
        Object_Ragdoll = transform.Find("Ragdoll").gameObject;
    }

    void VisibleRagdoll(bool isVisible)//���׵� �����ֱ�
    {

        if(isVisible)
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
            //ChangeFeetCollider();
            GravityCheck();
            //if(isGround)
            CharacterMove();
            //else
            //CharacterMove_Force();
        }
    }

    private void Update()
    {
        if(!isDead)
            CharacterInput();
    }

    void SyncWithParent()
    {
        if(transform.parent != null)
        {
            RigidBody_PlayerRigid.velocity = Vector3.zero;
        }
    }

    void CharacterInput()//����
    {

        //Hor/Ver값 수정
            Debug.DrawRay(transform.position, FloorDir * Input.GetAxis("Vertical") + FloorDir_right * Input.GetAxis("Horizontal"), Color.green);
            Debug.DrawRay(transform.position + new Vector3(0, 1, 0), FloorDir * Input.GetAxis("Vertical") + FloorDir_right * Input.GetAxis("Horizontal"), Color.green);

            if (Physics.Raycast(transform.position, FloorDir * Input.GetAxis("Vertical") + FloorDir_right * Input.GetAxis("Horizontal"), FeetCollider.radius * 2f, FeetCheckLayer) && Physics.Raycast(transform.position + new Vector3(0, PositionOffset.y, 0), FloorDir * Input.GetAxis("Vertical") + FloorDir_right * Input.GetAxis("Horizontal"), FeetCollider.radius * 2f, FeetCheckLayer))//벽에 닿으면 애니메 안되게
            {
                HorizontalValue = Mathf.Lerp(HorizontalValue, 0, 7.4f * Time.deltaTime);
                VerticalValue = Mathf.Lerp(VerticalValue, 0, 7.4f * Time.deltaTime);
            }
            else
            {
                HorizontalValue = Mathf.Lerp(HorizontalValue, Input.GetAxisRaw("Horizontal") * (1 - System.Convert.ToInt32(Animator_PlayerAnimator.GetBool("Randing"))), 7.4f * Time.deltaTime);
                VerticalValue = Mathf.Lerp(VerticalValue, Input.GetAxisRaw("Vertical") * (1 - System.Convert.ToInt32(Animator_PlayerAnimator.GetBool("Randing"))), 7.4f * Time.deltaTime);
            }

            Animator_PlayerAnimator.SetFloat("Horizontal", HorizontalValue);
            Animator_PlayerAnimator.SetFloat("Vertical", VerticalValue);

        //스프린트
            Sprint = (Input.GetAxis("Sprint") != 0);//이거 반대로 하려면 !=로 바꾸셈
            Animator_PlayerAnimator.SetBool("Sprint", Sprint);

        if (isGround)
        {
            //점프
            if (Input.GetKeyDown(KeyCode.Space) && !Animator_PlayerAnimator.GetBool("Randing"))//���� ���濡 �����Ϸ��� New InputSystem ����ҰŰ��⵵ �ϰ�
            {
                CharacterJump();
            }
        }

        //인터렉션
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
        if(isGround)
        {
            isGround = false;

            float RigidY = Mathf.Clamp(RigidBody_PlayerRigid.velocity.y,0,100);//반동점프 맨이야
            RigidBody_PlayerRigid.velocity = Vector3.zero;//리지드 멈춰라잉

            //단순히 위로가는 값 + 현재 물체값의 Y만큼 반동 값 + 현재 방향으로 이동 값
            RigidBody_PlayerRigid.AddForce(Vector3.up * JumpPower + new Vector3(0, RigidY, 0) + (FloorDir_right * HorizontalValue + FloorDir * VerticalValue) * 2f, ForceMode.VelocityChange);
            HorizontalValue = 0;
            VerticalValue = 0;
            Animator_PlayerAnimator.SetBool("isGround", isGround);
        }
    }

    void CharacterMove()
    {

        AimPoint = PlayerCamera.GetCameraDir();
        CharacterRotation2Forward();

        if(isGround)
            RigidBody_PlayerRigid.MovePosition(transform.position + (FloorDir_right * HorizontalValue + FloorDir*VerticalValue) * (Sprint ? SprintSpeed : MoveSpeed) * Time.deltaTime);
        else
            RigidBody_PlayerRigid.MovePosition(transform.position + (FloorDir_right * HorizontalValue + FloorDir*VerticalValue) * MoveSpeed * 0.5f * Time.deltaTime);
    }

    void CharacterMove_Force()//AddForce로 움직이는 거시기
    {
        RigidBody_PlayerRigid.AddForce(transform.right * HorizontalValue * AirMassSpeed * Time.deltaTime + transform.forward * VerticalValue * AirMassSpeed * Time.deltaTime, ForceMode.Acceleration);
    }

    void CharacterRotation2Forward()//카메라 방향을 바라보게 하기
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {

            float YValue = PlayerCamera.CameraTargetRotation();
            YValue = YValue > 180 ? YValue - 360 : YValue;
            transform.eulerAngles = new Vector3(0, Mathf.LerpAngle(transform.eulerAngles.y, PlayerCamera.CameraTargetRotation(), 7.2f * Time.deltaTime), 0);
        }
    }


    void GravityCheck()
    {
        var ray = new Ray((transform.position + FeetCollider.center - new Vector3(0, FeetCollider.height / 2, 0)) + Vector3.up * 0.1f, Vector3.down);
        var ray_forward = ray;
        ray_forward.origin += (FloorDir + Vector3.up) * (FeetCollider.radius / 2);
        var ray_back = ray;
        ray_back.origin += (-FloorDir + Vector3.up) * (FeetCollider.radius / 2);
        var ray_right = ray;
        ray_right.origin += (FloorDir_right + Vector3.up) * (FeetCollider.radius / 2);
        var ray_left = ray;
        ray_left.origin += (-FloorDir_right + Vector3.up) * (FeetCollider.radius / 2);
        //var ray_right = ray;
        //ray_right.origin += (FloorDir_right + Vector3.up) * (FeetCollider.radius / 2);
        //var ray_left = ray;
        //ray_left.origin += (-FloorDir_right + Vector3.up) * (FeetCollider.radius / 2);

        Debug.DrawRay(ray.origin, Vector3.down, Color.yellow);
        Debug.DrawRay(ray_forward.origin, Vector3.down, Color.yellow);
        Debug.DrawRay(ray_back.origin, Vector3.down, Color.yellow);
        Debug.DrawRay(ray_right.origin, Vector3.down, Color.yellow);
        Debug.DrawRay(ray_left.origin, Vector3.down, Color.yellow);
        //Debug.DrawRay(ray_right.origin, Vector3.down, Color.yellow);
        //Debug.DrawRay(ray_left.origin, Vector3.down, Color.yellow);

        RaycastHit rayHit;
        var maxDistance = 0.2f;
        LayerMask ExceptMask = LayerMask.NameToLayer("IgnorePlayer");
        if(Mathf.Abs(HorizontalValue) > Mathf.Abs(VerticalValue))
        isGround = Physics.Raycast(ray, out rayHit, maxDistance) || Physics.Raycast(ray_right, out rayHit, maxDistance * 2) || Physics.Raycast(ray_left, out rayHit, maxDistance * 2);//ExceptMask만 제외
        else
        isGround = Physics.Raycast(ray, out rayHit, maxDistance) || Physics.Raycast(ray_forward, out rayHit, maxDistance *2) || Physics.Raycast(ray_back, out rayHit, maxDistance * 2);//ExceptMask만 제외
        //isGround = Physics.Raycast(ray, out rayHit, maxDistance);//ExceptMask만 제외
        //에어본인데 공중에 뜨는 문제
        FeetCollider.material.staticFriction = System.Convert.ToInt32(isGround) * 0.5f;
        FeetCollider.material.frictionCombine = isGround ? PhysicMaterialCombine.Maximum : PhysicMaterialCombine.Minimum;

        if (isGround)//충돌 있음
        {
            //PointedsYPos = rayHit.point.y;//바닥 Y축 확인쓰

            FloorDir = Vector3.Cross(transform.right, rayHit.normal);
            FloorDir_right = Vector3.Cross(rayHit.normal, transform.forward);//rhs를 FloorDir로 사용하면 normal이 뒤집어질수록 값이 줄어든다(오르막에서 right의 값이 줄어드는 현상 발생)

            Debug.DrawRay(rayHit.point, FloorDir, Color.blue);
            Debug.DrawRay(rayHit.point, FloorDir_right, Color.blue);
        }
        else//충돌 없음
        { 
            FloorDir = transform.forward;
            FloorDir_right = transform.right;
        }

            Animator_PlayerAnimator.SetFloat("Velocity_Y",RigidBody_PlayerRigid.velocity.y);
            Animator_PlayerAnimator.SetBool("isGround", isGround);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isDead)
        {
            if (CheckBreakForce(collision, 4.5f, 12f) || CheckDangerObject(collision, "DangerObject", 0f))
                PlayerDead(true);//���� ó��
        }

        if(collision.transform.tag == "Platform")
        {
            transform.parent = collision.transform;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        transform.parent = null;
    }

    bool CheckDangerObject(Collision collision, string tag ,float DamageForce = 0f)
    {
        if (collision.transform.tag == tag)
        {
            return (Mathf.Abs(collision.relativeVelocity.x) > DamageForce || Mathf.Abs(collision.relativeVelocity.y) > DamageForce || Mathf.Abs(collision.relativeVelocity.z) > DamageForce);//���� �̻��� ����� ������
        }
        return false;
    }

    bool CheckBreakForce(Collision collision, float BreakForce, float YBreakForce)//Y가 더 충격을 많이 받는걸 감안
    {
        //Debug.Log(string.Format("relativeVelocity = {0}",collision.relativeVelocity));
        return (Mathf.Abs(collision.relativeVelocity.x) > BreakForce || Mathf.Abs(collision.relativeVelocity.y) > YBreakForce || Mathf.Abs(collision.relativeVelocity.z) > BreakForce);//���� �̻��� ����� ������
    }

    public void PlayerDead(bool Dead)//플레이어 죽는거 처리
    {
        isDead = Dead;
        RigidBody_PlayerRigid.velocity = Vector3.zero;
        RigidBody_PlayerRigid.isKinematic = Dead;
        VisibleRagdoll(Dead);
    }
}
