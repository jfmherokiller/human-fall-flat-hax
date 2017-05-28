using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


    class helperfunctions
    {
        public static GameObject fakelink;
        public static void GetGameObjectsInScene()
        {
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            foreach (GameObject go in allObjects)
                if (go.activeInHierarchy)
                    Debug.Log(go.ToString());
        }

        public static void CreateGameObjectAndAttachClass<T>() where T : MonoBehaviour
        {
            fakelink = new GameObject("SceneHack");
            fakelink.AddComponent<T>();
            GameObject.DontDestroyOnLoad(fakelink);
        }
    }
