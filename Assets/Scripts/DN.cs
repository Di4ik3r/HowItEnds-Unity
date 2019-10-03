using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DN
{ 
    private GameObject Sun;
    private GameObject Moon;

    public DN(GameObject Sunlight, GameObject Moonlight)
    {

        //GameObject sun = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //sun.transform.position = new Vector3(1, 1, 2);
        //dn.CreateSphere(gameObject: Luna, color: Color.yellow);

        //dn = new DayNightCycle(Ra, Sun);
        //dn.CreateSphere(gameObject: Sun, color: Color.yellow, intenstity: 2f,name: "Luna");
        CreateSphere(Sunlight, Color.yellow, 3f, "S");
    }
    public GameObject CreateSphere(GameObject gameObject, Color color, float intenstity, string name)
    {
        // Look what diff between Get and Add
        gameObject.name = name;
        gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //gameObject.transform.position = new Vector3(mapSize.x, map, 5);
        gameObject.GetComponent<Renderer>().material.color = color;
        gameObject.AddComponent<Light>().type = LightType.Directional;
        gameObject.GetComponent<Light>().shadows = LightShadows.Soft;
        gameObject.GetComponent<Light>().intensity = intenstity;

        return gameObject;
    }
}
