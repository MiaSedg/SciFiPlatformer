using System.Collections.Generic;
using UnityEngine;
 namespace game
{ 
    public enum ERotationBehavior
    {
        OrientRotationToMovement,
        UseControlRotation
    }


    [System.Serializable]
    public record RotationSettings
    {
        [Header("Control Rotation")]
        public float minPitchAngle = -45.0f;
        public float maxPitchAngle = 75.0f;

        [Header("Character Orientation")]
        public ERotationBehavior rotationBehavior = ERotationBehavior.OrientRotationToMovement;
        public float minRotationSpeed = 600.0f; // The turn speed when the player is at max speed (in degrees/second)
        public float maxRotationSpeed = 1200.0f; // The 

    }

    [System.Serializable]
    public record MovementSettings
    {
        // in meters per second

        public float acceleration = 25.0f;
        public float decceleration = 25.0f;
        public float maxHorizontalSpeed = 8.0f;
        public float jumpSpeed = 10.0f;
        public float jumpAbortSpeed = 10.0f;
    }

    [System.Serializable]
    public record GravitySettings
    {
        public float gravity = 20.0f;
        public float groundedGravity = 5.0f;
        public float maxFallSpeed = 40.0f;
    }

    [System.Serializable]
    public record SurfaceCollisions
    {
        public LayerMask surfaceLayers;
        public int numOfBoxes;
        public Vector3[] boxCastPositions;
        public Vector3[] boxCastScale;
        public Vector3[] surfaceNormals;
        public Dictionary<string,int> boxNames = new Dictionary<string,int>();
    }
    // ^can likely do similiar set up for hit box set up^

    public class Character : MonoBehaviour
    {
        public Controller controller;
        public MovementSettings movementSettings;
        public GravitySettings gravitySettings;
        public SurfaceCollisions surfaceCollisions;
        public RotationSettings rotationSettings;

        private CharacterController characterController;
        //private CharacterAnimator characterAnimator;

        private float targetHorizontalSpeed;
        private float horizontalSpeed;
        private float verticalSpeed;
        private bool justWalkedOffEdge;

        private Vector2 controlRotation; //X(pitch), Y(yaw)
        private Vector3 movementInput;
        private Vector3 lastMovementInput;
        private bool hasMovementInput;
        private bool jumpInput;
        private bool isGrounded;

        public Vector3 velocity => characterController.velocity;
        public Vector3 horizontalVelocity => characterController.velocity.SetY(0.0f);
        public Vector3 verticalVelocity => characterController.velocity.Multiply(0.0f, 1.0f, 0.0f);


        private void Awake()
        {
            controller.Init();
            controller.character = this;

            characterController = gameObject.GetComponent<CharacterController>();
            //characterAnimator = gameObject.GetComponent<CharacterAnimator>();

        }

        private void Update()
        {
            controller.OnCharacterUpdate();
        }

        private void FixedUpdate()
        {
            Tick(Time.deltaTime);
            
            controller.OnCharacterFixedUpdate();

        }

        private void Tick(float deltaTime)
        {
            UpdateHorizontalSpeed(deltaTime);
            UpdateVerticalSpeed(deltaTime); 
            
            Vector3 movement = horizontalSpeed * GetMovementInput() + verticalSpeed * Vector3.up;
            characterController.Move(movement * deltaTime);

            OrientToTargetRotation(movement.SetY(0.0f), deltaTime); 
            UpdateSurfaces(); 
//            _characterAnimator.UpdateState();
        }

        public void SetMovementInput(Vector3 movementInput)
        {
            bool hasMovementInput = movementInput.sqrMagnitude > 0.0f;

            if (!this.hasMovementInput && hasMovementInput ) 
            {
                lastMovementInput = this.movementInput;
            }

            this.movementInput = movementInput;
            this.hasMovementInput = hasMovementInput;
        }

        
        public Vector3 GetMovementInput()
        {
            Vector3 movementInput = hasMovementInput ? this.movementInput : lastMovementInput;
            if(movementInput.sqrMagnitude > 1f)
            {
                movementInput.Normalize();
            }
            return movementInput; 
        }
        
        public void SetJumpInput(bool jumpInput)
        {
            this.jumpInput = jumpInput;
        }

        public Vector2 GetControlRotation()
        {
            return controlRotation;
        }

        public void SetControlRotation(Vector2 controlRotation)
        {
            float pitchAngle = controlRotation.x;
            pitchAngle %= 360.0f;
            pitchAngle = Mathf.Clamp(pitchAngle, rotationSettings.minPitchAngle, rotationSettings.maxPitchAngle);

            float yawAngle = controlRotation.y;
            yawAngle %= 360.0f;

            this.controlRotation = new Vector2(pitchAngle, yawAngle); 
        }



        private void UpdateSurfaces()
        {
            justWalkedOffEdge = false;
           // need to figure out a good way to consitently check surfaces
           // and possibly a good way to change between which surface collission box is the ground checker
           // could have an orientation function that is always pointing up which what helps decide which boxes are facing down when the character
           // may rotate

            // have to see if current Check Surfaces function even works
        }

        private RaycastHit hit;
        private void CheckSurfaces()
        {
            Vector3 direction;
            for(int i = 0; i < surfaceCollisions.numOfBoxes; i++)
            {
                direction = transform.position - surfaceCollisions.boxCastPositions[i]; 
                Physics.BoxCast(surfaceCollisions.boxCastPositions[i], surfaceCollisions.boxCastScale[i], direction, out hit);
                surfaceCollisions.surfaceNormals[i] = hit.normal; 
            }

        }

        private void UpdateVerticalSpeed(float deltaTime) 
        {
            if(isGrounded)
            {
                verticalSpeed = -gravitySettings.groundedGravity;

                if (jumpInput)
                {
                    verticalSpeed = movementSettings.jumpSpeed;
                } 
            }
            else
            {
                // if falling
                if(!jumpInput && verticalSpeed > 0.0f)
                {
                    verticalSpeed = Mathf.MoveTowards(verticalSpeed, -gravitySettings.maxFallSpeed, movementSettings.jumpAbortSpeed * deltaTime); 
                }
                else if(justWalkedOffEdge)
                {
                    verticalSpeed = 0.0f;
                }

                verticalSpeed = Mathf.MoveTowards(verticalSpeed, -gravitySettings.maxFallSpeed, gravitySettings.gravity *  deltaTime);
            }
        }
        private void UpdateHorizontalSpeed(float deltaTIme)
        {
            Vector3 movementInput = this.movementInput;
            if(movementInput.sqrMagnitude > 1.0f)
            {
                movementInput.Normalize();
            }

            targetHorizontalSpeed = movementInput.magnitude * movementSettings.maxHorizontalSpeed;
            float acceleration = hasMovementInput ? movementSettings.acceleration : movementSettings.decceleration;

            horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, targetHorizontalSpeed, acceleration * deltaTIme);
        }

        private void OrientToTargetRotation(Vector3 horizontalMovement, float deltaTime) 
        {
            if (rotationSettings.rotationBehavior == ERotationBehavior.OrientRotationToMovement && horizontalMovement.sqrMagnitude > 0.0f)
            {
                float rotationSpeed = Mathf.Lerp(
                    rotationSettings.maxRotationSpeed, rotationSettings.minRotationSpeed, horizontalSpeed / targetHorizontalSpeed);

                Quaternion targetRotation = Quaternion.LookRotation(horizontalMovement, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * deltaTime);
            }
            else if (rotationSettings.rotationBehavior == ERotationBehavior.UseControlRotation)
            {
                Quaternion targetRotation = Quaternion.Euler(0.0f, controlRotation.y, 0.0f);
                transform.rotation = targetRotation;
            }
        }



    }
}
