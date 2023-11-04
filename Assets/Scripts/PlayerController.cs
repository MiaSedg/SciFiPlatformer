using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace game
{
    public class PlayerController : MonoBehaviour
    {
        private enum MoveState 
        {
            Disabled,
            Moving,
            Air,
            Wall
        }
        MoveState moveState = MoveState.Disabled;

        Transform cameraObject;
        InputHandler inputHandler;
        SurfaceHandler surfaceHandler;
        Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform;

        public new Rigidbody rb;
        public GameObject cam;

        [Header("Stats")]
        [SerializeField]
        float movementSpeed = 5;
        [SerializeField]
        float rotationSpeed = 10;
        [SerializeField]
        float jumpSpeed = 20;

        Vector3 velocity;

        public void Start()
        {
            rb = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            surfaceHandler = GetComponent<SurfaceHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;


        }

        public void Update()
        {
            if (!surfaceHandler.isGrounded)
                inputHandler.jumpFlag = false;
            else
                inputHandler.jumpFlag = true; 

            float delta = Time.deltaTime;

            inputHandler.TickInput(delta);

            moveDirection = Vector3.forward * inputHandler.vertical;
            moveDirection += Vector3.right * inputHandler.horizontal;
            moveDirection.Normalize(); 

            float speed = movementSpeed;
            moveDirection *= speed;

            moveDirection.y = 0;
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection,surfaceHandler.currentSurface); 

            // this all needs a good clean
            // add some acceleartion logic
            // loop in local velocity variable so I can add forces via: rb.velocity += velocity
            // this should allow rb.AddForce to function correctly 

            // before doing this clean up handler logics and put together the state machine
                // possibly make a player locomotion script


            rb.velocity = projectedVelocity; 

        }

        #region movement
        Vector3 normalVector;
        Vector3 targetPosition;

        private void HandleRotation(float delta)
        {
            Vector3 targetDir = Vector3.zero;
            float moveOverride = inputHandler.moveAmount;

            targetDir = cameraObject.forward * inputHandler.vertical;
            targetDir += cameraObject.right * inputHandler.horizontal;

            targetDir.Normalize();


            if (targetDir == Vector3.zero)
                targetDir = myTransform.forward;

            float rs = rotationSpeed;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

            myTransform.rotation = targetRotation;
        }

        #endregion 
    }
}
