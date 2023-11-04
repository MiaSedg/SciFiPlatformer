using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.VisualScripting;

namespace game
{
    public class CameraPivot : MonoBehaviour
    {


        // can add a connection to InputHandler so that scroll wheel can shorten or lengthen distance bewteen pivot and main camera handler transform

        public void FixedUpdate()
        {
            transform.rotation = CameraHandler.singleton.cameraPivotTransform.rotation;
        }
    }
}
