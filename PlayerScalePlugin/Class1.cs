using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginContract;
using UnityEngine;

namespace PlayerScalePlugin
{
    public class Class1: IPlugin
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

        public string Name => "Player Scale Plugin";
        public void initPlugin()
        {
            helperfunctions.CreateGameObjectAndAttachClass<GameObjectScaler>();
        }
    }

    public class GameObjectScaler : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                var playerinstance = Class1.GetPlayerInstance();
                if (playerinstance != null)
                {
                    playerinstance.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
                }
            }
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                var playerinstance = Class1.GetPlayerInstance();
                if (playerinstance != null)
                {
                    playerinstance.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                }
            }
        }
    }
}
