using System;

namespace plgm_DebugConsole.EasyConsole.Homens
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HelpAttribute : Attribute
    {
        public readonly string helpText;

        public HelpAttribute(string helpText)
        {
            this.helpText = helpText;
        }
    }
}