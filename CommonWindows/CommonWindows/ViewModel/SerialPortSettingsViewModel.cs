using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MVVMToolkit;
using PO3Configurator.Model;


namespace CommonWindows.ViewModel
{
    public class SerialPortSettingsViewModel
    {
        #region Fields        
        private readonly SerialPortSettingsModel _serialPortSettings;
        #endregion

        #region Constructor
        public SerialPortSettingsViewModel(string registryAppNode)
        {
            _serialPortSettings = new SerialPortSettingsModel(registryAppNode);

            ConnectCommand = new Command(arg => ConnectOnPort());
        }
        #endregion

        #region Properties

        public string MessageBoxTitle { get; set; }
        public SerialPort Port => _serialPortSettings.Port;
        public bool IsConnected => _serialPortSettings.Port.IsOpen;
        public int BaudRate { get { return _serialPortSettings.BaudRate; } set { if(_serialPortSettings.BaudRate != value) _serialPortSettings.BaudRate = value; } }
        public string ComPort { get { return _serialPortSettings.ComPort; } set { if (_serialPortSettings.ComPort != value) _serialPortSettings.ComPort = value; } }
        public Parity PortParity { get { return _serialPortSettings.PortParity; } set { if (_serialPortSettings.PortParity != value) _serialPortSettings.PortParity = value; } }
        public int ByteSize { get { return _serialPortSettings.ByteSize; } set { if (_serialPortSettings.ByteSize != value) _serialPortSettings.ByteSize = value; } }
        public int PortStopBits { get { return (int)_serialPortSettings.PortStopBits; } set { if ((int)_serialPortSettings.PortStopBits != value) _serialPortSettings.PortStopBits = (StopBits)value; } }
        static public ObservableCollection<string> AvailableComPorts
        {
            get
            {
                return new ObservableCollection<string>(SerialPort.GetPortNames());
            }
        }
        static public ObservableCollection<int> AvailableBaudRates
        {
            get
            {
                ObservableCollection<int> speeds = new ObservableCollection<int>
                {
                    /*75,
                    100,
                    110,
                    134,
                    150,
                    200,
                    300,
                    600,
                    1200,
                    1800,*/
                    2400,
                    4800,
                    //7200,
                    9600,
                    14400,
                    19200,
                    /*38400,
                    56000,
                    57600,
                    64000,
                    115200,
                    128000*/
                };
                return speeds;
            }
        }
        static public ObservableCollection<Parity> AvailablePortParities
        {
            get
            {
                ObservableCollection<Parity> parities = new ObservableCollection<Parity>
                {
                    Parity.None,
                    Parity.Even,
                    Parity.Odd,
                    Parity.Mark,
                    Parity.Space
                };
                return parities;
            }
        }
        static public ObservableCollection<int> AvailableByteSizes
        {
            get
            {
                ObservableCollection<int> sizes = new ObservableCollection<int>
                {
                    7,
                    8
                };
                return sizes;
            }
        }
        static public ObservableCollection<int> AvailablePortStopBits
        {
            get
            {
                ObservableCollection<int> stopBits = new ObservableCollection<int>
                {                    
                    (int)StopBits.One,
                    (int)StopBits.Two,                    
                };
                return stopBits;
            }
        }
        #endregion

        #region Commands       
        public ICommand ConnectCommand { get; set; }
        #endregion

        #region Methods        
        public void ConnectOnPort()
        {
            try
            {
                _serialPortSettings.Connect();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Невозможно открыть порт!\r\n" + exception.Message,
                    MessageBoxTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }            
        }
        public void DisconnectFromPort()
        {
            try
            {
                _serialPortSettings.Port.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Невозможно закрыть порт!\r\n" + exception.Message, MessageBoxTitle, MessageBoxButton.OK);
            }
        }        
        #endregion
    }
}
