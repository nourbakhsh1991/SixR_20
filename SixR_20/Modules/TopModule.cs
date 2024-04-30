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

namespace SixR_20.Modules
{
    class TopModule : IModule
    {

        private readonly IUnityContainer moduleContainer;

        private readonly string moduleName = "BottomModule";

        public TopModule(IUnityContainer container)
        {
            moduleContainer = container;
        }

        ~TopModule()
        {
            var eventAggregator = moduleContainer.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
            viewRequestedEvent.Unsubscribe(ViewRequestedEventHandler);
        }

        public void Initialize()
        {
            var eventAggregator = moduleContainer.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
            viewRequestedEvent.Subscribe(this.ViewRequestedEventHandler, true);
        }

        public void ViewRequestedEventHandler(string s)
        {
            dynamic Command = JsonConvert.DeserializeObject(s);
            if (Command.command != "ActivateView" || (Command.RegionName == null)) return;
            var moduleServices = moduleContainer.Resolve<IModuleServices>();
            if (Command.RegionName.ToString() == "HeaderRegion")
                moduleServices.ActivateView(Command.ModuleName.ToString(), Command.RegionName.ToString());
        }
    }
}
