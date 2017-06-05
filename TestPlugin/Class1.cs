using PluginContract;

namespace TestPlugin
{
    public class TestPlugin : IPlugin
    {
        public string Name => "First Plugin";
        public bool isloaded { get; set; }

        void scenetest()
        {
            tests.runtests();
        }

        public void initPlugin()
        {
            isloaded = true;
            tests.runtests();
        }
        
    }
}