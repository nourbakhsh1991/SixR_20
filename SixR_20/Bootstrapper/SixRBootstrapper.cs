using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FirstFloor.ModernUI.Windows.Controls;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Unity;
using SixR_20.Modules;
using SixR_20.Modules.MainRegion;
using SixR_20.Views;

namespace SixR_20.Bootstrapper
{
    class SixRBootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            var shell=new Shell(Container);
            return shell;
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            App.Current.MainWindow = (Window)this.Shell;
            App.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
            ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;
            moduleCatalog.AddModule(typeof(MainModule));
            moduleCatalog.AddModule(typeof(BottomModule));
            moduleCatalog.AddModule(typeof(RightModules));
            moduleCatalog.AddModule(typeof(TopModule));
            moduleCatalog.AddModule(typeof(LeftModule));
        }
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.RegisterType<IModuleServices, ModuleServices>();
        }

    }
}
