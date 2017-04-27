using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Modbus.Device;
using ModbusReaderSaver;
using PO3Core.Utils;

namespace PO3Core
{
    [Serializable]
    public class PO3Device
    {
        #region Ctors        
        public PO3Device()
        {
            SlaveAddress = 1;                       
            DeviceUnitCommonSettingsAndInfo = new PO3DeviceUnitCommonSettingsAndInfo(this);
            DeviceUnitCommunicationSettings = new PO3DeviceUnitCommunicationSettings(this);
            DeviceUnitWindowsSettings = new PO3DeviceUnitWindowsSettings(this);
            DeviceUnitMeasurmentCircuitSettings = new PO3DeviceUnitMeasurmentCircuitSettings(this);
            DeviceUnitParametersSettings = new PO3DeviceUnitParametersSettings(this);
        }
        #endregion

        #region Members                        

        #endregion

        #region Properties

        public PO3DeviceUnitCommonSettingsAndInfo DeviceUnitCommonSettingsAndInfo { get; set; }
        public PO3DeviceUnitCommunicationSettings DeviceUnitCommunicationSettings { get; set; }
        public PO3DeviceUnitWindowsSettings DeviceUnitWindowsSettings { get; set; }
        public PO3DeviceUnitMeasurmentCircuitSettings DeviceUnitMeasurmentCircuitSettings { get; set; }
        public PO3DeviceUnitParametersSettings DeviceUnitParametersSettings { get; set; }

        #region Additional properties               
        //заполняться будет в DeviceReaderSaver
        public byte SlaveAddress { get; set; }
        public List<ModbusExchangeableUnit> PO3DeviceAsUnits => new List<ModbusExchangeableUnit>()
        {
            DeviceUnitCommonSettingsAndInfo ,
            DeviceUnitCommunicationSettings,
            DeviceUnitWindowsSettings,
            DeviceUnitMeasurmentCircuitSettings,
            DeviceUnitParametersSettings
        };

        #endregion
               
        #endregion

        #region Methods               
        #endregion
    }
}
