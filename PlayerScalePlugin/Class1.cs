using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginContract;
using UnityEngine;
using HumanAPI;
using HumanFallFlatHelpers;

namespace PlayerScalePlugin
{
    public class Class1 : IPlugin
    {
        public bool isloaded { get; set; }
        public string Name => "Player Scale Plugin";

        public void initPlugin()
        {
            isloaded = true;
            GenericHelpers.CreateGameObjectAndAttachClass<GameObjectScaler>();
        }

    }

    public class GameObjectScaler : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                var playerinstance = PlayerHelpers.GetPlayerInstance();
                if (playerinstance != null)
                {
                    playerinstance.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
                }
            }
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                var playerinstance = PlayerHelpers.GetPlayerInstance();
                if (playerinstance != null)
                {
                    playerinstance.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                }
            }
        }
    }
}