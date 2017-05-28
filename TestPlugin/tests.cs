using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HumanFallFlatHelpers;
using UnityEngine;

class tests
{
    class bricktest : MonoBehaviour
    {
        void Start()
        {
            var playerposition = PlayerHelpers.GetPlayerHeadPosition();
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.AddComponent<Rigidbody>();
            cube.transform.position = new Vector3(playerposition.x, playerposition.y, playerposition.z);
        }
    }

    public class Hackobject : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F3))
            {
                GenericHelpers.CreateGameObjectAndAttachClassAndAllowDestory<bricktest>();
            }
        }

        void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 100, 20), "Hello World!");
        }
    }

    public static void runtests()
    {
        //GenericHelpers.CreateGameObjectAndAttachClass<bricktest>();
        GenericHelpers.CreateGameObjectAndAttachClass<Hackobject>();
    }
}