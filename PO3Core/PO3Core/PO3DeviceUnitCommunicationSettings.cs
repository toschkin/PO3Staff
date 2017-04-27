using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModbusReaderSaver;

namespace PO3Core
{
    [Serializable]
    public class PO3DeviceUnitCommunicationSettings : ModbusExchangeableUnit 
    {
        private PO3Device _container;//for future use        
        public PO3DeviceUnitCommunicationSettings(PO3Device container)
        {
            _container = container;
            DeviceAddress = 1;
            DeviceBaudRate = 2;
            DeviceStopBits = 0;
            DeviceParity = 0;
            DeviceMode = 0;
            DeviceModbusPollingFunctionCode = 4;
            DeviceModbusPollingStartingAddress = 0x03EC;
            DeviceModbusPollingRegistersCount = 38;
            DeviceSilentInterval = 10;
            DeviceModbusPollingRequestsInterval = 5;
            DeviceReplyDelayInSlaveMode = 0;
            DeviceModbusPollingFaultsCount = 5;
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
                        Offset = 0x4000,
                        Size = 12,
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
                        Offset = 0x4000,
                        Size = 12,
                        Type = ModbusDataType.HoldingRegister
                    }
                };
        }

        //0x4000
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort DeviceAddress { get; set; }
        //0x4001
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort DeviceBaudRate { get; set; }
        //0x4002
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort DeviceStopBits { get; set; }
        //0x4003
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort DeviceParity { get; set; }
        //0x4004
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort DeviceMode { get; set; }
        //0x4005
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort DeviceModbusPollingFunctionCode { get; set; }
        //0x4006
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort DeviceModbusPollingStartingAddress { get; set; }
        //0x4007
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort DeviceModbusPollingRegistersCount { get; set; }
        //0x4008
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort DeviceSilentInterval { get; set; }
        //0x4009
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort DeviceModbusPollingRequestsInterval { get; set; }
        //0x400A
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort DeviceReplyDelayInSlaveMode { get; set; }
        //0x400B
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort DeviceModbusPollingFaultsCount { get; set; }        
    }
}
