using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConversionHelper;
using ModbusReaderSaver;


namespace PO3Core
{
    [Serializable]
    public class PO3DeviceUnitMeasurmentCircuitSettings : ModbusExchangeableUnit
    {
        private PO3Device _container;//for future use        
        public PO3DeviceUnitMeasurmentCircuitSettings(PO3Device container)
        {
            _container = container;
            ConnectionType = 4;
            PrimaryVoltage = 110000;
            SecondaryVoltage = 100;
            PrimaryCurrent = 150;
            SecondaryCurrent = 1;
            VoltageDisplayUnits = 1;
            CurrentDisplayUnits = 1;
            ActivePowerDisplayUnits = 2;
            ReactivePowerDisplayUnits = 2;
            TotalPowerDisplayUnits = 2;
            CommonPowersDisplayUnits = 2;
        }
        public PO3Device GetContainer()
        {
            return _container;
        }
        public override List<ModbusDataBlock> GetReadMap()
        {
            return new List<ModbusDataBlock>
                {
                    new ModbusDataBlock
                    {
                        AbsoluteOffset = 0,
                        Offset = 0x3005,
                        Size = 15,
                        Type = ModbusDataType.HoldingRegister
                    }
                };
        }

        public override List<ModbusDataBlock> GetWriteMap()
        {
            return new List<ModbusDataBlock>
                {
                    new ModbusDataBlock
                    {
                        AbsoluteOffset = 0,
                        Offset = 0x3005,
                        Size = 15,
                        Type = ModbusDataType.HoldingRegister
                    }
                };
        }        

        //0x3005
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort ConnectionType { get; set; }

        //0x3006
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort PrimaryVoltageLow { get; set; }

        //0x3007
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort PrimaryVoltageHigh { get; set; }

        //0x3008
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort SecondaryVoltageLow { get; set; }

        //0x3009
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort SecondaryVoltageHigh { get; set; }

        //0x300A
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort PrimaryCurrentLow { get; set; }

        //0x300B
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort PrimaryCurrentHigh { get; set; }

        //0x300C
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort SecondaryCurrentLow { get; set; }

        //0x300D
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort SecondaryCurrentHigh { get; set; }

        //0x300E
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort VoltageDisplayUnits { get; set; }

        //0x300F
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort CurrentDisplayUnits { get; set; }

        //0x3010
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort ActivePowerDisplayUnits { get; set; }

        //0x3011
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort ReactivePowerDisplayUnits { get; set; }

        //0x3012
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort TotalPowerDisplayUnits { get; set; }

        //0x3013
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort CommonPowersDisplayUnits { get; set; }

        #region Floating point values
        public float PrimaryVoltage
        {
            get
            {
                return Convertor.ConvertUShortsToFloat(PrimaryVoltageLow,PrimaryVoltageHigh);
            }
            set
            {
                PrimaryVoltageLow = BitConverter.ToUInt16(BitConverter.GetBytes(value), 0);
                PrimaryVoltageHigh = BitConverter.ToUInt16(BitConverter.GetBytes(value), 2);               
            }
        }

        public float SecondaryVoltage
        {
            get
            {
                return Convertor.ConvertUShortsToFloat(SecondaryVoltageLow,SecondaryVoltageHigh);
            }
            set
            {
                SecondaryVoltageLow = BitConverter.ToUInt16(BitConverter.GetBytes(value), 0);
                SecondaryVoltageHigh = BitConverter.ToUInt16(BitConverter.GetBytes(value), 2);                
            }
        }

        public float PrimaryCurrent
        {
            get
            {
                return Convertor.ConvertUShortsToFloat(PrimaryCurrentLow,PrimaryCurrentHigh);
            }
            set
            {
                PrimaryCurrentLow = BitConverter.ToUInt16(BitConverter.GetBytes(value), 0);
                PrimaryCurrentHigh= BitConverter.ToUInt16(BitConverter.GetBytes(value), 2);                
            }
        }

        public float SecondaryCurrent
        {
            get
            {
                return Convertor.ConvertUShortsToFloat(SecondaryCurrentLow,SecondaryCurrentHigh);
            }
            set
            {
                SecondaryCurrentLow =BitConverter.ToUInt16(BitConverter.GetBytes(value), 0);
                SecondaryCurrentHigh= BitConverter.ToUInt16(BitConverter.GetBytes(value), 2);               
            }
        }        

        #endregion
    }
}
