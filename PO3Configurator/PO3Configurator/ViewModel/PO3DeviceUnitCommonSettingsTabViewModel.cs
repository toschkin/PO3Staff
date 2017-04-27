using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMToolkit;
using PO3Core;
using PO3Core.Utils;

namespace PO3Configurator.ViewModel
{
    class PO3DeviceUnitCommonSettingsTabViewModel : ViewModelBase
    {
        #region Fields
        private PO3DeviceUnitCommonSettingsAndInfo _po3DeviceCommonSettingsAndInfo;        
        #endregion

        #region Constructor
        public PO3DeviceUnitCommonSettingsTabViewModel(MainWindowViewModel parentViewModel)
        {            
            _po3DeviceCommonSettingsAndInfo = parentViewModel.Device.DeviceUnitCommonSettingsAndInfo;
        }
        #endregion

        #region Properties

        #region Model

        public string DevicePassword
        {
            get
            {                
                return _po3DeviceCommonSettingsAndInfo.DevicePassword.ToString();
            }
            set
            {
                if (_po3DeviceCommonSettingsAndInfo.DevicePassword.ToString() != value)
                {                                
                    _po3DeviceCommonSettingsAndInfo.DevicePassword = Convert.ToUInt16(value);                    
                }
                OnPropertyChanged("DevicePasswordFormatted");
            }
        }
        public string DevicePasswordFormatted => $"{_po3DeviceCommonSettingsAndInfo.DevicePassword:D4}";

        public string ModuleType
        {
            get
            {
                if (_po3DeviceCommonSettingsAndInfo.AssociatedDeviceType == 1)
                    return "МТЕ";                
                return "нет";
            }
        }
       public string FirmwareVersion =>
           $"{_po3DeviceCommonSettingsAndInfo.FirmwareVersion/100}.{_po3DeviceCommonSettingsAndInfo.FirmwareVersion%100}";
        public string ConfigurationVersion =>
            $"{_po3DeviceCommonSettingsAndInfo.ConfigurationVersion / 10}.{_po3DeviceCommonSettingsAndInfo.ConfigurationVersion % 10}";
        public string DeviceRestarted => (_po3DeviceCommonSettingsAndInfo.DeviceStatus & 0x0001) == 1 
            ? "Images/led_red.png" : "Images/led_green.png";
        public string NegativeEEPROMTest => (_po3DeviceCommonSettingsAndInfo.DeviceStatus & 0x0002) == 2 
            ? "Images/led_red.png" : "Images/led_green.png";
        public string IndicatorDriversOverHeating => (_po3DeviceCommonSettingsAndInfo.DeviceStatus & 0x0004) == 4 
            ? "Images/led_red.png" : "Images/led_green.png";
        public string IndicatorDriversSerialPortFault => (_po3DeviceCommonSettingsAndInfo.DeviceStatus & 0x0008) == 8 
            ? "Images/led_red.png" : "Images/led_green.png";
        #endregion

        #endregion
    }
}
