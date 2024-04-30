using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Unity;
using Prism.Events;
using SixR_20.Bootstrapper;

namespace SixR_20
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            FirstFloor.ModernUI.Presentation.AppearanceManager.Current.ThemeSource =
               new Uri("pack://application:,,,/Themes/AppTheme.xaml");
            var bootstrapper = new SixRBootstrapper();
            using (var p = Process.GetCurrentProcess())
            {
                p.PriorityClass = ProcessPriorityClass.RealTime;
            }
            bootstrapper.Run();
            bootstrapper.Container.Resolve<IEventAggregator>().GetEvent<ViewRequestedEvent>().Publish("{'command':'ActivateView','ModuleName':'SelfTestView','RegionName':'MainRegion'}");
            bootstrapper.Container.Resolve<IEventAggregator>().GetEvent<ViewRequestedEvent>().Publish("{'command':'ActivateView','ModuleName':'HeaderView','RegionName':'HeaderRegion'}");
            //bootstrapper.Container.Resolve<IEventAggregator>().GetEvent<ViewRequestedEvent>().Publish("{'command':'ActivateView','ModuleName':'EmptyPageView','RegionName':'RightRegion'}");
            //bootstrapper.Container.Resolve<IEventAggregator>().GetEvent<ViewRequestedEvent>().Publish("{'command':'ActivateView','ModuleName':'EmptyPageView','RegionName':'BottomRegion'}");
        }
    }
}
