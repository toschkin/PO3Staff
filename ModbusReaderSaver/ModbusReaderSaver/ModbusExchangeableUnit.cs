using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ReflectionHelper;

namespace ModbusReaderSaver
{   
    [Serializable]
    public abstract class ModbusExchangeableUnit
    {
        public abstract List<ModbusDataBlock> GetReadMap();
        public abstract List<ModbusDataBlock> GetWriteMap();
        public void Copy(ModbusExchangeableUnit unit)
        {
            ushort[] temp = unit.GetPropertiesDataAsRegisters();
            SetPropertiesDataFromRegisters(temp);          
        }
        public ushort[] GetPropertiesDataAsRegisters()
        {
            PropertyInfo [] properties = OrderedGetter.GetObjectPropertiesInDeclarationOrder(this);
            List<ushort> registers = new List<ushort>();

                foreach (var pi in properties)
                {
                    if (pi.PropertyType.IsArray && (pi.GetValue(this, null) is ModbusExchangeableUnit[]))
                    {                       
                        foreach (var elem in pi.GetValue(this, null) as ModbusExchangeableUnit[])
                        {
                            registers.AddRange(elem.GetPropertiesDataAsRegisters());
                        }                                                
                    }
                    
                    if (pi.GetCustomAttributes(typeof(ModbusPropertyAttribute), false).Length == 0)
                        continue;                    
                    registers.Add((ushort)pi.GetValue(this, null));
                }
                return registers.ToArray();
        }
        public void SetPropertiesDataFromRegisters(ushort[] value)
        {
            int index = 0;
            _setPropertiesDataFromRegisters(value, ref index);
        }
        private void _setPropertiesDataFromRegisters(ushort[] value, ref int currentIndex)
        {
            PropertyInfo[] properties = OrderedGetter.GetObjectPropertiesInDeclarationOrder(this);            
            foreach (var pi in properties)
            {
                if (pi.PropertyType.IsArray && (pi.GetValue(this, null) is ModbusExchangeableUnit[]))
                {
                    foreach (var elem in pi.GetValue(this, null) as ModbusExchangeableUnit[])
                    {
                        elem._setPropertiesDataFromRegisters(value,ref currentIndex);
                    }
                }

                if (pi.GetCustomAttributes(typeof(ModbusPropertyAttribute), false).Length == 0)//only props with Access
                    continue;
                if (value == null || value.Length == currentIndex)//get much as possible
                    return;

                pi.SetValue(this, value[currentIndex], null);
                currentIndex++;
            }
        }        
    }
}