using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace human_fall_flat_hax
{
   public class hookies
    {
        private Dictionary<string, IPlugin> _Plugins;



        public void LoadPluginsIntoList(string path)
        {
            _Plugins = new Dictionary<string, IPlugin>();
            var plugins = PluginLoader.LoadPlugins(path);
            foreach (var item in plugins)
            {
                _Plugins.Add(item.Name, item);
            }
        }


        public void allhooks()
        {
            var basedirectory = Path.GetDirectoryName(Application.dataPath);
            if (basedirectory != null)
            {
                var pluginloaddirectory = Path.GetFullPath(Path.Combine(basedirectory,"coreplugins"));
                Debug.Log(pluginloaddirectory);
                LoadPluginsIntoList(pluginloaddirectory);
                Debug.Log(_Plugins.ToString());
            }
        }
    }
}
