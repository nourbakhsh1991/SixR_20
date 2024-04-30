using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FirstFloor.ModernUI.Presentation;
using MathNet.Numerics.LinearAlgebra.Double;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using SixR_20.Bootstrapper;
using SixR_20.Models;

namespace SixR_20.ViewModels.MainRegion
{
    class JogViewModel : BaseViewModel
    {
        #region ' Private Variables '
        private bool[] _jogSelectedMotors = new bool[6];
        private decimal _jogSpeed = 100;
        private decimal _jogAcceleration = 100;
        private bool[] _cartesianJogSelectedMotors = new bool[SixRConstants.NumberOfAxis];
        private int[] _cartesianJogDirection = new int[SixRConstants.NumberOfAxis];
        public decimal[] _cartesianjogLastPosition = null;
        private decimal[] _cartesiancurrentJogSpeed = new decimal[SixRConstants.NumberOfAxis];
        private int _lastIkSolutionBranchNumber = 0;
        #endregion
        #region ' public Variables '
        public ICommand M1MoveLeftCommand { get; internal set; }
        public ICommand M2MoveLeftCommand { get; internal set; }
        public ICommand M3MoveLeftCommand { get; internal set; }
        public ICommand M4MoveLeftCommand { get; internal set; }
        public ICommand M5MoveLeftCommand { get; internal set; }
        public ICommand M6MoveLeftCommand { get; internal set; }
        public ICommand M1MoveRightCommand { get; internal set; }
        public ICommand M2MoveRightCommand { get; internal set; }
        public ICommand M3MoveRightCommand { get; internal set; }
        public ICommand M4MoveRightCommand { get; internal set; }
        public ICommand M5MoveRightCommand { get; internal set; }
        public ICommand M6MoveRightCommand { get; internal set; }
        public ICommand M1PMoveLeftCommand { get; internal set; }
        public ICommand M1PMoveRightCommand { get; internal set; }
        public ICommand M3PMoveLeftCommand { get; internal set; }
        public ICommand M3PMoveRightCommand { get; internal set; }

        #endregion

        public decimal JogSpeed
        {
            get { return _jogSpeed; }
            set
            {
                _jogSpeed = (value > 1) ? value : 1; OnPropertyChanged(nameof(JogSpeed));
            }
        }
        public decimal JogAcceleration
        {
            get { return _jogAcceleration; }
            set { _jogAcceleration = (value > 1) ? value : 1; OnPropertyChanged(nameof(JogAcceleration)); }
        }
        public JogViewModel(IUnityContainer container)
        {
            this.Initialize(container);
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                BeckhoffContext.Controller.PropertyChanged += Controller_PropertyChanged;
            }));
        }
        private void Controller_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "GUI_Alarms":
                case "GUI_Flags":
                    _cartesianjogLastPosition = null;
                    _cartesiancurrentJogSpeed = new decimal[SixRConstants.NumberOfAxis];
                    break;
                case "BufferNumber":
                    if (BeckhoffContext.Controller.CurrentCommand == 4 && BeckhoffContext.modeOfGUI == 2)
                    {
                        CartesionJog(BeckhoffContext.Controller.BufferNumber);
                    }
                    break;
            }
        }
        private void Initialize(IUnityContainer container)
        {
            this.Container = container;
            var eventAggregator = container.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
            JogSpeed = 100;
            M1MoveLeftCommand = new DelegateCommand<object>(M1LeftJog);
            M2MoveLeftCommand = new DelegateCommand<object>(M2LeftJog);
            M3MoveLeftCommand = new DelegateCommand<object>(M3LeftJog);
            M4MoveLeftCommand = new DelegateCommand<object>(M4LeftJog);
            M5MoveLeftCommand = new DelegateCommand<object>(M5LeftJog);
            M6MoveLeftCommand = new DelegateCommand<object>(M6LeftJog);

            M1MoveRightCommand = new DelegateCommand<object>(M1RightJog);
            M2MoveRightCommand = new DelegateCommand<object>(M2RightJog);
            M3MoveRightCommand = new DelegateCommand<object>(M3RightJog);
            M4MoveRightCommand = new DelegateCommand<object>(M4RightJog);
            M5MoveRightCommand = new DelegateCommand<object>(M5RightJog);
            M6MoveRightCommand = new DelegateCommand<object>(M6RightJog);

            M1PMoveLeftCommand = new DelegateCommand<object>(M1PLeftJog);
            M3PMoveLeftCommand = new DelegateCommand<object>(M3PLeftJog);

            M1PMoveRightCommand = new DelegateCommand<object>(M1PRightJog);
            M3PMoveRightCommand = new DelegateCommand<object>(M3PRightJog);
        }
        public void M1LeftJog(object param)
        {
            uint[] mSpeeds = new uint[6];
            for (int i = 0; i < 6; i++)
                mSpeeds[i] = (uint)(JogSpeed / Math.Abs(UnitConverter.PulsToDegFactor[i]));
            BeckhoffContext.Controller.GUI_JogMaxSpeed = (mSpeeds);
            if (bool.Parse(param.ToString()))
            {
                BeckhoffContext.Controller.JogDirection[0] = -1;
                BeckhoffContext.Controller.SetMotorNumber(0);
                var mSpeed = (int)(-1 * 70.0m / UnitConverter.PulsToDegFactor[0]);
                BeckhoffContext.Controller.SetRegisterTargetVelocity(mSpeed, 1);
                _jogSelectedMotors[0] = true;
                JogMove();
            }
            else
            {
                _jogSelectedMotors[0] = false;
                BeckhoffContext.Controller.JogDirection[0] = 1;
                var tmp = true;
                for (int j = 0; j < 6; j++)
                    tmp = tmp & !_jogSelectedMotors[j];
                if (tmp)
                    BeckhoffContext.Controller.StopJog();
            }
        }
        public void M1PLeftJog(object param)
        {

            _cartesianjogLastPosition = null;
            var toolParam = SixRConstants.toolParam;
            decimal[] theta =
                {
                    BeckhoffContext.Controller.MotorsEncoder[0]*UnitConverter.PulsToDegFactor[0],
                    BeckhoffContext.Controller.MotorsEncoder[1]*UnitConverter.PulsToDegFactor[1],
                    BeckhoffContext.Controller.MotorsEncoder[2]*UnitConverter.PulsToDegFactor[2],
                    BeckhoffContext.Controller.MotorsEncoder[3]*UnitConverter.PulsToDegFactor[3],
                    BeckhoffContext.Controller.MotorsEncoder[4]*UnitConverter.PulsToDegFactor[4],
                    BeckhoffContext.Controller.MotorsEncoder[5]*UnitConverter.PulsToDegFactor[5],
                };
            decimal[] cartesianPosition = null;

            cartesianPosition = BeckhoffContext.Traj.GetCartPos(theta.Select(a => a * Convert.ToDecimal(Math.PI / 180)).ToArray(),
                toolParam);
            var rpy = BeckhoffContext.Traj.toEulerianAngle(cartesianPosition);
            _cartesianjogLastPosition = new[]
            {
                    cartesianPosition[5], cartesianPosition[6], cartesianPosition[7],
                    rpy[0], rpy[1], rpy[2]
                };

            if (bool.Parse(param.ToString()))
            {
                var traj = new List<TrajectoryPointList<decimal>[]>();
                _cartesianJogSelectedMotors[0] = true;
                _cartesianJogDirection[0] = -1;
                var xyzrpy = new[]
                {
                    _cartesianjogLastPosition[0],
                    _cartesianjogLastPosition[1],
                    _cartesianjogLastPosition[2],
                    _cartesianjogLastPosition[3],
                    _cartesianjogLastPosition[4],
                    _cartesianjogLastPosition[5]
                };

                var output = new TrajectoryPointList<decimal>[SixRConstants.NumberOfAxis];

                _cartesiancurrentJogSpeed[0] = 0;
                var PositionActualValue = 0.0m;
                for (int i = 0; i < SixRConstants.NumberOfAxis; i++)
                    output[i] = new TrajectoryPointList<decimal>();
                for (int i = 0; i < SixRConstants.BufferLen * 2; i++)
                {
                    _cartesiancurrentJogSpeed[0] += .00001m * JogAcceleration > 5 ? 5 : .00001m * JogAcceleration;
                    if (_cartesiancurrentJogSpeed[0] > 50)
                        _cartesiancurrentJogSpeed[0] = 50;
                    PositionActualValue += .001m * _cartesiancurrentJogSpeed[0];
                    output[0].AddPoint(xyzrpy[0] + _cartesianJogDirection[0] * PositionActualValue, 0, 0);
                    output[1].AddPoint(xyzrpy[1], 0, 0);
                    output[2].AddPoint(xyzrpy[2], 0, 0);
                    output[3].AddPoint(xyzrpy[3], 0, 0);
                    output[4].AddPoint(xyzrpy[4], 0, 0);
                    output[5].AddPoint(xyzrpy[5], 0, 0);
                }
                for (int j = 0; j < 6; j++)
                {
                    _cartesianjogLastPosition[j] = output[j].q[SixRConstants.BufferLen * 2 - 1];
                }
                var quaternionOfRpy = new decimal[] { cartesianPosition[0], cartesianPosition[1], cartesianPosition[2], cartesianPosition[3] };
                var Ans = BeckhoffContext.Traj.Inversekinematic(new[]
                {
                                quaternionOfRpy[0], quaternionOfRpy[1], quaternionOfRpy[2], quaternionOfRpy[3],
                                0, output[0].q[0], output[1].q[0], output[2].q[0]
                            }, toolParam);
                bool[] ValidIkSolutionBranchNumber = { true, true, true, true, true, true, true, true };

                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        if (Math.Abs((double)Ans[i, j] * (180 / Math.PI)) > (double)BeckhoffContext.ValidJointSpace[j])
                            ValidIkSolutionBranchNumber[i] = false;
                    }
                }
                var jointDisplacement = new double[8];
                var min = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (ValidIkSolutionBranchNumber[i])
                    {
                        for (int j = 0; j < 6; j++)
                            jointDisplacement[i] += Math.Abs((double)Ans[i, j] - (double)theta[j] * Math.PI / 180.0);
                    }
                    else
                    {
                        jointDisplacement[i] = double.MaxValue;
                    }
                    if (jointDisplacement[i] < jointDisplacement[min])
                        min = i;

                }
                _lastIkSolutionBranchNumber = min;
                for (int i = 0; i < SixRConstants.BufferLen * 2; i++)
                {
                    quaternionOfRpy = BeckhoffContext.Traj.toQuaternion(output[3].q[i], output[4].q[i], output[5].q[i]);
                    Ans = BeckhoffContext.Traj.Inversekinematic(new[]
                    {
                        quaternionOfRpy[0],quaternionOfRpy[1],quaternionOfRpy[2],quaternionOfRpy[3],
                        0,output[0].q[i],output[1].q[i],output[2].q[i]
                    }, toolParam);


                    output[0].q[i] = Ans[_lastIkSolutionBranchNumber, 0];
                    output[1].q[i] = Ans[_lastIkSolutionBranchNumber, 1];
                    output[2].q[i] = Ans[_lastIkSolutionBranchNumber, 2];
                    output[3].q[i] = Ans[_lastIkSolutionBranchNumber, 3];
                    while (i > 0 && Math.Abs((double)(output[3].q[i] - output[3].q[i - 1])) > (Math.PI))
                    {
                        var sign = -Math.Sign(output[3].q[i] - output[3].q[i - 1]);
                        output[3].q[i] += sign * 2 * Convert.ToDecimal(Math.PI);
                    }
                    while (i == 0 && Math.Abs(((double)output[3].q[i] - ((double)theta[3] * Math.PI / 180.0))) > (Math.PI))
                    {
                        var sign = -Math.Sign((double)output[3].q[i] - ((double)theta[3] * Math.PI / 180.0));
                        output[3].q[i] += sign * 2 * Convert.ToDecimal(Math.PI);
                    }
                    output[4].q[i] = Ans[_lastIkSolutionBranchNumber, 4];
                    output[5].q[i] = Ans[_lastIkSolutionBranchNumber, 5];
                    while (i > 0 && Math.Abs((double)(output[5].q[i] - output[5].q[i - 1])) > (Math.PI))
                    {
                        var sign = -Math.Sign(output[5].q[i] - output[5].q[i - 1]);
                        output[5].q[i] += sign * 2 * Convert.ToDecimal(Math.PI);
                    }
                    while (i == 0 && Math.Abs(((double)output[5].q[i] - ((double)theta[3] * Math.PI / 180.0))) > (Math.PI))
                    {
                        var sign = -Math.Sign((double)output[5].q[i] - ((double)theta[3] * Math.PI / 180.0));
                        output[3].q[i] += sign * 2 * Convert.ToDecimal(Math.PI);
                    }
                    for (int j = 0; j < 6; j++)
                    {
                        if (Math.Abs((double)Ans[_lastIkSolutionBranchNumber, j] * (180 / Math.PI)) > (double)BeckhoffContext.ValidJointSpace[j])
                        {
                            BeckhoffContext.Controller.SetTrajLength(BeckhoffContext.Controller.TrajList[0].TrajLength);
                            throw new Exception("");

                        }
                    }
                }
                //output = BeckhoffContext.Traj.RotationCurrection(output);

                traj.Add(output);

                var points = new TrajectoryPointList<int>[SixRConstants.NumberOfAxis];
                if (traj.Count == 0)
                    return;
                for (int i = 0; i < SixRConstants.NumberOfAxis; i++)
                    points[i] = new TrajectoryPointList<int>();
                foreach (var val in traj)
                {
                    for (var j = 0; j < val[0].TrajLength; j++)
                    {
                        for (var i = 0; i < SixRConstants.NumberOfAxis; i++)
                        {
                            points[i].AddPoint((int)(((double)val[i].q[j] * (180 / Math.PI)) / (double)UnitConverter.PulsToDegFactor[i]), (int)val[i].v[j], (int)val[i].a[j]);
                        }
                    }
                }
                BeckhoffContext.Controller.SetRegisterCtrWord(new ushort[] { 15, 15, 15, 15, 15, 15 });
                BeckhoffContext.Controller.SetCommand((int)CommandsEnum.SetControlWord);
                BeckhoffContext.Controller.SetSelectedMotors(new[] { true, true, true, true, true, true });
                BeckhoffContext.Controller.InitilizeTrajJog(points);
                BeckhoffContext.Controller.SetCommand((int)CommandsEnum.StartTrajectory);
            }
            else
            {
                _cartesianJogSelectedMotors[0] = false;
                //_jogSelectedMotors[0] = false;
                //BeckhoffContext.Controller.JogDirection[0] = 1;
                //var tmp = true;
                //for (int j = 0; j < 6; j++)
                //    tmp = tmp & !_jogSelectedMotors[j];
                //if (tmp)
                //    BeckhoffContext.Controller.StopJog();
            }
        }
        public void M3PLeftJog(object param)
        {
            //var toolParam = SixRConstants.toolParam;
            //double[] theta =
            //    {
            //        BeckhoffContext.Controller.MotorsEncoder[0]*UnitConverter.PulsToDegFactor[0],
            //        BeckhoffContext.Controller.MotorsEncoder[1]*UnitConverter.PulsToDegFactor[1],
            //        BeckhoffContext.Controller.MotorsEncoder[2]*UnitConverter.PulsToDegFactor[2],
            //        BeckhoffContext.Controller.MotorsEncoder[3]*UnitConverter.PulsToDegFactor[3],
            //        BeckhoffContext.Controller.MotorsEncoder[4]*UnitConverter.PulsToDegFactor[4],
            //        BeckhoffContext.Controller.MotorsEncoder[5]*UnitConverter.PulsToDegFactor[5],
            //    };
            //double[] cartesianPosition = null;
            //if (_cartesianjogLastPosition == null)
            //{


            //    cartesianPosition = BeckhoffContext.Traj.GetCartPos(theta.Select(a => a * (Math.PI / 180)).ToArray(),
            //        toolParam);
            //    var rpy = BeckhoffContext.Traj.toEulerianAngle(new DenseVector(cartesianPosition));
            //    _cartesianjogLastPosition = new[]
            //    {
            //        cartesianPosition[5], cartesianPosition[6], cartesianPosition[7],
            //        rpy[0], rpy[1], rpy[2]
            //    };
            //}
            //if (bool.Parse(param.ToString()))
            //{
            //    var traj = new List<TrajectoryPointList[]>();
            //    _cartesianJogSelectedMotors[2] = true;
            //    _cartesianJogDirection[2] = -1;
            //    var xyzrpy = new[]
            //    {
            //        _cartesianjogLastPosition[0],
            //        _cartesianjogLastPosition[1],
            //        _cartesianjogLastPosition[2],
            //        _cartesianjogLastPosition[3],
            //        _cartesianjogLastPosition[4],
            //        _cartesianjogLastPosition[5]
            //    };

            //    TrajectoryPointList[] output = new TrajectoryPointList[SixRConstants.NumberOfAxis];

            //    _cartesiancurrentJogSpeed[2] = 0;
            //    var PositionActualValue = 0D;
            //    for (int i = 0; i < SixRConstants.NumberOfAxis; i++)
            //        output[i] = new TrajectoryPointList();
            //    for (int i = 0; i < SixRConstants.BufferLen * 2; i++)
            //    {
            //        _cartesiancurrentJogSpeed[2] += .00001 * JogAcceleration > 5 ? 5 : .00001 * JogAcceleration;
            //        if (_cartesiancurrentJogSpeed[2] > 50)
            //            _cartesiancurrentJogSpeed[2] = 50;
            //        PositionActualValue += .001 * _cartesiancurrentJogSpeed[2];
            //        output[0].AddPoint(xyzrpy[0], 0, 0);
            //        output[1].AddPoint(xyzrpy[1], 0, 0);
            //        output[2].AddPoint(xyzrpy[2] + _cartesianJogDirection[2] * PositionActualValue, 0, 0);
            //        output[3].AddPoint(xyzrpy[3], 0, 0);
            //        output[4].AddPoint(xyzrpy[4], 0, 0);
            //        output[5].AddPoint(xyzrpy[5], 0, 0);
            //    }
            //    for (int j = 0; j < 6; j++)
            //    {
            //        _cartesianjogLastPosition[j] = output[j].q[SixRConstants.BufferLen * 2 - 1];
            //    }
            //    var quaternionOfRpy = BeckhoffContext.Traj.toQuaternion(output[3].q[0], output[4].q[0], output[5].q[0]);
            //    if (cartesianPosition != null)
            //    {
            //        quaternionOfRpy[0] = cartesianPosition[0];
            //        quaternionOfRpy[1] = cartesianPosition[1];
            //        quaternionOfRpy[2] = cartesianPosition[2];
            //        quaternionOfRpy[3] = cartesianPosition[3];
            //    }
            //    var Ans = BeckhoffContext.Traj.Inversekinematic(new[]
            //    {
            //                    quaternionOfRpy[0], quaternionOfRpy[1], quaternionOfRpy[2], quaternionOfRpy[3],
            //                    0, output[0].q[0], output[1].q[0], output[2].q[0]
            //                }, toolParam);
            //    bool[] ValidIkSolutionBranchNumber = { true, true, true, true, true, true, true, true };

            //    for (int i = 0; i < 8; i++)
            //    {
            //        for (int j = 0; j < 6; j++)
            //        {
            //            if (Math.Abs(Ans[i, j] * (180 / Math.PI)) > BeckhoffContext.ValidJointSpace[j])
            //                ValidIkSolutionBranchNumber[i] = false;
            //        }
            //    }
            //    var jointDisplacement = new double[8];
            //    var min = 0;
            //    for (int i = 0; i < 8; i++)
            //    {
            //        if (ValidIkSolutionBranchNumber[i])
            //        {
            //            for (int j = 0; j < 6; j++)
            //                jointDisplacement[i] += Math.Abs(Ans[i, j] - theta[j] * Math.PI / 180.0);
            //        }
            //        else
            //        {
            //            jointDisplacement[i] = double.MaxValue;
            //        }
            //        if (jointDisplacement[i] < jointDisplacement[min])
            //            min = i;

            //    }
            //    _lastIkSolutionBranchNumber = min;
            //    for (int i = 0; i < SixRConstants.BufferLen * 2; i++)
            //    {
            //        quaternionOfRpy = BeckhoffContext.Traj.toQuaternion(output[3].q[i], output[4].q[i], output[5].q[i]);
            //        Ans = BeckhoffContext.Traj.Inversekinematic(new[]
            //        {
            //            quaternionOfRpy[0],quaternionOfRpy[1],quaternionOfRpy[2],quaternionOfRpy[3],
            //            0,output[0].q[i],output[1].q[i],output[2].q[i]
            //        }, toolParam);


            //        output[0].q[i] = Ans[_lastIkSolutionBranchNumber, 0];
            //        output[1].q[i] = Ans[_lastIkSolutionBranchNumber, 1];
            //        output[2].q[i] = Ans[_lastIkSolutionBranchNumber, 2];
            //        output[3].q[i] = Ans[_lastIkSolutionBranchNumber, 3];
            //        while (i > 0 && Math.Abs(output[3].q[i] - output[3].q[i - 1]) > (Math.PI - .1))
            //        {
            //            var sign = -Math.Sign(output[3].q[i] - output[3].q[i - 1]);
            //            output[3].q[i] += sign * 2 * Math.PI;
            //        }
            //        output[4].q[i] = Ans[_lastIkSolutionBranchNumber, 4];
            //        output[5].q[i] = Ans[_lastIkSolutionBranchNumber, 5];
            //        while (i > 0 && Math.Abs(output[5].q[i] - output[5].q[i - 1]) > (Math.PI - .1))
            //        {
            //            var sign = -Math.Sign(output[5].q[i] - output[5].q[i - 1]);
            //            output[5].q[i] += sign * 2 * Math.PI;
            //        }
            //        for (int j = 0; j < 6; j++)
            //        {
            //            if (Math.Abs(Ans[_lastIkSolutionBranchNumber, j] * (180 / Math.PI)) > BeckhoffContext.ValidJointSpace[j])
            //            {
            //                BeckhoffContext.Controller.SetTrajLength(BeckhoffContext.Controller.TrajList[0].TrajLength);
            //                throw new Exception("");

            //            }
            //        }
            //    }
            //    //output = BeckhoffContext.Traj.RotationCurrection(output);

            //    traj.Add(output);

            //    var points = new TrajectoryPointList[SixRConstants.NumberOfAxis];
            //    if (traj.Count == 0)
            //        return;
            //    for (int i = 0; i < SixRConstants.NumberOfAxis; i++)
            //        points[i] = new TrajectoryPointList();
            //    foreach (var val in traj)
            //    {
            //        for (var j = 0; j < val[0].TrajLength; j++)
            //        {
            //            for (var i = 0; i < SixRConstants.NumberOfAxis; i++)
            //            {

            //                points[i].AddPoint((val[i].q[j] * (180 / Math.PI)) / UnitConverter.PulsToDegFactor[i], val[i].v[j], val[i].a[j]);
            //            }
            //        }
            //    }
            //    BeckhoffContext.Controller.SetRegisterCtrWord(new ushort[] { 15, 15, 15, 15, 15, 15 });
            //    BeckhoffContext.Controller.SetCommand((int)CommandsEnum.SetControlWord);
            //    BeckhoffContext.Controller.SetSelectedMotors(new[] { true, true, true, true, true, true });
            //    BeckhoffContext.Controller.InitilizeTrajJog(points);
            //    BeckhoffContext.Controller.SetCommand((int)CommandsEnum.StartTrajectory);

            //}
            //else
            //{
            //    _cartesianJogSelectedMotors[2] = false;
            //    //_jogSelectedMotors[0] = false;
            //    //BeckhoffContext.Controller.JogDirection[0] = 1;
            //    //var tmp = true;
            //    //for (int j = 0; j < 6; j++)
            //    //    tmp = tmp & !_jogSelectedMotors[j];
            //    //if (tmp)
            //    //    BeckhoffContext.Controller.StopJog();
            //}
        }
        public void M2LeftJog(object param)
        {
            uint[] mSpeeds = new uint[6];
            for (int i = 0; i < 6; i++)
                mSpeeds[i] = (uint)(JogSpeed / Math.Abs(UnitConverter.PulsToDegFactor[i]));
            BeckhoffContext.Controller.GUI_JogMaxSpeed = (mSpeeds);
            if (bool.Parse(param.ToString()))
            {
                BeckhoffContext.Controller.JogDirection[1] = -1;
                BeckhoffContext.Controller.SetMotorNumber(1);
                var mSpeed = (int)(-1 * 70.0m / UnitConverter.PulsToDegFactor[1]);
                BeckhoffContext.Controller.SetRegisterTargetVelocity(mSpeed, 2);
                _jogSelectedMotors[1] = true;
                JogMove();
            }
            else
            {
                _jogSelectedMotors[1] = false;
                BeckhoffContext.Controller.JogDirection[1] = 1;
                var tmp = true;
                for (int j = 0; j < 6; j++)
                    tmp = tmp & !_jogSelectedMotors[j];
                if (tmp)
                    BeckhoffContext.Controller.StopJog();
            }
        }
        public void M3LeftJog(object param)
        {
            uint[] mSpeeds = new uint[6];
            for (int i = 0; i < 6; i++)
                mSpeeds[i] = (uint)(JogSpeed / Math.Abs(UnitConverter.PulsToDegFactor[i]));
            BeckhoffContext.Controller.GUI_JogMaxSpeed = (mSpeeds);
            if (bool.Parse(param.ToString()))
            {
                BeckhoffContext.Controller.JogDirection[2] = -1;
                BeckhoffContext.Controller.SetMotorNumber(2);
                var mSpeed = (int)(-1 * 70.0m/ UnitConverter.PulsToDegFactor[2]);
                BeckhoffContext.Controller.SetRegisterTargetVelocity(mSpeed, 3);
                _jogSelectedMotors[2] = true;
                JogMove();
            }
            else
            {
                _jogSelectedMotors[2] = false;
                BeckhoffContext.Controller.JogDirection[2] = 1;
                var tmp = true;
                for (int j = 0; j < 6; j++)
                    tmp = tmp & !_jogSelectedMotors[j];
                if (tmp)
                    BeckhoffContext.Controller.StopJog();
            }
        }
        public void M4LeftJog(object param)
        {
            uint[] mSpeeds = new uint[6];
            for (int i = 0; i < 6; i++)
                mSpeeds[i] = (uint)(JogSpeed / Math.Abs(UnitConverter.PulsToDegFactor[i]));
            BeckhoffContext.Controller.GUI_JogMaxSpeed = (mSpeeds);
            if (bool.Parse(param.ToString()))
            {
                BeckhoffContext.Controller.JogDirection[3] = -1;
                BeckhoffContext.Controller.SetMotorNumber(3);
                var mSpeed = (int)(-1 * 70.0m / UnitConverter.PulsToDegFactor[3]);
                BeckhoffContext.Controller.SetRegisterTargetVelocity(mSpeed, 4);
                _jogSelectedMotors[3] = true;
                JogMove();
            }
            else
            {
                _jogSelectedMotors[3] = false;
                BeckhoffContext.Controller.JogDirection[3] = 1;
                var tmp = true;
                for (int j = 0; j < 6; j++)
                    tmp = tmp & !_jogSelectedMotors[j];
                if (tmp)
                    BeckhoffContext.Controller.StopJog();
            }
        }
        public void M5LeftJog(object param)
        {
            uint[] mSpeeds = new uint[6];
            for (int i = 0; i < 6; i++)
                mSpeeds[i] = (uint)(JogSpeed / Math.Abs(UnitConverter.PulsToDegFactor[i]));
            BeckhoffContext.Controller.GUI_JogMaxSpeed = (mSpeeds);
            if (bool.Parse(param.ToString()))
            {
                BeckhoffContext.Controller.JogDirection[4] = -1;
                BeckhoffContext.Controller.SetMotorNumber(4);
                var mSpeed = (int)(-1 * 70.0m / UnitConverter.PulsToDegFactor[4]);
                BeckhoffContext.Controller.SetRegisterTargetVelocity(mSpeed, 5);
                _jogSelectedMotors[4] = true;
                JogMove();
            }
            else
            {
                _jogSelectedMotors[4] = false;
                BeckhoffContext.Controller.JogDirection[4] = 1;
                var tmp = true;
                for (int j = 0; j < 6; j++)
                    tmp = tmp & !_jogSelectedMotors[j];
                if (tmp)
                    BeckhoffContext.Controller.StopJog();
            }
        }
        public void M6LeftJog(object param)
        {
            uint[] mSpeeds = new uint[6];
            for (int i = 0; i < 6; i++)
                mSpeeds[i] = (uint)(JogSpeed / Math.Abs(UnitConverter.PulsToDegFactor[i]));
            BeckhoffContext.Controller.GUI_JogMaxSpeed = (mSpeeds);
            if (bool.Parse(param.ToString()))
            {
                BeckhoffContext.Controller.JogDirection[5] = -1;
                BeckhoffContext.Controller.SetMotorNumber(5);
                var mSpeed = (int)(-1 * 70.0m / UnitConverter.PulsToDegFactor[5]);
                BeckhoffContext.Controller.SetRegisterTargetVelocity(mSpeed, 6);
                _jogSelectedMotors[5] = true;
                JogMove();
            }
            else
            {
                _jogSelectedMotors[5] = false;
                BeckhoffContext.Controller.JogDirection[5] = 1;
                var tmp = true;
                for (int j = 0; j < 6; j++)
                    tmp = tmp & !_jogSelectedMotors[j];
                if (tmp)
                    BeckhoffContext.Controller.StopJog();
            }
        }
        public void M1RightJog(object param)
        {
            uint[] mSpeeds = new uint[6];
            for (int i = 0; i < 6; i++)
                mSpeeds[i] = (uint)(JogSpeed / Math.Abs(UnitConverter.PulsToDegFactor[i]));
            BeckhoffContext.Controller.GUI_JogMaxSpeed = (mSpeeds);
            if (bool.Parse(param.ToString()))
            {
                BeckhoffContext.Controller.JogDirection[0] = 1;
                BeckhoffContext.Controller.SetMotorNumber(0);
                var mSpeed = (int)(-1 * 70.0m / UnitConverter.PulsToDegFactor[0]);
                BeckhoffContext.Controller.SetRegisterTargetVelocity(mSpeed, 1);
                _jogSelectedMotors[0] = true;
                JogMove();
            }
            else
            {
                _jogSelectedMotors[0] = false;
                BeckhoffContext.Controller.JogDirection[0] = 1;
                var tmp = true;
                for (int j = 0; j < 6; j++)
                    tmp = tmp & !_jogSelectedMotors[j];
                if (tmp)
                    BeckhoffContext.Controller.StopJog();
            }
        }
        public void M1PRightJog(object param)
        {

            _cartesianjogLastPosition = null; 
            var toolParam = SixRConstants.toolParam;
            decimal[] theta =
                {
                    BeckhoffContext.Controller.MotorsEncoder[0]*UnitConverter.PulsToDegFactor[0],
                    BeckhoffContext.Controller.MotorsEncoder[1]*UnitConverter.PulsToDegFactor[1],
                    BeckhoffContext.Controller.MotorsEncoder[2]*UnitConverter.PulsToDegFactor[2],
                    BeckhoffContext.Controller.MotorsEncoder[3]*UnitConverter.PulsToDegFactor[3],
                    BeckhoffContext.Controller.MotorsEncoder[4]*UnitConverter.PulsToDegFactor[4],
                    BeckhoffContext.Controller.MotorsEncoder[5]*UnitConverter.PulsToDegFactor[5],
                };
            decimal[] cartesianPosition = null;

            cartesianPosition = BeckhoffContext.Traj.GetCartPos(theta.Select(a => a * Convert.ToDecimal(Math.PI / 180)).ToArray(),
                toolParam);
            var rpy = BeckhoffContext.Traj.toEulerianAngle(cartesianPosition);
            _cartesianjogLastPosition = new[]
            {
                    cartesianPosition[5], cartesianPosition[6], cartesianPosition[7],
                    rpy[0], rpy[1], rpy[2]
                };

            if (bool.Parse(param.ToString()))
            {
                var traj = new List<TrajectoryPointList<decimal>[]>();
                _cartesianJogSelectedMotors[0] = true;
                _cartesianJogDirection[0] = 1;
                var xyzrpy = new[]
                {
                    _cartesianjogLastPosition[0],
                    _cartesianjogLastPosition[1],
                    _cartesianjogLastPosition[2],
                    _cartesianjogLastPosition[3],
                    _cartesianjogLastPosition[4],
                    _cartesianjogLastPosition[5]
                };

                var output = new TrajectoryPointList<decimal>[SixRConstants.NumberOfAxis];

                _cartesiancurrentJogSpeed[0] = 0;
                var PositionActualValue = 0.0m;
                for (int i = 0; i < SixRConstants.NumberOfAxis; i++)
                    output[i] = new TrajectoryPointList<decimal>();
                for (int i = 0; i < SixRConstants.BufferLen * 2; i++)
                {
                    _cartesiancurrentJogSpeed[0] += .00001m * JogAcceleration > 5 ? 5 : .00001m * JogAcceleration;
                    if (_cartesiancurrentJogSpeed[0] > 50)
                        _cartesiancurrentJogSpeed[0] = 50;
                    PositionActualValue += .001m * _cartesiancurrentJogSpeed[0];
                    output[0].AddPoint(xyzrpy[0] + _cartesianJogDirection[0] * PositionActualValue, 0, 0);
                    output[1].AddPoint(xyzrpy[1], 0, 0);
                    output[2].AddPoint(xyzrpy[2], 0, 0);
                    output[3].AddPoint(xyzrpy[3], 0, 0);
                    output[4].AddPoint(xyzrpy[4], 0, 0);
                    output[5].AddPoint(xyzrpy[5], 0, 0);
                }
                for (int j = 0; j < 6; j++)
                {
                    _cartesianjogLastPosition[j] = output[j].q[SixRConstants.BufferLen * 2 - 1];
                }
                var quaternionOfRpy = new decimal[] { cartesianPosition[0], cartesianPosition[1], cartesianPosition[2], cartesianPosition[3] };
                var Ans = BeckhoffContext.Traj.Inversekinematic(new[]
                {
                                quaternionOfRpy[0], quaternionOfRpy[1], quaternionOfRpy[2], quaternionOfRpy[3],
                                0, output[0].q[0], output[1].q[0], output[2].q[0]
                            }, toolParam);
                bool[] ValidIkSolutionBranchNumber = { true, true, true, true, true, true, true, true };

                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        if (Math.Abs((double)Ans[i, j] * (180 / Math.PI)) > (double)BeckhoffContext.ValidJointSpace[j])
                            ValidIkSolutionBranchNumber[i] = false;
                    }
                }
                var jointDisplacement = new double[8];
                var min = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (ValidIkSolutionBranchNumber[i])
                    {
                        for (int j = 0; j < 6; j++)
                            jointDisplacement[i] += Math.Abs((double)Ans[i, j] - (double)theta[j] * Math.PI / 180.0);
                    }
                    else
                    {
                        jointDisplacement[i] = double.MaxValue;
                    }
                    if (jointDisplacement[i] < jointDisplacement[min])
                        min = i;

                }
                _lastIkSolutionBranchNumber = min;
                for (int i = 0; i < SixRConstants.BufferLen * 2; i++)
                {
                    quaternionOfRpy = BeckhoffContext.Traj.toQuaternion(output[3].q[i], output[4].q[i], output[5].q[i]);
                    Ans = BeckhoffContext.Traj.Inversekinematic(new[]
                    {
                        quaternionOfRpy[0],quaternionOfRpy[1],quaternionOfRpy[2],quaternionOfRpy[3],
                        0,output[0].q[i],output[1].q[i],output[2].q[i]
                    }, toolParam);


                    output[0].q[i] = Ans[_lastIkSolutionBranchNumber, 0];
                    output[1].q[i] = Ans[_lastIkSolutionBranchNumber, 1];
                    output[2].q[i] = Ans[_lastIkSolutionBranchNumber, 2];
                    output[3].q[i] = Ans[_lastIkSolutionBranchNumber, 3];
                    while (i > 0 && Math.Abs((double)(output[3].q[i] - output[3].q[i - 1])) > (Math.PI))
                    {
                        var sign = -Math.Sign(output[3].q[i] - output[3].q[i - 1]);
                        output[3].q[i] += sign * 2 * Convert.ToDecimal(Math.PI);
                    }
                    while(i==0 && Math.Abs(((double)output[3].q[i] - ((double)theta[3] * Math.PI / 180.0))) > (Math.PI))
                    {
                        var sign = -Math.Sign((double)output[3].q[i] - ((double)theta[3] * Math.PI / 180.0));
                        output[3].q[i] += sign * 2 * Convert.ToDecimal(Math.PI);
                    }
                    output[4].q[i] = Ans[_lastIkSolutionBranchNumber, 4];
                    output[5].q[i] = Ans[_lastIkSolutionBranchNumber, 5];
                    while (i > 0 && Math.Abs((double)(output[5].q[i] - output[5].q[i-1])) > (Math.PI))
                    {
                        var sign = -Math.Sign(output[5].q[i] - output[5].q[i-1]);
                        output[5].q[i] += sign * 2 * Convert.ToDecimal(Math.PI);
                    }
                    while (i == 0 && Math.Abs(((double)output[5].q[i] - ((double)theta[3] * Math.PI / 180.0))) > (Math.PI))
                    {
                        var sign = -Math.Sign((double)output[5].q[i] - ((double)theta[3] * Math.PI / 180.0));
                        output[3].q[i] += sign * 2 * Convert.ToDecimal(Math.PI);
                    }
                    for (int j = 0; j < 6; j++)
                    {
                        if (Math.Abs((double)Ans[_lastIkSolutionBranchNumber, j] * (180 / Math.PI)) > (double)BeckhoffContext.ValidJointSpace[j])
                        {
                            BeckhoffContext.Controller.SetTrajLength(BeckhoffContext.Controller.TrajList[0].TrajLength);
                            throw new Exception("");

                        }
                    }
                }
                //output = BeckhoffContext.Traj.RotationCurrection(output);

                traj.Add(output);

                var points = new TrajectoryPointList<int>[SixRConstants.NumberOfAxis];
                if (traj.Count == 0)
                    return;
                for (int i = 0; i < SixRConstants.NumberOfAxis; i++)
                    points[i] = new TrajectoryPointList<int>();
                foreach (var val in traj)
                {
                    for (var j = 0; j < val[0].TrajLength; j++)
                    {
                        for (var i = 0; i < SixRConstants.NumberOfAxis; i++)
                        {
                            points[i].AddPoint((int)(((double)val[i].q[j] * (180 / Math.PI)) / (double)UnitConverter.PulsToDegFactor[i]),(int) val[i].v[j], (int)val[i].a[j]);
                        }
                    }
                }
                BeckhoffContext.Controller.SetRegisterCtrWord(new ushort[] { 15, 15, 15, 15, 15, 15 });
                BeckhoffContext.Controller.SetCommand((int)CommandsEnum.SetControlWord);
                BeckhoffContext.Controller.SetSelectedMotors(new[] { true, true, true, true, true, true });
                BeckhoffContext.Controller.InitilizeTrajJog(points);
                BeckhoffContext.Controller.SetCommand((int)CommandsEnum.StartTrajectory);
            }
            else
            {
                _cartesianJogSelectedMotors[0] = false;
                //_jogSelectedMotors[0] = false;
                //BeckhoffContext.Controller.JogDirection[0] = 1;
                //var tmp = true;
                //for (int j = 0; j < 6; j++)
                //    tmp = tmp & !_jogSelectedMotors[j];
                //if (tmp)
                //    BeckhoffContext.Controller.StopJog();
            }
        }
        public void M3PRightJog(object param)
        {
            //var toolParam = SixRConstants.toolParam;
            //double[] theta =
            //    {
            //        BeckhoffContext.Controller.MotorsEncoder[0]*UnitConverter.PulsToDegFactor[0],
            //        BeckhoffContext.Controller.MotorsEncoder[1]*UnitConverter.PulsToDegFactor[1],
            //        BeckhoffContext.Controller.MotorsEncoder[2]*UnitConverter.PulsToDegFactor[2],
            //        BeckhoffContext.Controller.MotorsEncoder[3]*UnitConverter.PulsToDegFactor[3],
            //        BeckhoffContext.Controller.MotorsEncoder[4]*UnitConverter.PulsToDegFactor[4],
            //        BeckhoffContext.Controller.MotorsEncoder[5]*UnitConverter.PulsToDegFactor[5],
            //    };
            //double[] cartesianPosition = null;
            //if (_cartesianjogLastPosition == null)
            //{


            //    cartesianPosition = BeckhoffContext.Traj.GetCartPos(theta.Select(a => a * (Math.PI / 180)).ToArray(),
            //        toolParam);
            //    var rpy = BeckhoffContext.Traj.toEulerianAngle(new DenseVector(cartesianPosition));
            //    _cartesianjogLastPosition = new[]
            //    {
            //        cartesianPosition[5], cartesianPosition[6], cartesianPosition[7],
            //        rpy[0], rpy[1], rpy[2]
            //    };
            //}
            //if (bool.Parse(param.ToString()))
            //{
            //    var traj = new List<TrajectoryPointList[]>();
            //    _cartesianJogSelectedMotors[2] = true;
            //    _cartesianJogDirection[2] = 1;
            //    var xyzrpy = new[]
            //    {
            //        _cartesianjogLastPosition[0],
            //        _cartesianjogLastPosition[1],
            //        _cartesianjogLastPosition[2],
            //        _cartesianjogLastPosition[3],
            //        _cartesianjogLastPosition[4],
            //        _cartesianjogLastPosition[5]
            //    };

            //    TrajectoryPointList[] output = new TrajectoryPointList[SixRConstants.NumberOfAxis];

            //    _cartesiancurrentJogSpeed[2] = 0;
            //    var PositionActualValue = 0D;
            //    for (int i = 0; i < SixRConstants.NumberOfAxis; i++)
            //        output[i] = new TrajectoryPointList();
            //    for (int i = 0; i < SixRConstants.BufferLen * 2; i++)
            //    {
            //        _cartesiancurrentJogSpeed[2] += .00001 * JogAcceleration > 5 ? 5 : .00001 * JogAcceleration;
            //        if (_cartesiancurrentJogSpeed[2] > 50)
            //            _cartesiancurrentJogSpeed[2] = 50;
            //        PositionActualValue += .001 * _cartesiancurrentJogSpeed[2];
            //        output[0].AddPoint(xyzrpy[0], 0, 0);
            //        output[1].AddPoint(xyzrpy[1], 0, 0);
            //        output[2].AddPoint(xyzrpy[2] + _cartesianJogDirection[2] * PositionActualValue, 0, 0);
            //        output[3].AddPoint(xyzrpy[3], 0, 0);
            //        output[4].AddPoint(xyzrpy[4], 0, 0);
            //        output[5].AddPoint(xyzrpy[5], 0, 0);
            //    }
            //    for (int j = 0; j < 6; j++)
            //    {
            //        _cartesianjogLastPosition[j] = output[j].q[SixRConstants.BufferLen * 2 - 1];
            //    }
            //    var quaternionOfRpy = BeckhoffContext.Traj.toQuaternion(output[3].q[0], output[4].q[0], output[5].q[0]);
            //    if (cartesianPosition != null)
            //    {
            //        quaternionOfRpy[0] = cartesianPosition[0];
            //        quaternionOfRpy[1] = cartesianPosition[1];
            //        quaternionOfRpy[2] = cartesianPosition[2];
            //        quaternionOfRpy[3] = cartesianPosition[3];
            //    }
            //    var Ans = BeckhoffContext.Traj.Inversekinematic(new[]
            //    {
            //                    quaternionOfRpy[0], quaternionOfRpy[1], quaternionOfRpy[2], quaternionOfRpy[3],
            //                    0, output[0].q[0], output[1].q[0], output[2].q[0]
            //                }, toolParam);
            //    bool[] ValidIkSolutionBranchNumber = { true, true, true, true, true, true, true, true };

            //    for (int i = 0; i < 8; i++)
            //    {
            //        for (int j = 0; j < 6; j++)
            //        {
            //            if (Math.Abs(Ans[i, j] * (180 / Math.PI)) > BeckhoffContext.ValidJointSpace[j])
            //                ValidIkSolutionBranchNumber[i] = false;
            //        }
            //    }
            //    var jointDisplacement = new double[8];
            //    var min = 0;
            //    for (int i = 0; i < 8; i++)
            //    {
            //        if (ValidIkSolutionBranchNumber[i])
            //        {
            //            for (int j = 0; j < 6; j++)
            //                jointDisplacement[i] += Math.Abs(Ans[i, j] - theta[j] * Math.PI / 180.0);
            //        }
            //        else
            //        {
            //            jointDisplacement[i] = double.MaxValue;
            //        }
            //        if (jointDisplacement[i] < jointDisplacement[min])
            //            min = i;

            //    }
            //    _lastIkSolutionBranchNumber = min;
            //    for (int i = 0; i < SixRConstants.BufferLen * 2; i++)
            //    {
            //        quaternionOfRpy = BeckhoffContext.Traj.toQuaternion(output[3].q[i], output[4].q[i], output[5].q[i]);
            //        Ans = BeckhoffContext.Traj.Inversekinematic(new[]
            //        {
            //            quaternionOfRpy[0],quaternionOfRpy[1],quaternionOfRpy[2],quaternionOfRpy[3],
            //            0,output[0].q[i],output[1].q[i],output[2].q[i]
            //        }, toolParam);


            //        output[0].q[i] = Ans[_lastIkSolutionBranchNumber, 0];
            //        output[1].q[i] = Ans[_lastIkSolutionBranchNumber, 1];
            //        output[2].q[i] = Ans[_lastIkSolutionBranchNumber, 2];
            //        output[3].q[i] = Ans[_lastIkSolutionBranchNumber, 3];
            //        while (i > 0 && Math.Abs(output[3].q[i] - output[3].q[i - 1]) > (Math.PI - .1))
            //        {
            //            var sign = -Math.Sign(output[3].q[i] - output[3].q[i - 1]);
            //            output[3].q[i] += sign * 2 * Math.PI;
            //        }
            //        output[4].q[i] = Ans[_lastIkSolutionBranchNumber, 4];
            //        output[5].q[i] = Ans[_lastIkSolutionBranchNumber, 5];
            //        while (i > 0 && Math.Abs(output[5].q[i] - output[5].q[i - 1]) > (Math.PI - .1))
            //        {
            //            var sign = -Math.Sign(output[5].q[i] - output[5].q[i - 1]);
            //            output[5].q[i] += sign * 2 * Math.PI;
            //        }
            //        for (int j = 0; j < 6; j++)
            //        {
            //            if (Math.Abs(Ans[_lastIkSolutionBranchNumber, j] * (180 / Math.PI)) > BeckhoffContext.ValidJointSpace[j])
            //            {
            //                BeckhoffContext.Controller.SetTrajLength(BeckhoffContext.Controller.TrajList[0].TrajLength);
            //                throw new Exception("");

            //            }
            //        }
            //    }
            //    //output = BeckhoffContext.Traj.RotationCurrection(output);

            //    traj.Add(output);

            //    var points = new TrajectoryPointList[SixRConstants.NumberOfAxis];
            //    if (traj.Count == 0)
            //        return;
            //    for (int i = 0; i < SixRConstants.NumberOfAxis; i++)
            //        points[i] = new TrajectoryPointList();
            //    foreach (var val in traj)
            //    {
            //        for (var j = 0; j < val[0].TrajLength; j++)
            //        {
            //            for (var i = 0; i < SixRConstants.NumberOfAxis; i++)
            //            {

            //                points[i].AddPoint((val[i].q[j] * (180 / Math.PI)) / UnitConverter.PulsToDegFactor[i], val[i].v[j], val[i].a[j]);
            //            }
            //        }
            //    }
            //    BeckhoffContext.Controller.SetRegisterCtrWord(new ushort[] { 15, 15, 15, 15, 15, 15 });
            //    BeckhoffContext.Controller.SetCommand((int)CommandsEnum.SetControlWord);
            //    BeckhoffContext.Controller.SetSelectedMotors(new[] { true, true, true, true, true, true });
            //    BeckhoffContext.Controller.InitilizeTrajJog(points);
            //    BeckhoffContext.Controller.SetCommand((int)CommandsEnum.StartTrajectory);

            //}
            //else
            //{
            //    _cartesianJogSelectedMotors[2] = false;
            //    //_jogSelectedMotors[0] = false;
            //    //BeckhoffContext.Controller.JogDirection[0] = 1;
            //    //var tmp = true;
            //    //for (int j = 0; j < 6; j++)
            //    //    tmp = tmp & !_jogSelectedMotors[j];
            //    //if (tmp)
            //    //    BeckhoffContext.Controller.StopJog();
            //}
        }
        public void M2RightJog(object param)
        {
            uint[] mSpeeds = new uint[6];
            for (int i = 0; i < 6; i++)
                mSpeeds[i] = (uint)(JogSpeed / Math.Abs(UnitConverter.PulsToDegFactor[i]));
            BeckhoffContext.Controller.GUI_JogMaxSpeed = (mSpeeds);
            if (bool.Parse(param.ToString()))
            {
                BeckhoffContext.Controller.JogDirection[1] = 1;
                BeckhoffContext.Controller.SetMotorNumber(1);
                var mSpeed = (int)(-1 * 70.0m / UnitConverter.PulsToDegFactor[1]);
                BeckhoffContext.Controller.SetRegisterTargetVelocity(mSpeed, 2);
                _jogSelectedMotors[1] = true;
                JogMove();
            }
            else
            {
                _jogSelectedMotors[1] = false;
                BeckhoffContext.Controller.JogDirection[1] = 1;
                var tmp = true;
                for (int j = 0; j < 6; j++)
                    tmp = tmp & !_jogSelectedMotors[j];
                if (tmp)
                    BeckhoffContext.Controller.StopJog();
            }
        }
        public void M3RightJog(object param)
        {
            uint[] mSpeeds = new uint[6];
            for (int i = 0; i < 6; i++)
                mSpeeds[i] = (uint)(JogSpeed / Math.Abs(UnitConverter.PulsToDegFactor[i]));
            BeckhoffContext.Controller.GUI_JogMaxSpeed = (mSpeeds);
            if (bool.Parse(param.ToString()))
            {
                BeckhoffContext.Controller.JogDirection[2] = 1;
                BeckhoffContext.Controller.SetMotorNumber(2);
                var mSpeed = (int)(-1 * 70.0m / UnitConverter.PulsToDegFactor[2]);
                BeckhoffContext.Controller.SetRegisterTargetVelocity(mSpeed, 3);
                _jogSelectedMotors[2] = true;
                JogMove();
            }
            else
            {
                _jogSelectedMotors[2] = false;
                BeckhoffContext.Controller.JogDirection[2] = 1;
                var tmp = true;
                for (int j = 0; j < 6; j++)
                    tmp = tmp & !_jogSelectedMotors[j];
                if (tmp)
                    BeckhoffContext.Controller.StopJog();
            }
        }
        public void M4RightJog(object param)
        {
            uint[] mSpeeds = new uint[6];
            for (int i = 0; i < 6; i++)
                mSpeeds[i] = (uint)(JogSpeed / Math.Abs(UnitConverter.PulsToDegFactor[i]));
            BeckhoffContext.Controller.GUI_JogMaxSpeed = (mSpeeds);
            if (bool.Parse(param.ToString()))
            {
                BeckhoffContext.Controller.JogDirection[3] = 1;
                BeckhoffContext.Controller.SetMotorNumber(3);
                var mSpeed = (int)(-1 * 70.0m / UnitConverter.PulsToDegFactor[3]);
                BeckhoffContext.Controller.SetRegisterTargetVelocity(mSpeed, 4);
                _jogSelectedMotors[3] = true;
                JogMove();
            }
            else
            {
                _jogSelectedMotors[3] = false;
                BeckhoffContext.Controller.JogDirection[3] = 1;
                var tmp = true;
                for (int j = 0; j < 6; j++)
                    tmp = tmp & !_jogSelectedMotors[j];
                if (tmp)
                    BeckhoffContext.Controller.StopJog();
            }
        }
        public void M5RightJog(object param)
        {
            uint[] mSpeeds = new uint[6];
            for (int i = 0; i < 6; i++)
                mSpeeds[i] = (uint)(JogSpeed / Math.Abs(UnitConverter.PulsToDegFactor[i]));
            BeckhoffContext.Controller.GUI_JogMaxSpeed = (mSpeeds);
            if (bool.Parse(param.ToString()))
            {
                BeckhoffContext.Controller.JogDirection[4] = 1;
                BeckhoffContext.Controller.SetMotorNumber(4);
                var mSpeed = (int)(-1 * 70.0m / UnitConverter.PulsToDegFactor[4]);
                BeckhoffContext.Controller.SetRegisterTargetVelocity(mSpeed, 5);
                _jogSelectedMotors[4] = true;
                JogMove();
            }
            else
            {
                _jogSelectedMotors[4] = false;
                BeckhoffContext.Controller.JogDirection[4] = 1;
                var tmp = true;
                for (int j = 0; j < 6; j++)
                    tmp = tmp & !_jogSelectedMotors[j];
                if (tmp)
                    BeckhoffContext.Controller.StopJog();
            }
        }
        public void M6RightJog(object param)
        {
            uint[] mSpeeds = new uint[6];
            for (int i = 0; i < 6; i++)
                mSpeeds[i] = (uint)(JogSpeed / Math.Abs(UnitConverter.PulsToDegFactor[i]));
            BeckhoffContext.Controller.GUI_JogMaxSpeed = (mSpeeds);
            if (bool.Parse(param.ToString()))
            {
                BeckhoffContext.Controller.JogDirection[5] = 1;
                BeckhoffContext.Controller.SetMotorNumber(5);
                var mSpeed = (int)(-1 * 70.0m / UnitConverter.PulsToDegFactor[5]);
                BeckhoffContext.Controller.SetRegisterTargetVelocity(mSpeed, 6);
                _jogSelectedMotors[5] = true;
                JogMove();
            }
            else
            {
                _jogSelectedMotors[5] = false;
                BeckhoffContext.Controller.JogDirection[5] = 1;
                var tmp = true;
                for (int j = 0; j < 6; j++)
                    tmp = tmp & !_jogSelectedMotors[j];
                if (tmp)
                    BeckhoffContext.Controller.StopJog();
            }
        }

        public void CartesionJog(int buffer)
        {
            var toolParam = SixRConstants.toolParam;
            decimal[] theta =
                {
                    BeckhoffContext.Controller.MotorsEncoder[0]*UnitConverter.PulsToDegFactor[0],
                    BeckhoffContext.Controller.MotorsEncoder[1]*UnitConverter.PulsToDegFactor[1],
                    BeckhoffContext.Controller.MotorsEncoder[2]*UnitConverter.PulsToDegFactor[2],
                    BeckhoffContext.Controller.MotorsEncoder[3]*UnitConverter.PulsToDegFactor[3],
                    BeckhoffContext.Controller.MotorsEncoder[4]*UnitConverter.PulsToDegFactor[4],
                    BeckhoffContext.Controller.MotorsEncoder[5]*UnitConverter.PulsToDegFactor[5],
                };
            decimal[] cartesianPosition = null;
            if (_cartesianjogLastPosition == null)
            {
                cartesianPosition = BeckhoffContext.Traj.GetCartPos(theta.Select(a => a * Convert.ToDecimal(Math.PI / 180)).ToArray(),
                    toolParam);
                var rpy = BeckhoffContext.Traj.toEulerianAngle(cartesianPosition);
                _cartesianjogLastPosition = new[]
                {
                    cartesianPosition[5], cartesianPosition[6], cartesianPosition[7],
                    rpy[0], rpy[1], rpy[2]
                };
            }
            var Traj = new List<TrajectoryPointList<decimal>[]>();

            var output = new TrajectoryPointList<decimal>[SixRConstants.NumberOfAxis];
            var xyzrpy = new[]
            {
                            _cartesianjogLastPosition[0],
                            _cartesianjogLastPosition[1],
                            _cartesianjogLastPosition[2],
                            _cartesianjogLastPosition[3],
                            _cartesianjogLastPosition[4],
                            _cartesianjogLastPosition[5]
                        };
            var PositionActualValue = new decimal[SixRConstants.NumberOfAxis];
            for (int i = 0; i < SixRConstants.NumberOfAxis; i++)
                output[i] = new TrajectoryPointList<decimal>();
            for (int i = 0; i < SixRConstants.BufferLen; i++)
            {
                for (int j = 0; j < SixRConstants.NumberOfAxis; j++)
                {
                    _cartesiancurrentJogSpeed[j] += _cartesianJogSelectedMotors[j]
                        ? (.001m * JogAcceleration > 5 ? 5 : .001m * JogAcceleration)
                        : (.01m * JogAcceleration > 5 ? -5 : -.01m * JogAcceleration);
                    if (_cartesiancurrentJogSpeed[j] > 50)
                        _cartesiancurrentJogSpeed[j] = 50;
                    if (_cartesiancurrentJogSpeed[j] < 0)
                        _cartesiancurrentJogSpeed[j] = 0;
                    PositionActualValue[j] += (.001m * _cartesiancurrentJogSpeed[j]);
                    output[j].AddPoint(xyzrpy[j] + _cartesianJogDirection[j] * PositionActualValue[j], 0, 0);
                }
            }
            for (int j = 0; j < 6; j++)
                _cartesianjogLastPosition[j] = output[j].q[output[j].TrajLength - 1];
            var Ans = new decimal[8, 6];
            for (int i = 0; i < SixRConstants.BufferLen; i++)
            {
                var quaternionOfRpy = BeckhoffContext.Traj.toQuaternion(output[3].q[i], output[4].q[i], output[5].q[i]);
                Ans = BeckhoffContext.Traj.Inversekinematic(new[]
                {
                                quaternionOfRpy[0], quaternionOfRpy[1], quaternionOfRpy[2], quaternionOfRpy[3],
                                0, output[0].q[i], output[1].q[i], output[2].q[i]
                            }, toolParam);
                output[0].q[i] = Ans[_lastIkSolutionBranchNumber, 0];
                output[1].q[i] = Ans[_lastIkSolutionBranchNumber, 1];
                output[2].q[i] = Ans[_lastIkSolutionBranchNumber, 2];
                output[3].AddPoint(xyzrpy[3], 0, 0);
                while (i > 0 && Math.Abs((double)(output[3].q[i] - output[3].q[i - 1])) > (Math.PI))
                {
                    var sign = -Math.Sign(output[3].q[i] - output[3].q[i - 1]);
                    output[3].q[i] += sign * 2 * Convert.ToDecimal(Math.PI);
                }
                output[4].AddPoint(xyzrpy[4], 0, 0);
                output[5].AddPoint(xyzrpy[5], 0, 0);
                while (i > 0 && Math.Abs((double)(output[5].q[i] - output[5].q[i - 1])) > (Math.PI))
                {
                    var sign = -Math.Sign(output[5].q[i] - output[5].q[i - 1]);
                    output[5].q[i] += sign * 2 * Convert.ToDecimal(Math.PI);
                }
                for (int j = 0; j < 6; j++)
                {
                    if (Math.Abs((double)Ans[_lastIkSolutionBranchNumber, j] * (180 / Math.PI)) > (double)BeckhoffContext.ValidJointSpace[j])
                    {
                        BeckhoffContext.Controller.SetTrajLength(BeckhoffContext.Controller.TrajList[0].TrajLength);
                        return;

                    }
                }
            }
            Traj.Add(output);

            if (Traj.Count == 0)
                return;
            var len = BeckhoffContext.Controller.TrajList[0].TrajLength;
            foreach (var val in Traj)
            {
                for (var j = 0; j < val[0].TrajLength; j++)
                {
                    for (var i = 0; i < SixRConstants.NumberOfAxis; i++)
                    {
                        BeckhoffContext.Controller.TrajList[i].AddPoint((int)(((double)val[i].q[j] * (180 / Math.PI)) / (double)UnitConverter.PulsToDegFactor[i]), (int)val[i].v[j], (int)val[i].a[j]);
                    }
                }
            }
            BeckhoffContext.Controller.FillBuffer(buffer);
        }
        private bool IsMotorsMoving(int motorNumber=-1)
        {
            if (motorNumber > -1 && _cartesiancurrentJogSpeed[motorNumber] > 0)
                return true;
            for (int i = 0; i < SixRConstants.NumberOfAxis; i++)
                if (_cartesiancurrentJogSpeed[i] > 0)
                    return true;
            return false;
        }
        private void JogMove()
        {
            BeckhoffContext.Controller.MotorJogs = _jogSelectedMotors;
            BeckhoffContext.Controller.SetSelectedMotors(_jogSelectedMotors);
            BeckhoffContext.Controller.SetRegisterCtrWord(new ushort[] { 15, 15, 15, 15, 15, 15 });
            BeckhoffContext.Controller.SetCommand((int)CommandsEnum.SetControlWord);
            BeckhoffContext.Controller.SetCommand((int)CommandsEnum.JogTrajectory);
            BeckhoffContext.Controller.InitilizeJog();
        }


    }
}
