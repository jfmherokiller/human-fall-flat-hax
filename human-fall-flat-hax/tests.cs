using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace human_fall_flat_hax
{
    class tests
    {
        class bricktest : MonoBehaviour
        {
            public Transform brick;
            void Start()
            {
                for (int y = 0; y < 5; y++)
                {
                    for (int x = 0; x < 5; x++)
                    {
                        Instantiate(brick, new Vector3(x, y, 0), Quaternion.identity);
                    }
                }
            }
        }
        public class Hackobject : MonoBehaviour
        {

            void Update()
            {
                if (Input.GetKeyDown(KeyCode.F3))
                {
                    helperfunctions.GetGameObjectsInScene();
                }
            }
            void OnGUI()
            {
                GUI.Label(new Rect(10, 10, 100, 20), "Hello World!");
            }

        }
        public static void runtests()
        {
            //helperfunctions.CreateGameObjectAndAttachClass<bricktest>();
            helperfunctions.CreateGameObjectAndAttachClass<Hackobject>();
        }
    }
}
