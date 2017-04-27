using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using MVVMToolkit;
using PO3Core;

namespace PO3Configurator.ViewModel
{
    class PO3DeviceUnitWindowSettingsViewModel : ViewModelBase
    {
        private int _windowIndex = 0;
        private PO3DeviceUnitWindowSettings _po3DeviceUnitWindowSettings;
        private PO3DeviceUnitWindowsSettings _po3DeviceUnitWindowsSettings;
        public PO3DeviceUnitWindowSettingsViewModel(int windowIndex, PO3DeviceUnitWindowsSettings po3DeviceUnitWindowsSettings)
        {
            _windowIndex = windowIndex;
            _po3DeviceUnitWindowsSettings = po3DeviceUnitWindowsSettings;
            _po3DeviceUnitWindowSettings = po3DeviceUnitWindowsSettings.Windows[_windowIndex];            
        }

        public Visibility IsVisible
        {
            get
            {
                if(_windowIndex >= _po3DeviceUnitWindowsSettings.WindowsCount)
                    return Visibility.Collapsed;
                return Visibility.Visible;
            }            
        }

        public FontWeight TabItemFontWeight
        {
            get
            {
                if (_windowIndex == _po3DeviceUnitWindowsSettings.DefaultWindowIndex)
                    return FontWeights.Bold;
                return FontWeights.Normal;
            }
        }
        public string Header => "Окно "+ (_windowIndex+1);

        public string FirstStringParameterIndex
        {
            get { return AvailableParameters[_po3DeviceUnitWindowSettings.FirstStringParameterIndex]; }
            set
            {
                for (int i = 0; i < AvailableParameters.Count; i++)
                {
                    if (AvailableParameters[i] == value)
                        _po3DeviceUnitWindowSettings.FirstStringParameterIndex = (ushort)i;
                }                
            }
        }

        public string SecondStringParameterIndex
        {
            get { return AvailableParameters[_po3DeviceUnitWindowSettings.SecondStringParameterIndex]; }
            set
            {
                for (int i = 0; i < AvailableParameters.Count; i++)
                {
                    if (AvailableParameters[i] == value)
                        _po3DeviceUnitWindowSettings.SecondStringParameterIndex = (ushort)i;
                }
            }
        }

        public string ThirdStringParameterIndex
        {
            get { return AvailableParameters[_po3DeviceUnitWindowSettings.ThirdStringParameterIndex]; }
            set
            {
                for (int i = 0; i < AvailableParameters.Count; i++)
                {
                    if (AvailableParameters[i] == value)
                        _po3DeviceUnitWindowSettings.ThirdStringParameterIndex = (ushort)i;
                }
            }
        }

        public string AnalogBarParameterIndex
        {
            get { return AvailableParameters[_po3DeviceUnitWindowSettings.AnalogBarParameterIndex]; }
            set
            {
                for (int i = 0; i < AvailableParameters.Count; i++)
                {
                    if (AvailableParameters[i] == value)
                        _po3DeviceUnitWindowSettings.AnalogBarParameterIndex = (ushort)i;
                }
            }
        }

        public ObservableCollection<string> AvailableParameters => new ObservableCollection<string>
        {
            "Ua",
            "Ub",
            "Uc",
            "Uab",
            "Uac",
            "Ubc",
            "Ia",
            "Ib",
            "Ic",
            "Pa",
            "Pb",
            "Pc",
            "Qa",
            "Qb",
            "Qc",
            "Sa",
            "Sb",
            "Sc",
            "P",
            "Q",
            "S",
            "Cos A",
            "Cos B",
            "Cos C",
            "Cos",
            "F",
            "пусто"
        };
    }
}
