using UnityEngine;
using UnityEngine.SceneManagement;

partial class tests
{
    public class Hackobject : MonoBehaviour
    {
        void findCanvas()
        {
            var canvases = GameObject.FindObjectsOfType<UICanvas>();
            if (canvases.Length > 0)
                Debug.Log(canvases.Length);
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F3))
            {
                //GenericHelpers.CreateGameObjectAndAttachClassAndAllowDestory<Bricktest>();
                for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
                {
                    var sceneobject = SceneManager.GetSceneAt(sceneIndex);
                    var scenename = sceneobject.name;
                    var scenepath = sceneobject.path;
                    var scenerootobjects = sceneobject.GetRootGameObjects();

                    Debug.Log(scenename +":" +scenepath + ":" + scenerootobjects);
                }
            }
        }

        void OnGUI()
        {
            //GUI.Label(new Rect(10, 10, 100, 20), "Hello World!");
        }
    }
}