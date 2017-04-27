using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using ModbusReaderSaver;
using MVVMToolkit;
using PO3Configurator.Utils;
using PO3Core;
using PO3Core.Utils;

namespace PO3Configurator.ViewModel
{
    internal class PO3DeviceUnitCommunicationSettingsTabViewModel : ViewModelBase, IDataErrorInfo
    {
        #region Fields

        private PO3DeviceUnitCommunicationSettings _po3DeviceUnitCommunicationSettings;
        private MainWindowViewModel _parentViewModel;
        //private uint _deviceModbusPollingRequestsInterval;

        #endregion

        #region Constructor

        public PO3DeviceUnitCommunicationSettingsTabViewModel(MainWindowViewModel parentViewModel)
        {
            _po3DeviceUnitCommunicationSettings = parentViewModel.Device.DeviceUnitCommunicationSettings;
            _parentViewModel = parentViewModel;
        }

        #endregion

        #region Commands
        public ICommand ReadCommSettingsFromFileCommand { get { return new Command(param => ReadCommSettingsFromFile()); } }
        public ICommand SaveCommSettingsToFileCommand { get { return new Command(param => SaveCommSettingsToFile()); } }
        public ICommand DefaultCommSettingsCommand { get { return new Command(param => DefaultCommSettings()); } }

        private void DefaultCommSettings()
        {
            if (MessageBoxResult.No ==
                MessageBox.Show("Выполнить сброс настроек?", Constants.messageBoxTitle, MessageBoxButton.YesNo,
                    MessageBoxImage.Question))
                return;
            _po3DeviceUnitCommunicationSettings.Copy(new PO3DeviceUnitCommunicationSettings(_po3DeviceUnitCommunicationSettings.GetContainer()));
            UpdateAllViewModelProperties();
        }
        private void ReadCommSettingsFromFile()
        {
            OpenFileDialog dlgOpenFileDialog = new OpenFileDialog
            {
                Filter = "Настройки коммуникаций|*.dc0",
                Title = "Импорт настроек коммуникаций из файла"
            };
            if (dlgOpenFileDialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                if (dlgOpenFileDialog.FileName != "")
                {
                    _parentViewModel.OperationStatus = "Импорт из файла...";
                    FileReaderSaver reader = new FileReaderSaver(dlgOpenFileDialog.FileName);
                    ModbusExchangeableUnit configuration = null;
                    _parentViewModel.OperationStatus = reader.ReadDeviceUnitConfiguration(ref configuration);
                    _po3DeviceUnitCommunicationSettings.Copy((PO3DeviceUnitCommunicationSettings)configuration);
                    UpdateAllViewModelProperties();
                }
            }            
        }
        private void SaveCommSettingsToFile()
        {
            SaveFileDialog dlgSaveFileDialog = new SaveFileDialog
            {
                Filter = "Настройки коммуникаций|*.dc0",
                Title = "Экспорт настроек коммуникаций в файл",
                FileName = "commsettings.dc0"
            };
            if (dlgSaveFileDialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                if (dlgSaveFileDialog.FileName != "")
                {
                    _parentViewModel.OperationStatus = "Экспорт в файл...";
                    FileReaderSaver saver = new FileReaderSaver(dlgSaveFileDialog.FileName);
                    _parentViewModel.OperationStatus = saver.SaveDeviceUnitConfiguration(_po3DeviceUnitCommunicationSettings);
                }
            }
        }
        #endregion

        #region Properties        

        public int DeviceBaudRate
        {
            get
            {
                switch (_po3DeviceUnitCommunicationSettings.DeviceBaudRate)
                {
                    case 0:
                        return 2400;
                    case 1:
                        return 4800;
                    case 2:
                        return 9600;
                    case 3:
                        return 14400;
                    case 4:
                        return 19200;
                    default:
                        return 9600;
                }
            }
            set
            {
                switch (value)
                {
                    case 2400:
                        DeviceSilentInterval = 40;
                        _po3DeviceUnitCommunicationSettings.DeviceBaudRate = 0;
                        break;
                    case 4800:
                        DeviceSilentInterval = 20;
                        _po3DeviceUnitCommunicationSettings.DeviceBaudRate = 1;
                        break;
                    case 9600:
                        DeviceSilentInterval = 10;
                        _po3DeviceUnitCommunicationSettings.DeviceBaudRate = 2;
                        break;
                    case 14400:
                        DeviceSilentInterval = 5;
                        _po3DeviceUnitCommunicationSettings.DeviceBaudRate = 3;
                        break;
                    case 19200:
                        DeviceSilentInterval = 5;
                        _po3DeviceUnitCommunicationSettings.DeviceBaudRate = 4;
                        break;
                    default:
                        DeviceSilentInterval = 10;
                        _po3DeviceUnitCommunicationSettings.DeviceBaudRate = 2;
                        break;
                }
                OnPropertyChanged("DeviceSilentInterval");
            }
        }
        public ObservableCollection<int> AvailableBaudRates
        {
            get
            {
                ObservableCollection<int> speeds = new ObservableCollection<int>
                {
                    2400,
                    4800,
                    9600,
                    14400,
                    19200
                };
                return speeds;
            }            
        }
        public string DeviceParity
        {
            get
            {
                switch (_po3DeviceUnitCommunicationSettings.DeviceParity)
                {
                    case 0:
                        return "нет";                    
                    case 2:
                        return "нечет";
                    case 3:
                        return "чёт";
                    default:
                        return "нет";
                }
            }
            set
            {
                switch (value)
                {
                    case "нет":
                        _po3DeviceUnitCommunicationSettings.DeviceParity = 0;
                        break;
                    case "нечет":
                        _po3DeviceUnitCommunicationSettings.DeviceParity = 2;
                        break;
                    case "чёт":
                        _po3DeviceUnitCommunicationSettings.DeviceParity = 3;
                        break;                   
                    default:
                        _po3DeviceUnitCommunicationSettings.DeviceParity = 0;
                        break;
                }
            }
        }
        public ObservableCollection<string> AvailableParities
        {
            get
            {
                ObservableCollection<string> parities = new ObservableCollection<string>
                {
                    "нет",
                    "нечет",
                    "чёт"                 
                };
                return parities;
            }
        }
        public int DeviceStopBits
        {
            get
            {
                switch (_po3DeviceUnitCommunicationSettings.DeviceStopBits)
                {
                    case 0:
                        return 1;
                    case 1:
                        return 2;                    
                    default:
                        return 1;
                }
            }
            set
            {
                switch (value)
                {
                    case 1:
                        _po3DeviceUnitCommunicationSettings.DeviceStopBits = 0;
                        break;
                    case 2:
                        _po3DeviceUnitCommunicationSettings.DeviceStopBits = 1;
                        break;                   
                    default:
                        _po3DeviceUnitCommunicationSettings.DeviceParity = 0;
                        break;
                }
            }
        }
        public ObservableCollection<int> AvailableStopBits
        {
            get
            {
                ObservableCollection<int> parities = new ObservableCollection<int>
                {
                    1,
                    2
                };
                return parities;
            }
        }
        public ushort DeviceAddress
        {
            get { return _po3DeviceUnitCommunicationSettings.DeviceAddress; }
            set { _po3DeviceUnitCommunicationSettings.DeviceAddress = value; }
        }
        public string DeviceMode
        {
            get
            {
                switch (_po3DeviceUnitCommunicationSettings.DeviceMode)
                {
                    case 0:
                        return "MASTER";
                    case 1:
                        return "SLAVE";                    
                    default:
                        return "MASTER";
                }
            }
            set
            {
                switch (value)
                {
                    case "MASTER":
                        _po3DeviceUnitCommunicationSettings.DeviceMode = 0;
                        break;
                    case "SLAVE":
                        _po3DeviceUnitCommunicationSettings.DeviceMode = 1;
                        break;                    
                    default:
                        _po3DeviceUnitCommunicationSettings.DeviceMode = 0;
                        break;
                }
            }
        }
        public ObservableCollection<string> AvailableDeviceModes
        {
            get
            {
                ObservableCollection<string> deviceModes = new ObservableCollection<string>
                {
                    "MASTER",
                    "SLAVE",                    
                };
                return deviceModes;
            }
        }        
        public ushort DeviceSilentInterval
        {
            get { return _po3DeviceUnitCommunicationSettings.DeviceSilentInterval; }
            set { _po3DeviceUnitCommunicationSettings.DeviceSilentInterval = value; }
        }
        public string DeviceModbusPollingRequestsInterval
        {
            get { return (_po3DeviceUnitCommunicationSettings.DeviceModbusPollingRequestsInterval*100).ToString(); }
            set
            {
                ushort selectedValue = Convert.ToUInt16(value);
                _po3DeviceUnitCommunicationSettings.DeviceModbusPollingRequestsInterval = (ushort) (selectedValue/100);                
            }
        }
        public ObservableCollection<string> AvailableDeviceModbusPollingRequestsIntervals
        {
            get
            {
                ObservableCollection<string> deviceModbusPollingRequestsIntervals = new ObservableCollection<string>
                {
                    "100",
                    "200",
                    "300",
                    "400",
                    "500",
                    "600",
                    "700",
                    "800",
                    "900",
                    "1000",
                };
                return deviceModbusPollingRequestsIntervals;
            }
        }
        public ushort DeviceModbusPollingFaultsCount
        {
            get { return _po3DeviceUnitCommunicationSettings.DeviceModbusPollingFaultsCount; }
            set { _po3DeviceUnitCommunicationSettings.DeviceModbusPollingFaultsCount = value; }
        }
        public ObservableCollection<ushort> AvailableDeviceModbusPollingFaultsCounts
        {
            get
            {
                ObservableCollection<ushort> deviceModbusPollingFaultsCounts = new ObservableCollection<ushort>
                {
                    1,2,3,4,5
                };
                return deviceModbusPollingFaultsCounts;
            }
        }        
        public string DeviceModbusPollingFunctionCode
        {
            get
            {
                switch (_po3DeviceUnitCommunicationSettings.DeviceModbusPollingFunctionCode)
                {
                    case 3:
                        return "03: Read Holding registers";
                    case 4:
                        return "04: Read Input registers";
                    default:
                        return "04: Read Input registers";
                }
            }
            set
            {
                switch (value)
                {
                    case "03: Read Holding registers":
                        _po3DeviceUnitCommunicationSettings.DeviceModbusPollingFunctionCode = 3;
                        break;
                    case "04: Read Input registers":
                        _po3DeviceUnitCommunicationSettings.DeviceModbusPollingFunctionCode = 4;
                        break;
                    default:
                        _po3DeviceUnitCommunicationSettings.DeviceModbusPollingFunctionCode = 4;
                        break;
                }
            }
        }
        public ObservableCollection<string> AvailableDeviceModbusPollingFunctionCodes
        {
            get
            {
                ObservableCollection<string> deviceModbusPollingFunctionCodes = new ObservableCollection<string>
                {
                    "03: Read Holding registers",
                    "04: Read Input registers",
                };
                return deviceModbusPollingFunctionCodes;
            }
        }
        public ushort DeviceModbusPollingStartingAddress
        {
            get { return _po3DeviceUnitCommunicationSettings.DeviceModbusPollingStartingAddress; }
            set
            {
                _po3DeviceUnitCommunicationSettings.DeviceModbusPollingStartingAddress = value;
                OnPropertyChanged("DeviceModbusPollingStartingAddressHex");
                OnPropertyChanged("DeviceModbusPollingRegistersCount");
            }
        }
        public string DeviceModbusPollingStartingAddressHex
        {
            get
            {
                return string.Format("Hex: {0:X4}", _po3DeviceUnitCommunicationSettings.DeviceModbusPollingStartingAddress);
            }             
        }        
        public ushort DeviceModbusPollingRegistersCount
        {
            get { return _po3DeviceUnitCommunicationSettings.DeviceModbusPollingRegistersCount; }
            set
            {
                _po3DeviceUnitCommunicationSettings.DeviceModbusPollingRegistersCount = value;
                OnPropertyChanged("DeviceModbusPollingStartingAddress");
            }
        }
        public ushort DeviceReplyDelayInSlaveMode
        {
            get { return _po3DeviceUnitCommunicationSettings.DeviceReplyDelayInSlaveMode; }
            set { _po3DeviceUnitCommunicationSettings.DeviceReplyDelayInSlaveMode = value; }
        }
        
        #endregion
        

        #region IDataError members
        public string Error
        {
            get { return string.Empty; }
        }

        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;

                switch (columnName)
                {
                    case "DeviceModbusPollingStartingAddress":
                        if (DeviceModbusPollingStartingAddress + DeviceModbusPollingRegistersCount > 0xFFFF)
                        {
                            result = "Запрос выходит за пределы адресного поля";
                        }
                        break;
                    case "DeviceModbusPollingRegistersCount":
                        if (DeviceModbusPollingStartingAddress + DeviceModbusPollingRegistersCount > 0xFFFF)
                        {
                            result = "Запрос выходит за пределы адресного поля";
                        }
                        break;
                };

                return result;
            }
        }
        #endregion
    }
}
