using UnityEngine;
using Helper;

namespace game
{
    public class InputHandler : Singleton<InputHandler>
    {


        public float moveAmount; 
        public float jumpInput;
        public Vector2 mouseInput;
        public Vector2 movementInput;

        public PlayerControls inputActions; 
        
        Vector2 movementIn;
        Vector2 mouseIn;
        float jumpIn; 

        public void OnEnable()
        {

            if(inputActions == null)
            { 
                inputActions = new PlayerControls();
                inputActions.PlayerActMap.Camera.performed += inAction1 => mouseIn = inAction1.ReadValue<Vector2>();
                inputActions.PlayerActMap.Movement.performed += inAction2 =>  movementIn = inAction2.ReadValue<Vector2>();
                inputActions.PlayerActMap.Jump.performed += inAction3 => jumpIn = inAction3.ReadValue<float>();
            }
            inputActions.Enable(); 
            
        }
        private void OnDisable()
        {
            inputActions.Disable();
        }

        float count = 0;
        public void TickInput(float delta)
        {
            MoveInput(delta);
        }

        // add the wasd reader
        private void MoveInput(float delta)
        {
            movementInput = movementIn;
            moveAmount = Mathf.Clamp01(Mathf.Abs(movementIn.x) + Mathf.Abs(movementIn.y));
            mouseInput = mouseIn;
            jumpInput = jumpIn;

        }


    }
}
