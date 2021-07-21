using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootPlacement : MonoBehaviour
{
    [SerializeField] LayerMask FeetCheckLayer;
    Animator animator;
    [Range(0,0.1f)]
    [SerializeField]float DistanceToGround;
    float WeightValue = 0;
    private Quaternion FootRot_L;
    private Quaternion FootRot_R;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            //float WalkValue = Mathf.Max(Mathf.Abs(animator.GetFloat("Horizontal")), Mathf.Abs(animator.GetFloat("Vertical")));

            WeightValue = Mathf.Lerp(WeightValue,System.Convert.ToInt32(animator.GetBool("isGround")), 2.6f*Time.deltaTime);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, WeightValue);
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, WeightValue);

            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, WeightValue);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, WeightValue);

            RaycastHit rayHit_l;
            Ray ray = new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up * 0.1f, Vector3.down);
            if (Physics.Raycast(ray, out rayHit_l, DistanceToGround + 1f, FeetCheckLayer))
            {
                Vector3 footPosition = rayHit_l.point;
                footPosition.y += DistanceToGround;
                animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                FootRot_L = Quaternion.LookRotation(transform.forward, rayHit_l.normal);
                animator.SetIKRotation(AvatarIKGoal.LeftFoot, FootRot_L);
            }
            RaycastHit rayHit_r;
            Ray ray_r = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up * 0.1f, Vector3.down);
            if (Physics.Raycast(ray_r, out rayHit_r, DistanceToGround + 1f, FeetCheckLayer))
            {
                Vector3 footPosition = rayHit_r.point;
                footPosition.y += DistanceToGround;
                animator.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                FootRot_R = Quaternion.LookRotation(transform.forward, rayHit_r.normal);
                animator.SetIKRotation(AvatarIKGoal.RightFoot, FootRot_R);
            }
        }
    }
}
