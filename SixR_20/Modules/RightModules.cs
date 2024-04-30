﻿using System;
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
using SixR_20.Views.RightRegion;

namespace SixR_20.Modules
{
    class RightModules : IModule
    {

        private readonly IUnityContainer moduleContainer;

        private readonly string moduleName = "RightModules";

        public RightModules(IUnityContainer container)
        {
            moduleContainer = container;
        }

        ~RightModules()
        {
            var eventAggregator = moduleContainer.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
            viewRequestedEvent.Unsubscribe(ViewRequestedEventHandler);
        }

        public void Initialize()
        {
            var regionManager = moduleContainer.Resolve<IRegionManager>();
            //regionManager.Regions["RightRegion"].Add(new ShowCartesianPositionView(moduleContainer), "ShowCartesianPositionView");

            var eventAggregator = moduleContainer.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
            viewRequestedEvent.Subscribe(this.ViewRequestedEventHandler, true);
        }

        public void ViewRequestedEventHandler(string s)
        {
            dynamic Command = JsonConvert.DeserializeObject(s);
            if (Command.command != "ActivateView" || (Command.RegionName == null)) return;
            var moduleServices = moduleContainer.Resolve<IModuleServices>();
            if (Command.RegionName.ToString() == "RightRegion")
                moduleServices.ActivateView(Command.ModuleName.ToString(), Command.RegionName.ToString());


        }
    }
}
