using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Prism.Events;
using SixR_20.Bootstrapper;
using SixR_20.Models;

namespace SixR_20.ViewModels.RightRegion
{
    class ShowCartesianPositionViewModel : BaseViewModel
    {
        private string _m1Angle;
        private string _m2Angle;
        private string _m3Angle;
        private string _m4Angle;
        private string _m5Angle;
        private string _m6Angle;
        private string _alarmsText;

        private Brush _alarmBrush;
        public string M1Angle
        {
            get { return _m1Angle; }
            set { _m1Angle = value; OnPropertyChanged(nameof(M1Angle)); }
        }
        public string M2Angle
        {
            get { return _m2Angle; }
            set { _m2Angle = value; OnPropertyChanged(nameof(M2Angle)); }
        }
        public string M3Angle
        {
            get { return _m3Angle; }
            set { _m3Angle = value; OnPropertyChanged(nameof(M3Angle)); }
        }
        public string M4Angle
        {
            get { return _m4Angle; }
            set { _m4Angle = value; OnPropertyChanged(nameof(M4Angle)); }
        }
        public string M5Angle
        {
            get { return _m5Angle; }
            set { _m5Angle = value; OnPropertyChanged(nameof(M5Angle)); }
        }
        public string M6Angle
        {
            get { return _m6Angle; }
            set { _m6Angle = value; OnPropertyChanged(nameof(M6Angle)); }
        }

        public string AlarmsText
        {
            get { return _alarmsText; }
            set { _alarmsText = value; OnPropertyChanged(nameof(AlarmsText)); }
        }

        public Brush AlarmBrush
        {
            get { return _alarmBrush; }
            set { _alarmBrush = value; OnPropertyChanged(nameof(AlarmBrush)); }
        }
        public ShowCartesianPositionViewModel(IUnityContainer container)
        {
            this.Initialize(container);

        }
        private void Initialize(IUnityContainer container)
        {
            this.Container = container;
            var eventAggregator = container.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
            //viewRequestedEvent.Subscribe(this.ViewRequestedEventHandler, true);
            if (BeckhoffContext.Controller == null)
                throw new Exception("");
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                BeckhoffContext.Controller.PropertyChanged += Controller_PropertyChanged;
                M1Angle = (BeckhoffContext.Controller.MotorsEncoder[0] * UnitConverter.PulsToDegFactor[0]).ToString("0.000");
                M2Angle = (BeckhoffContext.Controller.MotorsEncoder[1] * UnitConverter.PulsToDegFactor[1]).ToString("0.000");
                M3Angle = (BeckhoffContext.Controller.MotorsEncoder[2] * UnitConverter.PulsToDegFactor[2]).ToString("0.000");
                M4Angle = (BeckhoffContext.Controller.MotorsEncoder[3] * UnitConverter.PulsToDegFactor[3]).ToString("0.000");
                M5Angle = (BeckhoffContext.Controller.MotorsEncoder[4] * UnitConverter.PulsToDegFactor[4]).ToString("0.000");
                M6Angle = (BeckhoffContext.Controller.MotorsEncoder[5] * UnitConverter.PulsToDegFactor[5]).ToString("0.000");
                AlarmsText = "No Alarm Detected.";
                AlarmBrush = Application.Current.FindResource("SuccessBrush") as Brush;

            }));
        }
        private void ViewRequestedEventHandler(string s)
        {
            dynamic Command = JsonConvert.DeserializeObject(s);
            


        }

        private void Controller_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            switch (e.PropertyName)
            {
                case "M1_ActualPosition":
                    M1Angle = (BeckhoffContext.Controller.MotorsEncoder[0] * UnitConverter.PulsToDegFactor[0]).ToString("0.000");
                    break;
                case "M2_ActualPosition":
                    M2Angle = (BeckhoffContext.Controller.MotorsEncoder[1] * UnitConverter.PulsToDegFactor[1]).ToString("0.000");
                    break;
                case "M3_ActualPosition":
                    M3Angle = (BeckhoffContext.Controller.MotorsEncoder[2] * UnitConverter.PulsToDegFactor[2]).ToString("0.000");
                    break;
                case "M4_ActualPosition":
                    M4Angle = (BeckhoffContext.Controller.MotorsEncoder[3] * UnitConverter.PulsToDegFactor[3]).ToString("0.000");
                    break;
                case "M5_ActualPosition":
                    M5Angle = (BeckhoffContext.Controller.MotorsEncoder[4] * UnitConverter.PulsToDegFactor[4]).ToString("0.000");
                    break;
                case "M6_ActualPosition":
                    M6Angle = (BeckhoffContext.Controller.MotorsEncoder[5] * UnitConverter.PulsToDegFactor[5]).ToString("0.000");
                    break;
                case "GUI_Alarms":
                    var alarms = BeckhoffContext.Controller.GUI_Alarms;
                    int alarmNumber = alarms.Count(alarm => alarm != 0);
                    if (alarmNumber == 0)
                    {
                        AlarmsText = "No Alarm Detected.";
                        AlarmBrush = Application.Current.FindResource("SuccessBrush") as Brush;
                    }
                    else
                    {
                        AlarmsText = "Alarm Detected.";
                        AlarmBrush = Application.Current.FindResource("DangerBrush") as Brush;
                    }
                    break;
            }


        }
    }
}
