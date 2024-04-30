using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Prism.Events;
using Prism.Modularity;
using Prism.Regions;
using SixR_20.Bootstrapper;
using SixR_20.Views;
using SixR_20.Views.LeftRegion;

namespace SixR_20.Modules
{
    class LeftModule : IModule
    {

        private readonly IUnityContainer moduleContainer;

        private readonly string moduleName = "LeftModules";

        public LeftModule(IUnityContainer container)
        {
            moduleContainer = container;
        }

        ~LeftModule()
        {
            var eventAggregator = moduleContainer.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
            viewRequestedEvent.Unsubscribe(ViewRequestedEventHandler);
        }

        public void Initialize()
        {
            var regionManager = moduleContainer.Resolve<IRegionManager>();
            //regionManager.Regions["LeftRegion"].Add(new OperationModeView(moduleContainer), moduleName);
            var eventAggregator = moduleContainer.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
            viewRequestedEvent.Subscribe(this.ViewRequestedEventHandler, true);
        }

        public void ViewRequestedEventHandler(string s)
        {
            dynamic Command = JsonConvert.DeserializeObject(s);
            if (Command.command != "ActivateView" || (Command.RegionName == null)) return;
            var moduleServices = moduleContainer.Resolve<IModuleServices>();
            if (Command.RegionName.ToString() == "LeftRegion")
                moduleServices.ActivateView(Command.ModuleName.ToString(), Command.RegionName.ToString());

        }
    }
}
