using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using ModbusReaderSaver;

namespace PO3Core.Utils
{
    public class FileReaderSaver
    {                
        public FileReaderSaver()
        {
            FilePath = @"PO3device.dat";
        }

        public FileReaderSaver(String path)
        {
            FilePath = path;
        }

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                if (!FilePathValidator.IsValidPath(value))
                    _filePath = @"PO3device.dat";
                _filePath = value;
            }
        }

        public string SaveDeviceConfiguration(PO3Device configuration)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            try
            {
                FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate);
                formatter.Serialize(fs, configuration);
                fs.Close();
            }
            catch (Exception exception)
            {
                return "Невозможно сохранить файл!\r\n" + exception.Message;
            }
            return "Файл сохранен успешно.";
        }
        public string ReadDeviceConfiguration(ref PO3Device configuration)
        {                         
            BinaryFormatter formatter = new BinaryFormatter();

            try
            {
                FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate);
                configuration = (PO3Device)formatter.Deserialize(fs);
                fs.Close();
            }
            catch (Exception exception)
            {
                return "Невозможно прочитать файл!\r\n" + exception.Message;
            }
            return "Файл загружен успешно.";
        }

        public string SaveDeviceUnitConfiguration(ModbusExchangeableUnit configuration)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            try
            {
                FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate);
                formatter.Serialize(fs, configuration);
                fs.Close();
            }
            catch (Exception exception)
            {
                return "Невозможно сохранить файл!\r\n" + exception.Message;
            }
            return "Файл сохранен успешно.";
        }
        public string ReadDeviceUnitConfiguration(ref ModbusExchangeableUnit configuration)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            try
            {
                FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate);
                configuration = (ModbusExchangeableUnit)formatter.Deserialize(fs);
                fs.Close();
            }
            catch (Exception exception)
            {
                return "Невозможно прочитать файл!\r\n" + exception.Message;
            }
            return "Файл загружен успешно.";
        }
    }
}
