
using HumanFallFlatHelpers;
using PluginContract;

namespace plugin_PluginLoaderMenu
{
    public class PluginLoaderMenu: IPlugin
    {
        public bool isloaded { get; set; }
        public string Name => "PluginLoaderMenu";
        public void initPlugin()
        {
            isloaded = true;
           GenericHelpers.CreateGameObjectAndAttachClass<PluginMenu>();
        }
    }
}
