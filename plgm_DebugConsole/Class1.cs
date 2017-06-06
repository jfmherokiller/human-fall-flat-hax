using plgm_DebugConsole.EasyConsole.Commands;
using plgm_DebugConsole.EasyConsole.FrontEnd.UnityGUI;
using plgm_DebugConsole.EasyConsole.Parsers;
using PluginContract;
using UnityEngine;

namespace plgm_DebugConsole
{
    public class Class1 : IPlugin
    {
        public string Name => "DevConsolePlugin";

        public void initPlugin()
        {
            var gameobj = new GameObject();
            var console = gameobj.AddComponent<Console>();
            var parser = gameobj.AddComponent<ConsoleParsers>();
            var watch = gameobj.AddComponent<ConsoleWatch>();
            var gui = gameobj.AddComponent<ConsoleGUI>();
            GameObject.DontDestroyOnLoad(gameobj);
        }
    }
}