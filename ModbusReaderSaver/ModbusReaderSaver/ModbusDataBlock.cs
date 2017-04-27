using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModbusReaderSaver
{   
    public enum ModbusDataType
    {
        CoilStatus,
        InputStatus,
        HoldingRegister,
        InputRegister
    }
    
    public class ModbusDataBlock
    {
        public ModbusDataType Type { get; set; }
        public ushort Offset { get; set; }
        public ushort Size { get; set; }
        public ushort AbsoluteOffset { get; set; }
    }
}
