using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Modularity;
using Prism.Regions;
using SixR_20.Bootstrapper;
using SixR_20.Views;

namespace SixR_20.Modules
{
    // [Module(ModuleName = "HelloWorldModule")]
    class HelloWorldModule : IModule
    {
        private readonly IRegionViewRegistry regionViewRegistry;

        //public HelloWorldModule(IRegionViewRegistry registry)
        //{
        //    this.regionViewRegistry = registry;
        //}


        private readonly IUnityContainer moduleContainer;

        private readonly string moduleName = "HelloWorldModule";

        public HelloWorldModule(IUnityContainer container)
        {
            moduleContainer = container;
        }

        ~HelloWorldModule()
        {
            var eventAggregator = moduleContainer.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
            viewRequestedEvent.Unsubscribe(ViewRequestedEventHandler);
        }

        public void Initialize()
        {
            var regionManager = moduleContainer.Resolve<IRegionManager>();
            regionManager.Regions["MainRegion"].Add(new user(moduleContainer), moduleName);
            //regionViewRegistry.RegisterViewWithRegion("MainRegion", typeof(Views.user));
            var eventAggregator = moduleContainer.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
            viewRequestedEvent.Subscribe(this.ViewRequestedEventHandler, true);
            //viewRequestedEvent.Publish("Hello!");
        }

        public void ViewRequestedEventHandler(string moduleName)
        {
            if (this.moduleName != moduleName) return;

            var moduleServices = moduleContainer.Resolve<IModuleServices>();
            moduleServices.ActivateView(moduleName);
        }
    }
}
