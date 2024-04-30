using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prism.Events;
using SixR_20.Bootstrapper;

namespace SixR_20.ViewModels.Shared
{
    class EmptyPageViewModel : BaseViewModel
    {
        public EmptyPageViewModel(IUnityContainer container)
        {
            this.Initialize(container);
        }
        private void Initialize(IUnityContainer container)
        {
            this.Container = container;
            var eventAggregator = container.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
        }
    }
}
