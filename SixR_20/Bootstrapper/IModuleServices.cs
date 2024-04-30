using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixR_20.Bootstrapper
{
    public interface IModuleServices
    {
        void ActivateView(string viewName,string regionName= "MainRegion");
    }
}
