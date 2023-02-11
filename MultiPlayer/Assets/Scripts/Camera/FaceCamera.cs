using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform mainCamerTransform;

    private void Start()
    {
        mainCamerTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + mainCamerTransform.rotation * Vector3.forward, mainCamerTransform.rotation * Vector3.up);
    }
}
