using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using ConversionHelper;
using Microsoft.Win32;
using ModbusReaderSaver;
using MVVMToolkit;
using PO3Configurator.Utils;
using PO3Core;
using PO3Core.Utils;

namespace PO3Configurator.ViewModel
{
    class PO3DeviceUnitMeasurmentCircuitSettingsViewModel : ViewModelBase
    {
        #region Fields
        private PO3DeviceUnitMeasurmentCircuitSettings _po3DeviceUnitMeasurmentCircuitSettings;
        private MainWindowViewModel _parentViewModel;
        #endregion

        #region Constructor
        public PO3DeviceUnitMeasurmentCircuitSettingsViewModel(MainWindowViewModel parentViewModel)
        {
            _po3DeviceUnitMeasurmentCircuitSettings = parentViewModel.Device.DeviceUnitMeasurmentCircuitSettings;
            _parentViewModel = parentViewModel;
        }
        #endregion

        #region Commands
        public ICommand ReadMeasurmentCircuitSettingsFromFileCommand { get { return new Command(param => ReadMeasurmentCircuitSettingsFromFile()); } }
        public ICommand SaveMeasurmentCircuitSettingsToFileCommand { get { return new Command(param => SaveMeasurmentCircuitSettingsToFile()); } }
        public ICommand DefaultMeasurmentCircuitSettingsCommand { get { return new Command(param => DefaultMeasurmentCircuitSettings()); } }

        private void DefaultMeasurmentCircuitSettings()
        {
            if (MessageBoxResult.No ==
                MessageBox.Show("Выполнить сброс настроек?", Constants.messageBoxTitle, MessageBoxButton.YesNo,
                    MessageBoxImage.Question))
                return;
            _po3DeviceUnitMeasurmentCircuitSettings.Copy(new PO3DeviceUnitMeasurmentCircuitSettings(_po3DeviceUnitMeasurmentCircuitSettings.GetContainer()));
            UpdateAllViewModelProperties();
        }
        private void ReadMeasurmentCircuitSettingsFromFile()
        {
            OpenFileDialog dlgOpenFileDialog = new OpenFileDialog
            {
                Filter = "Настройки измерит. цепи|*.dm0",
                Title = "Импорт настроек  измерит. цепи из файла"
            };
            if (dlgOpenFileDialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                if (dlgOpenFileDialog.FileName != "")
                {
                    _parentViewModel.OperationStatus = "Импорт из файла...";
                    FileReaderSaver reader = new FileReaderSaver(dlgOpenFileDialog.FileName);
                    ModbusExchangeableUnit configuration = null;
                    _parentViewModel.OperationStatus = reader.ReadDeviceUnitConfiguration(ref configuration);
                    _po3DeviceUnitMeasurmentCircuitSettings.Copy((PO3DeviceUnitMeasurmentCircuitSettings)configuration);
                    UpdateAllViewModelProperties();
                }
            }
        }
        private void SaveMeasurmentCircuitSettingsToFile()
        {
            SaveFileDialog dlgSaveFileDialog = new SaveFileDialog
            {
                Filter = "Настройки  измерит. цепи|*.dm0",
                Title = "Экспорт настроек  измерит. цепи в файл",
                FileName = "measursettings.dm0"
            };
            if (dlgSaveFileDialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                if (dlgSaveFileDialog.FileName != "")
                {
                    _parentViewModel.OperationStatus = "Экспорт в файл...";
                    FileReaderSaver saver = new FileReaderSaver(dlgSaveFileDialog.FileName);
                    _parentViewModel.OperationStatus = saver.SaveDeviceUnitConfiguration(_po3DeviceUnitMeasurmentCircuitSettings);
                }
            }
        }
        #endregion

        #region Properties

        #region Model

        public float PrimaryVoltage
        {
            get
            {
                return _po3DeviceUnitMeasurmentCircuitSettings.PrimaryVoltage;
            }
            set
            {
                _po3DeviceUnitMeasurmentCircuitSettings.PrimaryVoltage = value;                
                OnPropertyChanged("Coef_VT");
            }
        }
        public ObservableCollection<string> AvailablePrimaryVoltages
        {
            get
            {
                ObservableCollection<string> voltages = new ObservableCollection<string>
                {
                    "100","6300","10000","15000","20000","24000","27000","35000","110000","220000","330000","500000","750000","1150000"
                };
                return voltages;
            }
        }
        
        public float SecondaryVoltage
        {
            get
            {
                return _po3DeviceUnitMeasurmentCircuitSettings.SecondaryVoltage;
            }
            set
            {               
                _po3DeviceUnitMeasurmentCircuitSettings.SecondaryVoltage = value;
                OnPropertyChanged("Coef_VT");
            }
        }
        public ObservableCollection<string> AvailableSecondaryVoltages
        {
            get
            {
                ObservableCollection<string> voltages = new ObservableCollection<string>
                {
                    "100"
                };
                return voltages;
            }
        }
        public float PrimaryCurrent
        {
            get
            {
                return _po3DeviceUnitMeasurmentCircuitSettings.PrimaryCurrent;
            }
            set
            {               
                _po3DeviceUnitMeasurmentCircuitSettings.PrimaryCurrent = value;
                OnPropertyChanged("Coef_CT");
            }
        }
        public ObservableCollection<string> AvailablePrimaryCurrents
        {
            get
            {
                ObservableCollection<string> voltages = new ObservableCollection<string>
                {
                    "1","5","10","15","20","30","100","200","300","400","500","1000","1500","2000","3000","4000","5000","10000","20000","30000","40000"
                };
                return voltages;
            }
        }
        public float SecondaryCurrent
        {
            get
            {
                return _po3DeviceUnitMeasurmentCircuitSettings.SecondaryCurrent;
            }
            set
            {             
                _po3DeviceUnitMeasurmentCircuitSettings.SecondaryCurrent = value;
                OnPropertyChanged("Coef_CT");
            }
        }
        public ObservableCollection<string> AvailableSecondaryCurrents
        {
            get
            {
                ObservableCollection<string> voltages = new ObservableCollection<string>
                {
                    "1","5"
                };
                return voltages;
            }
        }
        public float Coef_CT
        {
            get
            {
                return PrimaryCurrent / SecondaryCurrent;
            }
            set{}
        }

        public float Coef_VT
        {
            get
            {
                return PrimaryVoltage/SecondaryVoltage;                
            }
            set { }
        }

        public string ConnectionType
        {
            get
            {
                switch (_po3DeviceUnitMeasurmentCircuitSettings.ConnectionType)
                {
                    case 3:
                        return "3-х проводная";
                    case 4:
                        return "4-х проводная";                    
                    default:
                        return "4-х проводная";
                }
            }
            set
            {
                switch (value)
                {
                    case "3-х проводная":
                        _po3DeviceUnitMeasurmentCircuitSettings.ConnectionType = 3;
                        break;
                    case "4-х проводная":
                        _po3DeviceUnitMeasurmentCircuitSettings.ConnectionType = 4;
                        break;                    
                    default:
                        _po3DeviceUnitMeasurmentCircuitSettings.ConnectionType = 4;
                        break;
                }
            }
        }
        public ObservableCollection<string> AvailableConnectionTypes
        {
            get
            {
                ObservableCollection<string> types = new ObservableCollection<string>
                {
                    "3-х проводная",
                    "4-х проводная"
                };
                return types;
            }
        }

       public string CurrentDisplayUnits
        {
            get
            {
                switch (_po3DeviceUnitMeasurmentCircuitSettings.CurrentDisplayUnits)
                {
                    case 0:
                        return "А";
                    case 1:
                        return "кА";
                    default:
                        return "А";
                }
            }
            set
            {
                switch (value)
                {
                    case "А":
                        _po3DeviceUnitMeasurmentCircuitSettings.CurrentDisplayUnits = 0;
                        break;
                    case "кА":
                        _po3DeviceUnitMeasurmentCircuitSettings.CurrentDisplayUnits = 1;
                        break;
                    default:
                        _po3DeviceUnitMeasurmentCircuitSettings.CurrentDisplayUnits = 1;
                        break;
                }
            }
        }
        public ObservableCollection<string> AvailableCurrentDisplayUnits
        {
            get
            {
                ObservableCollection<string> values = new ObservableCollection<string>
                {
                    "А",
                    "кА"
                };
                return values;
            }
        }

        public string VoltageDisplayUnits
        {
            get
            {
                switch (_po3DeviceUnitMeasurmentCircuitSettings.VoltageDisplayUnits)
                {
                    case 0:
                        return "В";
                    case 1:
                        return "кВ";
                    default:
                        return "В";
                }
            }
            set
            {
                switch (value)
                {
                    case "В":
                        _po3DeviceUnitMeasurmentCircuitSettings.VoltageDisplayUnits = 0;
                        break;
                    case "кВ":
                        _po3DeviceUnitMeasurmentCircuitSettings.VoltageDisplayUnits = 1;
                        break;
                    default:
                        _po3DeviceUnitMeasurmentCircuitSettings.VoltageDisplayUnits = 1;
                        break;
                }
            }
        }

        public ObservableCollection<string> AvailableVoltageDisplayUnits
        {
            get
            {
                ObservableCollection<string> values = new ObservableCollection<string>
                {
                    "В",
                    "кВ"
                };
                return values;
            }
        }

        public string ActivePowerDisplayUnits
        {
            get
            {
                switch (_po3DeviceUnitMeasurmentCircuitSettings.ActivePowerDisplayUnits)
                {
                    case 0:
                        return "Вт";
                    case 1:
                        return "кВт";
                    case 2:
                        return "МВт";
                    default:
                        return "Вт";
                }
            }
            set
            {
                switch (value)
                {
                    case "Вт":
                        _po3DeviceUnitMeasurmentCircuitSettings.ActivePowerDisplayUnits = 0;
                        break;
                    case "кВт":
                        _po3DeviceUnitMeasurmentCircuitSettings.ActivePowerDisplayUnits = 1;
                        break;
                    case "МВт":
                        _po3DeviceUnitMeasurmentCircuitSettings.ActivePowerDisplayUnits = 2;
                        break;
                    default:
                        _po3DeviceUnitMeasurmentCircuitSettings.ActivePowerDisplayUnits = 2;
                        break;
                }
            }
        }

        public ObservableCollection<string> AvailableActivePowerDisplayUnits
        {
            get
            {
                ObservableCollection<string> values = new ObservableCollection<string>
                {
                    "Вт",
                    "кВт",
                    "МВт"
                };
                return values;
            }
        }

        public string ReactivePowerDisplayUnits
        {
            get
            {
                switch (_po3DeviceUnitMeasurmentCircuitSettings.ReactivePowerDisplayUnits)
                {
                    case 0:
                        return "Вар";
                    case 1:
                        return "кВар";
                    case 2:
                        return "МВар";
                    default:
                        return "Вар";
                }
            }
            set
            {
                switch (value)
                {
                    case "Вар":
                        _po3DeviceUnitMeasurmentCircuitSettings.ReactivePowerDisplayUnits = 0;
                        break;
                    case "кВар":
                        _po3DeviceUnitMeasurmentCircuitSettings.ReactivePowerDisplayUnits = 1;
                        break;
                    case "МВар":
                        _po3DeviceUnitMeasurmentCircuitSettings.ReactivePowerDisplayUnits = 2;
                        break;
                    default:
                        _po3DeviceUnitMeasurmentCircuitSettings.ReactivePowerDisplayUnits = 2;
                        break;
                }
            }
        }

        public ObservableCollection<string> AvailableReactivePowerDisplayUnits
        {
            get
            {
                ObservableCollection<string> values = new ObservableCollection<string>
                {
                    "Вар",
                    "кВар",
                    "МВар"
                };
                return values;
            }
        }

        public string TotalPowerDisplayUnits
        {
            get
            {
                switch (_po3DeviceUnitMeasurmentCircuitSettings.TotalPowerDisplayUnits)
                {
                    case 0:
                        return "ВА";
                    case 1:
                        return "кВА";
                    case 2:
                        return "МВА";
                    default:
                        return "ВА";
                }
            }
            set
            {
                switch (value)
                {
                    case "ВА":
                        _po3DeviceUnitMeasurmentCircuitSettings.TotalPowerDisplayUnits = 0;
                        break;
                    case "кВА":
                        _po3DeviceUnitMeasurmentCircuitSettings.TotalPowerDisplayUnits = 1;
                        break;
                    case "МВА":
                        _po3DeviceUnitMeasurmentCircuitSettings.TotalPowerDisplayUnits = 2;
                        break;
                    default:
                        _po3DeviceUnitMeasurmentCircuitSettings.TotalPowerDisplayUnits = 2;
                        break;
                }
            }
        }

        public ObservableCollection<string> AvailableTotalPowerDisplayUnits
        {
            get
            {
                ObservableCollection<string> values = new ObservableCollection<string>
                {
                    "ВА",
                    "кВА",
                    "МВА"
                };
                return values;
            }
        }

        public string CommonPowersDisplayUnits
        {
            get
            {
                switch (_po3DeviceUnitMeasurmentCircuitSettings.CommonPowersDisplayUnits)
                {
                    case 0:
                        return "Вт,Вар,ВА";
                    case 1:
                        return "кВт,кВар,кВА";
                    case 2:
                        return "МВт,МВар,МВА";
                    default:
                        return "Вт,Вар,ВА";
                }
            }
            set
            {
                switch (value)
                {
                    case "Вт,Вар,ВА":
                        _po3DeviceUnitMeasurmentCircuitSettings.CommonPowersDisplayUnits = 0;
                        break;
                    case "кВт,кВар,кВА":
                        _po3DeviceUnitMeasurmentCircuitSettings.CommonPowersDisplayUnits = 1;
                        break;
                    case "МВт,МВар,МВА":
                        _po3DeviceUnitMeasurmentCircuitSettings.CommonPowersDisplayUnits = 2;
                        break;
                    default:
                        _po3DeviceUnitMeasurmentCircuitSettings.CommonPowersDisplayUnits = 2;
                        break;
                }
            }
        }

        public ObservableCollection<string> AvailableCommonPowersDisplayUnits
        {
            get
            {
                ObservableCollection<string> values = new ObservableCollection<string>
                {
                    "Вт,Вар,ВА",
                    "кВт,кВар,кВА",
                    "МВт,МВар,МВА"
                };
                return values;
            }
        }
        #endregion

        #endregion
    }
}
