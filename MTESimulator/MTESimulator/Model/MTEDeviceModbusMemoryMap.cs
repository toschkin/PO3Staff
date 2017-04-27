using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Modbus.Data;

namespace MTESimulator.Model
{
    [Serializable]
    public class MTEDeviceModbusMemoryMap
    {
        #region Members
        [NonSerialized]
        private DataStore _dataStore;

        #region Values storage for serialization

        private double _storedFrequency;
        private byte _storedOutputModule1Type;
        private byte _storedOutputModule2Type;
        private byte _storedOutputModule3Type;
        private byte _storedMeasurementCircuitType;
        private byte _storedCurrentInterfaceType;
        private byte _storedSerie;
        private ushort _storedSerialNumber;
        private sbyte _storedTemperature;
        private ushort _storedPhi_ac;
        private ushort _storedPhi_ab;
        private double _storedCos_a;
        private double _storedCos_b;
        private double _storedCos_c;
        private double _storedCos_sys;
        private double _storedIa;
        private double _storedIc;
        private double _storedIb;
        private double _storedIn;        
        private double _storedPa;
        private double _storedPb;
        private double _storedPc;
        private double _storedQa;
        private double _storedQb;
        private double _storedQc;
        private double _storedSa;
        private double _storedSb;
        private double _storedSc;
        private double _storedPsys;
        private double _storedQsys;
        private double _storedSsys;
        private double _storedUa;
        private double _storedUb;
        private double _storedUc;
        private byte _storedSecondaryCurrent;
        private uint _storedPrimaryCurrent;
        private uint _storedPrimaryVoltage;
        private byte _storedSecondaryVoltage;

        #endregion

        #endregion
        public MTEDeviceModbusMemoryMap(DataStore dataStore)
        {
            _dataStore = dataStore;            
            MeasurementCircuitType = 3;
            _storedFrequency = 50.0;
            _storedPrimaryCurrent = 2000;
            _storedSecondaryCurrent = 5;            
            _storedPrimaryVoltage = 10000;
            _storedSecondaryVoltage = 100;
            _storedUa = 10000.0;
            _storedUb = 10000.0;
            _storedUc = 10000.0;
            _storedIa = 1000.0;
            _storedIb = 1000.0;
            _storedIc = 1000.0;
            _storedPa = 100000.0;
            _storedPb = 100000.0;
            _storedPc = 100000.0;
            _storedQa = 100000.0;
            _storedQb = 100000.0;
            _storedQc = 100000.0;
            _storedSa = 100000.0;
            _storedSb = 100000.0;
            _storedSc = 100000.0;
            _storedPsys = 200000.0;
            _storedQsys = 200000.0;
            _storedSsys = 200000.0;
            _storedCos_a = 0.7;
            _storedCos_b = 0.7;
            _storedCos_c = 0.7;
            _storedCos_sys = 1.0;        
            UpdateAllPropertiesFromStoredValues();
        }
        

        #region Measurement circuit

        public ushort Coef_CT
        {
            get { return (ushort)(_storedPrimaryCurrent / _storedSecondaryCurrent); }
            set
            {                                             
            }
        }
        public ushort Coef_VT
        {
            get { return (ushort)(_storedPrimaryVoltage / _storedSecondaryVoltage); }
            set
            {                                
            }
        }

        public double PowerQuantWeight
        {
            get
            {
                double k = ((Coef_CT * Coef_VT) / 10000000.0);
                return ((Coef_CT * Coef_VT) / 10000000.0);
            }
        }

        public uint PrimaryCurrent
        {
            get
            {
                return _storedPrimaryCurrent;
            }
            set
            {
                _storedPrimaryCurrent = value;
                UpdatePropertiesDependentOnCoef_CT();
            }
        }

        public byte SecondaryCurrent
        {
            get
            {
                return _storedSecondaryCurrent;
            }
            set
            {
                _storedSecondaryCurrent = value;
                UpdatePropertiesDependentOnCoef_CT();
            }
        }

        public uint PrimaryVoltage
        {
            get
            {
                return _storedPrimaryVoltage;
            }
            set
            {
                _storedPrimaryVoltage = value;
                UpdatePropertiesDependentOnCoef_VT();
            }
        }

        public byte SecondaryVoltage
        {
            get
            {
                return _storedSecondaryVoltage;
            }
            set
            {
                _storedSecondaryVoltage = value;
                UpdatePropertiesDependentOnCoef_VT();
            }
        }
        #endregion

        #region STATUS register               

        public byte OutputModule1Type
        {
            get { return (byte)(_dataStore.InputRegisters[1001] & 0x0003); }
            set
            {
                _storedOutputModule1Type = value;
                unchecked
                {
                    _dataStore.InputRegisters[1001] &= 0xFFFC;
                    _dataStore.InputRegisters[1001] |= value;
                }
            }
        }
        public byte OutputModule2Type
        {
            get { return (byte)((_dataStore.InputRegisters[1001] & 0x000C) >> 2); }
            set
            {
                _storedOutputModule2Type = value;
                unchecked
                {
                    _dataStore.InputRegisters[1001] &= 0xFFF3;
                    _dataStore.InputRegisters[1001] |= (ushort)((value << 2));
                }
            }
        }
        public byte OutputModule3Type
        {
            get { return (byte)((_dataStore.InputRegisters[1001] & 0x0030) >> 4); }
            set
            {
                _storedOutputModule3Type = value;
                unchecked
                {
                    _dataStore.InputRegisters[1001] &= 0xFFCF;
                    _dataStore.InputRegisters[1001] |= (ushort)((value << 4));
                }
            }
        }
        public byte MeasurementCircuitType
        {
            get { return (byte)((_dataStore.InputRegisters[1001] & 0x00C0) >> 6); }
            set
            {
                _storedMeasurementCircuitType = value;
                unchecked
                {
                    _dataStore.InputRegisters[1001] &= 0xFF3F;
                    _dataStore.InputRegisters[1001] |= (ushort)((value << 6));

                }
            }
        }
        public byte CurrentInterfaceType
        {
            get { return (byte)((_dataStore.InputRegisters[1001] & 0x0F00) >> 8); }
            set
            {
                _storedCurrentInterfaceType = value;
                unchecked
                {
                    _dataStore.InputRegisters[1001] &= 0xF0FF;
                    _dataStore.InputRegisters[1001] |= (ushort)((value << 8));
                }
            }
        }
        public byte Serie
        {
            get { return (byte)((_dataStore.InputRegisters[1001] & 0xF000) >> 12); }
            set
            {
                _storedSerie = value;
                unchecked
                {
                    _dataStore.InputRegisters[1001] &= 0x0FFF;
                    _dataStore.InputRegisters[1001] |= (ushort)((value << 12));
                }
            }
        }
        public ushort SerialNumber
        {
            get { return _dataStore.InputRegisters[1002]; }
            set
            {
                _storedSerialNumber = value;
                _dataStore.InputRegisters[1002] = value;
            }
        }
        #endregion

        #region INFO register
        public sbyte Temperature
        {
            get
            {
                unchecked
                {
                    return (sbyte)BitConverter.GetBytes(_dataStore.InputRegisters[1004])[1];
                }
            }
            set
            {
                _storedTemperature = value;
                unchecked
                {
                    _dataStore.InputRegisters[1004] = (ushort)((byte)value << 8);
                }
            }
        }
        #endregion

        #region VRMS_1

        public double Ua
        {
            get
            {
                return (_dataStore.InputRegisters[1005] * Coef_VT) / 100.0;
                //return Math.Round((double)(_dataStore.InputRegisters[1005] * Coef_VT) / 100.0, MidpointRounding.AwayFromZero);
            }
            set
            {
                _storedUa = value;                
                _dataStore.InputRegisters[1005] = 0;
                if (Coef_VT > 0)
                    _dataStore.InputRegisters[1005] = (ushort) (Math.Round((value* 100.0) / Coef_VT, MidpointRounding.AwayFromZero));                
            }           
        }

        public ushort Ua_quants => _dataStore.InputRegisters[1005];

        public double Ub
        {
            get
            {
                return (_dataStore.InputRegisters[1006] * Coef_VT) / 100.0;
                //return Math.Round((double)(_dataStore.InputRegisters[1006] * Coef_VT) / 100.0, MidpointRounding.AwayFromZero);
            }
            set
            {
                _storedUb = value;
                _dataStore.InputRegisters[1006] = 0;
                if (Coef_VT > 0)
                    _dataStore.InputRegisters[1006] = (ushort)(Math.Round((value * 100.0) / Coef_VT, MidpointRounding.AwayFromZero));
            }
        }
        public ushort Ub_quants => _dataStore.InputRegisters[1006];
        #endregion

        #region VRMS_2

        public double Uc
        {
            get
            {
                return (_dataStore.InputRegisters[1007] * Coef_VT) / 100.0;
                //return Math.Round((double)(_dataStore.InputRegisters[1007] * Coef_VT) / 100.0, MidpointRounding.AwayFromZero);
            }
            set
            {
                _storedUc = value;
                _dataStore.InputRegisters[1007] = 0;
                if (Coef_VT > 0)
                    _dataStore.InputRegisters[1007] = (ushort)(Math.Round((value * 100.0) / Coef_VT, MidpointRounding.AwayFromZero));
            }
        }
        public ushort Uc_quants => _dataStore.InputRegisters[1007];
        public double Frequency
        {
            get
            {                
                return ((double)_dataStore.InputRegisters[1008])/100;
            }
            set
            {
                _storedFrequency = value;
                _dataStore.InputRegisters[1008] = (ushort)(Math.Round(value * 100.0, MidpointRounding.AwayFromZero));
            }
        }
        public ushort Frequency_quants => _dataStore.InputRegisters[1008];
        #endregion

        #region IRMS_1

        public double Ia
        {
            get
            {                
                return (_dataStore.InputRegisters[1009] * Coef_CT) / 1000.0;
                //Math.Round((double)(_dataStore.InputRegisters[1009] * Coef_CT) / 1000, MidpointRounding.AwayFromZero);
            }
            set
            {
                _storedIa = value;
                _dataStore.InputRegisters[1009] = 0;               
                if (Coef_CT > 0)          
                    _dataStore.InputRegisters[1009] = (ushort)(Math.Round((value * 1000.0) / Coef_CT, MidpointRounding.AwayFromZero)); 
                
            }
        }
        public ushort Ia_quants => _dataStore.InputRegisters[1009];
        public double Ib
        {
            get
            {
                return (_dataStore.InputRegisters[1010] * Coef_CT) / 1000.0;
                //return Math.Round((_dataStore.InputRegisters[1010] * Coef_CT) / 1000.0, MidpointRounding.AwayFromZero);
            }
            set
            {
                _storedIb = value;
                _dataStore.InputRegisters[1010] = 0;
                if (Coef_CT > 0)
                    _dataStore.InputRegisters[1010] = (ushort)(Math.Round((value * 1000.0) / Coef_CT, MidpointRounding.AwayFromZero));
            }
        }
        public ushort Ib_quants => _dataStore.InputRegisters[1010];
        #endregion

        #region IRMS_2

        public double Ic
        {
            get
            {
                return (_dataStore.InputRegisters[1011] * Coef_CT) / 1000.0;
                //return Math.Round((double)(_dataStore.InputRegisters[1011] * Coef_CT) / 1000.0, MidpointRounding.AwayFromZero);
            }
            set
            {
                _storedIc = value;
                _dataStore.InputRegisters[1011] = 0;
                if (Coef_CT > 0)
                    _dataStore.InputRegisters[1011] = (ushort)(Math.Round((value * 1000.0) / Coef_CT, MidpointRounding.AwayFromZero));
            }
        }
        public ushort Ic_quants => _dataStore.InputRegisters[1011];
        public double In
        {
            get
            {
                //return Math.Round((double)(_dataStore.InputRegisters[1012] * Coef_CT) / 1000.0, MidpointRounding.AwayFromZero);
                return (_dataStore.InputRegisters[1012] * Coef_CT) / 1000.0;
            }
            set
            {
                _storedIn = value;
                _dataStore.InputRegisters[1012] = 0;
                if (Coef_CT > 0)
                    _dataStore.InputRegisters[1012] = (ushort)(Math.Round((value * 1000.0) / Coef_CT, MidpointRounding.AwayFromZero));
            }
        }
        public ushort In_quants => _dataStore.InputRegisters[1012];
        #endregion

        #region P
        public double Pa
        {
            get
            {
                //int quants = ((_dataStore.InputRegisters[HiByteFirst ? 1013 : 1014] << 16) | _dataStore.InputRegisters[HiByteFirst ? 1014 : 1013]);
                int quants = ((_dataStore.InputRegisters[1014] << 16) | _dataStore.InputRegisters[1013]);
                return  ((quants / 10000000.0 ) * Coef_CT * Coef_VT);                
            }
            set
            {
                _storedPa = value;
                unchecked
                {
                    double PowerQuantWeight = ((Coef_CT * Coef_VT)/10000000.0);                    
                    int quants = (PowerQuantWeight == 0.0) ? 0 : (int)(Math.Round(value / PowerQuantWeight, MidpointRounding.AwayFromZero));
                    //_dataStore.InputRegisters[HiByteFirst ? 1013 : 1014] = (ushort)(quants >> 16);
                    //_dataStore.InputRegisters[HiByteFirst ? 1014 : 1013] = (ushort)(quants & 0xFFFF);
                    _dataStore.InputRegisters[1014] = (ushort)(quants >> 16);
                    _dataStore.InputRegisters[1013] = (ushort)(quants & 0xFFFF);
                }
            }
        }
        public uint Pa_quants => (uint)((_dataStore.InputRegisters[1014] << 16) | _dataStore.InputRegisters[1013]);
        public double Pb
        {
            get
            {
                //int quants = ((_dataStore.InputRegisters[HiByteFirst ? 1015 : 1016] << 16) | _dataStore.InputRegisters[HiByteFirst ? 1016 : 1015]);
                int quants = ((_dataStore.InputRegisters[1016] << 16) | _dataStore.InputRegisters[1015]);
                return ((quants / 10000000.0) * Coef_CT * Coef_VT);
            }
            set
            {
                _storedPb = value;
                unchecked
                {
                    
                    int quants = (PowerQuantWeight == 0.0) ? 0 : (int)(Math.Round(value / PowerQuantWeight, MidpointRounding.AwayFromZero));
                    //_dataStore.InputRegisters[HiByteFirst ? 1015 : 1016] = (ushort)(quants >> 16);
                    //_dataStore.InputRegisters[HiByteFirst ? 1016 : 1015] = (ushort)(quants & 0xFFFF);
                    _dataStore.InputRegisters[1016] = (ushort)(quants >> 16);
                    _dataStore.InputRegisters[1015] = (ushort)(quants & 0xFFFF);
                }
            }
        }
        public uint Pb_quants => (uint)((_dataStore.InputRegisters[1016] << 16) | _dataStore.InputRegisters[1015]);
        public double Pc
        {
            get
            {
                //int quants = ((_dataStore.InputRegisters[HiByteFirst ? 1017 : 1018] << 16) | _dataStore.InputRegisters[HiByteFirst ? 1018 : 1017]);
                int quants = ((_dataStore.InputRegisters[1018] << 16) | _dataStore.InputRegisters[1017]);
                return ((quants / 10000000.0) * Coef_CT * Coef_VT);
            }
            set
            {
                _storedPc = value;
                unchecked
                {
                    
                    int quants = (PowerQuantWeight == 0.0) ? 0 : (int)(Math.Round(value / PowerQuantWeight, MidpointRounding.AwayFromZero));
                    //_dataStore.InputRegisters[HiByteFirst ? 1017 : 1018] = (ushort)(quants >> 16);
                    //_dataStore.InputRegisters[HiByteFirst ? 1018 : 1017] = (ushort)(quants & 0xFFFF);
                    _dataStore.InputRegisters[1018] = (ushort)(quants >> 16);
                    _dataStore.InputRegisters[1017] = (ushort)(quants & 0xFFFF);
                }
            }
        }
        public uint Pc_quants => (uint)((_dataStore.InputRegisters[1018] << 16) | _dataStore.InputRegisters[1017]);
        #endregion

        #region Q

        public double Qa
        {
            get
            {
                //int quants = ((_dataStore.InputRegisters[HiByteFirst ? 1017 : 1018] << 16) | _dataStore.InputRegisters[HiByteFirst ? 1018 : 1017]);
                int quants = ((_dataStore.InputRegisters[1020] << 16) | _dataStore.InputRegisters[1019]);
                return ((quants / 10000000.0) * Coef_CT * Coef_VT);
            }
            set
            {
                _storedQa = value;
                unchecked
                {
                    
                    int quants = (PowerQuantWeight == 0.0) ? 0 : (int)(Math.Round(value / PowerQuantWeight, MidpointRounding.AwayFromZero));
                    //_dataStore.InputRegisters[HiByteFirst ? 1017 : 1018] = (ushort)(quants >> 16);
                    //_dataStore.InputRegisters[HiByteFirst ? 1018 : 1017] = (ushort)(quants & 0xFFFF);
                    _dataStore.InputRegisters[1020] = (ushort)(quants >> 16);
                    _dataStore.InputRegisters[1019] = (ushort)(quants & 0xFFFF);
                }
            }
        }
        public uint Qa_quants => (uint)((_dataStore.InputRegisters[1020] << 16) | _dataStore.InputRegisters[1019]);
        public double Qb
        {
            get
            {
                //int quants = ((_dataStore.InputRegisters[HiByteFirst ? 1017 : 1018] << 16) | _dataStore.InputRegisters[HiByteFirst ? 1018 : 1017]);
                int quants = ((_dataStore.InputRegisters[1022] << 16) | _dataStore.InputRegisters[1021]);
                return ((quants / 10000000.0) * Coef_CT * Coef_VT);
            }
            set
            {
                _storedQb = value;
                unchecked
                {
                    
                    int quants = (PowerQuantWeight == 0.0) ? 0 : (int)(Math.Round(value / PowerQuantWeight, MidpointRounding.AwayFromZero));
                    //_dataStore.InputRegisters[HiByteFirst ? 1017 : 1018] = (ushort)(quants >> 16);
                    //_dataStore.InputRegisters[HiByteFirst ? 1018 : 1017] = (ushort)(quants & 0xFFFF);
                    _dataStore.InputRegisters[1022] = (ushort)(quants >> 16);
                    _dataStore.InputRegisters[1021] = (ushort)(quants & 0xFFFF);
                }
            }
        }
        public uint Qb_quants => (uint)((_dataStore.InputRegisters[1022] << 16) | _dataStore.InputRegisters[1021]);
        public double Qc
        {
            get
            {
                //int quants = ((_dataStore.InputRegisters[HiByteFirst ? 1017 : 1018] << 16) | _dataStore.InputRegisters[HiByteFirst ? 1018 : 1017]);
                int quants = ((_dataStore.InputRegisters[1024] << 16) | _dataStore.InputRegisters[1023]);
                return ((quants / 10000000.0) * Coef_CT * Coef_VT);
            }
            set
            {
                _storedQc = value;
                unchecked
                {
                    
                    int quants = (PowerQuantWeight == 0.0) ? 0 : (int)(Math.Round(value / PowerQuantWeight, MidpointRounding.AwayFromZero));
                    //_dataStore.InputRegisters[HiByteFirst ? 1017 : 1018] = (ushort)(quants >> 16);
                    //_dataStore.InputRegisters[HiByteFirst ? 1018 : 1017] = (ushort)(quants & 0xFFFF);
                    _dataStore.InputRegisters[1024] = (ushort)(quants >> 16);
                    _dataStore.InputRegisters[1023] = (ushort)(quants & 0xFFFF);
                }
            }
        }
        public uint Qc_quants => (uint)((_dataStore.InputRegisters[1024] << 16) | _dataStore.InputRegisters[1023]);
        #endregion

        #region S
        public double Sa
        {
            get
            {
                //int quants = ((_dataStore.InputRegisters[HiByteFirst ? 1017 : 1018] << 16) | _dataStore.InputRegisters[HiByteFirst ? 1018 : 1017]);
                int quants = ((_dataStore.InputRegisters[1026] << 16) | _dataStore.InputRegisters[1025]);
                return ((quants / 10000000.0) * Coef_CT * Coef_VT);
            }
            set
            {
                _storedSa = value;
                unchecked
                {
                    
                    int quants = (PowerQuantWeight == 0.0) ? 0 : (int)(Math.Round(value / PowerQuantWeight, MidpointRounding.AwayFromZero));
                    //_dataStore.InputRegisters[HiByteFirst ? 1017 : 1018] = (ushort)(quants >> 16);
                    //_dataStore.InputRegisters[HiByteFirst ? 1018 : 1017] = (ushort)(quants & 0xFFFF);
                    _dataStore.InputRegisters[1026] = (ushort)(quants >> 16);
                    _dataStore.InputRegisters[1025] = (ushort)(quants & 0xFFFF);
                }
            }
        }
        public uint Sa_quants => (uint)((_dataStore.InputRegisters[1026] << 16) | _dataStore.InputRegisters[1025]);
        public double Sb
        {
            get
            {
                //int quants = ((_dataStore.InputRegisters[HiByteFirst ? 1017 : 1018] << 16) | _dataStore.InputRegisters[HiByteFirst ? 1018 : 1017]);
                int quants = ((_dataStore.InputRegisters[1028] << 16) | _dataStore.InputRegisters[1027]);
                return ((quants / 10000000.0) * Coef_CT * Coef_VT);
            }
            set
            {
                _storedSb = value;
                unchecked
                {
                    
                    int quants = (PowerQuantWeight == 0.0) ? 0 : (int)(Math.Round(value / PowerQuantWeight, MidpointRounding.AwayFromZero));
                    //_dataStore.InputRegisters[HiByteFirst ? 1017 : 1018] = (ushort)(quants >> 16);
                    //_dataStore.InputRegisters[HiByteFirst ? 1018 : 1017] = (ushort)(quants & 0xFFFF);
                    _dataStore.InputRegisters[1028] = (ushort)(quants >> 16);
                    _dataStore.InputRegisters[1027] = (ushort)(quants & 0xFFFF);
                }
            }
        }
        public uint Sb_quants => (uint)((_dataStore.InputRegisters[1028] << 16) | _dataStore.InputRegisters[1027]);
        public double Sc
        {
            get
            {
                //int quants = ((_dataStore.InputRegisters[HiByteFirst ? 1017 : 1018] << 16) | _dataStore.InputRegisters[HiByteFirst ? 1018 : 1017]);
                int quants = ((_dataStore.InputRegisters[1030] << 16) | _dataStore.InputRegisters[1029]);
                return ((quants / 10000000.0) * Coef_CT * Coef_VT);
            }
            set
            {
                _storedSc = value;
                unchecked
                {
                    
                    int quants = (PowerQuantWeight == 0.0) ? 0 : (int)(Math.Round(value / PowerQuantWeight, MidpointRounding.AwayFromZero));
                    //_dataStore.InputRegisters[HiByteFirst ? 1017 : 1018] = (ushort)(quants >> 16);
                    //_dataStore.InputRegisters[HiByteFirst ? 1018 : 1017] = (ushort)(quants & 0xFFFF);
                    _dataStore.InputRegisters[1030] = (ushort)(quants >> 16);
                    _dataStore.InputRegisters[1029] = (ushort)(quants & 0xFFFF);
                }
            }
        }
        public uint Sc_quants => (uint)((_dataStore.InputRegisters[1030] << 16) | _dataStore.InputRegisters[1029]);
        #endregion

        #region SYS
        public double Psys
        {
            get
            {
                //int quants = ((_dataStore.InputRegisters[HiByteFirst ? 1017 : 1018] << 16) | _dataStore.InputRegisters[HiByteFirst ? 1018 : 1017]);
                int quants = ((_dataStore.InputRegisters[1032] << 16) | _dataStore.InputRegisters[1031]);
                return ((quants / 10000000.0) * Coef_CT * Coef_VT);
            }
            set
            {
                _storedPsys = value;
                unchecked
                {
                    
                    int quants = (PowerQuantWeight == 0.0) ? 0 : (int)(Math.Round(value / PowerQuantWeight, MidpointRounding.AwayFromZero));
                    //_dataStore.InputRegisters[HiByteFirst ? 1017 : 1018] = (ushort)(quants >> 16);
                    //_dataStore.InputRegisters[HiByteFirst ? 1018 : 1017] = (ushort)(quants & 0xFFFF);
                    _dataStore.InputRegisters[1032] = (ushort)(quants >> 16);
                    _dataStore.InputRegisters[1031] = (ushort)(quants & 0xFFFF);
                }
            }
        }
        public uint Psys_quants => (uint)((_dataStore.InputRegisters[1032] << 16) | _dataStore.InputRegisters[1031]);
        public double Qsys
        {
            get
            {
                //int quants = ((_dataStore.InputRegisters[HiByteFirst ? 1017 : 1018] << 16) | _dataStore.InputRegisters[HiByteFirst ? 1018 : 1017]);
                int quants = ((_dataStore.InputRegisters[1034] << 16) | _dataStore.InputRegisters[1033]);
                return ((quants / 10000000.0) * Coef_CT * Coef_VT);
            }
            set
            {
                _storedQsys = value;
                unchecked
                {
                    
                    int quants = (PowerQuantWeight == 0.0) ? 0 : (int)(Math.Round(value / PowerQuantWeight, MidpointRounding.AwayFromZero));
                    //_dataStore.InputRegisters[HiByteFirst ? 1017 : 1018] = (ushort)(quants >> 16);
                    //_dataStore.InputRegisters[HiByteFirst ? 1018 : 1017] = (ushort)(quants & 0xFFFF);
                    _dataStore.InputRegisters[1034] = (ushort)(quants >> 16);
                    _dataStore.InputRegisters[1033] = (ushort)(quants & 0xFFFF);
                }
            }
        }
        public uint Qsys_quants => (uint)((_dataStore.InputRegisters[1034] << 16) | _dataStore.InputRegisters[1033]);
        public double Ssys
        {
            get
            {
                //int quants = ((_dataStore.InputRegisters[HiByteFirst ? 1017 : 1018] << 16) | _dataStore.InputRegisters[HiByteFirst ? 1018 : 1017]);
                int quants = ((_dataStore.InputRegisters[1036] << 16) | _dataStore.InputRegisters[1035]);
                return (((double)quants / 10000000.0) * Coef_CT * Coef_VT);
            }
            set
            {
                _storedSsys = value;
                unchecked
                {
                    
                    int quants = (PowerQuantWeight == 0.0) ? 0 : (int)(Math.Round(value / PowerQuantWeight, MidpointRounding.AwayFromZero));
                    //_dataStore.InputRegisters[HiByteFirst ? 1017 : 1018] = (ushort)(quants >> 16);
                    //_dataStore.InputRegisters[HiByteFirst ? 1018 : 1017] = (ushort)(quants & 0xFFFF);
                    _dataStore.InputRegisters[1036] = (ushort)(quants >> 16);
                    _dataStore.InputRegisters[1035] = (ushort)(quants & 0xFFFF);
                }
            }
        }
        public uint Ssys_quants => (uint)((_dataStore.InputRegisters[1036] << 16) | _dataStore.InputRegisters[1035]);
        #endregion

        #region Ф
        public ushort Phi_ac
        {
            get { return _dataStore.InputRegisters[1039]; }
            set
            {
                _storedPhi_ac = value;
                _dataStore.InputRegisters[1039] = value;
            }
        }
        
        public ushort Phi_ab
        {
            get { return _dataStore.InputRegisters[1040]; }
            set
            {
                _storedPhi_ab = value;
                _dataStore.InputRegisters[1040] = value;
            }
        }

        public double Cos_a
        {
            get
            {
                unchecked
                {
                    return ((sbyte)BitConverter.GetBytes(_dataStore.InputRegisters[1041])[0]) / 100.0;
                }
            }
            set
            {
                _storedCos_a = value;
                unchecked
                {
                    _dataStore.InputRegisters[1041] &= 0xFF00;
                    _dataStore.InputRegisters[1041] |= (byte)(Math.Round(value * 100, MidpointRounding.AwayFromZero));
                }
            }
        }
        public byte Cos_a_quants => BitConverter.GetBytes(_dataStore.InputRegisters[1041])[0];
        public double Cos_b
        {
            get
            {
                unchecked
                {
                    return ((sbyte)BitConverter.GetBytes(_dataStore.InputRegisters[1041])[1]) / 100.0;
                }                
            }
            set
            {
                _storedCos_b = value;
                unchecked
                {
                    _dataStore.InputRegisters[1041] &= 0x00FF;                    
                    _dataStore.InputRegisters[1041] |= (ushort)((byte)(Math.Round(value * 100, MidpointRounding.AwayFromZero)) << 8);
                }                
            }
        }
        public byte Cos_b_quants => BitConverter.GetBytes(_dataStore.InputRegisters[1041])[1];
        public double Cos_c
        {
            get
            {
                unchecked
                {
                    return ((sbyte)BitConverter.GetBytes(_dataStore.InputRegisters[1042])[0]) / 100.0;
                }
            }
            set
            {
                _storedCos_c = value;
                unchecked
                {
                    _dataStore.InputRegisters[1042] &= 0xFF00;
                    _dataStore.InputRegisters[1042] |= (byte)(Math.Round(value * 100, MidpointRounding.AwayFromZero));
                }
            }
        }
        public byte Cos_c_quants => BitConverter.GetBytes(_dataStore.InputRegisters[1042])[0];
        public double Cos_sys
        {
            get
            {
                unchecked
                {
                    return ((sbyte)BitConverter.GetBytes(_dataStore.InputRegisters[1042])[1]) / 100.0;
                }
            }
            set
            {
                _storedCos_sys = value;
                unchecked
                {
                    _dataStore.InputRegisters[1042] &= 0x00FF;
                    _dataStore.InputRegisters[1042] |= (ushort)((byte)(Math.Round(value * 100, MidpointRounding.AwayFromZero)) << 8);
                }
            }
        }
        public byte Cos_sys_quants => BitConverter.GetBytes(_dataStore.InputRegisters[1042])[1];
        #endregion

        #region Methods

        public void UpdateAllPropertiesFromStoredValues()
        {                        
            foreach (var field in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (field.Name.Contains("_stored"))
                {
                    foreach (var prop in GetType().GetProperties())
                    {
                        if (prop.Name == field.Name.Replace("_stored", ""))
                        {
                            prop.SetValue(this,field.GetValue(this),null);                            
                        }
                    }
                }
            }
        }

        public void UpdatePropertiesFromStoredValues(string[] propertiesNames)
        {
            if (propertiesNames == null || propertiesNames.Length == 0)
                return;

            foreach (var field in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (field.Name.Contains("_stored"))
                {
                    foreach (var prop in GetType().GetProperties())
                    {                        
                        if (propertiesNames.Contains(prop.Name) && prop.Name == field.Name.Replace("_stored", ""))
                        {
                            prop.SetValue(this, field.GetValue(this), null);
                        }
                    }
                }
            }
        }

        public void UpdatePropertiesDependentOnCoef_CT()
        {
            UpdatePropertiesFromStoredValues(new []
               {
                    "Ia", "Ib", "Ic", "Pa", "Pb", "Pc", "Qa", "Qb", "Qc",
                    "Sa", "Sb", "Sc","Psys", "Qsys", "Ssys",
               });
        }

        public void UpdatePropertiesDependentOnCoef_VT()
        {
            UpdatePropertiesFromStoredValues(new []
                {
                    "Ua", "Ub", "Uc", "Pa", "Pb", "Pc", "Qa", "Qb", "Qc",
                    "Sa", "Sb", "Sc","Psys", "Qsys", "Ssys",
                });
        }

        public DataStore GetDataStore()
        {
            return _dataStore;
        }

        public void SetDataStore(DataStore dataStore)
        {
            _dataStore = dataStore;
        }
        #endregion
    }
}
