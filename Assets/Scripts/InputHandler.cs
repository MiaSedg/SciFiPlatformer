using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEngine.InputSystem;

namespace game
{
    public class InputHandler : MonoBehaviour
    {
        public static InputHandler singleton;

        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;
        public float jumpForce;

        public bool jumpFlag = true;
        PlayerControls inputActions;
        CameraHandler cameraHandler;
        Vector2 movementIn;
        Vector2 cameraIn;
        float jumpIn;

        public void Awake()
        {
            singleton = this;

        }
        public void Start()
        {
            cameraHandler = CameraHandler.singleton;
        }

        public void OnEnable()
        {

            if(inputActions == null)
            { 
                inputActions = new PlayerControls();
                inputActions.PlayerActMap.Camera.performed += inAction1 =>  cameraIn = inAction1.ReadValue<Vector2>();
                inputActions.PlayerActMap.Movement.performed += inAction2 =>  movementIn = inAction2.ReadValue<Vector2>();
                inputActions.PlayerActMap.Jump.performed += inAction3 => jumpIn = jumpFlag? inAction3.ReadValue<float>() : 0;
            }
            inputActions.Enable();
            
        }
        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void FixedUpdate()
        {
            float delta = Time.deltaTime;

            if(cameraHandler != null)
            { 
                cameraHandler.Followtarget(delta);
                cameraHandler.HandleCameraRotation(delta, mouseX, mouseY);
            }
        }

        public void TickInput(float delta)
        {
            MoveInput(delta);

        }

        // add the wasd reader
        private void MoveInput(float delta)
        {
            horizontal = movementIn.x;
            vertical = movementIn.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = cameraIn.x;
            mouseY = cameraIn.y;
            jumpForce = jumpIn; 
        }


    }
}
