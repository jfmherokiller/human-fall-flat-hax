

using FallFlatHelpers;
using NVIDIA;
using PluginContract;
using UnityEngine;
using static Player;

namespace plgm_AnselPlugin
{
    public class Class1 : IPlugin
    {
        public string Name => "Ansel";

        private static void LoadAnsel()
        {
            var player = PlayerHelpers.GetPlayerMonoBehaviour() as Player;
            var camera = player.cameraController.gameCam.gameObject;
            var session = new Ansel.SessionData
            {
                isAnselAllowed = true,
                isFovChangeAllowed = true,
                isHighresAllowed = true,
                isPauseAllowed = true,
                isRotationAllowed = false,
                isTranslationAllowed = false,
                is360StereoAllowed = false,
                is360MonoAllowed = false
            };
            var ansel = camera.AddComponent<Ansel>();
            Object.DontDestroyOnLoad(ansel);
            ansel.ConfigureSession(session);
        }
        public void initPlugin()
        {
            LoadAnsel();
        }
    }
}
