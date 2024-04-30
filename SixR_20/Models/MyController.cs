using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using SixR_20.Models;
using SixR_20.ViewModels.BottomRegion;
using static SixR_20.Models.SixRConstants;
namespace SixR_20.Models
{
    public class MyController : IDisposable, INotifyPropertyChanged
    {
        [Flags]
        public enum IndexModes
        {
            None = 0,
            M1 = 1,
            M2 = 2,
            M3 = 4,
            M4 = 8,
            M5 = 16,
            M6 = 32,
            All = 64
        }
        //private Matrix<double> currctionMatrix = DenseMatrix.OfArray(new double[,]
        //{
        //    { 0 , 0 , 0 , 0 , 0 , 0 },
        //    { 0 , 0 , 0 , 0 , 0 , 0 },
        //    { 0 , 0 , 0 , 0 , 0 , 0 },
        //    { 0 , 0 , 0 , 0 , 0 , 0 },
        //    { 0 , 0 , 0 , 1.0/50 , 0 , 0 },
        //    { 0 , 0 , 0 , -49.0/2500 , 1.0/50 , 0 }
        //});
        public TrajectoryPointList<int>[] TrajList;
        private bool isListTraj;
        private int _FillcurrentPoint;
        private int _ReadcurrentPoint;
        private PLCConnection _connection;
        public short[] JogDirection = new short[NumberOfAxis];
        //public int CurrentJogSpeed = 0;
        //public int MaxJogSpeed = int.MaxValue;
        //public int StopingJog = 1;
        //public int JogAcceleration = 1000000;
        //public int JogDeceleration = 1000000;
        private int _lineIndexer = 1;
        private bool _trajectoryPuls;

        private short _bufferNumber;
        private bool _pulse;
        private byte _GUI_Flags;
        private ushort[] _guiAlarms = new ushort[NumberOfAxis];
        //private bool[] _emerganceReacheds = new bool[6];
        //private bool[] _softwareLimitationsReached = new bool[6];
        //private bool[] _sensorLimitationsReached = new bool[6];

        private int[] _actualPositions = new int[NumberOfAxis];
        private int[] _motorsEncoder = new int[NumberOfAxis];

        //private double[] _minActualPosition = new double[6];
        //private double[] _maxActualPosition = new double[6];
        private bool[] _motorBreaks = new bool[NumberOfAxis];
        private bool[] _motorJogs = new bool[NumberOfAxis];
        //public int[] jogLastPosition = new int[6];
        //private int[,] BufferPos1 = new int[NumberOfAxis, BufferLen];
        //private int[,] BufferPos2 = new int[NumberOfAxis, BufferLen];
        //private int[,] BufferAccPos = new int[NumberOfAxis, BufferLen];
        //private int[,] BufferSpeed1 = new int[NumberOfAxis, BufferLen];
        //private int[,] BufferSpeed2 = new int[NumberOfAxis, BufferLen];
        private bool[] _digitalInputs = new bool[NumberOfInputs];
        private bool[] _digitalOutputs = new bool[NumberOfOutputs];
        public bool[] DigitalInputs => _digitalInputs;

        public bool[] DigitalOutputs
        {
            get { return _digitalOutputs; }
            set
            {
                _digitalOutputs = value;
                _connection[X.GUI_DigitalOutput] = _digitalOutputs;
            }
        }

        public int M1_ActualPosition { get { return _actualPositions[0]; } private set { _actualPositions[0] = value; OnPropertyChanged("M1_ActualPosition"); } }
        public int M2_ActualPosition { get { return _actualPositions[1]; } private set { _actualPositions[1] = value; OnPropertyChanged("M2_ActualPosition"); } }
        public int M3_ActualPosition { get { return _actualPositions[2]; } private set { _actualPositions[2] = value; OnPropertyChanged("M3_ActualPosition"); } }
        public int M4_ActualPosition { get { return _actualPositions[3]; } private set { _actualPositions[3] = value; OnPropertyChanged("M4_ActualPosition"); } }
        public int M5_ActualPosition { get { return _actualPositions[4]; } private set { _actualPositions[4] = value; OnPropertyChanged("M5_ActualPosition"); } }
        public int M6_ActualPosition { get { return _actualPositions[5]; } private set { _actualPositions[5] = value; OnPropertyChanged("M6_ActualPosition"); } }

        public int[] MotorsEncoder
        {
            get { return _motorsEncoder; }
            set { _motorsEncoder = value; OnPropertyChanged(nameof(MotorsEncoder)); }
        }

        public short BufferNumber { get { return _bufferNumber; } private set { _bufferNumber = value; OnPropertyChanged("BufferNumber"); } }
        public bool[] MotorBreaks { get { return _motorBreaks; } private set { _motorBreaks = value; OnPropertyChanged("MotorBreaks"); } }
        public ushort[] GUI_Alarms { get { return _guiAlarms; } private set { _guiAlarms = value; OnPropertyChanged("GUI_Alarms"); } }
        public bool[] MotorJogs { get { return _motorJogs; } set { _motorJogs = value; OnPropertyChanged("MotorJogs"); } }
        public bool Pulse { get { return _pulse; } private set { _pulse = value; OnPropertyChanged("Pulse"); } }
        public byte GUI_Flags { get { return _GUI_Flags; } private set { _GUI_Flags = value; OnPropertyChanged("GUI_Flags"); } }
        public int LineIndexer { get { return _lineIndexer; } private set { _lineIndexer = value; OnPropertyChanged("LineIndexer"); } }

        public bool TrajectoryPuls
        {
            get { return _trajectoryPuls; }
            set { _trajectoryPuls = value; OnPropertyChanged(nameof(TrajectoryPuls)); }
        }
        public int[][] TrajectoryErrorPosition
        {
            get; set;
        }

        private static int _actualTrajectoryPosValLength;
        private int _trajectoryBufferNumber;
        //public int GUI_JogAece
        //{
        //    get { return (int?)_connection[X.GUI_JogAcceleration] ?? 0; }
        //    set { _connection[X.GUI_JogAcceleration] = value; }
        //}
        public uint[] GUI_JogMaxSpeed
        {
            get { return (uint[])_connection[X.GUI_JogMaxSpeed] ?? new uint[6]; }
            set { _connection[X.GUI_JogMaxSpeed] = value; }
        }

        public short CurrentCommand = 100;

        public MyController(string beckhoffAddress, int port, int bufferSize = 10000)
        {
            _connection = new PLCConnection(beckhoffAddress, port, bufferSize);
            CommonInitialize();

        }

        private void CommonInitialize()
        {
            _connection.PropertyChanged += _connection_PropertyChanged;
            _connection.Strat();
            GUI_Flags = (byte)_connection[X.GUI_Flags];
            GUI_Alarms = (ushort[])_connection[X.GUI_ErrorCode];
        }

        //public void SetRegiterAll(ushort regAdr, uint regVal)
        //{
        //    _connection[X.GUI_RegisterAdress] = regAdr;
        //    _connection[X.GUI_RegisterValue] = regVal;
        //}
        public void SetRegisterTargetPosition(int position, int motorIndex)
        {
            var positions = (_connection[X.GUI_ActualPositions] as int[]);
            if (positions == null) return;
            // dont delete **********
            positions[motorIndex - 1] = position;
            _connection[X.GUI_TargetPosition] = positions;
        }

        public void SetModesOfOperation(byte mode)
        {
            //var modes = (_connection[X.GUI_ModesOfOperation] as byte[]);
            //if (modes == null) return;
            var modes = new byte[NumberOfAxis];
            for (var i = 0; i < NumberOfAxis; i++)
                modes[i] = mode;
            _connection[X.GUI_ModesOfOperation] = modes;
        }
        public void SetCommand(int cm)
        {
            _connection[X.GUI_Command] = (byte)cm;
            CurrentCommand = (byte)cm;
        }
        public void SetSelectedMotors(bool[] selInd)
        {
            _connection[X.GUI_MSelect] = selInd;
        }
        //public void SetMotorBreaks(bool[] selInd)
        //{
        //    _connection[X.GUI_MotorBreaks] = selInd;
        //}
        public void SetMotorNumber(ushort m)
        {
            _connection[X.GUI_MotorNum] = m;
        }

        //public ushort[] GetAlarmsNumbers()
        //{
        //    var alarms = (_connection[X.GUI_ErrorCode] as ushort[]);
        //    return alarms;
        //}
        public ushort[] GetStatusword()
        {
            var alarms = (_connection[X.GUI_Statusword] as ushort[]);
            return alarms;
        }

        public int GetActualPosition(int motorNumber)
        {
            return _actualPositions[motorNumber];
        }

        //public void ClearTargetVeloecities()
        //{
        //    int[] velos = { 0, 0, 0, 0, 0, 0 };
        //    _connection[X.GUI_TargetVelocity] = velos;
        //    SetCommand(3);
        //}
        //public void ClearTargetPositions()
        //{
        //    _connection[X.GUI_TargetPosition] = _actualPositions;
        //    SetCommand(2);
        //}
        public void SetProfileVelocities(int velo, int motorIndex)
        {
            var velos = (_connection[X.GUI_ProfileVelocity] as int[]);
            if (velos == null) return;
            velos[motorIndex - 1] = velo;
            _connection[X.GUI_ProfileVelocity] = velos;
        }

        public void SetRegisterCtrWord(ushort ctrWord, int motorIndex)
        {
            var ctrWords = (_connection[X.GUI_CtrWord] as ushort[]);
            if (ctrWords == null) return;
            ctrWords[motorIndex - 1] = ctrWord;
            _connection[X.GUI_CtrWord] = ctrWords;
        }
        public void SetRegisterCtrWord(ushort[] ctrWords)
        {
            _connection[X.GUI_CtrWord] = ctrWords;
        }
        //public void SetRegisterCtrWordAndPerform(ushort ctrWords)
        //{
        //    for (int i = 0; i < NumberOfAxis; i++)
        //    {
        //        _connection[X.GUI_CtrWord] = ctrWords;
        //    }
        //}

        public void SetRegisterTargetVelocity(int targetVelocity, int mIndx)
        {
            var tvelos = (_connection[X.GUI_TargetVelocity] as int[]); ;
            if (tvelos == null) return;
            tvelos[mIndx - 1] = targetVelocity;
            _connection[X.GUI_TargetVelocity] = tvelos;
        }

        public void SetTrajLength(int len)
        {
            _connection[X.TrajLen] = len;
        }

        //public void setLineNumbers(List<int> numbersList)
        //{
        //    _connection[X.ExecuteLineNumber] = numbersList.ToArray();
        //}

        //public void SetJogMaxSpeed(uint[] maxSpeed)
        //{
        //    _connection[X.GUI_JogMaxSpeed] = maxSpeed;
        //}
        //public void SetJogAece(int Aece)
        //{
        //    _connection[X.GUI_JogAcceleration] = Aece;
        //}
        //public void InitilizeTrajectory(TrajectoryPoints[] inpTraj)
        //{
        //    Traj = inpTraj;
        //    _connection[X.TrajLen] = Traj[0].TrajLength;
        //    _FillcurrentPoint = 0;
        //    int i, j;
        //    var pos = new int[NumberOfAxis, BufferLen];
        //    for (j = 0; j < BufferLen; j++, _FillcurrentPoint++)
        //    {
        //        if (_FillcurrentPoint == Traj[0].TrajLength)
        //            break;
        //        for (i = 0; i < NumberOfAxis; i++)
        //            pos[i, j] = (int)(Traj[i].q[_FillcurrentPoint]);
        //    }
        //    _connection[X.BufferPos1] = pos;

        //    for (j = 0; j < BufferLen; j++, _FillcurrentPoint++)
        //    {
        //        if (_FillcurrentPoint == Traj[0].TrajLength)
        //            break;
        //        for (i = 0; i < NumberOfAxis; i++)
        //        {
        //            pos[i, j] = (int)(Traj[i].q[_FillcurrentPoint]);
        //        }
        //    }
        //    _connection[X.BufferPos2] = pos;
        //}
        public void InitilizeTrajectory(TrajectoryPointList<int>[] inpTraj)
        {
            isListTraj = true;
            TrajList = inpTraj;
            if (TrajList != null && TrajList[0]!=null && TrajList[0].TrajLength > 0)
            {
                _connection[X.TrajLen] = TrajList[0].TrajLength;
                _FillcurrentPoint = 0;
                FillBuffer(2);
                FillBuffer(1);
                _trajectoryBufferNumber = 1;
                _actualTrajectoryPosValLength = 0;
                AngleChartViewModel.times = 0;
            }
        }
        public void InitilizeTrajJog(TrajectoryPointList<int>[] inpTraj)
        {
            isListTraj = true;
            TrajList = inpTraj;
            _connection[X.TrajLen] = int.MaxValue;
            _FillcurrentPoint = 0;
            FillBuffer(2);
            FillBuffer(1);
            _trajectoryBufferNumber = 1;
            _actualTrajectoryPosValLength = 0;
            AngleChartViewModel.times = 0;
        }
        public void InitilizeJog()
        {
            _connection[X.GUI_StopingJog] = (byte)1;
            _connection[X.GUI_JogDirection] = JogDirection;
        }

        public void StopJog()
        {
            //var counter = (uint) _connection[X.GUI_Counter] % BufferLen;
            //if (counter < BufferLen - 100)
            //{
            //    for (var i = 0; i < NumberOfAxis; i++)
            //        jogLastPosition[i] = BufferNumber == 1 ? BufferPos1[i, counter + 100] : BufferPos2[i, counter + 100];
            //    CurrentJogSpeed = BufferNumber == 1 ? BufferSpeed1[0, counter + 100] : BufferSpeed2[0, counter + 100];
            //    FillNextJogBuffer(BufferNumber, counter + 100);
            //}
            _connection[X.GUI_StopingJog] = (byte)0;
        }

        public void FillBuffer(int bufferNum)
        {
            var pos = new int[NumberOfAxis, BufferLen];
            var trjLen = TrajList[0].TrajLength;
            for (var j = 0; j < BufferLen; j++)
            {
                if (_FillcurrentPoint == trjLen)
                    break;
                for (var i = 0; i < NumberOfAxis; i++)
                {
                    pos[i, j] = (int)(TrajList[i].q[_FillcurrentPoint]);
                }
                _FillcurrentPoint++;
            }

            if (bufferNum == 2)
            {
                _connection[X.BufferPos1] = pos;
            }
            else
            {
                _connection[X.BufferPos2] = pos;
            }
        }
        private void WriteText(double[] inp)
        {
            StreamWriter log;
            FileStream fileStream = null;
            DirectoryInfo logDirInfo = null;
            FileInfo logFileInfo;
            string whatIsToBeLoged = "";

            string logFilePath = "C:\\Logs\\";
            logFilePath = logFilePath + "Log-Actual.txt";
            logFileInfo = new FileInfo(logFilePath);
            logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists) logDirInfo.Create();
            if (!logFileInfo.Exists)
            {
                fileStream = logFileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(logFilePath, FileMode.Append);
            }
            log = new StreamWriter(fileStream);
            for (int i = 0; i < inp.Length; i++)
                whatIsToBeLoged += inp[i] + ",";
            whatIsToBeLoged.Remove(whatIsToBeLoged.Length - 1, 1);
            whatIsToBeLoged += "\r\n";
            log.WriteLine(whatIsToBeLoged);
            log.Close();
        }

        //public void StopTrajectory()
        //{
        //    _connection[X.TrajLen] = 0;
        //    SetRegisterCtrWord(new ushort[] { 7, 7, 7, 7, 7, 7 });
        //    SetCommand((int)Cmd.SetControlWord);
        //}

        //public void ReadBuffer(int bufferNum)
        //{
        //    int[] TmpPos;
        //    if (bufferNum == 2)
        //    {
        //        TmpPos = (_connection[X.BufferPosAct1] as int[]);
        //    }
        //    else
        //    {
        //        TmpPos = (_connection[X.BufferPosAct2] as int[]);
        //    }

        //    for (var j = 0; j < BufferLen; j++)
        //    {
        //        if (MainWindow.ActualPosValLength == Traj[0].TrajLength)
        //            break;
        //        for (var i = 0; i < NumberOfAxis; i++)
        //        {
        //            MainWindow.ActualPosVal[i][MainWindow.ActualPosValLength] = TmpPos[i * BufferLen + j];
        //        }
        //        MainWindow.ActualPosValLength++;
        //    }
        //}

        //public void FillNextJogBuffer(int bufferNum, uint startPoint = 0)
        //{
        //    var pos = bufferNum == 2 ? (BufferPos2.Clone() as int[,]) : (BufferPos1.Clone() as int[,]);
        //    for (var j = startPoint; j < BufferLen; j++)
        //    {
        //        CurrentJogSpeed = (int)(.001 * JogAcceleration * StopingJog) + CurrentJogSpeed;
        //        if (StopingJog == -1 && CurrentJogSpeed <= 0)
        //        {
        //            uint tmp = Convert.ToUInt32(((uint)_connection[X.GUI_Counter] / 500) * 500 + j);
        //            _connection[X.TrajLen] = tmp;
        //            CurrentCommand = 100;
        //            break;
        //        }
        //        int tmpMovement = (int)(.001 * CurrentJogSpeed);
        //        for (var i = 0; i < NumberOfAxis; i++)
        //        {
        //            var lastPos = j == 0
        //                ? jogLastPosition[i]
        //                : pos[i, j - 1];
        //            pos[i, j] = MotorJogs[i]
        //                ? (jogLastPosition[i] = lastPos + JogDirection[i] * tmpMovement)
        //                : (jogLastPosition[i]);
        //            if (bufferNum == 1)
        //                BufferSpeed1[i, j] = CurrentJogSpeed;
        //            else
        //                BufferSpeed2[i, j] = CurrentJogSpeed;

        //        }
        //    }
        //    if ((bufferNum == 2 && startPoint == 0) || (bufferNum == 1 && startPoint != 0))
        //    {
        //        BufferPos1 = (pos.Clone() as int[,]);
        //        _connection[X.BufferPos1] = BufferPos1;
        //    }
        //    else
        //    {
        //        BufferPos2 = (pos.Clone() as int[,]);
        //        _connection[X.BufferPos2] = BufferPos2;
        //    }
        //}


        private void _connection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == X.GUI_ActualPositions.ToString())
            {
                var acs = (_connection[X.GUI_ActualPositions] as int[]);
                MotorsEncoder = acs;
                if (acs == null) return;
                if (M1_ActualPosition != acs[0] / 100)
                    M1_ActualPosition = acs[0] / 100;
                if (M2_ActualPosition != acs[1] / 100)
                    M2_ActualPosition = acs[1] / 100;
                if (M3_ActualPosition != acs[2] / 100)
                    M3_ActualPosition = acs[2] / 100;
                if (M4_ActualPosition != acs[3] / 100)
                    M4_ActualPosition = acs[3] / 100;
                if (M5_ActualPosition != acs[4] / 100)
                    M5_ActualPosition = acs[4] / 100;
                if (M6_ActualPosition != acs[5] / 100)
                    M6_ActualPosition = acs[5] / 100;
            }
            else if (e.PropertyName == X.BufferNumber.ToString())
            {
                BufferNumber = (short)_connection[X.BufferNumber];
            }
            else if (e.PropertyName == X.Pulse.ToString())
            {
                Pulse = (bool)_connection[X.Pulse];
                var tmp = new double[6];

                //for (int i = 0; i < _actualTrajectoryPosValLength; i++)
                //{
                //    for (int j = 0; j < 6; j++)
                //        tmp[j] = TrajectoryErrorPosition[j][i];
                //    WriteText(tmp);
                //}
            }
            else if (e.PropertyName == X.GUI_Flags.ToString())
            {
                GUI_Flags = (byte)_connection[X.GUI_Flags];
            }
            else if (e.PropertyName == X.GUI_ErrorCode.ToString())
            {
                GUI_Alarms = (ushort[])_connection[X.GUI_ErrorCode];
            }
            else if (e.PropertyName == X.GUI_MotorBreaks.ToString())
            {
                MotorBreaks = (bool[])_connection[X.GUI_MotorBreaks];
            }
            else if (e.PropertyName == X.LineNumberIndexer.ToString())
            {
                LineIndexer = (int)_connection[X.LineNumberIndexer];
            }
            else if (e.PropertyName == X.GUI_DigitalInput.ToString())
            {
                _digitalInputs = (_connection[X.GUI_DigitalInput] as bool[]);
            }
            else if(e.PropertyName==X.GUI_TrajectoryPuls.ToString())
            {
                //if(TrajList==null) return;
                //TrajectoryErrorPosition=new int[SixRConstants.NumberOfAxis][];
                //for(int ii=0;ii<SixRConstants.NumberOfAxis;ii++)
                //    TrajectoryErrorPosition[ii]=new int[TrajList[0].TrajLength];
                //int[] TmpPos;
                //if (_trajectoryBufferNumber == 1)
                //{
                //    TmpPos = (_connection[X.GUI_ErrorPosition1] as int[]);
                //    _trajectoryBufferNumber = 2;
                //}
                //else
                //{
                //    TmpPos = (_connection[X.GUI_ErrorPosition2] as int[]);
                //    _trajectoryBufferNumber = 1;
                //}

                //for (var j = 0; j < BufferLen; j++)
                //{
                //    if (_actualTrajectoryPosValLength == TrajList[0].TrajLength)
                //        break;
                //    for (var i = 0; i < NumberOfAxis; i++)
                //    {
                //        TrajectoryErrorPosition[i][_actualTrajectoryPosValLength] = TmpPos[i * BufferLen + j];
                //    }
                //    _actualTrajectoryPosValLength++;
                //}
                TrajectoryPuls = !TrajectoryPuls;
            }
        }

        public void NotifyAlarms()
        {
            GUI_Alarms = (ushort[])_connection[X.GUI_ErrorCode];
            var acs = (_connection[X.GUI_ActualPositions] as int[]);
            MotorsEncoder = acs;
            if (acs == null) return;
            if (M1_ActualPosition != acs[0] / 100)
                M1_ActualPosition = acs[0] / 100;
            if (M2_ActualPosition != acs[1] / 100)
                M2_ActualPosition = acs[1] / 100;
            if (M3_ActualPosition != acs[2] / 100)
                M3_ActualPosition = acs[2] / 100;
            if (M4_ActualPosition != acs[3] / 100)
                M4_ActualPosition = acs[3] / 100;
            if (M5_ActualPosition != acs[4] / 100)
                M5_ActualPosition = acs[4] / 100;
            if (M6_ActualPosition != acs[5] / 100)
                M6_ActualPosition = acs[5] / 100;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_connection == null) return;
            _connection.PropertyChanged -= _connection_PropertyChanged;
            _connection.Dispose();
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
