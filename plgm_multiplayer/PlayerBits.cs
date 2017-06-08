using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FallFlatHelpers;
using UnityEngine;
using UnityEngine.Networking;

namespace plgm_multiplayer
{
   public static class PlayerBits
    {
        public static void AddNetwork()
        {
            var playerinstance = PlayerHelpers.GetPlayerInstance();
            var netident = playerinstance.AddComponent<NetworkIdentity>();
            netident.localPlayerAuthority = true;
        }
    }
}
