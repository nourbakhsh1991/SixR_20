using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;

namespace SixR_20.Models
{
    public class PLCConnection : INotifyPropertyChanged, IDisposable
    {
        private class Parameter
        {
            private static int _lastIndex = 0;
            public Parameter()
            {
                this.Index = _lastIndex++;
            }
            public int Index { get; set; }
            public X ParameterName { get; set; }
            public int NotifyID { get; set; }
            public int StreamPosition { get; set; }
        }

        private TcAdsClient _tcClient;
        private AdsStream _dataStream;
        private BinaryReader _binReader;
        private int LastParameterIndex = 0;
        ObservableCollection<KeyValuePair<X, Parameter>> NotifyParameters = new ObservableCollection<KeyValuePair<X, Parameter>>();
        //private Dictionary<X, Parameter> NotificationParameters = new Dictionary<X, Parameter>();
        private Dictionary<X, object> ParametersValue = new Dictionary<X, object>();

        public PLCConnection(string beckhoffAddress, int port, int bufferSize = 1000)
        {
            _dataStream = new AdsStream(bufferSize);
            _binReader = new BinaryReader(_dataStream);
            _tcClient = new TcAdsClient();
            //_tcClient.Connect(beckhoffAddress, port);

            foreach (var item in Enum.GetNames(typeof(X)))
            {
                var en = item.CastToEnum<X>().Value;
                RegisterParameter(en);
            }
        }

        public void Strat()
        {
            _tcClient.AdsNotification += tcClient_AdsNotification;
        }

        private void RegisterParameter(X ParameterName, int CycleTime = 1, int MaxDelay = 0, bool CheckNotify = true)
        {

            var byteLen = ParameterName.GetParameterSize();
            var att = ParameterName.GetAttribute<TypeAttribute>();
            if (NotifyParameters.Any(a => a.Key == ParameterName))
                throw new Exception("This parameter already registered");


            var pName = $"{att.SourceFunction}.{ParameterName}";

            if (CheckNotify && !att.Notify) return;
            var readId = _tcClient.AddDeviceNotification(
                pName, _dataStream, LastParameterIndex, byteLen,
                AdsTransMode.OnChange, CycleTime, MaxDelay, null);

            NotifyParameters.Add(new KeyValuePair<X, Parameter>(ParameterName, new Parameter()
            {
                ParameterName = ParameterName,
                NotifyID = readId,
                StreamPosition = LastParameterIndex
            }));
            LastParameterIndex += byteLen;
            ParametersValue.Add(ParameterName, null);
        }
        public void UnregisterParameter(X parameterName)
        {
            if (NotifyParameters.Any(a => a.Key != parameterName)) return;
            _tcClient.DeleteDeviceNotification((NotifyParameters.First(a => a.Key == parameterName).Value.NotifyID));
            NotifyParameters.Remove(NotifyParameters.First(a => a.Key == parameterName));
            ParametersValue.Remove(parameterName);
        }
        public void WriteValue(X parameterName, object value, bool selfVerify = false)
        {
            var att = parameterName.GetAttribute<TypeAttribute>();
            if (att.RWStatus == TypeAttribute.RW.ReadOnly)
                return;

            var pName = $"{att.SourceFunction}.{parameterName}";

            var writeId = _tcClient.CreateVariableHandle(pName);
            _tcClient.WriteAny(writeId, value);
            _tcClient.DeleteVariableHandle(writeId);

            if (selfVerify)
                OnChange(parameterName);
        }
        public object ReadValue(X parameterName)
        {
            object result;
            var att = parameterName.GetAttribute<TypeAttribute>();
            if (att.RWStatus == TypeAttribute.RW.WriteOnly)
                return null;

            var pName = $"{att.SourceFunction}.{parameterName}";

            var readId = _tcClient.CreateVariableHandle(pName);
            Type type;
            if (att.Length == 1)
            {
                switch (att.Type)
                {
                    case TypeAttribute.Types.Bool:
                        type = typeof(bool);
                        break;
                    case TypeAttribute.Types.SInt:
                        type = typeof(byte);
                        break;
                    case TypeAttribute.Types.Int:
                        type = typeof(short);
                        break;
                    case TypeAttribute.Types.Word:
                        type = typeof(ushort);
                        break;
                    case TypeAttribute.Types.DInt:
                        type = typeof(int);
                        break;
                    case TypeAttribute.Types.UDInt:
                        type = typeof(uint);
                        break;
                    case TypeAttribute.Types.Real:
                        type = typeof(double);
                        break;
                    case TypeAttribute.Types.LReal:
                        type = typeof(double);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                result = _tcClient.ReadAny(readId, type);
            }
            else
            {
                switch (att.Type)
                {
                    case TypeAttribute.Types.Bool:
                        type = typeof(bool[]);
                        break;
                    case TypeAttribute.Types.SInt:
                        type = typeof(byte[]);
                        break;
                    case TypeAttribute.Types.Int:
                        type = typeof(short[]);
                        break;
                    case TypeAttribute.Types.Word:
                        type = typeof(ushort[]);
                        break;
                    case TypeAttribute.Types.DInt:
                        type = typeof(int[]);
                        break;
                    case TypeAttribute.Types.UDInt:
                        type = typeof(uint[]);
                        break;
                    case TypeAttribute.Types.Real:
                        type = typeof(double[]);
                        break;
                    case TypeAttribute.Types.LReal:
                        type = typeof(double[]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                result = _tcClient.ReadAny(readId, type, new[] { att.Length });
            }
            _tcClient.DeleteVariableHandle(readId);
            return result;
        }

        public object this[X parameterName]
        {
            get
            {
                var att = parameterName.GetAttribute<TypeAttribute>();
                if (att.RWStatus == TypeAttribute.RW.WriteOnly)
                    return null;
                if (att.Notify)
                {
                    if (NotifyParameters.All(a => a.Key != parameterName))
                        return null;
                    if (ParametersValue[parameterName] == null)
                        ParametersValue[parameterName] = ReadValue(parameterName);

                    return ParametersValue[parameterName];
                }
                return ReadValue(parameterName);
            }
            set
            {
                var att = parameterName.GetAttribute<TypeAttribute>();
                if (att.RWStatus == TypeAttribute.RW.ReadOnly)
                    return;

                WriteValue(parameterName, value);
            }
        }

        private void tcClient_AdsNotification(object sender, AdsNotificationEventArgs e)
        {
            foreach (var item in NotifyParameters)
            {
                if (item.Value.NotifyID != e.NotificationHandle) continue;
                var att = item.Value.ParameterName.GetAttribute<TypeAttribute>();
                if (att.Length == 1)
                    switch (att.Type)
                    {
                        case TypeAttribute.Types.Bool:
                            ParametersValue[item.Key] = _binReader.ReadBoolean();
                            break;
                        case TypeAttribute.Types.SInt:
                            ParametersValue[item.Key] = _binReader.ReadByte();
                            break;
                        case TypeAttribute.Types.Int:
                            ParametersValue[item.Key] = _binReader.ReadInt16();
                            break;
                        case TypeAttribute.Types.Word:
                            ParametersValue[item.Key] = _binReader.ReadUInt16();
                            break;
                        case TypeAttribute.Types.DInt:
                            ParametersValue[item.Key] = _binReader.ReadInt32();
                            break;
                        case TypeAttribute.Types.UDInt:
                            ParametersValue[item.Key] = _binReader.ReadUInt32();
                            break;
                        case TypeAttribute.Types.Real:
                            ParametersValue[item.Key] = _binReader.ReadSingle();
                            break;
                        case TypeAttribute.Types.LReal:
                            ParametersValue[item.Key] = _binReader.ReadDouble();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                else
                {
                    _dataStream.Position = item.Value.StreamPosition;
                    switch (att.Type)
                    {
                        case TypeAttribute.Types.Bool:
                            var ret = new bool[att.Length];
                            for (int i = 0; i < att.Length; i++)
                                ret[i] = _binReader.ReadBoolean();
                            ParametersValue[item.Key] = ret;
                            break;
                        case TypeAttribute.Types.Int:
                            var ret1 = new short[att.Length];
                            for (int i = 0; i < att.Length; i++)
                                ret1[i] = _binReader.ReadInt16();
                            ParametersValue[item.Key] = ret1;
                            break;
                        case TypeAttribute.Types.Word:
                            var ret2 = new ushort[att.Length];
                            for (int i = 0; i < att.Length; i++)
                                ret2[i] = _binReader.ReadUInt16();
                            ParametersValue[item.Key] = ret2;
                            break;
                        case TypeAttribute.Types.DInt:
                            var ret3 = new int[att.Length];
                            for (int i = 0; i < att.Length; i++)
                                ret3[i] = _binReader.ReadInt32();
                            ParametersValue[item.Key] = ret3;
                            break;
                        case TypeAttribute.Types.UDInt:
                            var ret4 = new uint[att.Length];
                            for (int i = 0; i < att.Length; i++)
                                ret4[i] = _binReader.ReadUInt32();
                            ParametersValue[item.Key] = ret4;
                            break;
                        case TypeAttribute.Types.Real:
                            var ret5 = new double[att.Length];
                            for (int i = 0; i < att.Length; i++)
                                ret5[i] = _binReader.ReadSingle();
                            ParametersValue[item.Key] = ret5;
                            break;
                        case TypeAttribute.Types.LReal:
                            var ret6 = new double[att.Length];
                            for (int i = 0; i < att.Length; i++)
                                ret6[i] = _binReader.ReadDouble();
                            ParametersValue[item.Key] = ret6;
                            break;
                        case TypeAttribute.Types.SInt:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                OnChange(item.Key);
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnChange(X propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName.ToString()));
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            foreach (var item in NotifyParameters)
                try
                {
                    _tcClient.DeleteDeviceNotification(item.Value.NotifyID);
                }
                catch (Exception)
                {
                    // ignored
                }
            try
            {
                _tcClient.AdsNotification -= tcClient_AdsNotification;
                _tcClient.Dispose();
                _dataStream.Close();
                _dataStream.Dispose();
                _binReader.Close();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        #endregion
    }
}
