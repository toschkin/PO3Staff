using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using ModbusReaderSaver;
using PO3Configurator.Utils;
using PO3Core;
using PO3Core.Utils;
using MVVMToolkit;

namespace PO3Configurator.ViewModel
{
    class PO3DeviceUnitWindowsSettingsTabViewModel : ViewModelBase
    {
        #region Fields

        private PO3DeviceUnitWindowsSettings _po3DeviceUnitWindowsSettings;
        private MainWindowViewModel _parentViewModel;
        private ObservableCollection<PO3DeviceUnitWindowSettingsViewModel> _windowsSettingsTabs = new ObservableCollection<PO3DeviceUnitWindowSettingsViewModel>();

        #endregion

        #region Ctor

        public PO3DeviceUnitWindowsSettingsTabViewModel(MainWindowViewModel parentViewModel)
        {
            _po3DeviceUnitWindowsSettings = parentViewModel.Device.DeviceUnitWindowsSettings;
            _parentViewModel = parentViewModel;
            for (int win = 0; win < parentViewModel.Device.DeviceUnitWindowsSettings.Windows.Length; win++)
            {
                _windowsSettingsTabs.Add(new PO3DeviceUnitWindowSettingsViewModel(win, 
                    parentViewModel.Device.DeviceUnitWindowsSettings));               
            }
        }
        #endregion

        #region Properties

        public int WindowsCount
        {
            get { return _po3DeviceUnitWindowsSettings.WindowsCount; }
            set
            {
                if (value != _po3DeviceUnitWindowsSettings.WindowsCount)
                {
                    _po3DeviceUnitWindowsSettings.WindowsCount = (ushort)value;
                    if (_po3DeviceUnitWindowsSettings.WindowsCount <
                        _po3DeviceUnitWindowsSettings.DefaultWindowIndex + 1)
                    {
                        _po3DeviceUnitWindowsSettings.DefaultWindowIndex = (ushort)(_po3DeviceUnitWindowsSettings.WindowsCount - 1);                        
                        OnPropertyChanged("DefaultWindowIndex");
                    }
                    OnPropertyChanged("AvailableWindowIndexes");
                    UpdateAllChildViewModelsProperties();
                }                                
            }
        }
        public ObservableCollection<int> AvailableWindowsCount
        {
            get
            {
                ObservableCollection<int> counts = new ObservableCollection<int>();
                for (int i = 1; i < 10; i++)
                {
                    counts.Add(i);
                }
                return counts;
            }
        }

        public int DefaultWindowIndex
        {
            get
            {
                return _po3DeviceUnitWindowsSettings.DefaultWindowIndex + 1;
            }
            set
            {
                if (value - 1 != _po3DeviceUnitWindowsSettings.DefaultWindowIndex)
                {
                    _po3DeviceUnitWindowsSettings.DefaultWindowIndex = (ushort)(value-1);
                    UpdateAllChildViewModelsProperties();
                }                
            }
        }
        public ObservableCollection<int> AvailableWindowIndexes
        {
            get
            {
                ObservableCollection<int> indexes = new ObservableCollection<int>();
                for (int i = 1; i < _po3DeviceUnitWindowsSettings.WindowsCount+1; i++)
                {
                    indexes.Add(i);
                }
                return indexes;
            }
        }

        public ObservableCollection<PO3DeviceUnitWindowSettingsViewModel> WindowsSettingsTabs => _windowsSettingsTabs;

        #endregion

        #region Commands
        public ICommand ReadWindowsSettingsFromFileCommand { get { return new Command(param => ReadWindowsSettingsFromFile()); } }
        public ICommand SaveWindowsSettingsToFileCommand { get { return new Command(param => SaveWindowsSettingsToFile()); } }
        public ICommand DefaultWindowsSettingsCommand { get { return new Command(param => DefaultWindowsSettings(param)); } }       

        private void DefaultWindowsSettings(object param)
        {
            ConnectionTypes connectionType = (ConnectionTypes)param;
            
            if (MessageBoxResult.No ==
            MessageBox.Show("Выполнить сброс настроек \"по умолчанию\" для " 
                            + (connectionType == ConnectionTypes.FourWires?"4":"3") 
                            + "-х проводного подключения?", Constants.messageBoxTitle, MessageBoxButton.YesNo,
                            MessageBoxImage.Question))
                return;

            if(connectionType == ConnectionTypes.FourWires)
                _po3DeviceUnitWindowsSettings.Copy(new PO3DeviceUnitWindowsSettings(_po3DeviceUnitWindowsSettings.GetContainer()));
            else
            {
                _po3DeviceUnitWindowsSettings.Copy(new PO3DeviceUnitWindowsSettings(_po3DeviceUnitWindowsSettings.GetContainer()));
                _po3DeviceUnitWindowsSettings.WindowsCount = 4;
                _po3DeviceUnitWindowsSettings.DefaultWindowIndex = 0;

                _po3DeviceUnitWindowsSettings.Windows[0].FirstStringParameterIndex = 3;
                _po3DeviceUnitWindowsSettings.Windows[0].SecondStringParameterIndex = 4;
                _po3DeviceUnitWindowsSettings.Windows[0].ThirdStringParameterIndex = 5;
                _po3DeviceUnitWindowsSettings.Windows[0].AnalogBarParameterIndex = 3;

                _po3DeviceUnitWindowsSettings.Windows[1].FirstStringParameterIndex = 6;
                _po3DeviceUnitWindowsSettings.Windows[1].SecondStringParameterIndex = 26;
                _po3DeviceUnitWindowsSettings.Windows[1].ThirdStringParameterIndex = 8;
                _po3DeviceUnitWindowsSettings.Windows[1].AnalogBarParameterIndex = 6;

                _po3DeviceUnitWindowsSettings.Windows[2].FirstStringParameterIndex = 18;
                _po3DeviceUnitWindowsSettings.Windows[2].SecondStringParameterIndex = 19;
                _po3DeviceUnitWindowsSettings.Windows[2].ThirdStringParameterIndex = 20;
                _po3DeviceUnitWindowsSettings.Windows[2].AnalogBarParameterIndex = 18;

                _po3DeviceUnitWindowsSettings.Windows[3].FirstStringParameterIndex = 24;
                _po3DeviceUnitWindowsSettings.Windows[3].SecondStringParameterIndex = 25;
                _po3DeviceUnitWindowsSettings.Windows[3].ThirdStringParameterIndex = 26;
                _po3DeviceUnitWindowsSettings.Windows[3].AnalogBarParameterIndex = 24;
            }
            UpdateAllViewModelProperties();
        }
        private void ReadWindowsSettingsFromFile()
        {
            OpenFileDialog dlgOpenFileDialog = new OpenFileDialog
            {
                Filter = "Настройки окон|*.dw0",
                Title = "Импорт настроек окон из файла"
            };
            if (dlgOpenFileDialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                if (dlgOpenFileDialog.FileName != "")
                {
                    _parentViewModel.OperationStatus = "Импорт из файла...";
                    FileReaderSaver reader = new FileReaderSaver(dlgOpenFileDialog.FileName);
                    ModbusExchangeableUnit configuration = null;
                    _parentViewModel.OperationStatus = reader.ReadDeviceUnitConfiguration(ref configuration);
                    _po3DeviceUnitWindowsSettings.Copy((PO3DeviceUnitWindowsSettings)configuration);
                    UpdateAllViewModelProperties();
                }
            }
        }
        private void SaveWindowsSettingsToFile()
        {
            SaveFileDialog dlgSaveFileDialog = new SaveFileDialog
            {
                Filter = "Настройки окон|*.dw0",
                Title = "Экспорт настроек окон в файл",
                FileName = "winsettings.dw0"
            };
            if (dlgSaveFileDialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                if (dlgSaveFileDialog.FileName != "")
                {
                    _parentViewModel.OperationStatus = "Экспорт в файл...";
                    FileReaderSaver saver = new FileReaderSaver(dlgSaveFileDialog.FileName);
                    _parentViewModel.OperationStatus = saver.SaveDeviceUnitConfiguration(_po3DeviceUnitWindowsSettings);
                }
            }
        }
        #endregion
    }
}
