using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using FirstFloor.ModernUI.Presentation;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using SixR_20.Bootstrapper;
using SixR_20.Models;

namespace SixR_20.ViewModels.MainRegion
{
    class PositionViewModel : BaseViewModel
    {

        private float _m1Angle;
        private float _m2Angle;
        private float _m3Angle;
        private float _m4Angle;
        private float _m5Angle;
        private float _m6Angle;
        private bool _trajectoryStarted;
        public ObservableCollection<bool> HasValidAngle = new ObservableCollection<bool>();
        private int _positionSpeed=100;

        public bool TrajectoryStarted
        {
            get { return _trajectoryStarted; }
            set { _trajectoryStarted = value;OnPropertyChanged(nameof(TrajectoryStarted)); }
        }

        public float M1Angle
        {
            get { return _m1Angle; }
            set { _m1Angle = value; OnPropertyChanged(nameof(M1Angle)); }
        }
        public float M2Angle
        {
            get { return _m2Angle; }
            set { _m2Angle = value; OnPropertyChanged(nameof(M2Angle)); }
        }
        public float M3Angle
        {
            get { return _m3Angle; }
            set { _m3Angle = value; OnPropertyChanged(nameof(M3Angle)); }
        }
        public float M4Angle
        {
            get { return _m4Angle; }
            set { _m4Angle = value; OnPropertyChanged(nameof(M4Angle)); }
        }
        public float M5Angle
        {
            get { return _m5Angle; }
            set { _m5Angle = value; OnPropertyChanged(nameof(M5Angle)); }
        }
        public float M6Angle
        {
            get { return _m6Angle; }
            set { _m6Angle = value; OnPropertyChanged(nameof(M6Angle)); }
        }

        public int PositionSpeed
        {
            get { return _positionSpeed; }
            set
            {
                _positionSpeed = value >= 1 ? value : 1;
                OnPropertyChanged(nameof(PositionSpeed));
            }
        }
        public object M1Error { get; set; }
        public ICommand M1MoveCommand { get; internal set; }
        public ICommand M2MoveCommand { get; internal set; }
        public ICommand M3MoveCommand { get; internal set; }
        public ICommand M4MoveCommand { get; internal set; }
        public ICommand M5MoveCommand { get; internal set; }
        public ICommand M6MoveCommand { get; internal set; }
        public ICommand AllMoveCommand { get; internal set; }
        public ICommand HomeCommand { get; internal set; }

        public PositionViewModel(IUnityContainer container)
        {
            this.Initialize(container);

        }
        private void Initialize(IUnityContainer container)
        {
            this.Container = container;
            var eventAggregator = container.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
            M1MoveCommand = new RelayCommand(M1Move, CanM1Move);
            M2MoveCommand = new RelayCommand(M2Move, CanM2Move);
            M3MoveCommand = new RelayCommand(M3Move, CanM3Move);
            M4MoveCommand = new RelayCommand(M4Move, CanM4Move);
            M5MoveCommand = new RelayCommand(M5Move, CanM5Move);
            M6MoveCommand = new RelayCommand(M6Move, CanM6Move);
            AllMoveCommand = new RelayCommand(AllMove, CanAllMove);
            HomeCommand = new DelegateCommand(HomeMove);
            for (int i = 0; i < SixRConstants.NumberOfAxis; i++)
                HasValidAngle.Add(true);
            TrajectoryStarted = false;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                BeckhoffContext.Controller.PropertyChanged += Controller_PropertyChanged;
            }));
        }

        private void Controller_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            switch (e.PropertyName)
            {
                case "Pulse":
                    TrajectoryStarted = false;
                    CommandManager.InvalidateRequerySuggested();
                    break;

            }
        }

        public void M1Move(object obj)
        {
            decimal[] theta =
                {
                    (BeckhoffContext.Controller.MotorsEncoder[0]*UnitConverter.PulsToDegFactor[0]),
                    (BeckhoffContext.Controller.MotorsEncoder[1]*UnitConverter.PulsToDegFactor[1]),
                    (BeckhoffContext.Controller.MotorsEncoder[2]*UnitConverter.PulsToDegFactor[2]),
                    (BeckhoffContext.Controller.MotorsEncoder[3]*UnitConverter.PulsToDegFactor[3]),
                    (BeckhoffContext.Controller.MotorsEncoder[4]*UnitConverter.PulsToDegFactor[4]),
                    (BeckhoffContext.Controller.MotorsEncoder[5]*UnitConverter.PulsToDegFactor[5]),
                };
            var tmpVals = new List<decimal>();
            tmpVals.AddRange(new decimal[] { Convert.ToDecimal( _m1Angle), theta[1], theta[2], theta[3], theta[4], theta[5], PositionSpeed, 1 });
            MoveWithValue(tmpVals);
        }
        private bool CanM1Move(object obj)
        {
            float tmp;
            if (TrajectoryStarted)
                return false;
            if (obj != null &&
                float.TryParse(obj.ToString(), out tmp) &&
                tmp < SixRConstants.MotorMovementRange[0] &&
                tmp > -SixRConstants.MotorMovementRange[0])
            {
                HasValidAngle[0] = true;
                AllMoveCommand.CanExecute(null);
                return true;
            }
            HasValidAngle[0] = false;
            AllMoveCommand.CanExecute(null);
            return false;
        }

        public void M2Move(object obj)
        {
            decimal[] theta =
                            {
                    (BeckhoffContext.Controller.MotorsEncoder[0]*UnitConverter.PulsToDegFactor[0]),
                    (BeckhoffContext.Controller.MotorsEncoder[1]*UnitConverter.PulsToDegFactor[1]),
                    (BeckhoffContext.Controller.MotorsEncoder[2]*UnitConverter.PulsToDegFactor[2]),
                    (BeckhoffContext.Controller.MotorsEncoder[3]*UnitConverter.PulsToDegFactor[3]),
                    (BeckhoffContext.Controller.MotorsEncoder[4]*UnitConverter.PulsToDegFactor[4]),
                    (BeckhoffContext.Controller.MotorsEncoder[5]*UnitConverter.PulsToDegFactor[5]),
                };
            var tmpVals = new List<decimal>();
            tmpVals.AddRange(new decimal[] { theta[0], Convert.ToDecimal( _m2Angle), theta[2], theta[3], theta[4], theta[5], PositionSpeed, 1 });
            MoveWithValue(tmpVals);
        }
        private bool CanM2Move(object obj)
        {
            if (TrajectoryStarted)
                return false;
            float tmp;

            if (obj != null &&
                float.TryParse(obj.ToString(), out tmp) &&
                tmp < SixRConstants.MotorMovementRange[1] &&
                tmp > -SixRConstants.MotorMovementRange[1])
            {
                HasValidAngle[1] = true;
                AllMoveCommand.CanExecute(null);
                return true;
            }
            HasValidAngle[1] = false;
            AllMoveCommand.CanExecute(null);
            return false;
        }

        public void M3Move(object obj)
        {
            decimal[] theta =
                            {
                    (BeckhoffContext.Controller.MotorsEncoder[0]*UnitConverter.PulsToDegFactor[0]),
                    (BeckhoffContext.Controller.MotorsEncoder[1]*UnitConverter.PulsToDegFactor[1]),
                    (BeckhoffContext.Controller.MotorsEncoder[2]*UnitConverter.PulsToDegFactor[2]),
                    (BeckhoffContext.Controller.MotorsEncoder[3]*UnitConverter.PulsToDegFactor[3]),
                    (BeckhoffContext.Controller.MotorsEncoder[4]*UnitConverter.PulsToDegFactor[4]),
                    (BeckhoffContext.Controller.MotorsEncoder[5]*UnitConverter.PulsToDegFactor[5]),
                };
            var tmpVals = new List<decimal>();
            tmpVals.AddRange(new decimal[] { theta[0], theta[1],Convert.ToDecimal( _m3Angle), theta[3], theta[4], theta[5], PositionSpeed, 1 });
            MoveWithValue(tmpVals);
        }
        private bool CanM3Move(object obj)
        {
            if (TrajectoryStarted)
                return false;
            float tmp;

            if (obj != null &&
                float.TryParse(obj.ToString(), out tmp) &&
                tmp < SixRConstants.MotorMovementRange[2] &&
                tmp > -SixRConstants.MotorMovementRange[2])
            {
                HasValidAngle[2] = true;
                AllMoveCommand.CanExecute(null);
                return true;
            }
            HasValidAngle[2] = false;
            AllMoveCommand.CanExecute(null);
            return false;
        }

        public void M4Move(object obj)
        {
            decimal[] theta =
                            {
                    (BeckhoffContext.Controller.MotorsEncoder[0]*UnitConverter.PulsToDegFactor[0]),
                    (BeckhoffContext.Controller.MotorsEncoder[1]*UnitConverter.PulsToDegFactor[1]),
                    (BeckhoffContext.Controller.MotorsEncoder[2]*UnitConverter.PulsToDegFactor[2]),
                    (BeckhoffContext.Controller.MotorsEncoder[3]*UnitConverter.PulsToDegFactor[3]),
                    (BeckhoffContext.Controller.MotorsEncoder[4]*UnitConverter.PulsToDegFactor[4]),
                    (BeckhoffContext.Controller.MotorsEncoder[5]*UnitConverter.PulsToDegFactor[5]),
                };
            var tmpVals = new List<decimal>();
            tmpVals.AddRange(new[] { theta[0], theta[1], theta[2],Convert.ToDecimal( _m4Angle), theta[4], theta[5], PositionSpeed, 1 });
            MoveWithValue(tmpVals);
        }
        private bool CanM4Move(object obj)
        {
            if (TrajectoryStarted)
                return false;
            float tmp;

            if (obj != null &&
                float.TryParse(obj.ToString(), out tmp) &&
                tmp < SixRConstants.MotorMovementRange[3] &&
                tmp > -SixRConstants.MotorMovementRange[3])
            {
                HasValidAngle[3] = true;
                AllMoveCommand.CanExecute(null);
                return true;
            }
            HasValidAngle[3] = false;
            AllMoveCommand.CanExecute(null);
            return false;
        }

        public void M5Move(object obj)
        {
            decimal[] theta =
                            {
                    (BeckhoffContext.Controller.MotorsEncoder[0]*UnitConverter.PulsToDegFactor[0]),
                    (BeckhoffContext.Controller.MotorsEncoder[1]*UnitConverter.PulsToDegFactor[1]),
                    (BeckhoffContext.Controller.MotorsEncoder[2]*UnitConverter.PulsToDegFactor[2]),
                    (BeckhoffContext.Controller.MotorsEncoder[3]*UnitConverter.PulsToDegFactor[3]),
                    (BeckhoffContext.Controller.MotorsEncoder[4]*UnitConverter.PulsToDegFactor[4]),
                    (BeckhoffContext.Controller.MotorsEncoder[5]*UnitConverter.PulsToDegFactor[5]),
                };
            var tmpVals = new List<decimal>();
            tmpVals.AddRange(new decimal[] { theta[0], theta[1], theta[2], theta[3],Convert.ToDecimal( _m5Angle), theta[5], PositionSpeed, 1 });
            MoveWithValue(tmpVals);
        }
        private bool CanM5Move(object obj)
        {
            if (TrajectoryStarted)
                return false;
            float tmp;

            if (obj != null &&
                float.TryParse(obj.ToString(), out tmp) &&
                tmp < SixRConstants.MotorMovementRange[4] &&
                tmp > -SixRConstants.MotorMovementRange[4])
            {
                HasValidAngle[4] = true;
                AllMoveCommand.CanExecute(null);
                return true;
            }
            HasValidAngle[4] = false;
            AllMoveCommand.CanExecute(null);
            return false;
        }

        public void M6Move(object obj)
        {
            decimal[] theta =
                            {
                    (BeckhoffContext.Controller.MotorsEncoder[0]*UnitConverter.PulsToDegFactor[0]),
                    (BeckhoffContext.Controller.MotorsEncoder[1]*UnitConverter.PulsToDegFactor[1]),
                    (BeckhoffContext.Controller.MotorsEncoder[2]*UnitConverter.PulsToDegFactor[2]),
                    (BeckhoffContext.Controller.MotorsEncoder[3]*UnitConverter.PulsToDegFactor[3]),
                    (BeckhoffContext.Controller.MotorsEncoder[4]*UnitConverter.PulsToDegFactor[4]),
                    (BeckhoffContext.Controller.MotorsEncoder[5]*UnitConverter.PulsToDegFactor[5]),
                };
            var tmpVals = new List<decimal>();
            tmpVals.AddRange(new[] { theta[0], theta[1], theta[2], theta[3], theta[4],Convert.ToDecimal( _m6Angle), PositionSpeed, 1 });
            MoveWithValue(tmpVals);
        }
        private bool CanM6Move(object obj)
        {
            if (TrajectoryStarted)
                return false;
            float tmp;

            if (obj != null &&
                float.TryParse(obj.ToString(), out tmp) &&
                tmp < SixRConstants.MotorMovementRange[5] &&
                tmp > -SixRConstants.MotorMovementRange[5])
            {
                HasValidAngle[5] = true;
                AllMoveCommand.CanExecute(null);
                return true;
            }
            HasValidAngle[5] = false;
            AllMoveCommand.CanExecute(null);
            return false;
        }

        public void AllMove(object obj)
        {
            var tmpVals = new List<double>();
            tmpVals.AddRange(new double[] { _m1Angle, _m2Angle, _m3Angle, _m4Angle, _m5Angle, _m6Angle, PositionSpeed, 1 });
            MoveWithValue(tmpVals.Select(a=>Convert.ToDecimal(a)).ToList());
        }
        private bool CanAllMove(object obj)
        {
            if (TrajectoryStarted)
                return false;
            foreach (var itm in HasValidAngle)
            {
                if (!itm)
                    return false;
            }
            return true;
        }

        public void HomeMove()
        {
            var tmpVals = new List<decimal>();
            tmpVals.AddRange(new decimal[] { 0, 0, 0, 0, 0, 0, PositionSpeed, 1 });
            MoveWithValue(tmpVals);
        }
        private void MoveWithValue(List<decimal> vals)
        {
            TrajectoryStarted = true;
            //OnPropertyChanged("");
            decimal[] theta =
            {
                    (BeckhoffContext.Controller.MotorsEncoder[0]*UnitConverter.PulsToDegFactor[0]),
                    (BeckhoffContext.Controller.MotorsEncoder[1]*UnitConverter.PulsToDegFactor[1]),
                    (BeckhoffContext.Controller.MotorsEncoder[2]*UnitConverter.PulsToDegFactor[2]),
                    (BeckhoffContext.Controller.MotorsEncoder[3]*UnitConverter.PulsToDegFactor[3]),
                    (BeckhoffContext.Controller.MotorsEncoder[4]*UnitConverter.PulsToDegFactor[4]),
                    (BeckhoffContext.Controller.MotorsEncoder[5]*UnitConverter.PulsToDegFactor[5]),
                };
            var tmpKeys = new List<string>();
            tmpKeys.AddRange(new[] { "J1", "J2", "J3", "J4", "J5", "J6", "F", "CON" });
            var tmpVals = vals;
            var trajectory = BeckhoffContext.Traj.PTPList(theta, tmpKeys, tmpVals);
            var points = new TrajectoryPointList<int>[SixRConstants.NumberOfAxis];
            for (var j = 0; j < trajectory[0].TrajLength; j++)
            {
                for (var i = 0; i < SixRConstants.NumberOfAxis; i++)
                {
                    if (points[i] == null)
                        points[i] = new TrajectoryPointList<int>();
                    points[i].AddPoint((int)( trajectory[i].q[j] / UnitConverter.PulsToDegFactor[i]),(int) trajectory[i].v[j], (int)trajectory[i].a[j]);
                }
            }
            if (trajectory.Length == 0)
                return;
            Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            BeckhoffContext.Controller.SetRegisterCtrWord(new ushort[] { 15, 15, 15, 15, 15, 15 });
                            BeckhoffContext.Controller.SetCommand((int)CommandsEnum.SetControlWord);
                            BeckhoffContext.Controller.SetSelectedMotors(new bool[] { true, true, true, true, true, true });
                            BeckhoffContext.Controller.InitilizeTrajectory(points);
                            BeckhoffContext.Controller.SetCommand((int)CommandsEnum.StartTrajectory);
                        }));
        }
    }
}
