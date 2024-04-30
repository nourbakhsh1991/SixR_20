using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Prism.Events;
using SixR_20.Bootstrapper;
using SixR_20.Models;

namespace SixR_20.ViewModels.MainRegion
{
    enum SelfTestLevel
    {
        NONE,
        startingSelfTest,
        startedSelfTest,
        startingController,
        startedController,
        startingTestDrives,
        startedTestDrives,
        startingCheckDriveError,
        startedCheckDriveError,
        startingMove,
        startedMove
    }
    class SelfTestViewModel : BaseViewModel
    {
        private string _initText;
        private string _createControllerText;
        private string _testDrivesText;
        private string _checkDriveErrorText;
        private string _movingText;

        private SelfTestLevel _selfTestLevel = SelfTestLevel.startingSelfTest;
        public SelfTestLevel Level => _selfTestLevel;
        private int _selfTestVisibility;
        private int _createControllerVisibility;
        private int _testDrivesVisibility;
        private int _checkDriveErrorVisibility;
        private int _movingVisibility;

        public int SelfTestVisibility
        {
            get { return _selfTestVisibility; }
            set { _selfTestVisibility = value; OnPropertyChanged(nameof(SelfTestVisibility)); }
        }

        public int CreateControllerVisibility
        {
            get { return _createControllerVisibility; }
            set { _createControllerVisibility = value; OnPropertyChanged(nameof(CreateControllerVisibility)); }
        }
        public int TestDrivesVisibility
        {
            get { return _testDrivesVisibility; }
            set { _testDrivesVisibility = value; OnPropertyChanged(nameof(TestDrivesVisibility)); }
        }
        public int CheckDriveErrorVisibility
        {
            get { return _checkDriveErrorVisibility; }
            set { _checkDriveErrorVisibility = value; OnPropertyChanged(nameof(CheckDriveErrorVisibility)); }
        }
        public int MovingVisibility
        {
            get { return _movingVisibility; }
            set { _movingVisibility = value; OnPropertyChanged(nameof(MovingVisibility)); }
        }
        public string InitText
        {
            get { return _initText; }
            set { _initText = value; OnPropertyChanged(nameof(InitText)); }
        }

        public string CreateControllerText
        {
            get { return _createControllerText; }
            set { _createControllerText = value; OnPropertyChanged(nameof(CreateControllerText)); }
        }
        public string TestDrivesText
        {
            get { return _testDrivesText; }
            set { _testDrivesText = value; OnPropertyChanged(nameof(TestDrivesText)); }
        }
        public string CheckDriveErrorText
        {
            get { return _checkDriveErrorText; }
            set { _checkDriveErrorText = value; OnPropertyChanged(nameof(CheckDriveErrorText)); }
        }

        public string MovingText
        {
            get { return _movingText; }
            set { _movingText = value; OnPropertyChanged(nameof(MovingText)); }
        }

        private void HandleLevels(object obj)
        {
            try
            {
                switch (Level)
                {
                    case SelfTestLevel.startingSelfTest:
                        Thread.Sleep(1000);
                        Container.Resolve<IEventAggregator>().GetEvent<ViewRequestedEvent>().Publish("{'command':'StartingSelfTestFinished'}");
                        break;
                    case SelfTestLevel.startedSelfTest:
                        Container.Resolve<IEventAggregator>().GetEvent<ViewRequestedEvent>().Publish("{'command':'StartedSelfTestFinished'}");
                        break;
                    case SelfTestLevel.startingController:
                        Application.Current.Dispatcher.Invoke(new Action(BeckhoffContext.StartController));

                        Thread.Sleep(500);
                        Container.Resolve<IEventAggregator>().GetEvent<ViewRequestedEvent>().Publish("{'command':'StartingControllerFinished'}");
                        break;
                    case SelfTestLevel.startedController:
                        Container.Resolve<IEventAggregator>().GetEvent<ViewRequestedEvent>().Publish("{'command':'StartedControllerFinished'}");
                        break;
                    case SelfTestLevel.startingTestDrives:
                        BeckhoffContext.Controller.SetCommand((byte)CommandsEnum.ClearServoAlarms);
                        var DriveStatus = BeckhoffContext.Controller.GetStatusword();
                        var i = 1;
                        foreach (var stat in DriveStatus)
                        {
                            //if ((stat & (int)StatuswordEnum.ReadyToSwitchOn) == 0)
                            //    //throw new Exception("");
                            //    return;
                            TestDrivesText = i++ + "/" + SixRConstants.NumberOfAxis + " " + SixRConstants.ResourceManager.GetString("AA0110007");
                            Thread.Sleep(200);
                        }
                        Container.Resolve<IEventAggregator>().GetEvent<ViewRequestedEvent>().Publish("{'command':'StartingTestDrivesFinished'}");
                        break;
                    case SelfTestLevel.startedTestDrives:
                        Container.Resolve<IEventAggregator>().GetEvent<ViewRequestedEvent>().Publish("{'command':'StartedTestDrivesrFinished'}");
                        break;
                    case SelfTestLevel.startingCheckDriveError:
                        var alarms = BeckhoffContext.Controller.GUI_Alarms;
                        int alarmNumber = alarms.Count(alarm => alarm != 0);
                        if (alarmNumber > 0)
                        {
                            CheckDriveErrorText = alarmNumber + "/" + SixRConstants.NumberOfAxis + " " + SixRConstants.ResourceManager.GetString("AA0110009") + SixRConstants.ResourceManager.GetString("AA0110010");
                            CheckDriveErrorVisibility = 4;
                            BeckhoffContext.Controller.SetCommand((byte)CommandsEnum.ClearServoAlarms);
                            Container.Resolve<IEventAggregator>().GetEvent<ViewRequestedEvent>().Publish("{'command':'StartedTestDrivesrFinished'}");
                        }
                        else
                        {
                            Container.Resolve<IEventAggregator>().GetEvent<ViewRequestedEvent>().Publish("{'command':'StartingCheckDriveFinished'}");
                        }
                        Thread.Sleep(1000);
                        break;
                    case SelfTestLevel.startedCheckDriveError:

                        Container.Resolve<IEventAggregator>().GetEvent<ViewRequestedEvent>().Publish("{'command':'StartedCheckDriveFinished'}");
                        break;
                    case SelfTestLevel.startingMove:
                        Thread.Sleep(1000);
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            BeckhoffContext.Controller.SetRegisterCtrWord(new ushort[] { 7, 7, 7, 7, 7, 7 });
                            BeckhoffContext.Controller.SetCommand((int)CommandsEnum.SetControlWord);
                            BeckhoffContext.Controller.SetModesOfOperation(1);
                            BeckhoffContext.Controller.SetCommand((int)CommandsEnum.SetMode);
                            var Flag = (BeckhoffContext.Controller.GUI_Flags & (1 << 0)) != 0;
                            if (!Flag)
                                BeckhoffContext.Controller.SetCommand((int)CommandsEnum.SwitchPower);
                            var no = 0;
                            for (var j = 0; j < SixRConstants.NumberOfAxis; j++)
                            {
                                BeckhoffContext.Controller.SetMotorNumber((ushort)(j + 1));
                                BeckhoffContext.Controller.SetRegisterTargetPosition(BeckhoffContext.Controller.GetActualPosition(j) * 100 + 200, j + 1);
                                var mSpeed = (int)(10.0m / Math.Abs(UnitConverter.PulsToDegFactor[j]));
                                BeckhoffContext.Controller.SetProfileVelocities(mSpeed, j + 1);
                                BeckhoffContext.Controller.SetCommand((int)CommandsEnum.MoveAbs);
                            }

                            PropertyChangedEventHandler handler = null;
                            handler = (sender, args) =>
                             {
                                 switch ((args).PropertyName)
                                 {
                                     case "M1_ActualPosition":
                                     case "M2_ActualPosition":
                                     case "M3_ActualPosition":
                                     case "M4_ActualPosition":
                                     case "M5_ActualPosition":
                                     case "M6_ActualPosition":
                                         no++;
                                         break;
                                 }
                                 if (no >= 6)
                                 {

                                     Container.Resolve<IEventAggregator>().GetEvent<ViewRequestedEvent>().Publish("{'command':'StartingMoveFinished'}");
                                     BeckhoffContext.Controller.PropertyChanged -= handler;
                                 }

                             };
                            BeckhoffContext.Controller.PropertyChanged += handler;
                        }));
                        Container.Resolve<IEventAggregator>().GetEvent<ViewRequestedEvent>().Publish("{'command':'NONE'}");
                        break;
                    case SelfTestLevel.startedMove:
                        Thread.Sleep(200);
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            BeckhoffContext.Controller.SetRegisterCtrWord(new ushort[] { 7, 7, 7, 7, 7, 7 });
                            BeckhoffContext.Controller.SetCommand((int)CommandsEnum.SetControlWord);
                            BeckhoffContext.Controller.SetModesOfOperation(8);
                            BeckhoffContext.Controller.SetCommand((int)CommandsEnum.SetMode);
                        }));
                        Container.Resolve<IEventAggregator>().GetEvent<ViewRequestedEvent>().Publish("{'command':'StartedMoveFinished'}");
                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }

        public SelfTestViewModel(IUnityContainer container)
        {
            this.Initialize(container);

        }
        private void Initialize(IUnityContainer container)
        {
            this.Container = container;
            var eventAggregator = container.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
            viewRequestedEvent.Publish("{'command':'SetMainRegionColspan','MainRegionColspan':'2'}");
            viewRequestedEvent.Publish("{'command':'SetMainRegionThickness','MainRegionThickness':{'left':10,'top':10,'right':20,'bottom':5}}");
            viewRequestedEvent.Publish("{'command':'HideLeftGrid'}");
            viewRequestedEvent.Subscribe(this.ViewRequestedEventHandler, true);
            SelfTestVisibility = 2;
            InitText = SixRConstants.ResourceManager.GetString("AA0110001");
            CreateControllerVisibility = 1;
            CreateControllerText = SixRConstants.ResourceManager.GetString("AA0110003");
            TestDrivesVisibility = 1;
            TestDrivesText = SixRConstants.ResourceManager.GetString("AA0110003");
            CheckDriveErrorVisibility = 1;
            CheckDriveErrorText = SixRConstants.ResourceManager.GetString("AA0110003");
            MovingVisibility = 1;
            MovingText = SixRConstants.ResourceManager.GetString("AA0110003");
            ThreadPool.QueueUserWorkItem(HandleLevels);
        }
        private void ViewRequestedEventHandler(string s)
        {
            dynamic Command = JsonConvert.DeserializeObject(s);
            if (Command.command == "NONE")
            {
                _selfTestLevel = SelfTestLevel.NONE;
            }
            else if (Command.command == "StartingSelfTestFinished")
            {
                SelfTestVisibility = 3;
                InitText = SixRConstants.ResourceManager.GetString("AA0110002");
                _selfTestLevel = SelfTestLevel.startedSelfTest;
                ThreadPool.QueueUserWorkItem(HandleLevels);
            }
            else if (Command.command == "StartedSelfTestFinished")
            {
                CreateControllerVisibility = 2;
                _selfTestLevel = SelfTestLevel.startingController;
                CreateControllerText = SixRConstants.ResourceManager.GetString("AA0110004");
                ThreadPool.QueueUserWorkItem(HandleLevels);
            }
            else if (Command.command == "StartingControllerFinished")
            {
                CreateControllerVisibility = 3;
                _selfTestLevel = SelfTestLevel.startedController;
                CreateControllerText = SixRConstants.ResourceManager.GetString("AA0110005");
                ThreadPool.QueueUserWorkItem(HandleLevels);
            }
            else if (Command.command == "StartedControllerFinished")
            {
                TestDrivesVisibility = 2;
                _selfTestLevel = SelfTestLevel.startingTestDrives;
                TestDrivesText = SixRConstants.ResourceManager.GetString("AA0110006");
                ThreadPool.QueueUserWorkItem(HandleLevels);
            }
            else if (Command.command == "StartingTestDrivesFinished")
            {
                TestDrivesVisibility = 3;
                _selfTestLevel = SelfTestLevel.startedTestDrives;
                ThreadPool.QueueUserWorkItem(HandleLevels);
            }
            else if (Command.command == "StartedTestDrivesrFinished")
            {
                CheckDriveErrorVisibility = 2;
                _selfTestLevel = SelfTestLevel.startingCheckDriveError;
                CheckDriveErrorText = SixRConstants.ResourceManager.GetString("AA0110011");
                ThreadPool.QueueUserWorkItem(HandleLevels);
            }
            else if (Command.command == "StartingCheckDriveFinished")
            {
                CheckDriveErrorVisibility = 3;
                _selfTestLevel = SelfTestLevel.startedCheckDriveError;
                CheckDriveErrorText = SixRConstants.ResourceManager.GetString("AA0110012");
                ThreadPool.QueueUserWorkItem(HandleLevels);
            }
            else if (Command.command == "StartedCheckDriveFinished")
            {
                MovingVisibility = 2;
                _selfTestLevel = SelfTestLevel.startingMove;
                MovingText = SixRConstants.ResourceManager.GetString("AA0110013");
                ThreadPool.QueueUserWorkItem(HandleLevels);
            }
            else if (Command.command == "StartingMoveFinished")
            {
                MovingVisibility = 3;
                _selfTestLevel = SelfTestLevel.startedMove;
                MovingText = SixRConstants.ResourceManager.GetString("AA0110013");
                ThreadPool.QueueUserWorkItem(HandleLevels);
            }
            else if (Command.command == "StartedMoveFinished")
            {
                _selfTestLevel = SelfTestLevel.NONE;
                MovingText = SixRConstants.ResourceManager.GetString("AA0110014");
                var eventAggregator = Container.Resolve<IEventAggregator>();
                var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
                viewRequestedEvent.Publish("{'command':'SetMainRegionColspan','MainRegionColspan':'1'}");
                viewRequestedEvent.Publish("{'command':'SetMainRegionThickness','MainRegionThickness':{'left':10,'top':10,'right':5,'bottom':5}}");
                viewRequestedEvent.Publish("{'command':'ShowLeftGrid'}");
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    viewRequestedEvent.Publish("{'command':'ActivateView','ModuleName':'PositionView','RegionName':'MainRegion'}");
                    viewRequestedEvent.Publish("{'command':'ActivateView','ModuleName':'OperationModeView','RegionName':'LeftRegion'}");
                    viewRequestedEvent.Publish("{'command':'ActivateView','ModuleName':'AngleChartView','RegionName':'BottomRegion'}");
                    viewRequestedEvent.Publish("{'command':'ActivateView','ModuleName':'ShowCartesianPositionView','RegionName':'RightRegion'}");
                    
                }));
            }
        }

    }
}
