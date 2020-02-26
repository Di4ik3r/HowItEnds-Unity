using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverEffect : MonoBehaviour
{
    Color MouseOverColor = Color.red;
    Color OriginalColor;
    MeshRenderer Renderer;

    bool IsScaled = false;

    void Start()
    {
        Renderer = GetComponent<MeshRenderer>();
        OriginalColor = Renderer.material.color;
    }
    
    void OnMouseOver()
    {
        if (!IsScaled)
        {
            Renderer.material.color = MouseOverColor;
            transform.localScale += new Vector3(20.0f, 20f, 20f);
            IsScaled = true;
        }
    }

    void OnMouseExit()
    {
        Renderer.material.color = OriginalColor;
        transform.localScale -= new Vector3(20.0f, 20f, 20f);
        IsScaled = false;
    }
}
