using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Collections.ObjectModel;
using Microsoft.Win32;

namespace PO3Configurator.Model
{
    public class SerialPortSettingsModel
    {
        private readonly string _registryNode;

        public SerialPort Port { get; private set;}

        #region Constructor
        public SerialPortSettingsModel(string registryNode)
        {
            _registryNode = registryNode;
            Port = new SerialPort();
        }
        #endregion

        #region Properties

        #region Port          
        public string ComPort
        {
            get
            {
                if (Registry.CurrentUser.OpenSubKey(_registryNode) == null) return "COM1";
                var openSubKey = Registry.CurrentUser.OpenSubKey(_registryNode);
                if (openSubKey != null)
                    return (string)openSubKey.GetValue("Port", "COM1");
                return "COM1";
            }
            set
            {
                if (Registry.CurrentUser.OpenSubKey(_registryNode, RegistryKeyPermissionCheck.ReadWriteSubTree) == null)
                    return;
                var openSubKey = Registry.CurrentUser.OpenSubKey(_registryNode, RegistryKeyPermissionCheck.ReadWriteSubTree);
                openSubKey?.SetValue("Port", value);                                
            }
        }               
        #endregion

        #region Baud Rate
        public int BaudRate
        {
            get
            {
                if (Registry.CurrentUser.OpenSubKey(_registryNode) == null) return 9600;
                var openSubKey = Registry.CurrentUser.OpenSubKey(_registryNode);
                return openSubKey != null ? Convert.ToInt32(openSubKey.GetValue("BaudRate", "9600")) : 9600;
            }
            set
            {
                if (Registry.CurrentUser.OpenSubKey(_registryNode, RegistryKeyPermissionCheck.ReadWriteSubTree) == null)
                    return;
                var openSubKey = Registry.CurrentUser.OpenSubKey(_registryNode, RegistryKeyPermissionCheck.ReadWriteSubTree);
                openSubKey?.SetValue("BaudRate", value.ToString());
            }
        }
        #endregion

        #region Parity
        public Parity PortParity
        {
            get
            {
                if (Registry.CurrentUser.OpenSubKey(_registryNode) == null) return Parity.None;
                var openSubKey = Registry.CurrentUser.OpenSubKey(_registryNode);
                if (openSubKey != null)
                    return (Parity)Enum.Parse(typeof(Parity), (string)openSubKey.GetValue("Parity", Parity.None.ToString()));
                return Parity.None;
            }
            set
            {
                if (Registry.CurrentUser.OpenSubKey(_registryNode, RegistryKeyPermissionCheck.ReadWriteSubTree) != null)
                {
                    var openSubKey = Registry.CurrentUser.OpenSubKey(_registryNode, RegistryKeyPermissionCheck.ReadWriteSubTree);
                    openSubKey?.SetValue("Parity", value.ToString());
                }
            }
        }                       
        #endregion

        #region DataBits
        public int ByteSize
        {
            get
            {
                if (Registry.CurrentUser.OpenSubKey(_registryNode) == null) return 8;
                var openSubKey = Registry.CurrentUser.OpenSubKey(_registryNode);
                return openSubKey != null ? Convert.ToInt32(openSubKey.GetValue("ByteSize", "8")) : 8;
            }
            set
            {
                if (Registry.CurrentUser.OpenSubKey(_registryNode, RegistryKeyPermissionCheck.ReadWriteSubTree) == null)
                    return;
                var openSubKey = Registry.CurrentUser.OpenSubKey(_registryNode, RegistryKeyPermissionCheck.ReadWriteSubTree);
                openSubKey?.SetValue("ByteSize", value.ToString());
            }
        }        
        #endregion

        #region StopBits
        public StopBits PortStopBits
        {
            get
            {
                if (Registry.CurrentUser.OpenSubKey(_registryNode) == null) return StopBits.One;
                var openSubKey = Registry.CurrentUser.OpenSubKey(_registryNode);
                if (openSubKey != null)
                    return (StopBits)Enum.Parse(typeof(StopBits), (string)openSubKey.GetValue("StopBits", StopBits.Two.ToString()));
                return StopBits.One;
            }
            set
            {
                if (Registry.CurrentUser.OpenSubKey(_registryNode, RegistryKeyPermissionCheck.ReadWriteSubTree) == null)
                    return;
                var openSubKey = Registry.CurrentUser.OpenSubKey(_registryNode, RegistryKeyPermissionCheck.ReadWriteSubTree);
                openSubKey?.SetValue("StopBits", value.ToString());
            }
        }
        #endregion

        #endregion

        public void Connect()
        {
            Port.PortName = ComPort;
            Port.BaudRate = BaudRate;
            Port.DataBits = ByteSize;
            Port.StopBits = PortStopBits;
            Port.Parity = PortParity;
            Port.Open();
        }
    }
}