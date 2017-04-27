using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using Modbus.Data;
using MTESimulator.Model;
using MTESimulator.Utils;
using MVVMToolkit;

namespace MTESimulator.ViewModel
{
    public class MTEDeviceModbusMemoryMapViewModel : ViewModelBase
    {
        #region Fields
        private MTEDeviceModbusMemoryMap _memoryMap;
        private MainWindowViewModel _parentViewModel;        
        #endregion

        public MTEDeviceModbusMemoryMapViewModel(DataStore dataStore, MainWindowViewModel parentViewModel)
        {
            _memoryMap = new MTEDeviceModbusMemoryMap(dataStore);
            _parentViewModel = parentViewModel;            
        }

        #region Prperties

        public sbyte Temperature { get { return _memoryMap.Temperature; }  set { _memoryMap.Temperature = value; } }
        public ushort SerialNumber { get { return _memoryMap.SerialNumber; } set { _memoryMap.SerialNumber = value; } }
        public string MeasurementCircuitType
        {
            get
            {
                return AvailableMeasurementCircuitTypes[_memoryMap.MeasurementCircuitType-2];                              
            }
            set
            {
                _memoryMap.MeasurementCircuitType = (byte)(AvailableMeasurementCircuitTypes.IndexOf(value)+2);                                        
            }
        }

        public ObservableCollection<string> AvailableMeasurementCircuitTypes => new ObservableCollection<string>
        {
            //"Без измерений",
            //"2-х проводная схема",
            "3-х проводная схема",
            "4-х проводная схема"
        };

        public ObservableCollection<string> AvailableOutputModuleTypes => new ObservableCollection<string>
        {
            "нет модуля",
            "модуль токового выхода",
            "релейный модуль"            
        };

        public string OutputModule1Type
        {
            get
            {
                return AvailableOutputModuleTypes[_memoryMap.OutputModule1Type];
            }
            set
            {
                _memoryMap.OutputModule1Type = (byte)AvailableOutputModuleTypes.IndexOf(value);
            }
        }
        public string OutputModule2Type
        {
            get
            {
                return AvailableOutputModuleTypes[_memoryMap.OutputModule2Type];
            }
            set
            {
                _memoryMap.OutputModule2Type = (byte)AvailableOutputModuleTypes.IndexOf(value);
            }
        }
        public string OutputModule3Type
        {
            get
            {
                return AvailableOutputModuleTypes[_memoryMap.OutputModule3Type];
            }
            set
            {
                _memoryMap.OutputModule3Type = (byte)AvailableOutputModuleTypes.IndexOf(value);
            }
        }

        public ObservableCollection<string> AvailableCurrentInterfaceTypes => new ObservableCollection<string>
        {
            "отсутствует",
            "0..5 mA",
            "(-5)..0..5 mA",
            "0..2,5..5 mA",
            "4..12..20 mA",
            "(-20)..0..20 mA",
            "0..10..20 mA",
            "программируемый",
            "0..20 mA",
            "4..20 mA"
        };

        public string CurrentInterfaceType
        {
            get
            {
                return AvailableCurrentInterfaceTypes[_memoryMap.CurrentInterfaceType];
            }
            set
            {
                _memoryMap.CurrentInterfaceType = (byte)AvailableCurrentInterfaceTypes.IndexOf(value);
            }
        }

        public byte Serie
        {
            get
            {
                return _memoryMap.Serie;
            }
            set
            {
                _memoryMap.Serie = value;
            }
        }

        public uint PrimaryCurrent
        {
            get
            {
                return _memoryMap.PrimaryCurrent;
            }
            set
            {
                _memoryMap.PrimaryCurrent = value;
                OnPropertyChanged("Coef_CT");
                OnPropertyChanged("CurrentQuantWeight");
                OnPropertyChanged("PowerQuantWeight");
                OnPropertyChanged("PowerMaxValue");
                OnPropertyChanged("PowerMinValue");
                OnPropertyChanged("Imax");
            }
        }
        public byte SecondaryCurrent
        {
            get
            {
                return _memoryMap.SecondaryCurrent;
            }
            set
            {
                _memoryMap.SecondaryCurrent = value;
                OnPropertyChanged("Coef_CT");
                OnPropertyChanged("CurrentQuantWeight");
                OnPropertyChanged("PowerQuantWeight");
                OnPropertyChanged("PowerMaxValue");
                OnPropertyChanged("PowerMinValue");
                OnPropertyChanged("Imax");
            }
        }
        public uint PrimaryVoltage
        {
            get
            {
                return _memoryMap.PrimaryVoltage;
            }
            set
            {
                _memoryMap.PrimaryVoltage = value;
                OnPropertyChanged("Coef_VT");                
                OnPropertyChanged("Uab");
                OnPropertyChanged("Uac");
                OnPropertyChanged("Ubc");
                OnPropertyChanged("VoltageQuantWeight");
                OnPropertyChanged("PowerQuantWeight");
                OnPropertyChanged("PowerMaxValue");
                OnPropertyChanged("PowerMinValue");
                OnPropertyChanged("Umax");
            }
        }
        public byte SecondaryVoltage
        {
            get
            {
                return _memoryMap.SecondaryVoltage;
            }
            set
            {
                _memoryMap.SecondaryVoltage = value;
                OnPropertyChanged("Coef_VT");                                
                OnPropertyChanged("Uab");
                OnPropertyChanged("Uac");
                OnPropertyChanged("Ubc");
                OnPropertyChanged("VoltageQuantWeight");
                OnPropertyChanged("PowerQuantWeight");
                OnPropertyChanged("PowerMaxValue");
                OnPropertyChanged("PowerMinValue");
                OnPropertyChanged("Umax");
            }
        }

        public double CurrentQuantWeight => Coef_CT / 1000.0;

        public double VoltageQuantWeight => Coef_VT / 100000.0;

        public double PowerQuantWeight => _memoryMap.PowerQuantWeight;

        public double Imax => SecondaryCurrent * 1.2 * Coef_CT;

        public double Umax => VoltageQuantWeight * 30000.0;
        public double PowerMaxValue => PowerQuantWeight * 2147483647;
        public double PowerMinValue => PowerQuantWeight * -2147483648;

        public ushort Coef_CT
        {
            get
            {
                return _memoryMap.Coef_CT;
            }
            set
            {
                _memoryMap.Coef_CT = value;                
            }
        }
        public ushort Coef_VT
        {
            get
            {
                return _memoryMap.Coef_VT;
            }
            set
            {
                _memoryMap.Coef_VT = value;                
            }
        }
        public double Ua
        {
            get
            {
                return _memoryMap.Ua / 1000;
            }
            set
            {
                _memoryMap.Ua = value * 1000;
                OnPropertyChanged("Uac");
                OnPropertyChanged("Uab");
                OnPropertyChanged("Ua_quants");
            }
        }
        public double Ua_quants => _memoryMap.Ua_quants;
        public double Ub
        {
            get
            {
                return _memoryMap.Ub / 1000;
            }
            set
            {
                _memoryMap.Ub = value * 1000;
                OnPropertyChanged("Ubc");
                OnPropertyChanged("Uab");
                OnPropertyChanged("Ub_quants");
            }
        }
        public double Ub_quants => _memoryMap.Ub_quants;
        public double Uc
        {
            get
            {
                return _memoryMap.Uc / 1000;
            }
            set
            {
                _memoryMap.Uc = value * 1000;
                OnPropertyChanged("Ubc");
                OnPropertyChanged("Uac");
                OnPropertyChanged("Uc_quants");
            }
        }
        public double Uc_quants => _memoryMap.Uc_quants;
        public double Uab
        {
            get
            {
                return Math.Sqrt(Math.Pow(_memoryMap.Ua,2) + Math.Pow(_memoryMap.Ub, 2) + _memoryMap.Ub * _memoryMap.Ua) / 1000; 
            }
            set
            { }
        }
        public double Ubc
        {
            get
            {
                return Math.Sqrt(Math.Pow(_memoryMap.Uc, 2) + Math.Pow(_memoryMap.Ub, 2) + _memoryMap.Ub * _memoryMap.Uc) / 1000;
            }
            set
            { }
        }
        public double Uac
        {
            get
            {
                return Math.Sqrt(Math.Pow(_memoryMap.Ua, 2) + Math.Pow(_memoryMap.Uc, 2) + _memoryMap.Uc * _memoryMap.Ua) / 1000;
            }
            set
            { }
        }

        public double Frequency
        {
            get
            {
                return _memoryMap.Frequency;
            }
            set
            {
                _memoryMap.Frequency = value;
                OnPropertyChanged("Frequency_quants");
            }
        }
        public double Frequency_quants => _memoryMap.Frequency_quants;        
        public double Ia
        {
            get
            {
                return _memoryMap.Ia;
            }
            set
            {
                _memoryMap.Ia = value;
                OnPropertyChanged("Ia_quants");                
            }
        }
        public double Ia_quants => _memoryMap.Ia_quants;

        public double Ib
        {
            get
            {
                return _memoryMap.Ib;
            }
            set
            {
                _memoryMap.Ib = value;
                OnPropertyChanged("Ib_quants");
            }
        }

        public double Ib_quants => _memoryMap.Ib_quants;

        public double Ic
        {
            get
            {
                return _memoryMap.Ic;
            }
            set
            {
                _memoryMap.Ic = value;
                OnPropertyChanged("Ic_quants");
            }
        }

        public double Ic_quants => _memoryMap.Ic_quants;

        public double In
        {
            get
            {
                return _memoryMap.In;
            }
            set
            {
                _memoryMap.In = value;
                OnPropertyChanged("In_quants");
            }
        }
        public double In_quants => _memoryMap.Ic_quants;        

        public double Pa
        {
            get
            {
                return _memoryMap.Pa;
            }
            set
            {
                _memoryMap.Pa = value;
                OnPropertyChanged("Pa_quants");
            }
        }

        public double Pa_quants => _memoryMap.Pa_quants;

        public double Pb
        {
            get
            {
                return _memoryMap.Pb;
            }
            set
            {
                _memoryMap.Pb = value;
                OnPropertyChanged("Pb_quants");
            }
        }

        public double Pb_quants => _memoryMap.Pb_quants;

        public double Pc
        {
            get
            {
                return _memoryMap.Pc;
            }
            set
            {
                _memoryMap.Pc = value;
                OnPropertyChanged("Pc_quants");
            }
        }

        public double Pc_quants => _memoryMap.Pc_quants;

        public double Qa
        {
            get
            {
                return _memoryMap.Qa;
            }
            set
            {
                _memoryMap.Qa = value;
                OnPropertyChanged("Qa_quants");
            }
        }

        public double Qa_quants => _memoryMap.Qa_quants;

        public double Qb
        {
            get
            {
                return _memoryMap.Qb;
            }
            set
            {
                _memoryMap.Qb = value;
                OnPropertyChanged("Qb_quants");
            }
        }

        public double Qb_quants => _memoryMap.Qb_quants;

        public double Qc
        {
            get
            {
                return _memoryMap.Qc;
            }
            set
            {
                _memoryMap.Qc = value;
                OnPropertyChanged("Qc_quants");
            }
        }

        public double Qc_quants => _memoryMap.Qc_quants;

        public double Sa
        {
            get
            {
                return _memoryMap.Sa;
            }
            set
            {
                _memoryMap.Sa = value;
                OnPropertyChanged("Sa_quants");
            }
        }

        public double Sa_quants => _memoryMap.Sa_quants;

        public double Sb
        {
            get
            {
                return _memoryMap.Sb;
            }
            set
            {
                _memoryMap.Sb = value;
                OnPropertyChanged("Sb_quants");
            }
        }

        public double Sb_quants => _memoryMap.Sb_quants;

        public double Sc
        {
            get
            {
                return _memoryMap.Sc;
            }
            set
            {
                _memoryMap.Sc = value;
                OnPropertyChanged("Sc_quants");
            }
        }

        public double Sc_quants => _memoryMap.Sc_quants;

        public double Psys
        {
            get
            {
                return _memoryMap.Psys;
            }
            set
            {
                _memoryMap.Psys = value;
                OnPropertyChanged("Psys_quants");
            }
        }

        public double Psys_quants => _memoryMap.Psys_quants;

        public double Qsys
        {
            get
            {
                return _memoryMap.Qsys;
            }
            set
            {
                _memoryMap.Qsys = value;
                OnPropertyChanged("Qsys_quants");
            }
        }

        public double Qsys_quants => _memoryMap.Qsys_quants;

        public double Ssys
        {
            get
            {
                return _memoryMap.Ssys;
            }
            set
            {
                _memoryMap.Ssys = value;
                OnPropertyChanged("Ssys_quants");
            }
        }

        public double Ssys_quants => _memoryMap.Ssys_quants;

        public ushort Phi_ab
        {
            get
            {
                return _memoryMap.Phi_ab;
            }
            set
            {
                _memoryMap.Phi_ab = value;
                OnPropertyChanged("Phi_ab");
            }
        }

        public ushort Phi_ac
        {
            get
            {
                return _memoryMap.Phi_ac;
            }
            set
            {
                _memoryMap.Phi_ac = value;
                OnPropertyChanged("Phi_ac");
            }
        }

        public double Cos_a
        {
            get
            {
                return _memoryMap.Cos_a;
            }
            set
            {
                _memoryMap.Cos_a = value;
                OnPropertyChanged("Cos_a_quants");
            }
        }
        public double Cos_a_quants => _memoryMap.Cos_a_quants;
        public double Cos_b
        {
            get
            {
                return _memoryMap.Cos_b;
            }
            set
            {
                _memoryMap.Cos_b = value;
                OnPropertyChanged("Cos_b_quants");
            }
        }
        public double Cos_b_quants => _memoryMap.Cos_a_quants;
        public double Cos_c
        {
            get
            {
                return _memoryMap.Cos_c;
            }
            set
            {
                _memoryMap.Cos_c = value;
                OnPropertyChanged("Cos_c_quants");
            }
        }
        public double Cos_c_quants => _memoryMap.Cos_a_quants;
        public double Cos_sys
        {
            get
            {
                return _memoryMap.Cos_sys;
            }
            set
            {
                _memoryMap.Cos_sys = value;
                OnPropertyChanged("Cos_sys_quants");
            }
        }
        public double Cos_sys_quants => _memoryMap.Cos_a_quants;

        #endregion

        #region Methods

        public void Import()
        {
            OpenFileDialog dlgOpenFileDialog = new OpenFileDialog
            {
                Filter = "Тестовые конфигурации|*.mte",
                Title = "Импорт тестовой конфигурации из файла"
            };
            if (dlgOpenFileDialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                if (dlgOpenFileDialog.FileName != "")
                {
                    _parentViewModel.OperationStatus = "Импорт из файла...";

                    BinaryFormatter formatter = new BinaryFormatter();

                    try
                    {
                        DataStore tempDataStore = _memoryMap.GetDataStore();
                        FileStream fs = new FileStream(dlgOpenFileDialog.FileName, FileMode.OpenOrCreate);
                        _memoryMap = (MTEDeviceModbusMemoryMap)formatter.Deserialize(fs);
                        _memoryMap.SetDataStore(tempDataStore);
                        _memoryMap.UpdateAllPropertiesFromStoredValues();
                        UpdateAllViewModelProperties();
                        fs.Close();
                    }
                    catch (Exception exception)
                    {
                        _parentViewModel.OperationStatus = "Невозможно прочитать файл!\r\n" + exception.Message;
                    }
                    _parentViewModel.OperationStatus = "Файл загружен успешно.";                                        
                }
            }                                   
        }

        public void Export()
        {

            SaveFileDialog dlgSaveFileDialog = new SaveFileDialog
            {
                Filter = "Тестовые конфигурации|*.mte",
                Title = "Экспорт тестовой конфигурации в файл",
                FileName = "test1.mte"
            };
            if (dlgSaveFileDialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                if (dlgSaveFileDialog.FileName != "")
                {
                    _parentViewModel.OperationStatus = "Экспорт в файл...";
                    BinaryFormatter formatter = new BinaryFormatter();
                    try
                    {
                        FileStream fs = new FileStream(dlgSaveFileDialog.FileName, FileMode.OpenOrCreate);
                        formatter.Serialize(fs, _memoryMap);
                        fs.Close();
                    }
                    catch (Exception exception)
                    {
                        _parentViewModel.OperationStatus = "Невозможно сохранить файл!\r\n" + exception.Message;
                    }
                    _parentViewModel.OperationStatus = "Файл сохранен успешно.";                    
                }
            }            
        }

        #endregion

        public void Dafault()
        {   
            if(MessageBox.Show("Задать значения по умолчанию (номинальные)?",Constants.messageBoxTitle,MessageBoxButton.YesNo,MessageBoxImage.Question) == MessageBoxResult.No)
                return;
            _memoryMap = new MTEDeviceModbusMemoryMap(_memoryMap.GetDataStore());                       
            UpdateAllViewModelProperties();
        }
    }
}
