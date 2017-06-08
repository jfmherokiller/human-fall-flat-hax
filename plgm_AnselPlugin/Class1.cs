

using FallFlatHelpers;
using NVIDIA;
using PluginContract;
using static Player;

namespace plgm_AnselPlugin
{
    public class Class1 : IPlugin
    {
        public string Name => "Ansel";
        public void initPlugin()
        {
            var playerstuff = PlayerHelpers.GetPlayerInstance();
            var ansel = playerstuff.gameObject.AddComponent<Ansel>();
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
            // set to false to completely disable Ansel 
            ansel.ConfigureSession(session);

        }
    }
}
