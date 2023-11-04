using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game
{
    public class SurfaceHandler : MonoBehaviour
    {

 
        int layerMask = 1 << 8 | 1 << 4;
        RaycastHit hit;
        Vector3 rayOrigin;
        public Vector3 currentSurface;
        float groundCheckLen = 1.2f;
        Vector3 groundCheckDir => (currentSurface * -1);

        public bool isGrounded = true;


         public void Start()
        {
            FindCurrentSurface();


        }

        public void FixedUpdate()
        {
 
            if(Physics.Raycast(transform.position, groundCheckDir, out hit, groundCheckLen, layerMask))
            {
                isGrounded = true;
            }
            else
                isGrounded= false;
            
        }
        public void FindCurrentSurface()
        {
            if(currentSurface == null || currentSurface == Vector3.zero)
            {
                currentSurface = Vector3.up;
            }
            // can have a collision box on body that collects potential surface normals
            // can select the surface by moving a direction
        }

        
    }
}
