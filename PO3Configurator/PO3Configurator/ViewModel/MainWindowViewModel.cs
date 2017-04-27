using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using CommonWindows.View;
using CommonWindows.ViewModel;
using PO3Configurator.Utils;
using PO3Core;
using PO3Core.Utils;
using MVVMToolkit;
using PO3Configurator.View;

namespace PO3Configurator.ViewModel
{
    enum WorkerTasks
    {
        Reading,
        Writing,
        ExecutingCommand,
        //SetSlaveMode,
        Connect,
        Disconnect      
    }
    class OperationResult
    {
        public string OperationResultMessage { get; set; }
        public WorkerTasks OperationExecuted { get; set; }
        public OperationTask NextTask { get; set; }
    }
    class OperationTask
    {
        public object OperationArgument { get; set; }
        public WorkerTasks OperationToExecute { get; set; }
    }   

    class MainWindowViewModel : ViewModelBase
    {        
        #region Constructor
        public MainWindowViewModel(MainWindowView parentWindow)
        {
            _parentWindow = parentWindow;
            
            _worker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _worker.ProgressChanged += worker_ProgressChanged;
            _worker.DoWork += worker_DoWork_InteractWithDevice;
            _worker.RunWorkerCompleted += worker_WorkerCompleted;
            //Models
            Device = new PO3Device();
            ReaderSaver = new PO3ModbusReaderSaver();
            //child viewmodels           
            PO3DeviceUnitCommonSettingsTabViewModel = new PO3DeviceUnitCommonSettingsTabViewModel(this);
            PO3DeviceUnitCommunicationSettingsTabViewModel = new PO3DeviceUnitCommunicationSettingsTabViewModel(this);
            PO3DeviceUnitWindowsSettingsTabViewModel = new PO3DeviceUnitWindowsSettingsTabViewModel(this);
            PO3DeviceUnitMeasurmentCircuitSettingsViewModel = new PO3DeviceUnitMeasurmentCircuitSettingsViewModel(this);
            PO3DeviceUnitParametersSettingsViewModel = new PO3DeviceUnitParametersSettingsViewModel(this);
            _serialPortSettingsViewModel = new SerialPortSettingsViewModel(Constants.registryAppNode);            
        }
        #endregion

        #region Fields

        private bool _channelReady = true;
        private BackgroundWorker _worker;
        private string _currentOperationStatus = "Готов";
        private int _progressPercentage = 0;
        private string _сonnectionStatus = "Отключено";
        private bool _portIsOpen;
        private bool _isConfigurationLoaded;        
        private byte[] _deviceSnapshotBefore;
        private SerialPortSettingsViewModel _serialPortSettingsViewModel;
        private MainWindowView _parentWindow;

        #region TabViewModels

        public PO3DeviceUnitCommonSettingsTabViewModel PO3DeviceUnitCommonSettingsTabViewModel { get; set; }
        public PO3DeviceUnitCommunicationSettingsTabViewModel PO3DeviceUnitCommunicationSettingsTabViewModel { get; set; }
        public PO3DeviceUnitWindowsSettingsTabViewModel PO3DeviceUnitWindowsSettingsTabViewModel { get; set; }
        public PO3DeviceUnitMeasurmentCircuitSettingsViewModel PO3DeviceUnitMeasurmentCircuitSettingsViewModel { get; set; }
        public PO3DeviceUnitParametersSettingsViewModel PO3DeviceUnitParametersSettingsViewModel { get; set; }
        #endregion

        #endregion

        #region Properties

        #region Models
        public PO3Device Device { get; set; }
        public PO3ModbusReaderSaver ReaderSaver { get; set; }
        #endregion

        #region Configuration
        public bool IsConfigurationLoaded
        {
            get { return _isConfigurationLoaded; }
            set
            {
                if (_isConfigurationLoaded != value)
                {
                    _isConfigurationLoaded = value;
                    OnPropertyChanged("IsConfigurationLoaded");
                }
            }
        }
        public bool DeviceConfigurationChanged
        {
            get
            {
                if (_deviceSnapshotBefore == null && IsConfigurationLoaded)
                    return true;

                if (_deviceSnapshotBefore == null)
                    return false;

                MemoryStream buffer = new MemoryStream();
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(buffer, Device);
                byte[] deviceSnapshotAfter = buffer.ToArray();
                if (deviceSnapshotAfter.Length != _deviceSnapshotBefore.Length)
                    return true;
                for (int i = 0; i < _deviceSnapshotBefore.Length; i++)
                {
                    if (deviceSnapshotAfter[i] != _deviceSnapshotBefore[i])
                        return true;
                }                              
                return false;
            }
            set
            {
                if (value == false)
                {
                    MemoryStream buffer = new MemoryStream();
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(buffer, Device);
                    _deviceSnapshotBefore = buffer.ToArray();
                }
                else
                    _deviceSnapshotBefore = null;
            }
        }
        #endregion

        #region Connection             

        public bool ChannelReady
        {
            get { return _channelReady; }
            set {
                    if (value != _channelReady)
                    {
                        _channelReady = value;
                        OnPropertyChanged("ChannelReady");
                        OnPropertyChanged("MenuItemEnabledOnChannelReady");                    
                    }
                }
        } 
        public string OperationStatus
        {
            get { return _currentOperationStatus; }
            set
            {                
                if (_currentOperationStatus != value)
                {
                    _currentOperationStatus = value;
                    OnPropertyChanged("OperationStatus");
                    OnPropertyChanged("OperationStatusTip");                    
                }
            }
        }

        public string OperationStatusTip
        {
            get { return WordWrap(_currentOperationStatus, 100); }            
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
        #endregion

        #region Misc                

        public int ProgressPercentage
        {
            get { return _progressPercentage; }
            set
            {
                if (_progressPercentage != value)
                {
                    _progressPercentage = value;
                    OnPropertyChanged("ProgressPercentage");
                }
            }
        }
        
        public bool MenuItemEnabledOnChannelReady => _channelReady;
        #endregion

        #endregion

        #region Commands  

        public ICommand ConnectToDeviceCommand { get { return new Command(param => ConnectToDevice(), param => CanExecuteConnectToDevice()); } }
        public ICommand ConnectOnPortCommand { get { return new Command(param => ConnectOnPort(), param => CanExecuteConnectOnPort()); } }
        public ICommand DisconnectFromDeviceCommand { get { return new Command(param => DisconnectFromDevice(),param => CanExecuteDisconnectFromDevice()); } }
        public ICommand WriteConfigurationToDeviceCommand { get { return new Command(param => WriteConfigurationToDevice(), param => CanExecuteWriteConfigurationToDevice()); } }
        public ICommand DeviceCommand { get { return new Command(param => ExecuteDeviceCommand(param)); } }   
             
        #endregion

        #region Methods

        #region Command implementation
        private void ConnectOnPort()
        {
            SerialPortSettingsView serialPortSettingsView = new SerialPortSettingsView
            {
                Owner = _parentWindow,
                DataContext = _serialPortSettingsViewModel,
                MainStackPannel = {Background = _parentWindow.DefaultBackgroundBrush}
            };

            if (serialPortSettingsView.ShowDialog() == false)
                return;
            
            if (!_serialPortSettingsViewModel.IsConnected)
                return;

            ReaderSaver.Port = _serialPortSettingsViewModel.Port;            

            try
            {
                if(!ReaderSaver.Port.IsOpen)
                    ReaderSaver.Port.Open();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Ошибка подключения:" + exception.Message, Constants.messageBoxTitle,
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                СonnectionStatus = "Отключено";
            }
            СonnectionStatus = "Подключено: " + ReaderSaver.GetConnectionParametersString();
            PortIsOpen = true;
        }

        private bool CanExecuteConnectOnPort()
        {
            return !PortIsOpen;
        }
        private void ConnectToDevice()
        {
            if (IsUserWantToSaveDeviceConfiguration())
            {
                OperationTask nextConnectTask = new OperationTask
                {
                    OperationToExecute = WorkerTasks.Connect,
                    OperationArgument = null
                };
                OperationTask writeTask = new OperationTask
                {
                    OperationToExecute = WorkerTasks.Writing,
                    OperationArgument = nextConnectTask
                };
                ChannelReady = false;
                _worker.RunWorkerAsync(writeTask);
                return;
            }
            try
            {
                OperationStatus = "Подключение...";
                ReaderSaver.ConnectToDevice(true);                
            }
            catch (Exception exception)
            {
                MessageBox.Show("Ошибка подключения:" + exception.Message,
                    Constants.messageBoxTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            /*OperationTask task = new OperationTask
            {
                OperationToExecute = WorkerTasks.SetSlaveMode,
                OperationArgument = null
            };*/
            OperationTask task = new OperationTask
            {
                OperationToExecute = WorkerTasks.Reading,
                OperationArgument = null
            };            
            ChannelReady = false;
            _worker.RunWorkerAsync(task);
        }
        
        private bool CanExecuteConnectToDevice()
        {
            return ChannelReady;
        }
        private void DisconnectFromDevice()
        {
            if (IsUserWantToSaveDeviceConfiguration())
            {
                OperationTask nextDisconnectTask = new OperationTask
                {
                    OperationToExecute = WorkerTasks.Disconnect,
                    OperationArgument = null
                };
                OperationTask writeTask = new OperationTask
                {
                    OperationToExecute = WorkerTasks.Writing,
                    OperationArgument = nextDisconnectTask
                };
                ChannelReady = false;
                _worker.RunWorkerAsync(writeTask);
                return;
            }

            ReaderSaver.DisconnectFromDevice();
            СonnectionStatus = "Отключено";
            OperationStatus = "";
            PortIsOpen = false;
            IsConfigurationLoaded = false;
            _deviceSnapshotBefore = null;            
        }
        private bool CanExecuteDisconnectFromDevice()
        {
            return PortIsOpen;
        }
        private void WriteConfigurationToDevice()
        {
            if (MessageBoxResult.No == MessageBox.Show("Записать конфигурацию в устройство?", Constants.messageBoxTitle,
                MessageBoxButton.YesNo, MessageBoxImage.Question))
                return;

            OperationTask task = new OperationTask
            {
                OperationToExecute = WorkerTasks.Writing,
                OperationArgument = null
            };
            ChannelReady = false;
            _worker.RunWorkerAsync(task);
        }
        private bool CanExecuteWriteConfigurationToDevice()
        {
            return IsConfigurationLoaded && ChannelReady;
        }        
        public void ExecuteDeviceCommand(object param)
        {
            DeviceControlCommands command = (DeviceControlCommands) param;
            OperationTask task = new OperationTask
            {
                OperationToExecute = WorkerTasks.ExecutingCommand,
                OperationArgument = command
            };
            ChannelReady = false;
            _worker.RunWorkerAsync(task);
        }
        #endregion

        #region BackgroundWorker Events
        private void worker_DoWork_InteractWithDevice(object sender, DoWorkEventArgs e)
        {            
            var worker = (BackgroundWorker)sender;
            
            var operationResult = new OperationResult();           
            string mainOperation="";
            string operation = "";
            var operationTask = (OperationTask)e.Argument;
            switch (operationTask.OperationToExecute)
            {
                case WorkerTasks.Reading:
                    mainOperation = "Операция чтения";                    
                    break;                
                case WorkerTasks.Writing:
                    mainOperation = "Операция записи";                     
                    break;                
                case WorkerTasks.ExecutingCommand:
                    mainOperation = "Выполнение команды";
                    break;
                /*case WorkerTasks.SetSlaveMode:
                    mainOperation = "Перевод устройства в режим";
                    break;*/
                    
            }
            if (operationTask.OperationToExecute == WorkerTasks.Reading ||
                operationTask.OperationToExecute == WorkerTasks.Writing /*||
                operationTask.OperationToExecute == WorkerTasks.SetSlaveMode*/)
            {
                mainOperation += " конфигурации";
                OperationStatus = mainOperation + "...";
            }                
            else           
                OperationStatus = mainOperation + "...";

            //always set SLAVE mode before any operation           
            OperationStatus = operation + "Перевод устройства в режим конфигурирования...";
            PO3SlaveModeSetter setter = new PO3SlaveModeSetter(ReaderSaver.Port);
            while (true)
            {
                bool retVal = false;
                for (int i = 0; i < 3; i++)//3 attempts
                {
                    retVal = setter.SetSlaveMode();
                    if (retVal)
                        break;
                }

                if (!retVal)
                {
                    if (MessageBox.Show(
                        "Не удалось перевести устройство в режим конфигурирования!\r\nПовторить попытку?",
                        Constants.messageBoxTitle, MessageBoxButton.YesNo, MessageBoxImage.Question)
                        == MessageBoxResult.No)
                    {
                        operationResult.OperationResultMessage = "Готов";
                        operationResult.OperationExecuted = operationTask.OperationToExecute;
                        e.Result = operationResult;
                        return;
                    }
                }
                else
                    break;
            }
            if (setter.DetectedSlaveAddress != 0)
                ReaderSaver.SlaveAddress = setter.DetectedSlaveAddress;

            /*if (operationTask.OperationToExecute == WorkerTasks.SetSlaveMode)
            {
                PO3SlaveModeSetter setter = new PO3SlaveModeSetter(ReaderSaver.Port);
                while (true)
                {
                    bool retVal = false;                    
                    for (int i = 0; i < 3; i++)//3 attempts
                    {
                        retVal = setter.SetSlaveMode();
                        if (retVal)
                            break;
                    }
                    
                    if (!retVal)
                    {
                        if (MessageBox.Show(
                            "Не удалось перевести устройство в режим конфигурирования!\r\nПовторить попытку?",
                            Constants.messageBoxTitle, MessageBoxButton.YesNo, MessageBoxImage.Question)
                            == MessageBoxResult.No)
                        {
                            operationResult.OperationResultMessage = "Готов";
                            operationResult.OperationExecuted = operationTask.OperationToExecute;
                            e.Result = operationResult;
                            return;
                        }
                    }
                    else
                        break;
                }
                if (setter.DetectedSlaveAddress != 0)
                    ReaderSaver.SlaveAddress = setter.DetectedSlaveAddress;

                operationResult.OperationResultMessage = mainOperation + " - успешно";
                operationResult.OperationExecuted = operationTask.OperationToExecute;
                operationResult.NextTask = new OperationTask();
                operationResult.NextTask.OperationToExecute = WorkerTasks.Reading;
                operationResult.NextTask.OperationArgument = null;
                e.Result = operationResult;
                return;
            }*/

            int percentComplete;
            string returnedCode;

            if (operationTask.OperationToExecute == WorkerTasks.ExecutingCommand)
            {
                percentComplete = 50;
                returnedCode = ReaderSaver.ExecuteDeviceCommand((DeviceControlCommands) operationTask.OperationArgument);
               
                worker.ReportProgress(percentComplete, null);
                if (returnedCode != "OK")
                {
                    operationResult.OperationResultMessage = mainOperation + " завершено с ошибкой: " + returnedCode;                    
                    operationResult.OperationExecuted = operationTask.OperationToExecute;
                    e.Result = operationResult;
                    return;
                }

                if (((DeviceControlCommands) operationTask.OperationArgument) != DeviceControlCommands.SetMasterMode)//all commands except SetMasterMode
                {
                    //setting up MASTER mode after each operation
                    percentComplete = 100;
                    operation = "Перевод устройства в режим MASTER";
                    OperationStatus = operation + "...";
                    int commandsCount = 0;
                    while (true)
                    {
                        returnedCode = ReaderSaver.ExecuteDeviceCommand(DeviceControlCommands.SetMasterMode);
                        worker.ReportProgress(percentComplete, null);

                        if (((DeviceControlCommands)operationTask.OperationArgument) != DeviceControlCommands.Test) //all commands except Test & SetMasterMode
                        {
                            if (returnedCode != "OK")
                            {
                                operationResult.OperationResultMessage = operation + " завершен с ошибкой: " + returnedCode;
                                operationResult.OperationExecuted = operationTask.OperationToExecute;
                                e.Result = operationResult;
                                return;
                            }
                            break;
                        }
                        else//Test command
                        {
                            if (returnedCode == "OK")
                                break;

                            commandsCount++;
                            if (commandsCount > 13)
                            {
                                if (MessageBox.Show(
                                    "Не удалось перевести устройство в режим MASTER!\r\nПовторить попытку?",
                                    Constants.messageBoxTitle, MessageBoxButton.YesNo, MessageBoxImage.Question)
                                    == MessageBoxResult.No)
                                {
                                    operationResult.OperationResultMessage = operation + " завершен с ошибкой: " + returnedCode;
                                    operationResult.OperationExecuted = operationTask.OperationToExecute;
                                    e.Result = operationResult;
                                    return;
                                }
                                commandsCount = 0;
                                continue;
                            }
                            Thread.Sleep(1000);
                        }
                    }
                }                                
            }
            if (operationTask.OperationToExecute == WorkerTasks.Reading
                || operationTask.OperationToExecute == WorkerTasks.Writing)
            {
                if (operationTask.OperationToExecute == WorkerTasks.Writing)//reset PO-3 before writing config
                {
                    percentComplete = 1 * 100 / (Device.PO3DeviceAsUnits.Count + 2);

                    operation = "Cброс перед записью конфигурации";
                    OperationStatus = operation + "...";
                    returnedCode = ReaderSaver.ExecuteDeviceCommand(DeviceControlCommands.Reset);
                    worker.ReportProgress(percentComplete, null);
                    if (returnedCode != "OK")
                    {
                        operationResult.OperationResultMessage = operation + " завершен с ошибкой: " + returnedCode;
                        operationResult.OperationExecuted = operationTask.OperationToExecute;
                        e.Result = operationResult;
                        return;
                    }
                    Thread.Sleep(1500);
                }

                OperationStatus = mainOperation + "...";
                int step = 0;
                foreach (var unit in Device.PO3DeviceAsUnits)
                {                    
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                    step++;
                    percentComplete = (step * 100 / (Device.PO3DeviceAsUnits.Count + 2));

                    returnedCode = (operationTask.OperationToExecute == WorkerTasks.Reading)
                        ? ReaderSaver.ReadUnitData(unit)
                        : ReaderSaver.WriteUnitData(unit);
                                    
                    worker.ReportProgress(percentComplete, null);

                    if (returnedCode != "OK")
                    {
                        operationResult.OperationResultMessage = mainOperation + " завершена с ошибкой: " + returnedCode;
                        operationResult.OperationExecuted = operationTask.OperationToExecute;
                        e.Result = operationResult;
                        return;
                    }
                }
                /*if (operationTask.OperationToExecute == WorkerTasks.Writing)//reset PO-3 after writing config - уже не надо
                {
                    percentComplete = 100;
                    operation = "Cброс после записи конфигурации";
                    OperationStatus = operation + "...";

                    returnedCode = ReaderSaver.ExecuteDeviceCommand(DeviceControlCommands.Reset);

                    Thread.Sleep(1500);

                    worker.ReportProgress(percentComplete, null);
                    if (returnedCode != "OK")
                    {
                        operationResult.OperationResultMessage = operation + " завершен с ошибкой: " + returnedCode;
                        operationResult.OperationExecuted = operationTask.OperationToExecute;
                        e.Result = operationResult;
                        return;
                    }                    
                }*/

                //setting up MASTER mode after each operation
                percentComplete = 100;
                operation = "Перевод устройства в режим MASTER";
                OperationStatus = operation + "...";
                returnedCode = ReaderSaver.ExecuteDeviceCommand(DeviceControlCommands.SetMasterMode);
                worker.ReportProgress(percentComplete, null);
                if (returnedCode != "OK")
                {
                    operationResult.OperationResultMessage = operation + " завершен с ошибкой: " + returnedCode;
                    operationResult.OperationExecuted = operationTask.OperationToExecute;
                    e.Result = operationResult;
                    return;
                }

                if (operationTask.OperationToExecute == WorkerTasks.Writing)//reconnect with new PO3DeviceUnitCommunicationSettings                    
                {
                    СonnectionStatus = "Переподключение...";
                    ReaderSaver.Port.Close();
                    PortIsOpen = false;

                    try
                    {
                        switch (Device.DeviceUnitCommunicationSettings.DeviceBaudRate)
                        {
                            case 0:
                                ReaderSaver.Port.BaudRate = 2400;
                                break;
                            case 1:
                                ReaderSaver.Port.BaudRate = 4800;
                                break;
                            case 2:
                                ReaderSaver.Port.BaudRate = 9600;
                                break;
                            case 3:
                                ReaderSaver.Port.BaudRate = 14400;
                                break;
                            case 4:
                                ReaderSaver.Port.BaudRate = 19200;
                                break;                            
                        }
                        switch (Device.DeviceUnitCommunicationSettings.DeviceParity)
                        {
                            case 0:
                                ReaderSaver.Port.Parity = Parity.None;
                                break;
                            case 2:
                                ReaderSaver.Port.Parity = Parity.Odd;
                                break;
                            case 3:
                                ReaderSaver.Port.Parity = Parity.Even;
                                break;                            
                        }
                        switch (Device.DeviceUnitCommunicationSettings.DeviceStopBits)
                        {
                            case 0:
                                ReaderSaver.Port.StopBits = StopBits.One;
                                break;
                            case 1:
                                ReaderSaver.Port.StopBits = StopBits.Two;
                                break;
                        }
                        ReaderSaver.SlaveAddress = (byte)Device.DeviceUnitCommunicationSettings.DeviceAddress;
                        ReaderSaver.ConnectToDevice(true);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("Ошибка подключения:" + exception.Message, Constants.messageBoxTitle,
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        СonnectionStatus = "Отключено";
                    }
                    СonnectionStatus = "Подключено: " + ReaderSaver.GetConnectionParametersString();
                    PortIsOpen = true;
                }

                if (operationTask.OperationArgument != null &&
                    operationTask.OperationToExecute == WorkerTasks.Writing)
                {
                    operationResult.NextTask = (OperationTask)(operationTask.OperationArgument);
                }                
            }            
            operationResult.OperationResultMessage = mainOperation + " - успешно";
            operationResult.OperationExecuted = operationTask.OperationToExecute;            
            e.Result = operationResult;
        }
        private void worker_WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ChannelReady = true;
            if (e.Error != null)
            {
                OperationStatus = e.Error.Message;
            }
            else
            {                
                if (((OperationResult) e.Result).OperationExecuted == WorkerTasks.Reading)
                {
                    if(((OperationResult)e.Result).OperationResultMessage.IndexOf("успешно") >=0 )
                        IsConfigurationLoaded = true;
                    Device.SlaveAddress = ReaderSaver.SlaveAddress;
                    //seialize loaded config for "is dirty" pattern
                    DeviceConfigurationChanged = false;

                    UpdateAllChildViewModelsProperties();                   
                }
                
                OperationStatus = ((OperationResult)e.Result).OperationResultMessage;
                ProgressPercentage = 0;

                if (((OperationResult) e.Result).OperationExecuted == WorkerTasks.Writing)
                {
                    DeviceConfigurationChanged = false;
                    if (((OperationResult)e.Result).NextTask != null &&
                    ((OperationResult)e.Result).NextTask.OperationToExecute == WorkerTasks.Connect)
                    {                        
                        ConnectToDevice();
                        return;
                    }
                }
                
                if (((OperationResult)e.Result).NextTask != null)
                {
                    if (((OperationResult) e.Result).NextTask.OperationToExecute == WorkerTasks.Disconnect)
                    {
                        ReaderSaver.DisconnectFromDevice();
                        СonnectionStatus = "Отключено";
                        OperationStatus = "";
                        PortIsOpen = false;
                        IsConfigurationLoaded = false;
                        _deviceSnapshotBefore = null;
                        return;
                    }
                    ChannelReady = false;
                    _worker.RunWorkerAsync(((OperationResult)e.Result).NextTask);
                }
            }            
        }
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressPercentage = e.ProgressPercentage;            
        }

        #endregion

        #region Misc

        /// <summary>
        /// Word wraps the given text to fit within the specified width.
        /// </summary>
        /// <param name="text">Text to be word wrapped</param>
        /// <param name="width">Width, in characters, to which the text
        /// should be word wrapped</param>
        /// <returns>The modified text</returns>
        public static string WordWrap(string text, int width)
        {
            int pos, next;
            StringBuilder sb = new StringBuilder();

            // Lucidity check
            if (width < 1)
                return text;

            // Parse each line of text
            for (pos = 0; pos < text.Length; pos = next)
            {
                // Find end of line
                int eol = text.IndexOf(Environment.NewLine, pos);
                if (eol == -1)
                    next = eol = text.Length;
                else
                    next = eol + Environment.NewLine.Length;

                // Copy this line of text, breaking into smaller lines as needed
                if (eol > pos)
                {
                    do
                    {
                        int len = eol - pos;
                        if (len > width)
                            len = BreakLine(text, pos, width);
                        sb.Append(text, pos, len);
                        sb.Append(Environment.NewLine);

                        // Trim whitespace following break
                        pos += len;
                        while (pos < eol && Char.IsWhiteSpace(text[pos]))
                            pos++;
                    } while (eol > pos);
                }
                else
                    sb.Append(Environment.NewLine); // Empty line
            }
            return sb.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }

        /// <summary>
        /// Locates position to break the given line so as to avoid
        /// breaking words.
        /// </summary>
        /// <param name="text">String that contains line of text</param>
        /// <param name="pos">Index where line of text starts</param>
        /// <param name="max">Maximum line length</param>
        /// <returns>The modified line length</returns>
        private static int BreakLine(string text, int pos, int max)
        {
            // Find last whitespace in line
            int i = max;
            while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
                i--;

            // If no whitespace found, break at maximum length
            if (i < 0)
                return max;

            // Find start of whitespace
            while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
                i--;

            // Return length of text before whitespace
            return i + 1;
        }

        private bool IsUserWantToSaveDeviceConfiguration()
        {
            if (DeviceConfigurationChanged)
            {
                if (MessageBoxResult.Yes ==
                    MessageBox.Show("Сохранить конфигурацию устройства?", Constants.messageBoxTitle,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question))
                {
                    return true;
                }
            }
            return false;
        }        
        #endregion

        #endregion
    }
}
