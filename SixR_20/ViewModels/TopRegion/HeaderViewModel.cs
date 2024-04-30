using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using SixR_20.Bootstrapper;
using SixR_20.Models;

namespace SixR_20.ViewModels.TopRegion
{
    class HeaderViewModel : BaseViewModel
    {
        private string _clearError;
        private Brush _clearErrorBrush;
        private Visibility _clearErrorVisibility;
        public ICommand ClearErrorCommand { get; internal set; }
        private string _powerState;
        private Brush _powerStateBrush;
        private Visibility _powerStateVisibility;
        public ICommand PowerSwitchCommand { get; internal set; }

        public ICommand CloseCommand { get; internal set; }
        public string PowerState
        {
            get { return _powerState; }
            set { _powerState = value; OnPropertyChanged(nameof(PowerState)); }
        }

        public Brush PowerStateBrush
        {
            get { return _powerStateBrush; }
            set { _powerStateBrush = value; OnPropertyChanged(nameof(PowerStateBrush)); }
        }

        public Visibility PowerStateVisibility
        {
            get { return _powerStateVisibility; }
            set { _powerStateVisibility = value; OnPropertyChanged(nameof(PowerStateVisibility)); }
        }
        public string ClearError
        {
            get { return _clearError; }
            set { _clearError = value; OnPropertyChanged(nameof(PowerState)); }
        }

        public Brush ClearErrorBrush
        {
            get { return _clearErrorBrush; }
            set { _clearErrorBrush = value; OnPropertyChanged(nameof(ClearErrorBrush)); }
        }

        public Visibility ClearErrorVisibility
        {
            get { return _clearErrorVisibility; }
            set { _clearErrorVisibility = value; OnPropertyChanged(nameof(ClearErrorVisibility)); }
        }
        public HeaderViewModel(IUnityContainer container)
        {
            this.Initialize(container);
        }
        private void Initialize(IUnityContainer container)
        {
            this.Container = container;
            var eventAggregator = container.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
            ClearError = "ClearErrors";
            PowerSwitchCommand = new DelegateCommand(SwitchPower);
            CloseCommand = new DelegateCommand(CloseForm);
            ClearErrorCommand = new DelegateCommand(ClearErrors);
            viewRequestedEvent.Subscribe(this.ViewRequestedEventHandler, true);
            PowerStateVisibility = Visibility.Collapsed;
            ClearErrorVisibility = Visibility.Collapsed;
            ClearErrorBrush = Application.Current.FindResource("TextBrush") as Brush;
        }
        private void ViewRequestedEventHandler(string s)
        {
            dynamic Command = JsonConvert.DeserializeObject(s);
            if (Command.command == "StartingControllerFinished")
            {
                PowerStateVisibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    BeckhoffContext.Controller.PropertyChanged += Controller_PropertyChanged;
                    PowerState = "ActivePowerSwitch";
                    PowerStateBrush = Application.Current.FindResource("TextBrush") as Brush;
                    var flags = BeckhoffContext.Controller.GUI_Flags;
                    if ((flags & 1) == 0)
                    {
                        PowerState = "DectivePowerSwitch";
                        PowerStateBrush = Application.Current.FindResource("DangerBrush") as Brush;
                    }
                }));
            }

        }
        private void Controller_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var flags = new bool[8];
            switch (e.PropertyName)
            {
                case "GUI_Alarms":
                    var alarms = BeckhoffContext.Controller.GUI_Alarms;
                    int alarmNumber = alarms.Count(alarm => alarm != 0);
                    if (alarmNumber == 0)
                        ClearErrorVisibility = Visibility.Collapsed;
                    else
                        ClearErrorVisibility = Visibility.Visible;
                    break;
                case "GUI_Flags":
                    for (var i = 0; i < 8; i++)
                        flags[i] = (BeckhoffContext.Controller.GUI_Flags & (1 << i)) != 0;
                    if (flags[0])
                    {
                        PowerState = "ActivePowerSwitch";
                        PowerStateBrush = Application.Current.FindResource("TextBrush") as Brush;
                    }
                    else
                    {
                        PowerState = "DectivePowerSwitch";
                        PowerStateBrush = Application.Current.FindResource("DangerBrush") as Brush;
                    }
                    break;
            }


        }

        public void SwitchPower()
        {
            BeckhoffContext.Controller.SetCommand((int)CommandsEnum.SwitchPower);
        }

        public void CloseForm()
        {
            Application.Current.Shutdown();
            
        }

        public void ClearErrors()
        {
            BeckhoffContext.Controller.SetCommand((byte)CommandsEnum.ClearServoAlarms);
            BeckhoffContext.Controller.NotifyAlarms();
        }
    }
}
