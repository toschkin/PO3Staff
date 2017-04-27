using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modbus.Data;
using Modbus.Device;

namespace ModbusReaderSaver
{
    
    public class ModbusReaderSaver
    {
        #region Ctors
        public ModbusReaderSaver()
        {
            Port = new SerialPort((string)SerialPort.GetPortNames()?.GetValue(0));            
            SlaveAddress = 1;            
        }

        public ModbusReaderSaver(SerialPort port)
        {
            Port = port;
            SlaveAddress = 1;
        }
        #endregion

        #region Fields

        protected ModbusSerialMaster _modbusSerial;

        #endregion

        #region Properties       

        public SerialPort Port { get; set; }
        public byte SlaveAddress { get; set; }
        public bool IsConnected { get; private set; }
        public int Timeout
        {
            get
            {
                return _modbusSerial?.Transport.ReadTimeout ?? 0;
            }
            set
            {
                if (_modbusSerial != null)
                {
                    _modbusSerial.Transport.WriteTimeout = _modbusSerial.Transport.ReadTimeout = value;
                }
                    
            }
        }
        public int Retries
        {
            get
            {
                return _modbusSerial?.Transport.Retries ?? 0;
            }
            set
            {
                if (_modbusSerial != null)                
                    _modbusSerial.Transport.Retries = value;                
            }
        }
        public int WaitToRetryMilliseconds
        {
            get
            {
                return _modbusSerial?.Transport.WaitToRetryMilliseconds ?? 0;
            }
            set
            {
                if (_modbusSerial != null)
                    _modbusSerial.Transport.WaitToRetryMilliseconds = value;
            }
        }        

        #endregion

        #region Methods

        #region Connection
        public void ConnectToDevice(bool autoSetTimeout)
        {
            if (Port.IsOpen == false)
                Port.Open();
            if(_modbusSerial == null)
                _modbusSerial = ModbusSerialMaster.CreateRtu(Port);
            
            _modbusSerial.Transport.Retries = 0;
            
            if (autoSetTimeout)
            {
                switch (Port.BaudRate)
                {
                    //programmatically set timeouts
                    case 2400:
                        _modbusSerial.Transport.WriteTimeout = _modbusSerial.Transport.ReadTimeout = 5000;
                        break;
                    case 4800:
                        _modbusSerial.Transport.WriteTimeout = _modbusSerial.Transport.ReadTimeout = 3500;
                        break;
                    case 9600:
                        _modbusSerial.Transport.WriteTimeout = _modbusSerial.Transport.ReadTimeout = 2500;
                        break;
                    case 14400:
                        _modbusSerial.Transport.WriteTimeout = _modbusSerial.Transport.ReadTimeout = 2000;
                        break;
                    case 19200:
                        _modbusSerial.Transport.WriteTimeout = _modbusSerial.Transport.ReadTimeout = 1000;
                        break;
                    default:
                        _modbusSerial.Transport.WriteTimeout = _modbusSerial.Transport.ReadTimeout = 5000;
                        break;
                }
            }            
            IsConnected = true;            
        }                     
        public void DisconnectFromDevice()
        {
            _modbusSerial?.Dispose();
            _modbusSerial = null;
            Port.Close();
            IsConnected = false;
        }
        public string GetConnectionParametersString()
        {
            return $"{Port.PortName} {Port.BaudRate} {Port.DataBits}{Port.Parity.ToString()[0]}{(int)Port.StopBits}";
        }
        #endregion       

        #region Reading & Writing methods
        public string ReadUnitData(ModbusExchangeableUnit unit)
        {
            if (_modbusSerial == null)
                return "Нет подключения";
            try
            {
                List<ushort> readRegisters = new List<ushort>();               

                foreach (ModbusDataBlock block in unit.GetReadMap())
                {
                    readRegisters.AddRange(_modbusSerial.ReadHoldingRegisters(SlaveAddress,
                        block.Offset, block.Size));
                }
                unit.SetPropertiesDataFromRegisters(readRegisters.ToArray()); 
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            return "OK";
        }       
        public string WriteUnitData(ModbusExchangeableUnit unit)    
        {
            if (_modbusSerial == null)
                return "Нет подключения";
            try
            {
                foreach (ModbusDataBlock block in unit.GetWriteMap())
                {
                    /*if (block.Size == 1)
                        _modbusSerial.WriteSingleRegister(SlaveAddress, block.Offset,
                            unit.GetPropertiesDataAsRegisters()[block.AbsoluteOffset]);
                    else
                    {
                        List<ushort> registersToWrite = new List<ushort>();
                        for (int j = block.AbsoluteOffset; j < block.AbsoluteOffset + block.Size; j++)
                            registersToWrite.Add(unit.GetPropertiesDataAsRegisters()[j]);

                        _modbusSerial.WriteMultipleRegisters(SlaveAddress, block.Offset, registersToWrite.ToArray());
                    }*/
                    //петя попросил только 16-ю функцию
                    List<ushort> registersToWrite = new List<ushort>();
                    for (int j = block.AbsoluteOffset; j < block.AbsoluteOffset + block.Size; j++)
                        registersToWrite.Add(unit.GetPropertiesDataAsRegisters()[j]);

                    _modbusSerial.WriteMultipleRegisters(SlaveAddress, block.Offset, registersToWrite.ToArray());
                }                            
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            return "OK";
        }
        #endregion        
        
        #endregion
    }
}
