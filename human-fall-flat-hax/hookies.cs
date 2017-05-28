using System.IO;
using UnityEngine;
namespace human_fall_flat_hax
{
   public class hookies
    {
        public void allhooks()
        {
                var pluginloaddirectory = Path.GetFullPath(Path.Combine(Application.dataPath, "Managed"));
                PluginLoader.InitializePlugins(pluginloaddirectory);
        }
    }
}
