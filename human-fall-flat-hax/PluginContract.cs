using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace human_fall_flat_hax
{
    public interface IPlugin
    {
        string Name { get; }
        void initPlugin();
    }
}
