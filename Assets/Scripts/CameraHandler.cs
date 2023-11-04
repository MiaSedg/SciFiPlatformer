using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game {
    public class CameraHandler : MonoBehaviour
    {

         
        public Transform targetTransform;
        public Transform cameraTransform;
        public Transform cameraPivotTransform;
        [HideInInspector]
        public Transform myTransform;
        public Vector3 cameraTransformPosition;
        public LayerMask ignoreLayers;

        public static CameraHandler singleton;

        public float lookSpeed = .1f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = .1f;

        public float defaultPosition;
        public float lookAngle;
        public float pivotAngle;
        public float minPivot = 10;
        public float maxPivot = 90;

        
        public void Awake()
        {
            if(singleton == null) 
            {
                singleton = this;

            }
            else
            {

                Destroy(gameObject);
            }


            myTransform = transform; 
            defaultPosition = cameraTransform.localPosition.z;
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

        }
        public void Start()
        { 

        }


        public void Followtarget(float delta)
        {
            Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);
            myTransform.position = targetPosition;
        }

        private float DistToRot(Quaternion from, Quaternion to)
        {
            return (from.eulerAngles.magnitude - to.eulerAngles.magnitude) *.0001f;
        }

        // choppy turns
        // needs to utilize either quats and or slerp

        // it somehow gets worse by the minute
        public void HandleCameraRotation(float delta, float mouseXIn, float mouseYIn)
        {
/* 
            if (Mathf.Abs(mouseXIn) != 0f || Mathf.Abs(mouseYIn) != 0f)
            {
                pivotAngle = Mathf.Clamp(pivotAngle, minPivot, maxPivot);
                Quaternion myRot = transform.rotation;
                Quaternion camRot = cameraTransform.rotation;

                // Calculate target rotations based on input
                Quaternion targetPivotRotation = Quaternion.Euler(0, mouseYIn * pivotSpeed, 0);
                Quaternion targetLookRotation = Quaternion.Euler(mouseXIn * lookSpeed, 0, 0);

                // Apply rotations to current rotation
                targetPivotRotation = Quaternion.Slerp(myRot, myRot * targetPivotRotation, delta);//DistToRot(myRot,myRot*targetLookRotation));
//                targetLookRotation = Quaternion.Slerp(camRot, camRot * targetLookRotation, delta);

                targetPivotRotation.Normalize();
                // Apply the slerped rotations
                transform.rotation = targetPivotRotation;
                Debug.Log(transform.rotation);
//                cameraPivotTransform.rotation = targetLookRotation;

            }
            else
            {
                // No input - potentially log or handle the 'idle' state here
            }

*/

            /*
            // I have to store the look and pivot data as quats
            pivotAngle = Mathf.Clamp(pivotAngle, minPivot, maxPivot);
            Quaternion myRot = transform.rotation;
            Quaternion camRot = cameraPivotTransform.rotation;
            Quaternion pivotRotation = Quaternion.Euler(0, mouseYIn * pivotSpeed * delta, 0);
            Quaternion lookRotation = Quaternion.Euler(0, 0, mouseXIn * lookSpeed * delta);

            pivotRotation = myRot * pivotRotation; 
            lookRotation = camRot * lookRotation;
            pivotRotation = Quaternion.Slerp(myRot, pivotRotation, delta);
            lookRotation = Quaternion.Slerp(camRot, lookRotation,  delta); 

            transform.rotation = lookRotation;
            cameraTransform.rotation = pivotRotation;
            */
            lookAngle += ((mouseXIn * lookSpeed)/delta)%360;
            pivotAngle -= ((mouseYIn * pivotSpeed)/delta)%360;

            lookAngle = Mathf.Clamp(pivotAngle, minPivot, maxPivot);
            Vector3 rotation = Vector3.zero;
            rotation.y = -1 * lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation); 
            targetRotation.Normalize();
            transform.rotation = targetRotation;

            rotation = Vector3.zero;
            rotation.z = pivotAngle;

            targetRotation = Quaternion.Euler(rotation); 
            targetRotation.Normalize();
            cameraPivotTransform.localRotation = targetRotation;

        }
    }
}
