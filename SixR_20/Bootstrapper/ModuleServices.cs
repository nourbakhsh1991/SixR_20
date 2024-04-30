using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prism.Regions;
using SixR_20.Views.BottomRegion;
using SixR_20.Views.LeftRegion;
using SixR_20.Views.MainRegion;
using SixR_20.Views.RightRegion;
using SixR_20.Views.Shared;
using SixR_20.Views.TopRegion;

namespace SixR_20.Bootstrapper
{
    public class ModuleServices : IModuleServices
    {
        private readonly IUnityContainer m_Container;
        private static Dictionary<string, Dictionary<string,object>> views = new Dictionary<string, Dictionary<string, object>>();

        public ModuleServices(IUnityContainer container)
        {
            m_Container = container;
        }

        public void ActivateView(string viewName, string regionName = "MainRegion")
        {
            var regionManager = m_Container.Resolve<IRegionManager>();

            // غیر فعال کردن ویو
            IRegion workspaceRegion = regionManager.Regions[regionName];
            var views = workspaceRegion.Views;
            //foreach (var view in views)
            //{
            //    workspaceRegion.Deactivate(view);
            //}
            var region = ModuleServices.views.FirstOrDefault(a => a.Key == regionName);
            KeyValuePair<string, object> view = new KeyValuePair<string, object>(null,null);
            if (region.Value != null)
            {
                view = region.Value.FirstOrDefault(a => a.Key == viewName);
            }
            workspaceRegion.RemoveAll();
            if (region.Value != null &&  view.Value!=null )
            {
                regionManager.Regions[regionName].Add(view.Value, viewName);
            }
            else
            {
                switch (viewName)
                {
                    case "SelfTestView":
                        regionManager.Regions[regionName].Add(new SelfTestView(m_Container), "SelfTestView");
                        break;
                    case "PositionView":
                        regionManager.Regions[regionName].Add(new PositionView(m_Container), "PositionView");
                        break;
                    case "EmptyPageView":
                        regionManager.Regions[regionName].Add(new EmptyPageView(m_Container), "EmptyPageView");
                        break;
                    case "AngleChartView":
                        regionManager.Regions[regionName].Add(new AngleChartView(m_Container), "AngleChartView");
                        break;
                    case "ShowCartesianPositionView":
                        regionManager.Regions[regionName].Add(new ShowCartesianPositionView(m_Container), "ShowCartesianPositionView");
                        break;
                    case "HeaderView":
                        regionManager.Regions[regionName].Add(new HeaderView(m_Container), "HeaderView");
                        break;
                    case "OperationModeView":
                        regionManager.Regions[regionName].Add(new OperationModeView(m_Container), "OperationModeView");
                        break;
                    case "JogView":
                        regionManager.Regions[regionName].Add(new JogView(m_Container), "JogView");
                        break;
                    case "TrajectoryView":
                        regionManager.Regions[regionName].Add(new TrajectoryView(m_Container), "TrajectoryView");
                        break;
                }
            }

            if (region.Value == null)
            {
                ModuleServices.views.Add(regionName, new Dictionary<string, object>());
            }
            if (view.Value == null)
            {
                ModuleServices.views[regionName].Add(viewName, workspaceRegion.Views.First());
            }
            //فعال کردن ویو انتخاب شده
            //var viewToActivate = regionManager.Regions[regionName].GetView(viewName);
            //  regionManager.Regions[regionName].Activate(viewToActivate);
        }
    }
}
