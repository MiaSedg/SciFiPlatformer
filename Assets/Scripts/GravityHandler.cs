using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace game
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SurfaceHandler))]
    public class GravityHandler : MonoBehaviour
    {
        public static Vector3 globalGravity = Vector3.down * 10f;
        public Vector3 localGravity;

        Rigidbody rb;
        SurfaceHandler surfaceHandler;

        public void Awake()
        {
            rb = GetComponent<Rigidbody>();
            surfaceHandler = GetComponent<SurfaceHandler>();
        }
        public void FixedUpdate()
        {
            if (!surfaceHandler.isGrounded)
            {
                ApplyGravity();

                Debug.Log("we are doing gravity");

            }
            
        }

        public void ApplyGravity()
        {
            rb.AddForce(globalGravity, ForceMode.Force);
        }
    }
}
