using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class helperfunctions
{
    public static void GetGameObjectsInScene()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
            if (go.activeInHierarchy)
                Debug.Log(go.ToString());
    }
}