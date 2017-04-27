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
    class PO3DeviceUnitParametersSettingsViewModel : ViewModelBase
    {
        #region Fields
        private PO3DeviceUnitParametersSettings _po3DeviceUnitParametersSettings;
        private MainWindowViewModel _parentViewModel;
        private PO3DeviceUnitParameterSettings _selectedParameter;

        #endregion

        #region Constructor
        public PO3DeviceUnitParametersSettingsViewModel(MainWindowViewModel parentViewModel)
        {
            _po3DeviceUnitParametersSettings = parentViewModel.Device.DeviceUnitParametersSettings;
            _parentViewModel = parentViewModel;
            _selectedParameter = parentViewModel.Device.DeviceUnitParametersSettings.Parameters[0];
        }
        #endregion

        #region Commands
        public ICommand ReadParametersSettingsFromFileCommand { get { return new Command(param => ReadParametersSettingsFromFile()); } }
        public ICommand SaveParametersSettingsToFileCommand { get { return new Command(param => SaveParametersSettingsToFile()); } }
        /*public ICommand DefaultMeasurmentCircuitSettingsCommand { get { return new Command(param => DefaultMeasurmentCircuitSettings()); } }

        private void DefaultMeasurmentCircuitSettings()
        {
            if (MessageBoxResult.No ==
                MessageBox.Show("Выполнить сброс настроек?", Constants.messageBoxTitle, MessageBoxButton.YesNo,
                    MessageBoxImage.Question))
                return;
            _po3DeviceUnitMeasurmentCircuitSettings.Copy(new PO3DeviceUnitMeasurmentCircuitSettings(_po3DeviceUnitMeasurmentCircuitSettings.GetContainer()));
            UpdateAllViewModelProperties();
        }*/
        private void ReadParametersSettingsFromFile()
        {
            OpenFileDialog dlgOpenFileDialog = new OpenFileDialog
            {
                Filter = "Настройки параметров|*.dp0",
                Title = "Импорт настроек параметров из файла"
            };
            if (dlgOpenFileDialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                if (dlgOpenFileDialog.FileName != "")
                {
                    _parentViewModel.OperationStatus = "Импорт из файла...";
                    FileReaderSaver reader = new FileReaderSaver(dlgOpenFileDialog.FileName);
                    ModbusExchangeableUnit configuration = null;
                    _parentViewModel.OperationStatus = reader.ReadDeviceUnitConfiguration(ref configuration);
                    _po3DeviceUnitParametersSettings.Copy((PO3DeviceUnitParametersSettings)configuration);
                    UpdateAllViewModelProperties();
                }
            }
        }
        private void SaveParametersSettingsToFile()
        {
            SaveFileDialog dlgSaveFileDialog = new SaveFileDialog
            {
                Filter = "Настройки параметров|*.dp0",
                Title = "Экспорт настроек параметров в файл",
                FileName = "paramsettings.dp0"
            };
            if (dlgSaveFileDialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                if (dlgSaveFileDialog.FileName != "")
                {
                    _parentViewModel.OperationStatus = "Экспорт в файл...";
                    FileReaderSaver saver = new FileReaderSaver(dlgSaveFileDialog.FileName);
                    _parentViewModel.OperationStatus = saver.SaveDeviceUnitConfiguration(_po3DeviceUnitParametersSettings);
                }
            }
        }
        #endregion

        #region Properties

        #region Model
        
        public ObservableCollection<PO3DeviceUnitParameterSettings> Parameters => new ObservableCollection<PO3DeviceUnitParameterSettings>(_po3DeviceUnitParametersSettings.Parameters);
        public PO3DeviceUnitParameterSettings SelectedParameter
        {
            get
            {
                return _selectedParameter;
            }
            set
            {
                _selectedParameter = value;
                OnPropertyChanged("IsZnVisible");

                OnPropertyChanged("SelectedZn");
                OnPropertyChanged("SelectedApPlus");
                OnPropertyChanged("SelectedApMinus");
                OnPropertyChanged("SelectedPercentage");                
            }
        }
        
        public string SelectedZn
        {
            get
            {
                return AvailableZnValues[SelectedParameter.Zn];
            }
            set
            {
                SelectedParameter.Zn = (byte)AvailableZnValues.IndexOf(value);
            }
        }
        public ObservableCollection<string> AvailableZnValues => new ObservableCollection<string> { "нет", "1 младший", "2 младших", "3 младших" };

        public float SelectedApPlus
        {
            get
            {
                return SelectedParameter.ApPlus;
            }
            set
            {
                if (value > 9999)
                    value = 9999.0f;
                if (value < -9999)
                    value = -9999.0f;
                SelectedParameter.ApPlus = value;
                OnPropertyChanged("SelectedApPlus");
            }
        }
        public float SelectedApMinus
        {
            get { return SelectedParameter.ApMinus; }
            set
            {
                if (value > 9999)
                    value = 9999.0f;
                if (value < -9999)
                    value = -9999.0f;
                SelectedParameter.ApMinus = value;
                OnPropertyChanged("SelectedApMinus");
            }
        }
        public float SelectedPercentage
        {
            get { return SelectedParameter.Percentage; }
            set
            {
                if (value < 0)
                    value = 0.0f;
                if (value > 9999)
                    value = 9999.0f;
                SelectedParameter.Percentage = value;                
                OnPropertyChanged("SelectedPercentage");
            }
        }
        public Visibility IsVisible
        {
            get
            {
                if (_selectedParameter.ParameterName == "F")
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }       
        #endregion

        #endregion        
    }
}
