using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommonWindows.View;
using CommonWindows.ViewModel;
using Microsoft.Win32;
using Modbus.Data;
using Modbus.Device;
using MTESimulator.Model;
using MTESimulator.Utils;
using MVVMToolkit;

namespace MTESimulator.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(Window parentWindow)
        {
            _parentWindow = parentWindow;
            if (Registry.CurrentUser.OpenSubKey(Constants.registryAppNode) == null)
                Registry.CurrentUser.CreateSubKey(Constants.registryAppNode);
            _serialPortSettingsViewModel = new SerialPortSettingsViewModel(Constants.registryAppNode);
            _defaultBackgroundBrush.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/MTESimulator;component/Images/defaultBG.jpg"));
            _defaultBackgroundBrush.Opacity = 0.3;
            _slaveDataStore = DataStoreFactory.CreateDefaultDataStore();
            MTEDeviceModbusMemoryMapViewModel = new MTEDeviceModbusMemoryMapViewModel(_slaveDataStore,this);
        }

        #region Fields

        private Window _parentWindow;
        private SerialPort _port;         
        private Thread _slaveThread;
        private ModbusSlave _slave;
        private DataStore _slaveDataStore;
        private string _currentOperationStatus = "";        
        private string _сonnectionStatus = "Отключено";
        private bool _portIsOpen;
        private SerialPortSettingsViewModel _serialPortSettingsViewModel;
        private ImageBrush _defaultBackgroundBrush = new ImageBrush();
        private ulong _queriesCounter;        
        #endregion

        #region Commands
        public ICommand ConnectCommand { get { return new Command(param => Connect(), param => CanExecuteConnect()); } }
        public ICommand DisconnectCommand { get { return new Command(param => Disconnect(), param => CanExecuteDisconnect()); } }
        public ICommand DafaultCommand { get { return new Command(param => Dafault(), param => true); } }        
        public ICommand ImportCommand { get { return new Command(param => Import(), param => true); } }
        public ICommand ExportCommand { get { return new Command(param => Export(), param => true); } }
        #endregion

        #region Properties
        public string OperationStatus
        {
            get { return _currentOperationStatus; }
            set
            {
                if (_currentOperationStatus != value)
                {
                    _currentOperationStatus = value;
                    OnPropertyChanged("OperationStatus");
                }
            }
        }
        public string СonnectionStatus
        {
            get { return _сonnectionStatus; }
            set
            {
                if (_сonnectionStatus != value)
                {
                    _сonnectionStatus = value;
                    OnPropertyChanged("СonnectionStatus");
                }
            }
        }
        public bool PortIsOpen
        {
            get { return _portIsOpen; }
            set
            {
                if (_portIsOpen != value)
                {
                    _portIsOpen = value;
                    OnPropertyChanged("PortIsOpen");
                }
            }
        }

        public MTEDeviceModbusMemoryMapViewModel MTEDeviceModbusMemoryMapViewModel { get; set; }
        #endregion

        #region Methods

        #region Commands
        private void Connect()
        {
            SerialPortSettingsView serialPortSettingsView = new SerialPortSettingsView
            {
                Owner = _parentWindow,
                DataContext = _serialPortSettingsViewModel,
                MainStackPannel = {Background = (Brush)(new BrushConverter().ConvertFrom("#FFECEAEA")) }
            };            

            if (serialPortSettingsView.ShowDialog() != true)
                return;
            if (!_serialPortSettingsViewModel.IsConnected)
                return;
            _port = _serialPortSettingsViewModel.Port;
            PortIsOpen = true;
            СonnectionStatus =  $"Подключено: {_port.PortName} {_port.BaudRate} {_port.DataBits}{_port.Parity.ToString()[0]}{(int)_port.StopBits}";
            
            _slave = ModbusSerialSlave.CreateRtu(1, _port);
            _slave.DataStore = _slaveDataStore; 
            //TODO need to add data map of MTE device            

            _slave.ModbusSlaveRequestReceived += OnModbusSlaveRequestReceived;
            _queriesCounter = 0;
            _slaveThread = new Thread(_slave.Listen);
            _slaveThread.Start();

        }

        private bool CanExecuteConnect()
        {
            return !PortIsOpen;
        }

        private void Disconnect()
        {
            if (PortIsOpen)
            {
                _slave.Dispose();
                _slaveThread.Abort();
                _port.Close();
                PortIsOpen = false;
                OperationStatus = "";
                СonnectionStatus = "Отключено";
            }            
        }

        private bool CanExecuteDisconnect()
        {
            return PortIsOpen;
        }

        private void Import()
        {
            MTEDeviceModbusMemoryMapViewModel.Import();            
        }

        private void Export()
        {
            MTEDeviceModbusMemoryMapViewModel.Export();            
        }
        private void Dafault()
        {
            MTEDeviceModbusMemoryMapViewModel.Dafault();
        }

        #endregion

        public void OnModbusSlaveRequestReceived(object sender, ModbusSlaveRequestEventArgs e)
        {
            if (_queriesCounter + 1 == UInt64.MaxValue)
                _queriesCounter = 0;
            _queriesCounter++;
            OperationStatus = "RX["+ _queriesCounter + "]:  "+BitConverter.ToString(e.Message.MessageFrame).Replace("-", " "); 
        }

        #endregion

    }

}
