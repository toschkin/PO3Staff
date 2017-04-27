using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PO3Core.Utils
{
    public class SimpleFileLogger
    {
        private string _filePath;
        public SimpleFileLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Log(string message)
        {            
            File.AppendAllText(_filePath, DateTime.Now+"." + DateTime.Now.Millisecond + "\t"+ message + "\r\n");
        }
    }
}
