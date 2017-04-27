using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConversionHelper;
using ModbusReaderSaver;

namespace PO3Core
{
    [Serializable]
    public class PO3DeviceUnitParameterSettings : ModbusExchangeableUnit
    {
        enum MeasurementType
        {
            Voltage,
            Current,
            Power
        }
        private PO3Device _container;//for future use  
        private ushort _chsHigh;
        private ushort _chsLow;
        private ushort _deHigh;
        private ushort _deLow;
        private ulong _valueChs;
        private ulong _valueDe;
        private ulong _divisor;

        public PO3DeviceUnitParameterSettings(PO3Device container)
        {
            _container = container;                        
        }

        //we don't need map for child elements
        public override List<ModbusDataBlock> GetReadMap()
        {
            return null;
        }

        public override List<ModbusDataBlock> GetWriteMap()
        {
            return null;
        }
        #region Methods
        private ulong GCD(ulong a, ulong b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            if (a == 0)
                return b;
            else
                return a;
        }

        ulong GetVoltageScale(ushort voltageDisplayUnits)
        {
            switch (voltageDisplayUnits)
            {
                case 0:
                    return 1000;
                case 1:
                    return 1;
                default:
                    return 1;
            }
        }
        ulong GetCurrentScale(ushort currentDisplayUnits)
        {
            switch (currentDisplayUnits)
            {
                case 0:
                    return 1000;
                case 1:
                    return 1;
                default:
                    return 1;
            }
        }

        ulong GetPowerScale(ushort powerDisplayUnits)
        {
            switch (powerDisplayUnits)
            {
                case 0:
                    return 1000000;
                case 1:
                    return 1000;
                case 2:
                    return 1;
                default:
                    return 1;
            }
        }

        private void GetChsDeDivisor()
        {
            if (ParameterName == "Ua,Ub,Uc" || ParameterName == "Uab,Uac,Ubc")
            {
                _valueChs =
                    ((ulong)_container.DeviceUnitMeasurmentCircuitSettings.PrimaryVoltage *
                     GetVoltageScale(_container.DeviceUnitMeasurmentCircuitSettings.VoltageDisplayUnits));
                _valueDe =
                    ((ulong)_container.DeviceUnitMeasurmentCircuitSettings.SecondaryVoltage * 100);                
            }

            if (ParameterName == "Ia,Ib,Ic")
            {
                _valueChs =
                    ((ulong)_container.DeviceUnitMeasurmentCircuitSettings.PrimaryCurrent *
                     GetCurrentScale(_container.DeviceUnitMeasurmentCircuitSettings.CurrentDisplayUnits));
                _valueDe =
                    ((ulong)_container.DeviceUnitMeasurmentCircuitSettings.SecondaryCurrent * 1000);                
            }

            if (ParameterName == "Pa,Pb,Pc")
            {
                _valueChs =
                    ((ulong)_container.DeviceUnitMeasurmentCircuitSettings.PrimaryCurrent *
                     //GetCurrentScale(_container.DeviceUnitMeasurmentCircuitSettings.CurrentDisplayUnits) *
                     (ulong)_container.DeviceUnitMeasurmentCircuitSettings.PrimaryVoltage *
                     //GetVoltageScale(_container.DeviceUnitMeasurmentCircuitSettings.VoltageDisplayUnits) *
                     GetPowerScale(_container.DeviceUnitMeasurmentCircuitSettings.TotalPowerDisplayUnits));
                _valueDe =
                    ((ulong)_container.DeviceUnitMeasurmentCircuitSettings.SecondaryCurrent * 1000 *
                    (ulong)_container.DeviceUnitMeasurmentCircuitSettings.SecondaryVoltage * 100);                
            }

            if (ParameterName == "Qa,Qb,Qc")
            {
                _valueChs =
                    ((ulong)_container.DeviceUnitMeasurmentCircuitSettings.PrimaryCurrent *
                     //GetCurrentScale(_container.DeviceUnitMeasurmentCircuitSettings.CurrentDisplayUnits) *
                     (ulong)_container.DeviceUnitMeasurmentCircuitSettings.PrimaryVoltage *
                     //GetVoltageScale(_container.DeviceUnitMeasurmentCircuitSettings.VoltageDisplayUnits) *
                     GetPowerScale(_container.DeviceUnitMeasurmentCircuitSettings.ReactivePowerDisplayUnits));
                _valueDe =
                    ((ulong)_container.DeviceUnitMeasurmentCircuitSettings.SecondaryCurrent * 1000 *
                    (ulong)_container.DeviceUnitMeasurmentCircuitSettings.SecondaryVoltage * 100);
            }

            if (ParameterName == "Sa,Sb,Sc")
            {
                _valueChs =
                    ((ulong)_container.DeviceUnitMeasurmentCircuitSettings.PrimaryCurrent *
                     //GetCurrentScale(_container.DeviceUnitMeasurmentCircuitSettings.CurrentDisplayUnits) *
                     (ulong)_container.DeviceUnitMeasurmentCircuitSettings.PrimaryVoltage *
                     //GetVoltageScale(_container.DeviceUnitMeasurmentCircuitSettings.VoltageDisplayUnits) *
                     GetPowerScale(_container.DeviceUnitMeasurmentCircuitSettings.TotalPowerDisplayUnits));
                _valueDe =
                    ((ulong)_container.DeviceUnitMeasurmentCircuitSettings.SecondaryCurrent * 1000 *
                    (ulong)_container.DeviceUnitMeasurmentCircuitSettings.SecondaryVoltage * 100);
            }

            if (ParameterName == "P,Q,S")
            {
                _valueChs =
                    ((ulong)_container.DeviceUnitMeasurmentCircuitSettings.PrimaryCurrent *
                     //GetCurrentScale(_container.DeviceUnitMeasurmentCircuitSettings.CurrentDisplayUnits) *
                     (ulong)_container.DeviceUnitMeasurmentCircuitSettings.PrimaryVoltage *
                     //GetVoltageScale(_container.DeviceUnitMeasurmentCircuitSettings.VoltageDisplayUnits) *
                     GetPowerScale(_container.DeviceUnitMeasurmentCircuitSettings.CommonPowersDisplayUnits));
                _valueDe =
                    ((ulong)_container.DeviceUnitMeasurmentCircuitSettings.SecondaryCurrent * 1000 *
                    (ulong)_container.DeviceUnitMeasurmentCircuitSettings.SecondaryVoltage * 100);
            }

            _divisor = GCD(_valueChs, _valueDe);            
        }
        #endregion
        #region Registers
        //0x6XX0
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort MatrixSymbolRows1 { get; set; }
        //0x6X01
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort MatrixSymbolRows2 { get; set; }
        //0x6X02
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort MatrixSymbolRows3 { get; set; }
        //0x6X03
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort MatrixSymbolRows4 { get; set; }
        //0x6X04
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort MatrixSymbolRows5 { get; set; }       
        //0x6X05
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort MatrixSymbolRows6 { get; set; }
        //0x6X06
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort MatrixSymbolRows7 { get; set; }
        //0x6X07
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort MatrixSymbolRows8 { get; set; }
        //0x6X08
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort BytePosition1 { get; set; }
        //0x6X09
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort BytePosition2 { get; set; }
        //0x6X0A
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort BytePosition3 { get; set; }
        //0x6X0B
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort BytePosition4 { get; set; }
        //0x6X0C
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort Pm { get; set; }
        //0x6X0D
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort Po { get; set; }
        //0x6X0E
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort No { get; set; }
        //0x6X0F
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort ChsLow
        {
            get
            {                
                if (ParameterName == "Ua,Ub,Uc" || ParameterName == "Uab,Uac,Ubc" || ParameterName == "Ia,Ib,Ic"
                    || ParameterName == "Pa,Pb,Pc" || ParameterName == "Qa,Qb,Qc" || ParameterName == "Sa,Sb,Sc"
                    || ParameterName == "P,Q,S")
                {
                    GetChsDeDivisor();                    
                    _chsLow = BitConverter.ToUInt16(BitConverter.GetBytes((float)_valueChs / _divisor), 0);
                }
                return _chsLow;
            }
            set { _chsLow = value; }
        }
        //0x6X10
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort ChsHigh
        {
            get
            {                
                if (ParameterName == "Ua,Ub,Uc" || ParameterName == "Uab,Uac,Ubc" || ParameterName == "Ia,Ib,Ic"
                    || ParameterName == "Pa,Pb,Pc" || ParameterName == "Qa,Qb,Qc" || ParameterName == "Sa,Sb,Sc"
                    || ParameterName == "P,Q,S")
                {
                    GetChsDeDivisor();                                        
                    _chsHigh = BitConverter.ToUInt16(BitConverter.GetBytes((float)_valueChs / _divisor), 2);
                }                
                return _chsHigh;
            }
            set { _chsHigh = value; }
        }
        //0x6X11
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort DeLow
        {
            get
            {                
                if (ParameterName == "Ua,Ub,Uc" || ParameterName == "Uab,Uac,Ubc" || ParameterName == "Ia,Ib,Ic"
                    || ParameterName == "Pa,Pb,Pc" || ParameterName == "Qa,Qb,Qc" || ParameterName == "Sa,Sb,Sc"
                    || ParameterName == "P,Q,S")
                {
                    GetChsDeDivisor();
                    _deLow = BitConverter.ToUInt16(BitConverter.GetBytes((float)_valueDe / _divisor), 0);
                }
                return _deLow;
            }
            set { _deLow = value; }
        }
        //0x6X12
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort DeHigh
        {
            get
            {                
                if (ParameterName == "Ua,Ub,Uc" || ParameterName == "Uab,Uac,Ubc" || ParameterName == "Ia,Ib,Ic"
                    || ParameterName == "Pa,Pb,Pc" || ParameterName == "Qa,Qb,Qc" || ParameterName == "Sa,Sb,Sc"
                    || ParameterName == "P,Q,S")
                {
                    GetChsDeDivisor();
                    _deHigh = BitConverter.ToUInt16(BitConverter.GetBytes((float)_valueDe / _divisor), 2);
                }
                return _deHigh;
            }
            set { _deHigh = value; }
        }
        //0x6X13
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort SmLow { get; set; }
        //0x6X14
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort SmHigh { get; set; }
        //0x6X15
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort VsLow { get; set; }
        //0x6X16
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort VsHigh { get; set; }
        //0x6X17
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort ApPlusLow { get; set; }
        //0x6X18
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort ApPlusHigh { get; set; }
        //0x6X19
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort ApMinusLow { get; set; }
        //0x6X1A
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort ApMinusHigh { get; set; }
        //0x6X1B
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort PercentageLow { get; set; }
        //0x6X1C
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort PercentageHigh { get; set; }        
        //0x6X1D
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort ZnKcTaZpRzZvTfNp { get; set; }
        //0x6X1E
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort Reserve1 { get; set; }
        //0x6X1F
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort Reserve2 { get; set; }
        #endregion

        #region RzZvTfNpZnKcTaZpAndSmSign register decoding
        public byte Zn
        {
            get
            {
                return (byte)(BitConverter.GetBytes(ZnKcTaZpRzZvTfNp)[0] & 0x03);
            }
            set
            {
                if (value > 3)
                    value = 3;
                ZnKcTaZpRzZvTfNp = BitConverter.ToUInt16(new[] 
                {
                    (byte)((BitConverter.GetBytes(ZnKcTaZpRzZvTfNp)[0] & 0xFC) | value) ,
                    BitConverter.GetBytes(ZnKcTaZpRzZvTfNp)[1]
                }, 0);
            }
        }
        #endregion                
        public float ApPlus
        {
            get
            {
                return Convertor.ConvertUShortsToFloat(ApPlusLow, ApPlusHigh);
            }
            set
            {
                ApPlusLow = BitConverter.ToUInt16(BitConverter.GetBytes(value), 0);
                ApPlusHigh = BitConverter.ToUInt16(BitConverter.GetBytes(value), 2);
            }
        }
        public float ApMinus
        {
            get
            {
                return Convertor.ConvertUShortsToFloat(ApMinusLow, ApMinusHigh);
            }
            set
            {
                ApMinusLow = BitConverter.ToUInt16(BitConverter.GetBytes(value), 0);
                ApMinusHigh = BitConverter.ToUInt16(BitConverter.GetBytes(value), 2);
            }
        }
        public float Percentage
        {
            get
            {
                return Convertor.ConvertUShortsToFloat(PercentageLow, PercentageHigh);
            }
            set
            {
                PercentageLow = BitConverter.ToUInt16(BitConverter.GetBytes(value), 0);
                PercentageHigh = BitConverter.ToUInt16(BitConverter.GetBytes(value), 2);
            }
        }
        //Misc
        public string ParameterName { get; set; }     
    }

    [Serializable]
    public class PO3DeviceUnitParametersSettings : ModbusExchangeableUnit
    {
        #region Fields
        private PO3Device _container;//for future use
        public const int ParametersCount = 9;
        [NonSerialized]
        public string[] ParametersNames = {
                                            "Ua,Ub,Uc",
                                            "Uab,Uac,Ubc",
                                            "Ia,Ib,Ic",
                                            "Pa,Pb,Pc",
                                            "Qa,Qb,Qc",
                                            "Sa,Sb,Sc",
                                            "P,Q,S",
                                            "Cos A, Cos B, Cos C, Cos",
                                            "F"                                            
                                          };        
        #endregion
        public PO3DeviceUnitParametersSettings(PO3Device container)
        {
            _container = container;           
            Parameters = new PO3DeviceUnitParameterSettings[ParametersCount];
            for (int i = 0; i < Parameters.Length; i++)
            {
                Parameters[i] = new PO3DeviceUnitParameterSettings(container)
                {
                    ParameterName = ParametersNames[i]                    
                };
            }
        }
        public PO3Device GetContainer()
        {
            return _container;
        }
        public override List<ModbusDataBlock> GetReadMap()
        {
            List<ModbusDataBlock> map = new List<ModbusDataBlock>(ParametersCount);
            for (int param = 0; param < ParametersCount; param++)
            {
                map.Add(new ModbusDataBlock
                        {
                            AbsoluteOffset = (ushort)(param * 32),
                            Offset = (ushort)(0x6000 + param * 32),
                            Size = 32,
                            Type = ModbusDataType.HoldingRegister
                        });
            }
            return map;            
        }

        public override List<ModbusDataBlock> GetWriteMap()
        {
            List<ModbusDataBlock> map = new List<ModbusDataBlock>(ParametersCount);
            for (int param = 0; param < ParametersCount; param++)
            {
                map.Add(new ModbusDataBlock
                        {
                            AbsoluteOffset = (ushort)(param * 32),
                            Offset = (ushort)(0x6000 + param * 32),
                            Size = 32,
                            Type = ModbusDataType.HoldingRegister
                        });
            }
            return map;            
        }

        public PO3DeviceUnitParameterSettings [] Parameters { get; set; }        

    }
}
