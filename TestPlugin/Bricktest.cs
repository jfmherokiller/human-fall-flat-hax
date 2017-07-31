using FallFlatHelpers;
using UnityEngine;

partial class tests
{
    private class Bricktest : MonoBehaviour
    {
        void Start()
        {
            var playerposition = PlayerHelpers.GetPlayerHeadPosition();
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.AddComponent<Rigidbody>();
            cube.transform.position = new Vector3(playerposition.x, playerposition.y, playerposition.z);
        }
    }
}