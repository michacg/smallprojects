using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeckoController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform headBone;
    [SerializeField] Transform leftEyeBone;
    [SerializeField] Transform rightEyeBone;

    [SerializeField] float headMaxTurnAngle;
    [SerializeField] float headTrackingSpeed;
    [SerializeField] float eyeTrackingSpeed;
    [SerializeField] float leftEyeMaxYRotation;
    [SerializeField] float leftEyeMinYRotation;
    [SerializeField] float rightEyeMaxYRotation;
    [SerializeField] float rightEyeMinYRotation;

    //animation code
    private void LateUpdate()
    {
        HeadTrackingUpdate();
        EyeTrackingUpdate();
    }

    void HeadTrackingUpdate()
    {
        Quaternion currentLocalRotation = headBone.localRotation;

        headBone.localRotation = Quaternion.identity;

        Vector3 targetWorldLookDir = target.position - headBone.position;
        Vector3 targetLocalLookDir = headBone.InverseTransformDirection(targetWorldLookDir);

        targetLocalLookDir = Vector3.RotateTowards(
            Vector3.forward,
            targetLocalLookDir,
            Mathf.Deg2Rad * headMaxTurnAngle,
            0
        );

        Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);

        //Apply smoothing
        headBone.localRotation = Quaternion.Slerp(
            currentLocalRotation,
            targetLocalRotation,
            1 - Mathf.Exp(-headTrackingSpeed * Time.deltaTime)
        );
    }

    void EyeTrackingUpdate()
    {
        Quaternion targetEyeRotation = Quaternion.LookRotation(
            target.position - headBone.position, // toward target
            transform.up
        );

        leftEyeBone.rotation = Quaternion.Slerp(
            leftEyeBone.rotation,
            targetEyeRotation,
            1 - Mathf.Exp(-eyeTrackingSpeed * Time.deltaTime)
        );

        rightEyeBone.rotation = Quaternion.Slerp(
            rightEyeBone.rotation,
            targetEyeRotation,
            1 - Mathf.Exp(-eyeTrackingSpeed * Time.deltaTime)
        );

        float leftEyeCurrentYRotation = leftEyeBone.localEulerAngles.y;
        float rightEyeCurrentYRotation = rightEyeBone.localEulerAngles.y;

        // Move the rotation to a -180 ~ 180 range
        if (leftEyeCurrentYRotation > 180)
        {
            leftEyeCurrentYRotation -= 360;
        }
        if (rightEyeCurrentYRotation > 180)
        {
            rightEyeCurrentYRotation -= 360;
        }

        // Clamp the Y axis rotation
        float leftEyeClampedYRotation = Mathf.Clamp(
            leftEyeCurrentYRotation,
            leftEyeMinYRotation,
            leftEyeMaxYRotation
        );
        float rightEyeClampedYRotation = Mathf.Clamp(
            rightEyeCurrentYRotation,
            rightEyeMinYRotation,
            rightEyeMaxYRotation
        );

        // Apply the clamped Y rotation without changing the X and Z rotations
        leftEyeBone.localEulerAngles = new Vector3(
            leftEyeBone.localEulerAngles.x,
            leftEyeClampedYRotation,
            leftEyeBone.localEulerAngles.z
        );
        rightEyeBone.localEulerAngles = new Vector3(
            rightEyeBone.localEulerAngles.x,
            rightEyeClampedYRotation,
            rightEyeBone.localEulerAngles.z
        );
    }
}
