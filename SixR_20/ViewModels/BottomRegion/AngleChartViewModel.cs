using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using FirstFloor.ModernUI.Presentation;
using LiveCharts;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using SixR_20.Bootstrapper;
using SixR_20.Commands;
using SixR_20.Models;
using SixR_20.ViewModels.MainRegion;

namespace SixR_20.ViewModels.BottomRegion
{
    class AngleChartViewModel : BaseViewModel
    {
        public bool[] Flags = new bool[8];
        private string _m1AlarmText;
        private string _m2AlarmText;
        private string _m3AlarmText;
        private string _m4AlarmText;
        private string _m5AlarmText;
        private string _m6AlarmText;
        private Brush _m1StateBrush;
        private Brush _m2StateBrush;
        private Brush _m3StateBrush;
        private Brush _m4StateBrush;
        private Brush _m5StateBrush;
        private Brush _m6StateBrush;
        public static int times;

        public string M1AlarmText
        {
            get { return _m1AlarmText; }
            set { _m1AlarmText = value; OnPropertyChanged(nameof(M1AlarmText)); }
        }
        public string M2AlarmText
        {
            get { return _m2AlarmText; }
            set { _m2AlarmText = value; OnPropertyChanged(nameof(M2AlarmText)); }
        }
        public string M3AlarmText
        {
            get { return _m3AlarmText; }
            set { _m3AlarmText = value; OnPropertyChanged(nameof(M3AlarmText)); }
        }
        public string M4AlarmText
        {
            get { return _m4AlarmText; }
            set { _m4AlarmText = value; OnPropertyChanged(nameof(M4AlarmText)); }
        }
        public string M5AlarmText
        {
            get { return _m5AlarmText; }
            set { _m5AlarmText = value; OnPropertyChanged(nameof(M5AlarmText)); }
        }
        public string M6AlarmText
        {
            get { return _m6AlarmText; }
            set { _m6AlarmText = value; OnPropertyChanged(nameof(M6AlarmText)); }
        }

        public Brush M1StateBrush
        {
            get { return _m1StateBrush; }
            set { _m1StateBrush = value; OnPropertyChanged(nameof(M1StateBrush)); }
        }
        public Brush M2StateBrush
        {
            get { return _m2StateBrush; }
            set { _m2StateBrush = value; OnPropertyChanged(nameof(M2StateBrush)); }
        }
        public Brush M3StateBrush
        {
            get { return _m3StateBrush; }
            set { _m3StateBrush = value; OnPropertyChanged(nameof(M3StateBrush)); }
        }
        public Brush M4StateBrush
        {
            get { return _m4StateBrush; }
            set { _m4StateBrush = value; OnPropertyChanged(nameof(M4StateBrush)); }
        }
        public Brush M5StateBrush
        {
            get { return _m5StateBrush; }
            set { _m5StateBrush = value; OnPropertyChanged(nameof(M5StateBrush)); }
        }
        public Brush M6StateBrush
        {
            get { return _m6StateBrush; }
            set { _m6StateBrush = value; OnPropertyChanged(nameof(M6StateBrush)); }
        }

        public ChartValues<double> M1Angles { get; set; }
        public ChartValues<double> M2Angles { get; set; }
        public ChartValues<double> M3Angles { get; set; }
        public ChartValues<double> M4Angles { get; set; }
        public ChartValues<double> M5Angles { get; set; }
        public ChartValues<double> M6Angles { get; set; }
        public ChartValues<double> Values2 { get; set; }
        public AngleChartViewModel(IUnityContainer container)
        {
            this.Initialize(container);

        }
        private void Initialize(IUnityContainer container)
        {
            this.Container = container;
            var eventAggregator = container.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
            viewRequestedEvent.Subscribe(this.ViewRequestedEventHandler, true);
            M1Angles = new ChartValues<double>();
            M2Angles = new ChartValues<double>();
            M3Angles = new ChartValues<double>();
            M4Angles = new ChartValues<double>();
            M5Angles = new ChartValues<double>();
            M6Angles = new ChartValues<double>();
            M1AlarmText = "OK";
            M2AlarmText = "OK";
            M3AlarmText = "OK";
            M4AlarmText = "OK";
            M5AlarmText = "OK";
            M6AlarmText = "OK";
            M1StateBrush = Application.Current.FindResource("SuccessBrush") as Brush;
            M2StateBrush = Application.Current.FindResource("SuccessBrush") as Brush;
            M3StateBrush = Application.Current.FindResource("SuccessBrush") as Brush;
            M4StateBrush = Application.Current.FindResource("SuccessBrush") as Brush;
            M5StateBrush = Application.Current.FindResource("SuccessBrush") as Brush;
            M6StateBrush = Application.Current.FindResource("SuccessBrush") as Brush;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                BeckhoffContext.Controller.PropertyChanged += Controller_PropertyChanged;
            }));
            Timer tim = new Timer(500);
            tim.Elapsed += Tim_Elapsed;
            tim.Enabled = true;
        }

        private void Tim_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (BeckhoffContext.Controller == null) return;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var deg = (BeckhoffContext.Controller.MotorsEncoder[0] * UnitConverter.PulsToDegFactor[0]);
                var deg1 = (BeckhoffContext.Controller.MotorsEncoder[1] * UnitConverter.PulsToDegFactor[1]);
                var deg2 = (BeckhoffContext.Controller.MotorsEncoder[2] * UnitConverter.PulsToDegFactor[2]);
                var deg3 = (BeckhoffContext.Controller.MotorsEncoder[3] * UnitConverter.PulsToDegFactor[3]);
                var deg4 = (BeckhoffContext.Controller.MotorsEncoder[4] * UnitConverter.PulsToDegFactor[4]);
                var deg5 = (BeckhoffContext.Controller.MotorsEncoder[5] * UnitConverter.PulsToDegFactor[5]);

                if (M1Angles.Count > 20)
                {
                    M1Angles.RemoveAt(0);
                    M3Angles.RemoveAt(0);
                    M4Angles.RemoveAt(0);
                    M5Angles.RemoveAt(0);
                    M6Angles.RemoveAt(0);
                    M2Angles.RemoveAt(0);
                }
                M1Angles.Add((double)deg);
                M2Angles.Add((double)deg1);
                M3Angles.Add((double)deg2);
                M4Angles.Add((double)deg3);
                M5Angles.Add((double)deg4);
                M6Angles.Add((double)deg5);

                OnPropertyChanged(nameof(M4Angles));
                OnPropertyChanged(nameof(M2Angles));
                OnPropertyChanged(nameof(M5Angles));
                OnPropertyChanged(nameof(M3Angles));
                OnPropertyChanged(nameof(M1Angles));
                OnPropertyChanged(nameof(M6Angles));
            }));
        }

        private void Controller_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            for (var i = 0; i < 8; i++)
                Flags[i] = (BeckhoffContext.Controller.GUI_Flags & (1 << i)) != 0;
            switch (e.PropertyName)
            {
                case "GUI_Alarms":
                case "GUI_Flags":
                    SyncAlarms();
                    break;
                //case "TrajectoryPuls":
                //    var deg = (BeckhoffContext.Controller.TrajectoryErrorPosition[0] );
                //    var deg1 = (BeckhoffContext.Controller.TrajectoryErrorPosition[1]);
                //    var deg2 = (BeckhoffContext.Controller.TrajectoryErrorPosition[2] );
                //    var deg3 = (BeckhoffContext.Controller.TrajectoryErrorPosition[3] );
                //    var deg4 = (BeckhoffContext.Controller.TrajectoryErrorPosition[4] );
                //    var deg5 = (BeckhoffContext.Controller.TrajectoryErrorPosition[5]);
                //    for (int i = times*SixRConstants.BufferLen; i < deg.Length; i++)
                //    {
                //        M1Angles.Add(deg[i]);
                //        M2Angles.Add(deg1[i]);
                //        M3Angles.Add(deg2[i]);
                //        M4Angles.Add(deg3[i]);
                //        M5Angles.Add(deg4[i]);
                //        M6Angles.Add(deg5[i]);
                //    }

                //    OnPropertyChanged(nameof(M4Angles));
                //    OnPropertyChanged(nameof(M2Angles));
                //    OnPropertyChanged(nameof(M5Angles));
                //    OnPropertyChanged(nameof(M3Angles));
                //    OnPropertyChanged(nameof(M1Angles));
                //    OnPropertyChanged(nameof(M6Angles));
                //    break;
            }
        }
        private void SyncAlarms()
        {
            var alarms = BeckhoffContext.Controller.GUI_Alarms;
            M1AlarmText = alarms[0] == 0 ? "OK" : ("AL-" + alarms[0].ToString("X"));
            M1StateBrush =
                alarms[0] == 0 ? (Flags[0] ? (Application.Current.FindResource("SuccessBrush") as Brush) :
                (Application.Current.FindResource("WarningBrush") as Brush)) :
                (Application.Current.FindResource("DangerBrush") as Brush);
            M2AlarmText = alarms[1] == 0 ? "OK" : ("AL-" + alarms[1].ToString("X"));
            M2StateBrush =
                alarms[1] == 0 ? (Flags[0] ? (Application.Current.FindResource("SuccessBrush") as Brush) :
                (Application.Current.FindResource("WarningBrush") as Brush)) :
                (Application.Current.FindResource("DangerBrush") as Brush);
            M3AlarmText = alarms[2] == 0 ? "OK" : ("AL-" + alarms[2].ToString("X"));
            M3StateBrush =
                alarms[2] == 0 ? (Flags[0] ? (Application.Current.FindResource("SuccessBrush") as Brush) :
                (Application.Current.FindResource("WarningBrush") as Brush)) :
                (Application.Current.FindResource("DangerBrush") as Brush);
            M4AlarmText = alarms[3] == 0 ? "OK" : ("AL-" + alarms[3].ToString("X"));
            M4StateBrush =
                alarms[3] == 0 ? (Flags[0] ? (Application.Current.FindResource("SuccessBrush") as Brush) :
                (Application.Current.FindResource("WarningBrush") as Brush)) :
                (Application.Current.FindResource("DangerBrush") as Brush);
            M5AlarmText = alarms[4] == 0 ? "OK" : ("AL-" + alarms[4].ToString("X"));
            M5StateBrush =
                alarms[4] == 0 ? (Flags[0] ? (Application.Current.FindResource("SuccessBrush") as Brush) :
                (Application.Current.FindResource("WarningBrush") as Brush)) :
                (Application.Current.FindResource("DangerBrush") as Brush);
            M6AlarmText = alarms[5] == 0 ? "OK" : ("AL-" + alarms[5].ToString("X"));
            M6StateBrush =
                alarms[5] == 0 ? (Flags[0] ? (Application.Current.FindResource("SuccessBrush") as Brush) :
                (Application.Current.FindResource("WarningBrush") as Brush)) :
                (Application.Current.FindResource("DangerBrush") as Brush);
        }
        private void ViewRequestedEventHandler(string s)
        {

        }
    }
}
