using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using ConversionHelper;
using CRCCalc;
using Modbus.Device;

namespace PO3Core.Utils
{
    public class PO3SlaveModeSetter
    {
        
        #region Fields
        private readonly SerialPort _serialPort = new SerialPort();
        private byte[] _recievedPacket = new byte[0];
        private ModbusSerialMaster _modbusSerial;
        private SimpleFileLogger _logger;

        #endregion

        #region Properties

        public byte DetectedSlaveAddress { get; private set; }

        #endregion
        
        #region Methods
        public PO3SlaveModeSetter(SerialPort port)
        {
            if(port != null)
                _serialPort = port;
            _modbusSerial = ModbusSerialMaster.CreateRtu(_serialPort);
            _modbusSerial.Transport.Retries = 0;
            _logger = new SimpleFileLogger("PO3log.txt");
        }
        public bool SetSlaveMode()
        {
            DetectedSlaveAddress = 0; //redundant,only for notification

            _logger.Log("SetSlaveMode...");

            if (IsSlaveModeActive())
            {
                _logger.Log("Already in slave mode");
                return true;
            }

            _logger.Log("RX: " + Convertor.ConvertByteArrayToHexString(_recievedPacket));

            if (!Crc16.CheckCrc(_recievedPacket))
                return false;

            byte[] packetToSend = { _recievedPacket[0], (byte)(_recievedPacket[1] + 0x80), (byte)0x05 };
            Crc16.AddCrc(ref packetToSend);
            if(!SendPacket(packetToSend))
                return false;

            _logger.Log("TX: " + Convertor.ConvertByteArrayToHexString(packetToSend));

            try
            {    
                Thread.Sleep(2000);
                _logger.Log("TX: <ReadHoldingRegisters>");
                _modbusSerial.ReadHoldingRegisters(_recievedPacket[0], RegisterMappingOffsets.PasswordRegisterOffset, 1);                
            }
            catch (Exception)
            {
                return false;
            }
            _logger.Log("SetSlaveMode - OK");
            DetectedSlaveAddress = _recievedPacket[0];
            return true;
        }
        private bool IsSlaveModeActive()
        {
            if (_serialPort.IsOpen == false)
                _serialPort.Open();
            int oldTimeout = _serialPort.ReadTimeout;
            _serialPort.ReadTimeout = 3000;
                   
            if (RecivePacket(ref _recievedPacket))
                return false;
            _serialPort.ReadTimeout = oldTimeout;
            return true;
        }
        private int CalcSilentInterval()
        {
            switch (_serialPort.BaudRate)
            {
                case 2400:
                    return 80;
                   
                case 4800:
                    return 40;

                default:
                    return 20;                   
            }            
        }
        private bool RecivePacket(ref byte[] packet)
        {            
            int silentInterval = CalcSilentInterval();            
            
            if (_serialPort.IsOpen == false)                            
                return false;
            
            try
            {
                int bytesTotallyRecieved = 0;
                bool smthRead = false;
                int sleepTime = (silentInterval / 3 == 0) ? 1 : silentInterval / 3;
                int stepsCount = _serialPort.ReadTimeout / sleepTime;
                int silenceTime = 0;

                for (int i = 0; i < stepsCount; i++)
                {
                    int bytesRecieved = _serialPort.BytesToRead;
                    if (bytesRecieved > 0)
                    {
                        silenceTime = 0;
                        smthRead = true;
                        Array.Resize<Byte>(ref packet, bytesTotallyRecieved + bytesRecieved);                        
                        _serialPort.Read(packet, bytesTotallyRecieved, bytesRecieved);
                        bytesTotallyRecieved += bytesRecieved;
                        continue;
                    }
                                        
                    silenceTime += sleepTime;
                    if (smthRead && silenceTime > silentInterval)                        
                        break;                        
                    
                    Thread.Sleep(sleepTime);
                }

                if (smthRead == false)               
                    return false;
                                
                return true;
            }            
            catch (Exception)
            {                
                return false;
            }
        }
        private bool SendPacket(byte[] packet)
        {
            if (_serialPort.IsOpen == false)
            {                
                return false;
            }

            try
            {                
                Thread.Sleep(CalcSilentInterval());
                _serialPort.Write(packet, 0, packet.Length);                
            }
            catch (Exception)
            {                
                return false;
            }            
            return true;
        }
        #endregion
    }
}
