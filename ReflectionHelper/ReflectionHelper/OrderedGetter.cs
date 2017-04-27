﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ReflectionHelper
{
    internal class DeclarationOrderPropertiesComparator : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            PropertyInfo first = x as PropertyInfo;
            PropertyInfo second = y as PropertyInfo;
            if (first.MetadataToken < second.MetadataToken)
                return -1;
            else if (first.MetadataToken > second.MetadataToken)
                return 1;

            return 0;
        }
    }

    internal class DeclarationOrderFieldsComparator : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            FieldInfo first = x as FieldInfo;
            FieldInfo second = y as FieldInfo;
            if (first.MetadataToken < second.MetadataToken)
                return -1;
            else if (first.MetadataToken > second.MetadataToken)
                return 1;

            return 0;
        }
    }

    public class OrderedGetter
    {
        public static PropertyInfo[] GetObjectPropertiesInDeclarationOrder(object obj)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();
            Array.Sort(properties, new DeclarationOrderPropertiesComparator());
            return properties;
        }
        public static FieldInfo[] GetObjectFieldsInDeclarationOrder(object obj)
        {
            Type type = obj.GetType();
            FieldInfo[] fields = type.GetFields();
            Array.Sort(fields, new DeclarationOrderFieldsComparator());
            return fields;
        }
    }
}
