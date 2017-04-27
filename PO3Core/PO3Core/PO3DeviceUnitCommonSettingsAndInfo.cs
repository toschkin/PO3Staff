using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModbusReaderSaver;

namespace PO3Core
{
    [Serializable]
    public class PO3DeviceUnitCommonSettingsAndInfo : ModbusExchangeableUnit
    {
        private PO3Device _container;//for future use
        public PO3DeviceUnitCommonSettingsAndInfo(PO3Device container)
        {
            _container = container;
        }
        public PO3Device GetContainer()
        {
            return _container;
        }
        public override List<ModbusDataBlock> GetReadMap ()
        {
            return new List<ModbusDataBlock>             
                {
                    new ModbusDataBlock
                    {
                        AbsoluteOffset = 0,
                        Offset = 0x3000,
                        Size = 5,
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
                            AbsoluteOffset = 3,
                            Offset = 0x3003,
                            Size = 1,
                            Type = ModbusDataType.HoldingRegister
                        }
                };
        }
       
        #region Data Properties
        //0x3000
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessRead)]
        public ushort FirmwareVersion { get; set; }
        //0x3001
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessRead)]
        public ushort ConfigurationVersion { get; set; }
        //0x3002
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessRead)]
        public ushort AssociatedDeviceType { get; set; }
        //0x3003
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessReadWrite)]
        public ushort DevicePassword { get; set; }
        //0x3004
        [ModbusProperty(Access = ModbusRegisterAccessType.AccessRead)]
        public ushort DeviceStatus { get; set; }
        #endregion       
    }
}
