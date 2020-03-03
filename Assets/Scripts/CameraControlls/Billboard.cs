using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform mainCamera;
    
    void Awake()
    {
        mainCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (gameObject.activeSelf)
        {
            transform.LookAt(mainCamera.position + mainCamera.forward);
        }
    }
}
