


using UnityEngine;

namespace FallFlatHelpers
{
    static class PlayerHelpers
    {
        public static GameObject GetPlayerInstance()
        {
            var player = GameObject.Find("Player");
            var playerclone = GameObject.Find("Player(Clone)");
            if (player != null)
            {
                return player;
            }
            if (playerclone != null)
            {
                return playerclone;
            }
            return null;
        }

        public static MonoBehaviour GetPlayerMonoBehaviour()
        {
            var playerinstance = GetPlayerInstance();
            if (playerinstance != null)
            {
                return playerinstance.GetComponent<MonoBehaviour>();
            }
            return null;
        }

        public static Vector3 GetPlayerLocation()
        {
            var playerinstance = GetPlayerInstance();
            if (playerinstance != null)
            {
                return playerinstance.transform.position;
            }
            return new Vector3(0, 0, 0);
        }

        public static Vector3 GetPlayerHeadPosition()
        {
            var playerinstance = GetPlayerMonoBehaviour() as Player;
            if (playerinstance != null)
            {
                return playerinstance.human.ragdoll.partHead.transform.position;
            }
            return new Vector3(0, 0, 0);
        }

        public static void AddComponentToPlayer<T>() where T : Component
        {
            var playerinstance = GetPlayerInstance();
            if (playerinstance != null)
            {
                playerinstance.AddComponent<T>();
            }
        }

        public static void RemoveComponentFromPlayer<T>() where T : Component
        {
            var playerinstance = GetPlayerInstance();
            if (playerinstance != null)
            {
                Object.Destroy(playerinstance.GetComponent<T>());
            }
        }
    }
}