using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModbusReaderSaver;

namespace PO3Core
{
    [Serializable]
    public class PO3DeviceUnitWindowSettings : ModbusExchangeableUnit
    {
        //we don't need map for child elements
        public override List<ModbusDataBlock> GetReadMap()
        {
            return null;
        }

        public override List<ModbusDataBlock> GetWriteMap()
        {
            return null;
        }

        //0x5003+window index + 0
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort FirstStringParameterIndex { get; set; }
        //0x5003+window index + 1
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort SecondStringParameterIndex { get; set; }
        //0x5003+window index + 2
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort ThirdStringParameterIndex { get; set; }
        //0x5003+window index + 3
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort AnalogBarParameterIndex { get; set; }
    }

    [Serializable]
    public class PO3DeviceUnitWindowsSettings : ModbusExchangeableUnit
    {
        private PO3Device _container;//for future use
        public PO3DeviceUnitWindowsSettings(PO3Device container)
        {
            _container = container;
            ParametersCount = 27;
            WindowsCount = 9;
            DefaultWindowIndex = 0;
            Windows = new PO3DeviceUnitWindowSettings[9];
            
            for (int i = 0, k = 0;  i < Windows.Length; i++,k+=3)
            {
                Windows[i] = new PO3DeviceUnitWindowSettings
                {
                    FirstStringParameterIndex = (ushort) k,
                    SecondStringParameterIndex = (ushort) (k + 1),
                    ThirdStringParameterIndex = (ushort) (k + 2),
                    AnalogBarParameterIndex = (ushort) (k)
                };
            }            
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
                        Offset = 0x5000,
                        Size = 39,
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
                        Offset = 0x5000,
                        Size = 39,
                        Type = ModbusDataType.HoldingRegister
                    }
                };
        }

        //0x5000
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort ParametersCount { get; set; }
        //0x5001
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort WindowsCount { get; set; }
        //0x5002
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort DefaultWindowIndex { get; set; }
        //0x5003-0x5026
        public PO3DeviceUnitWindowSettings[] Windows { get; set; }

    }
}
