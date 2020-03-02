using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    public float Speed = 2f;
    private float X;
    private float Y;

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y") * Speed, Input.GetAxis("Mouse X") * Speed, 0));
            X = transform.rotation.eulerAngles.x;
            Y = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(X, Y, 0);
        }
    }   
}

