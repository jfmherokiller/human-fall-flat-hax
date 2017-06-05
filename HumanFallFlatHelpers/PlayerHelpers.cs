﻿using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;

namespace HumanFallFlatHelpers
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

        public static Player GetPlayerMonoBehaviour()
        {
            var playerinstance = GetPlayerInstance();
            if (playerinstance != null)
            {
                return playerinstance.GetComponent<MonoBehaviour>() as Player;
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
            var playerinstance = GetPlayerMonoBehaviour();
            if (playerinstance != null)
            {
                return playerinstance.human.ragdoll.partHead.transform.position;
            }
            return new Vector3(0, 0, 0);
        }
    }
}