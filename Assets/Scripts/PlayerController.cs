using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace game
{
   
    [CreateAssetMenu(fileName = "PlayerController", menuName = "Naughtycharacter/PlayerController")]
    public class PlayerController : Controller
    {
        public float ControlRotationSensitivity = 1.0f;

        public InputHandler playerInput;
        public PlayerCamera playerCam;
        


        public override void Init()
        {

            playerInput = InputHandler.Instance;
            playerCam = PlayerCamera.Instance;

        }

        public override void OnCharacterUpdate()
        {
            playerInput.TickInput(Time.deltaTime); 
            UpdateControlRotation();
            character.SetMovementInput(GetMovementInput());
            character.SetJumpInput(playerInput.jumpInput > 0f);
        }

        public override void OnCharacterFixedUpdate()
        {
            playerCam.SetPosition(character.transform.position);
            playerCam.SetControlRotation(character.GetControlRotation());
        }

        private void UpdateControlRotation()
        {
            Vector2 camInput = playerInput.mouseInput;
            Vector2 controlRotation = character.GetControlRotation();

            // Adjust the pitch angle (X Rotation)
            float pitchAngle = controlRotation.x;
            pitchAngle -= camInput.y * ControlRotationSensitivity;

            // Adjust the yaw angle (Y Rotation)
            float yawAngle = controlRotation.y;
            yawAngle += camInput.x * ControlRotationSensitivity;

            controlRotation = new Vector2(pitchAngle, yawAngle);
            character.SetControlRotation(controlRotation);
        }

        private Vector3 GetMovementInput()
        {
            // Calculate the move direction relative to the character's yaw rotation
            Quaternion yawRotation = Quaternion.Euler(0.0f, character.GetControlRotation().y, 0.0f);
            Vector3 forward = yawRotation * Vector3.forward;
            Vector3 right = yawRotation * Vector3.right;
            Vector3 movementInput = (forward * playerInput.movementInput.y + right * playerInput.movementInput.x);

            if (movementInput.sqrMagnitude > 1f)
            {
                movementInput.Normalize();
            }

            return movementInput;
        }
    }

}
