using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModbusReaderSaver
{
    public enum ModbusRegisterAccessType
    {
        AccessRead = 0,
        AccessReadWrite = 1
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ModbusPropertyAttribute : Attribute
    {
        public ModbusPropertyAttribute()
        {
            Access = ModbusRegisterAccessType.AccessReadWrite;
        }
        public ModbusRegisterAccessType Access { get; set; }
    }
}
