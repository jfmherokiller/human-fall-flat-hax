using FallFlatHelpers;
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
                Loadme();
                //DumpSceneData();
            }
        }

        private static void Loadme()
        {
            var noahmodel = AssetBundle.LoadFromFile(Application.dataPath + "/" + "noah_model");
            var asset = Instantiate(noahmodel.LoadAsset<GameObject>("Model_Noah_prefab"));
            var playerposition = PlayerHelpers.GetPlayerHeadPosition();
            var playerinstance = PlayerHelpers.GetPlayerMonoBehaviour() as Player;
            asset.transform.position = new Vector3(playerposition.x, playerposition.y, playerposition.z);
            //asset.AddComponent<SphereCollider>();

            foreach (var bone in asset.GetComponents<GameObject>())
            {
                Debug.Log(bone);
                bone.AddComponent<Rigidbody>();
                bone.AddComponent<SphereCollider>();
            }
            //asset.transform.SetParent(playerinstance.gameObject.transform,false);

            noahmodel.Unload(false);
        }

        private static void DumpSceneData()
        {
            //GenericHelpers.CreateGameObjectAndAttachClassAndAllowDestory<Bricktest>();
            for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
            {
                var sceneobject = SceneManager.GetSceneAt(sceneIndex);
                var scenename = sceneobject.name;
                var scenepath = sceneobject.path;
                var scenerootobjects = sceneobject.GetRootGameObjects();

                Debug.Log(scenename + ":" + scenepath + ":" + scenerootobjects);
            }
        }

        void OnGUI()
        {
            //GUI.Label(new Rect(10, 10, 100, 20), "Hello World!");
        }
    }
}