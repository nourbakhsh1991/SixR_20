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
using SixR_20.Views.BottomRegion;
using SixR_20.Views.MainRegion;
using SixR_20.Views.Shared;

namespace SixR_20.Modules
{
    class BottomModule : IModule
    {

        private readonly IUnityContainer moduleContainer;

        private readonly string moduleName = "BottomModule";

        public BottomModule(IUnityContainer container)
        {
            moduleContainer = container;
        }

        ~BottomModule()
        {
            var eventAggregator = moduleContainer.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
            viewRequestedEvent.Unsubscribe(ViewRequestedEventHandler);
        }

        public void Initialize()
        {
            var regionManager = moduleContainer.Resolve<IRegionManager>();

            //regionManager.Regions["BottomRegion"].Add(new AngleChartView(moduleContainer), "SelfTestView");
            //regionManager.Regions["BottomRegion"].Add(new EmptyPageView(moduleContainer), "EmptyPageView");
            var eventAggregator = moduleContainer.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
            viewRequestedEvent.Subscribe(this.ViewRequestedEventHandler, true);
        }

        public void ViewRequestedEventHandler(string s)
        {
            dynamic Command = JsonConvert.DeserializeObject(s);
            if (Command.command != "ActivateView" || (Command.RegionName == null)) return;
            var moduleServices = moduleContainer.Resolve<IModuleServices>();
            if (Command.RegionName.ToString() == "BottomRegion")
                moduleServices.ActivateView(Command.ModuleName.ToString(), Command.RegionName.ToString());
        }
    }
}
