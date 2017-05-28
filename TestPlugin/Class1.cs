



namespace TestPlugin
{
    public class TestPlugin: IPlugin
    {
        public string _Name = "TestPlugin";
        void scenetest()
        {
            tests.runtests();
        }

        public string Name => _Name;

        public void initPlugin()
        {
            tests.runtests();
        }
    }
}
