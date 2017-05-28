using PluginContract;

namespace TestPlugin
{
    public class TestPlugin : IPlugin
    {
        public string Name => "First Plugin";

        void scenetest()
        {
            tests.runtests();
        }

        public void initPlugin()
        {
            tests.runtests();
        }
    }
}