using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modbus.Data;
using Modbus.Device;
using ModbusReaderSaver;

namespace PO3Core.Utils
{
    
    public class PO3ModbusReaderSaver : ModbusReaderSaver.ModbusReaderSaver
    {
        
        #region Additional methods
        public string ExecuteDeviceCommand(DeviceControlCommands command)
        {
            if (_modbusSerial == null)
                return "Нет подключения";
            try
            {
                _modbusSerial.WriteMultipleRegisters(SlaveAddress, RegisterMappingOffsets.ControlRegisterOffset, new [] { (ushort)command } );
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            return "OK";
        }
        #endregion
               
    }
}
