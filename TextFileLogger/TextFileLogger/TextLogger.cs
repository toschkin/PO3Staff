using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TextFileLogger
{
    public class TextLogger
    {
        private static readonly object _syncObject = new object();

        private string _logFilePath;
        public string LogFilePath
        {
            get { return _logFilePath; }
            set
            {
                if (IsValidPath(value))
                {
                    lock (_syncObject)
                    {
                        _logFilePath = value;
                    }
                }
            }
        }

        private static bool IsValidPath(string path)
        {
            //Simple pattern
            //@"^(([a-zA-Z]:)|(\))(\{1}|((\{1})[^\]([^/:*?<>""|]*))+)$"
            string DmitriyBorysovPattern =
                @"^(([a-zA-Z]:|\\)\\)?(((\.)|(\.\.)|([^\\/:\*\?\|<>\. ](([^\\/:\*\?\|<>\. ])|([^\\/:\*\?\|<>]*[^\\/:\*\?\|<>\. ]))?))\\)*[^\\/:\*\?\|<>\. ](([^\\/:\*\?\|<>\. ])|([^\\/:\*\?\|<>]*[^\\/:\*\?\|<>\. ]))?$";
            Regex r = new Regex(DmitriyBorysovPattern);

            return r.IsMatch(path);
        }

        public TextLogger()
        {
            _logFilePath = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName.Remove(AppDomain.CurrentDomain.FriendlyName.LastIndexOf('.')) + "_LOG.txt";
        }

        public void LogTextMessage(string message)
        {
            lock (_syncObject)
            {
                if (!File.Exists(_logFilePath))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(_logFilePath))
                    {
                        sw.WriteLine(DateTime.Now.ToString() + '\t' + message);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(_logFilePath))
                    {
                        sw.WriteLine(DateTime.Now.ToString() + '\t' + message);
                    }
                }
            }                        
        }
    }
}
