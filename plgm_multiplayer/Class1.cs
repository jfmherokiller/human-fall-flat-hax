
using FallFlatHelpers;
using PluginContract;
using UnityEngine;
using UnityEngine.Networking;

namespace plgm_multiplayer
{
    public class Class1 : IPlugin
    {
        public string Name => "Multiplayer";
        public void initPlugin()
        {
            PlayerBits.AddNetwork();
            var gameobj = new GameObject();
            var netman = gameobj.AddComponent<NetworkManager>();
            gameobj.AddComponent<NetworkManagerHUD>();
            netman.playerPrefab = PlayerHelpers.GetPlayerInstance();
            Object.DontDestroyOnLoad(gameobj);
            
        }
    }
}
