using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FirstFloor.ModernUI.Presentation;
using Microsoft.Practices.Unity;
using Prism.Events;
using SixR_20.Bootstrapper;
using SixR_20.Models;

namespace SixR_20.ViewModels.LeftRegion
{
    class OperationModeViewModel : BaseViewModel
    {

        public ICommand PositionModeCommand { get; internal set; }
        public ICommand JogModeCommand { get; internal set; }
        public ICommand TrajectoryModeCommand { get; internal set; }
        private int _currentMode;


        public int CurrentMode
        {
            get { return _currentMode; }
            set { _currentMode = value;OnPropertyChanged(nameof(CurrentMode)); }
        }
        public OperationModeViewModel(IUnityContainer container)
        {
            this.Initialize(container);
            
        }
        private void Initialize(IUnityContainer container)
        {
            this.Container = container;
            var eventAggregator = container.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
            PositionModeCommand = new RelayCommand(GotoPositionMode, CanGotoPositionMode);
            JogModeCommand = new RelayCommand(GotoJogMode, CanGotoJogMode);
            TrajectoryModeCommand = new RelayCommand(GotoTrajectoryMode, CanGotoTrajectoryMode);
            CurrentMode = 1;
            BeckhoffContext.modeOfGUI = 1;
        }

        private void GotoPositionMode(object obj)
        {
            var eventAggregator = Container.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
            viewRequestedEvent.Publish("{'command':'ActivateView','ModuleName':'PositionView','RegionName':'MainRegion'}");
            viewRequestedEvent.Publish("{'command':'ChangeHeader','Text':'Position Mode'}");
            viewRequestedEvent.Publish("{'command':'ActivateView','ModuleName':'AngleChartView','RegionName':'BottomRegion'}");
            viewRequestedEvent.Publish("{'command':'ActivateView','ModuleName':'ShowCartesianPositionView','RegionName':'RightRegion'}");
            CurrentMode = 1;
            BeckhoffContext.modeOfGUI = 1;
        }

        private bool CanGotoPositionMode(object obj)
        {
            return CurrentMode != 1;
        }
        private void GotoJogMode(object obj)
        {
            var eventAggregator = Container.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
            viewRequestedEvent.Publish("{'command':'ActivateView','ModuleName':'JogView','RegionName':'MainRegion'}");
            viewRequestedEvent.Publish("{'command':'ChangeHeader','Text':'Jog Mode'}");
            viewRequestedEvent.Publish("{'command':'ActivateView','ModuleName':'AngleChartView','RegionName':'BottomRegion'}");
            viewRequestedEvent.Publish("{'command':'ActivateView','ModuleName':'ShowCartesianPositionView','RegionName':'RightRegion'}");
            CurrentMode = 2;
            BeckhoffContext.modeOfGUI = 2;
        }
        private bool CanGotoJogMode(object obj)
        {
            return CurrentMode != 2;
        }
        private void GotoTrajectoryMode(object obj)
        {
            var eventAggregator = Container.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
            viewRequestedEvent.Publish("{'command':'ActivateView','ModuleName':'TrajectoryView','RegionName':'MainRegion'}");
            viewRequestedEvent.Publish("{'command':'ChangeHeader','Text':'Trajectory Mode'}");
            viewRequestedEvent.Publish("{'command':'ActivateView','ModuleName':'AngleChartView','RegionName':'BottomRegion'}");
            viewRequestedEvent.Publish("{'command':'ActivateView','ModuleName':'ShowCartesianPositionView','RegionName':'RightRegion'}");
            CurrentMode = 3;
            BeckhoffContext.modeOfGUI = 3;
        }
        private bool CanGotoTrajectoryMode(object obj)
        {
            return CurrentMode != 3;
        }
    }
}
